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
            var cmdText = string.Format(@"SELECT
                backing_veh_cd, crash_location_cd, crossing_driveway_cd, crossing_roadway_cd,
                failure_to_yield_cd, leg_of_intrsect_cd, motorist_dir_travel_cd, motorist_maneuver_cd,
                non_roadway_loc_cd, other_ped_action_cd, ped_dir_travel_cd, ped_failure_to_yield_cd,
                ped_in_roadway_cd, ped_movement_cd, ped_position_cd, right_turn_on_red_cd, turn_merge_cd,
                typical_ped_action_cd, unusual_circumstances_cd, unusual_ped_action_cd, unusual_veh_type_cd,
                waiting_to_cross_cd, walking_along_roadway_cd
                FROM pbcat_ped WHERE hsmv_rpt_nbr = {0}", hsmvRptNbr);
            var da = new OracleDataAdapter(cmdText, _warehouseConnStr);
            var ds = new DataSet();
            da.Fill(ds);
            var dt = ds.Tables[0];
            PBCATPedestrianInfo info = null;
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                info = new PBCATPedestrianInfo();
                info.BackingVehicleCd = ConvertToEnum(dr["backing_veh_cd"], BackingVehicleLocation.NotSpecified);
                info.CrashLocationCd = ConvertToEnum(dr["crash_location_cd"], CrashLocationPed.NotSpecified);
                info.CrossingDrivewayCd = ConvertToEnum(dr["crossing_driveway_cd"], CrossingDrivewayOrAlleyCircumstances.NotSpecified);
                info.CrossingRoadwayCd = ConvertToEnum(dr["crossing_roadway_cd"], CrossingRoadwayCircumstances.NotSpecified);
                info.FailureToYieldCd = ConvertToEnum(dr["failure_to_yield_cd"], FailureToYield.NotSpecified);
                info.LegOfIntrsectCd = ConvertToEnum(dr["leg_of_intrsect_cd"], LegOfIntersection.NotSpecified);
                info.MotoristDirTravelCd = ConvertToEnum(dr["motorist_dir_travel_cd"], DirectionOfTravel.NotSpecified);
                info.MotoristManeuverCd = ConvertToEnum(dr["motorist_maneuver_cd"], MotoristManeuver.NotSpecified);
                info.NonRoadwayLocationCd = ConvertToEnum(dr["non_roadway_loc_cd"], NonRoadwayLocation.NotSpecified);
                info.OtherPedActionCd = ConvertToEnum(dr["other_ped_action_cd"], OtherPedAction.NotSpecified);
                info.PedestrianDirTravelCd = ConvertToEnum(dr["ped_dir_travel_cd"], DirectionOfTravel.NotSpecified);
                info.PedestrianFailedToYieldCd = ConvertToEnum(dr["ped_failure_to_yield_cd"], PedFailedToYield.NotSpecified);
                info.PedestrianInRoadwayCd = ConvertToEnum(dr["ped_in_roadway_cd"], PedActionInRoadway.NotSpecified);
                info.PedestrianMovementCd = ConvertToEnum(dr["ped_movement_cd"], PedMovementScenario.NotSpecified);
                info.PedestrianPositionCd = ConvertToEnum(dr["ped_position_cd"], PedPosition.NotSpecified);
                info.RightTurnOnRedCd = ConvertToEnum(dr["right_turn_on_red_cd"], TurningRightOnRed.NotSpecified);
                info.TurnMergeCd = ConvertToEnum(dr["turn_merge_cd"], TurnMergeCircumstances.NotSpecified);
                info.TypicalPedActionCd = ConvertToEnum(dr["typical_ped_action_cd"], TypicalPedAction.NotSpecified);
                info.UnusualCircumstancesCd = ConvertToEnum(dr["unusual_circumstances_cd"], UnusualCircumstancesPed.NotSpecified);
                info.UnusualPedActionCd = ConvertToEnum(dr["unusual_ped_action_cd"], UnusualPedAction.NotSpecified);
                info.UnusualVehicleTypeOrActionCd = ConvertToEnum(dr["unusual_veh_type_cd"], UnusualVehicleTypeOrAction.NotSpecified);
                info.WaitingToCrossCd = ConvertToEnum(dr["waiting_to_cross_cd"], WaitingToCrossVehicleMovement.NotSpecified);
                info.WalkingAlongRoadwayCd = ConvertToEnum(dr["walking_along_roadway_cd"], WalkingAlongRoadwayCircumstances.NotSpecified);
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

        /// <summary>
        /// Convert a numeric value from the database to an enum value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="dbValue">Numeric value from the database</param>
        /// <param name="defaultValue">Default enum value to use if database value is DBNull</param>
        /// <returns></returns>
        private T ConvertToEnum<T>(object dbValue, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("T must be an enumeration."); }
            return dbValue == DBNull.Value
                ? defaultValue
                : (T)(object)Convert.ToInt32(dbValue);
        }
    }
}
