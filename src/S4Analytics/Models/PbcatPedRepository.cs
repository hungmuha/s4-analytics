using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Lib.PBCAT;

namespace S4Analytics.Models
{
    public class PbcatPedRepository : IPbcatPedRepository
    {
        private string _warehouseConnStr;

        public PbcatPedRepository(IOptions<DbOptions> dbOptions)
        {
            _warehouseConnStr = dbOptions.Value.WarehouseConnStr;
        }

        public PBCATPedestrianInfo Find(int hsmvRptNbr)
        {
            var queryText = string.Format(
                @"SELECT
                BACKING_VEH_CD,
                CRASH_LOCATION_CD,
                CROSSING_DRIVEWAY_CD,
                CROSSING_ROADWAY_CD,
                FAILURE_TO_YIELD_CD,
                LEG_OF_INTRSECT_CD,
                MOTORIST_DIR_TRAVEL_CD,
                MOTORIST_MANEUVER_CD,
                NON_ROADWAY_LOC_CD,
                OTHER_PED_ACTION_CD,
                PED_DIR_TRAVEL_CD,
                PED_FAILURE_TO_YIELD_CD,
                PED_IN_ROADWAY_CD,
                PED_MOVEMENT_CD,
                PED_POSITION_CD,
                RIGHT_TURN_ON_RED_CD,
                TURN_MERGE_CD,
                TYPICAL_PED_ACTION_CD,
                UNUSUAL_CIRCUMSTANCES_CD,
                UNUSUAL_PED_ACTION_CD,
                UNUSUAL_VEH_TYPE_CD,
                WAITING_TO_CROSS_CD,
                WALKING_ALONG_ROADWAY_CD
                FROM PBCAT_PED WHERE HSMV_RPT_NBR = {0}", hsmvRptNbr);
            var da = new OracleDataAdapter(queryText, _warehouseConnStr);
            var ds = new DataSet();
            da.Fill(ds);
            var dt = ds.Tables[0];
            PBCATPedestrianInfo info = null;
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                info = new PBCATPedestrianInfo();
                info.BackingVehicleCd = (BackingVehicleLocation)Convert.ToInt32(dr["BACKING_VEH_CD"]);
                // etc.
            }
            return info;
        }

        public void Add(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {

        }

        public void Update(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {

        }

        public void Remove(int hsmvRptNbr)
        {

        }

        public CrashTypePedestrian GetCrashType(PBCATPedestrianInfo pedInfo)
        {
            return new CrashTypePedestrian();
        }

        public bool HsmvNumberExists(int hsmvRptNbr)
        {
            return true;
        }
    }
}
