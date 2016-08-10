using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace S4Analytics.Models
{
    public class Agency
    {
        public int Id { get; set; }
        public string AgencyNumber { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
        public string UnitType { get; set; }
    }

    public class AgencyRepository : IAgencyRepository
    {
        private string _warehouseConnStr;

        public AgencyRepository(IOptions<DbOptions> dbOptions)
        {
            _warehouseConnStr = dbOptions.Value.WarehouseConnStr;
        }

        public IEnumerable<Agency> GetAll()
        {
            var agencies = new List<Agency>();
            // todo: use stored proc instead
            var cmdText = "select id, agncy_nbr, agncy_nm, agncy_short_nm, agncy_type_nm, org_unit_type_nm from dim_agncy order by agncy_nm";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(cmdText, conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var agency = new Agency()
                            {
                                Id = rdr.GetInt32(0),
                                AgencyNumber = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                                Name = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                ShortName = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                Type = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                                UnitType = rdr.IsDBNull(5) ? "" : rdr.GetString(5)
                            };
                            agencies.Add(agency);
                        }
                    }
                }
            }
            return agencies;
        }
    }
}