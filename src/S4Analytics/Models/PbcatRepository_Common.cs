using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace S4Analytics.Models
{
    public class PbcatParticipantInfo
    {
        public bool HasPedestrianParticipant { get; set; }
        public bool HasBicyclistParticipant { get; set; }
        public bool HasPedestrianTyping { get; set; }
        public bool HasBicyclistTyping { get; set; }
    }

    public class BikePedQueue
    {
        public Guid Token { get; set; }
        public int InitialHsmvReportNumber { get; set; }
        public IList<int> HsmvReportNumbers { get; set; }
    }

    public partial class PbcatRepository : S4Repository, IPbcatRepository
    {
        private string _warehouseConnStr;

        public PbcatRepository(IOptions<ServerOptions> serverOptions)
        {
            _warehouseConnStr = serverOptions.Value.WarehouseConnStr;
        }

        public string GetSessionJson(Guid token)
        {
            string queueJson = "{}";
            var cmdText = "SELECT json_payload FROM v_html5_conduit WHERE token = :token";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                var cmd = new OracleCommand(cmdText, conn);
                cmd.Parameters.Add("token", OracleDbType.Raw).Value = token;
                var da = new OracleDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    queueJson = dr.Field<string>("json_payload");
                }
            }
            return "{ \"data\": " + queueJson + " }";
        }

        public PbcatParticipantInfo GetParticipantInfo(int hsmvRptNbr)
        {
            PbcatParticipantInfo info = null;
            var cmdText = @"SELECT
                CASE WHEN ce.ped_cnt > 0 OR he.harmful_evt_tx = 'Pedestrian' THEN 1 ELSE 0 END AS has_ped_participant,
                CASE WHEN ce.bike_cnt > 0 OR he.harmful_evt_tx = 'Pedalcycle' THEN 1 ELSE 0 END AS has_bike_participant,
                nvl2(p.hsmv_rpt_nbr, 1, 0) AS has_ped_typing,
                nvl2(b.hsmv_rpt_nbr, 1, 0) AS has_bike_typing
                FROM fact_crash_evt ce
                LEFT OUTER JOIN dim_harmful_evt he
                    ON he.ID = ce.key_1st_he
                LEFT OUTER JOIN pbcat_ped p
                    ON p.hsmv_rpt_nbr = ce.hsmv_rpt_nbr
                LEFT OUTER JOIN pbcat_bike b
                    ON b.hsmv_rpt_nbr = ce.hsmv_rpt_nbr
                WHERE ce.hsmv_rpt_nbr = :hsmvRptNbr";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                var cmd = new OracleCommand(cmdText, conn);
                cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                var da = new OracleDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    info = new PbcatParticipantInfo();
                    info.HasPedestrianParticipant = dr.Field<decimal>("has_ped_participant") == 1;
                    info.HasBicyclistParticipant = dr.Field<decimal>("has_bike_participant") == 1;
                    info.HasPedestrianTyping = dr.Field<decimal>("has_ped_typing") == 1;
                    info.HasBicyclistTyping = dr.Field<decimal>("has_bike_typing") == 1;
                }
            }
            return info;
        }

        public bool CrashReportExists(int hsmvRptNbr)
        {
            var cmdText = "SELECT COUNT(*) FROM fact_crash_evt WHERE hsmv_rpt_nbr = :hsmvRptNbr";
            int ct;
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
                {
                    cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                    ct = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return ct > 0;
        }
    }
}
