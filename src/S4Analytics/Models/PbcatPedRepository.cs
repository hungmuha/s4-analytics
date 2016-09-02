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
            var cmdText = @"INSERT INTO pbcat_ped (
                hsmv_rpt_nbr, backing_veh_cd, crash_location_cd, crossing_driveway_cd, crossing_roadway_cd,
                failure_to_yield_cd, leg_of_intrsect_cd, motorist_dir_travel_cd, motorist_maneuver_cd,
                non_roadway_loc_cd, other_ped_action_cd, ped_dir_travel_cd, ped_failure_to_yield_cd,
                ped_in_roadway_cd, ped_movement_cd, ped_position_cd, right_turn_on_red_cd, turn_merge_cd,
                typical_ped_action_cd, unusual_circumstances_cd, unusual_ped_action_cd, unusual_veh_type_cd,
                waiting_to_cross_cd, walking_along_roadway_cd
            ) VALUES (
                :hsmvRptNbr, :backingVehicleCd, :crashLocationCd, :crossingDrivewayCd, :crossingRoadwayCd,
                :failureToYieldCd, :legOfIntrsectCd, :motoristDirTravelCd, :motoristManeuverCd,
                :nonRoadwayLocationCd, :otherPedActionCd, :pedestrianDirTravelCd, :pedestrianFailedToYieldCd,
                :pedestrianInRoadwayCd, :pedestrianMovementCd, :pedestrianPositionCd, :rightTurnOnRedCd,
                :turnMergeCd, :typicalPedActionCd, :unusualCircumstancesCd, :unusualPedActionCd,
                :unusualVehicleTypeOrActionCd, :waitingToCrossCd, :walkingAlongRoadwayCd
            )";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new OracleCommand(cmdText, conn))
                    {
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.Parameters.Add("backingVehicleCd", OracleDbType.Decimal).Value = pedInfo.BackingVehicleCd;
                        cmd.Parameters.Add("crashLocationCd", OracleDbType.Decimal).Value = pedInfo.CrashLocationCd;
                        cmd.Parameters.Add("crossingDrivewayCd", OracleDbType.Decimal).Value = pedInfo.CrossingDrivewayCd;
                        cmd.Parameters.Add("crossingRoadwayCd", OracleDbType.Decimal).Value = pedInfo.CrossingRoadwayCd;
                        cmd.Parameters.Add("failureToYieldCd", OracleDbType.Decimal).Value = pedInfo.FailureToYieldCd;
                        cmd.Parameters.Add("legOfIntrsectCd", OracleDbType.Decimal).Value = pedInfo.LegOfIntrsectCd;
                        cmd.Parameters.Add("motoristDirTravelCd", OracleDbType.Decimal).Value = pedInfo.MotoristDirTravelCd;
                        cmd.Parameters.Add("motoristManeuverCd", OracleDbType.Decimal).Value = pedInfo.MotoristManeuverCd;
                        cmd.Parameters.Add("nonRoadwayLocationCd", OracleDbType.Decimal).Value = pedInfo.NonRoadwayLocationCd;
                        cmd.Parameters.Add("otherPedActionCd", OracleDbType.Decimal).Value = pedInfo.OtherPedActionCd;
                        cmd.Parameters.Add("pedestrianDirTravelCd", OracleDbType.Decimal).Value = pedInfo.PedestrianDirTravelCd;
                        cmd.Parameters.Add("pedestrianFailedToYieldCd", OracleDbType.Decimal).Value = pedInfo.PedestrianFailedToYieldCd;
                        cmd.Parameters.Add("pedestrianInRoadwayCd", OracleDbType.Decimal).Value = pedInfo.PedestrianInRoadwayCd;
                        cmd.Parameters.Add("pedestrianMovementCd", OracleDbType.Decimal).Value = pedInfo.PedestrianMovementCd;
                        cmd.Parameters.Add("pedestrianPositionCd", OracleDbType.Decimal).Value = pedInfo.PedestrianPositionCd;
                        cmd.Parameters.Add("rightTurnOnRedCd", OracleDbType.Decimal).Value = pedInfo.RightTurnOnRedCd;
                        cmd.Parameters.Add("turnMergeCd", OracleDbType.Decimal).Value = pedInfo.TurnMergeCd;
                        cmd.Parameters.Add("typicalPedActionCd", OracleDbType.Decimal).Value = pedInfo.TypicalPedActionCd;
                        cmd.Parameters.Add("unusualCircumstancesCd", OracleDbType.Decimal).Value = pedInfo.UnusualCircumstancesCd;
                        cmd.Parameters.Add("unusualPedActionCd", OracleDbType.Decimal).Value = pedInfo.UnusualPedActionCd;
                        cmd.Parameters.Add("unusualVehicleTypeOrActionCd", OracleDbType.Decimal).Value = pedInfo.UnusualVehicleTypeOrActionCd;
                        cmd.Parameters.Add("waitingToCrossCd", OracleDbType.Decimal).Value = pedInfo.WaitingToCrossCd;
                        cmd.Parameters.Add("walkingAlongRoadwayCd", OracleDbType.Decimal).Value = pedInfo.WalkingAlongRoadwayCd;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
            }
        }

        public void Update(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {
            var cmdText = @"UPDATE pbcat_ped SET
                backing_veh_cd = :backingVehicleCd,
                crash_location_cd = :crashLocationCd,
                crossing_driveway_cd = :crossingDrivewayCd,
                crossing_roadway_cd = :crossingRoadwayCd,
                failure_to_yield_cd = :failureToYieldCd,
                leg_of_intrsect_cd = :legOfIntrsectCd,
                motorist_dir_travel_cd = :motoristDirTravelCd,
                motorist_maneuver_cd = :motoristManeuverCd,
                non_roadway_loc_cd = :nonRoadwayLocationCd,
                other_ped_action_cd = :otherPedActionCd,
                ped_dir_travel_cd = :pedestrianDirTravelCd,
                ped_failure_to_yield_cd = :pedestrianFailedToYieldCd,
                ped_in_roadway_cd = :pedestrianInRoadwayCd,
                ped_movement_cd = :pedestrianMovementCd,
                ped_position_cd = :pedestrianPositionCd,
                right_turn_on_red_cd = :rightTurnOnRedCd,
                turn_merge_cd = :turnMergeCd,
                typical_ped_action_cd = :typicalPedActionCd,
                unusual_circumstances_cd = :unusualCircumstancesCd,
                unusual_ped_action_cd = :unusualPedActionCd,
                unusual_veh_type_cd = :unusualVehicleTypeOrActionCd,
                waiting_to_cross_cd = :waitingToCrossCd,
                walking_along_roadway_cd = :walkingAlongRoadwayCd
            WHERE hsmv_rpt_nbr = :hsmvRptNbr";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new OracleCommand(cmdText, conn))
                    {
                        cmd.Parameters.Add("backingVehicleCd", OracleDbType.Decimal).Value = pedInfo.BackingVehicleCd;
                        cmd.Parameters.Add("crashLocationCd", OracleDbType.Decimal).Value = pedInfo.CrashLocationCd;
                        cmd.Parameters.Add("crossingDrivewayCd", OracleDbType.Decimal).Value = pedInfo.CrossingDrivewayCd;
                        cmd.Parameters.Add("crossingRoadwayCd", OracleDbType.Decimal).Value = pedInfo.CrossingRoadwayCd;
                        cmd.Parameters.Add("failureToYieldCd", OracleDbType.Decimal).Value = pedInfo.FailureToYieldCd;
                        cmd.Parameters.Add("legOfIntrsectCd", OracleDbType.Decimal).Value = pedInfo.LegOfIntrsectCd;
                        cmd.Parameters.Add("motoristDirTravelCd", OracleDbType.Decimal).Value = pedInfo.MotoristDirTravelCd;
                        cmd.Parameters.Add("motoristManeuverCd", OracleDbType.Decimal).Value = pedInfo.MotoristManeuverCd;
                        cmd.Parameters.Add("nonRoadwayLocationCd", OracleDbType.Decimal).Value = pedInfo.NonRoadwayLocationCd;
                        cmd.Parameters.Add("otherPedActionCd", OracleDbType.Decimal).Value = pedInfo.OtherPedActionCd;
                        cmd.Parameters.Add("pedestrianDirTravelCd", OracleDbType.Decimal).Value = pedInfo.PedestrianDirTravelCd;
                        cmd.Parameters.Add("pedestrianFailedToYieldCd", OracleDbType.Decimal).Value = pedInfo.PedestrianFailedToYieldCd;
                        cmd.Parameters.Add("pedestrianInRoadwayCd", OracleDbType.Decimal).Value = pedInfo.PedestrianInRoadwayCd;
                        cmd.Parameters.Add("pedestrianMovementCd", OracleDbType.Decimal).Value = pedInfo.PedestrianMovementCd;
                        cmd.Parameters.Add("pedestrianPositionCd", OracleDbType.Decimal).Value = pedInfo.PedestrianPositionCd;
                        cmd.Parameters.Add("rightTurnOnRedCd", OracleDbType.Decimal).Value = pedInfo.RightTurnOnRedCd;
                        cmd.Parameters.Add("turnMergeCd", OracleDbType.Decimal).Value = pedInfo.TurnMergeCd;
                        cmd.Parameters.Add("typicalPedActionCd", OracleDbType.Decimal).Value = pedInfo.TypicalPedActionCd;
                        cmd.Parameters.Add("unusualCircumstancesCd", OracleDbType.Decimal).Value = pedInfo.UnusualCircumstancesCd;
                        cmd.Parameters.Add("unusualPedActionCd", OracleDbType.Decimal).Value = pedInfo.UnusualPedActionCd;
                        cmd.Parameters.Add("unusualVehicleTypeOrActionCd", OracleDbType.Decimal).Value = pedInfo.UnusualVehicleTypeOrActionCd;
                        cmd.Parameters.Add("waitingToCrossCd", OracleDbType.Decimal).Value = pedInfo.WaitingToCrossCd;
                        cmd.Parameters.Add("walkingAlongRoadwayCd", OracleDbType.Decimal).Value = pedInfo.WalkingAlongRoadwayCd;
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
            }
        }

        public void Remove(int hsmvRptNbr)
        {
            var cmdText = "DELETE FROM pbcat_ped WHERE hsmv_rpt_nbr = :hsmvRptNbr";
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new OracleCommand(cmdText, conn))
                    {
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
            }
        }

        public CrashTypePedestrian GetCrashType(PBCATPedestrianInfo pedInfo)
        {
            var engine = PBCATFactory.GetCrashTypingEngine(
                CrashParticipantType.Pedestrian,
                pedInfo.EnableGroupTyping,
                pedInfo.EnablePedestrianLocationOption);
            var crashType = (CrashTypePedestrian)engine.GetCrashType(pedInfo);
            return crashType;
        }

        public bool CrashReportExists(int hsmvRptNbr)
        {
            var cmdText = "SELECT COUNT(*) FROM fact_crash_evt WHERE hsmv_rpt_nbr = :hsmvRptNbr";
            int ct;
            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(cmdText, conn))
                {
                    cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                    ct = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return ct > 0;
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
