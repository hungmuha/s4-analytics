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
    public class PbcatPedRepository : S4Repository, IPbcatPedRepository
    {
        private string _warehouseConnStr;

        public PbcatPedRepository(IOptions<ServerOptions> serverOptions)
        {
            _warehouseConnStr = serverOptions.Value.WarehouseConnStr;
        }

        public PBCATPedestrianInfo Find(int hsmvRptNbr)
        {
            var cmdText = string.Format(@"SELECT
                backing_veh_cd, crash_location_cd, crossing_driveway_cd, crossing_roadway_cd,
                failure_to_yield_cd, leg_of_intrsect_cd, motorist_dir_travel_cd, motorist_maneuver_cd,
                non_roadway_loc_cd, other_ped_action_cd, ped_dir_travel_cd, ped_failure_to_yield_cd,
                ped_in_roadway_cd, ped_movement_cd, ped_position_cd, right_turn_on_red_cd, turn_merge_cd,
                typical_ped_action_cd, unusual_circumstances_cd, unusual_ped_action_cd, unusual_veh_type_cd,
                waiting_to_cross_cd, walking_along_roadway_cd, ped_loc_option_enabled, group_typing_enabled
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
                info.BackingVehicleCd = ConvertToEnum<BackingVehicleLocation>(dr.Field<decimal>("backing_veh_cd"));
                info.CrashLocationCd = ConvertToEnum<CrashLocationPed>(dr.Field<decimal>("crash_location_cd"));
                info.CrossingDrivewayCd = ConvertToEnum<CrossingDrivewayOrAlleyCircumstances>(dr.Field<decimal>("crossing_driveway_cd"));
                info.CrossingRoadwayCd = ConvertToEnum<CrossingRoadwayCircumstances>(dr.Field<decimal>("crossing_roadway_cd"));
                info.FailureToYieldCd = ConvertToEnum<FailureToYield>(dr.Field<decimal>("failure_to_yield_cd"));
                info.LegOfIntrsectCd = ConvertToEnum<LegOfIntersection>(dr.Field<decimal>("leg_of_intrsect_cd"));
                info.MotoristDirTravelCd = ConvertToEnum<DirectionOfTravel>(dr.Field<decimal>("motorist_dir_travel_cd"));
                info.MotoristManeuverCd = ConvertToEnum<MotoristManeuver>(dr.Field<decimal>("motorist_maneuver_cd"));
                info.NonRoadwayLocationCd = ConvertToEnum<NonRoadwayLocation>(dr.Field<decimal>("non_roadway_loc_cd"));
                info.OtherPedActionCd = ConvertToEnum<OtherPedAction>(dr.Field<decimal>("other_ped_action_cd"));
                info.PedestrianDirTravelCd = ConvertToEnum<DirectionOfTravel>(dr.Field<decimal>("ped_dir_travel_cd"));
                info.PedestrianFailedToYieldCd = ConvertToEnum<PedFailedToYield>(dr.Field<decimal>("ped_failure_to_yield_cd"));
                info.PedestrianInRoadwayCd = ConvertToEnum<PedActionInRoadway>(dr.Field<decimal>("ped_in_roadway_cd"));
                info.PedestrianMovementCd = ConvertToEnum<PedMovementScenario>(dr.Field<decimal>("ped_movement_cd"));
                info.PedestrianPositionCd = ConvertToEnum<PedPosition>(dr.Field<decimal>("ped_position_cd"));
                info.RightTurnOnRedCd = ConvertToEnum<TurningRightOnRed>(dr.Field<decimal>("right_turn_on_red_cd"));
                info.TurnMergeCd = ConvertToEnum<TurnMergeCircumstances>(dr.Field<decimal>("turn_merge_cd"));
                info.TypicalPedActionCd = ConvertToEnum<TypicalPedAction>(dr.Field<decimal>("typical_ped_action_cd"));
                info.UnusualCircumstancesCd = ConvertToEnum<UnusualCircumstancesPed>(dr.Field<decimal>("unusual_circumstances_cd"));
                info.UnusualPedActionCd = ConvertToEnum<UnusualPedAction>(dr.Field<decimal>("unusual_ped_action_cd"));
                info.UnusualVehicleTypeOrActionCd = ConvertToEnum<UnusualVehicleTypeOrAction>(dr.Field<decimal>("unusual_veh_type_cd"));
                info.WaitingToCrossCd = ConvertToEnum<WaitingToCrossVehicleMovement>(dr.Field<decimal>("waiting_to_cross_cd"));
                info.WalkingAlongRoadwayCd = ConvertToEnum<WalkingAlongRoadwayCircumstances>(dr.Field<decimal>("walking_along_roadway_cd"));
                info.EnablePedestrianLocationOption = dr.Field<decimal>("ped_loc_option_enabled") != 0;
                info.EnableGroupTyping = dr.Field<decimal>("group_typing_enabled") != 0;
            }
            return info;
        }

        public void Add(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {
            // todo: user real user id
            var lastUpdateUserId = "pbcat";

            // convert some values to int
            int crashGroupNbr;
            int crashTypeNbr;
            int crashGroupExpanded;
            int crashTypeExpanded;
            int.TryParse(crashType.CrashGroupNbr, out crashGroupNbr);
            int.TryParse(crashType.CrashTypeNbr, out crashTypeNbr);
            int.TryParse(crashType.CrashGroupExpanded, out crashGroupExpanded);
            int.TryParse(crashType.CrashTypeExpanded, out crashTypeExpanded);
            int enableGroupTyping = pedInfo.EnableGroupTyping ? 1 : 0;
            int enablePedestrianLocationOption = pedInfo.EnablePedestrianLocationOption ? 1 : 0;

            var cmdText = @"INSERT INTO pbcat_ped (
                hsmv_rpt_nbr, backing_veh_cd, crash_location_cd, crossing_driveway_cd, crossing_roadway_cd,
                failure_to_yield_cd, leg_of_intrsect_cd, motorist_dir_travel_cd, motorist_maneuver_cd,
                non_roadway_loc_cd, other_ped_action_cd, ped_dir_travel_cd, ped_failure_to_yield_cd,
                ped_in_roadway_cd, ped_movement_cd, ped_position_cd, right_turn_on_red_cd, turn_merge_cd,
                typical_ped_action_cd, unusual_circumstances_cd, unusual_ped_action_cd, unusual_veh_type_cd,
                waiting_to_cross_cd, walking_along_roadway_cd, crash_type_nbr, crash_grp_nbr,
                crash_type_expanded, crash_grp_expanded, ped_loc_option_enabled, group_typing_enabled,
                last_updt_user_id
            ) VALUES (
                :hsmvRptNbr, :backingVehicleCd, :crashLocationCd, :crossingDrivewayCd, :crossingRoadwayCd,
                :failureToYieldCd, :legOfIntrsectCd, :motoristDirTravelCd, :motoristManeuverCd,
                :nonRoadwayLocationCd, :otherPedActionCd, :pedestrianDirTravelCd, :pedestrianFailedToYieldCd,
                :pedestrianInRoadwayCd, :pedestrianMovementCd, :pedestrianPositionCd, :rightTurnOnRedCd,
                :turnMergeCd, :typicalPedActionCd, :unusualCircumstancesCd, :unusualPedActionCd,
                :unusualVehicleTypeOrActionCd, :waitingToCrossCd, :walkingAlongRoadwayCd, :crashTypeNbr,
                :crashGroupNbr, :crashTypeExpanded, :crashGroupExpanded, :enablePedestrianLocationOption,
                :enableGroupTyping, :lastUpdateUserId
            )";

            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
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
                        cmd.Parameters.Add("crashTypeNbr", OracleDbType.Decimal).Value = crashTypeNbr;
                        cmd.Parameters.Add("crashGroupNbr", OracleDbType.Decimal).Value = crashGroupNbr;
                        cmd.Parameters.Add("crashTypeExpanded", OracleDbType.Decimal).Value = crashTypeExpanded;
                        cmd.Parameters.Add("crashGroupExpanded", OracleDbType.Decimal).Value = crashGroupExpanded;
                        cmd.Parameters.Add("enablePedestrianLocationOption", OracleDbType.Decimal).Value = enablePedestrianLocationOption;
                        cmd.Parameters.Add("enableGroupTyping", OracleDbType.Decimal).Value = enableGroupTyping;
                        cmd.Parameters.Add("lastUpdateUserId", OracleDbType.Varchar2).Value = lastUpdateUserId;
                        cmd.ExecuteNonQuery();
                    }
                    UpdateCrashReport(conn, hsmvRptNbr);
                    trans.Commit();
                }
            }
        }

        public void Update(int hsmvRptNbr, PBCATPedestrianInfo pedInfo, CrashTypePedestrian crashType)
        {
            // todo: user real user id
            var lastUpdateUserId = "pbcat";

            // convert some values to int
            int crashGroupNbr;
            int crashTypeNbr;
            int crashGroupExpanded;
            int crashTypeExpanded;
            int.TryParse(crashType.CrashGroupNbr, out crashGroupNbr);
            int.TryParse(crashType.CrashTypeNbr, out crashTypeNbr);
            int.TryParse(crashType.CrashGroupExpanded, out crashGroupExpanded);
            int.TryParse(crashType.CrashTypeExpanded, out crashTypeExpanded);
            int enableGroupTyping = pedInfo.EnableGroupTyping ? 1 : 0;
            int enablePedestrianLocationOption = pedInfo.EnablePedestrianLocationOption ? 1 : 0;

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
                walking_along_roadway_cd = :walkingAlongRoadwayCd,
                crash_type_nbr = :crashTypeNbr,
                crash_grp_nbr = :crashGroupNbr,
                crash_type_expanded = :crashTypeExpanded,
                crash_grp_expanded = :crashGroupExpanded,
                ped_loc_option_enabled = :enablePedestrianLocationOption,
                group_typing_enabled = :enableGroupTyping,
                last_updt_user_id = :lastUpdateUserId
            WHERE hsmv_rpt_nbr = :hsmvRptNbr";

            using (var conn = new OracleConnection(_warehouseConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
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
                        cmd.Parameters.Add("crashTypeNbr", OracleDbType.Decimal).Value = crashTypeNbr;
                        cmd.Parameters.Add("crashGroupNbr", OracleDbType.Decimal).Value = crashGroupNbr;
                        cmd.Parameters.Add("crashTypeExpanded", OracleDbType.Decimal).Value = crashTypeExpanded;
                        cmd.Parameters.Add("crashGroupExpanded", OracleDbType.Decimal).Value = crashGroupExpanded;
                        cmd.Parameters.Add("enablePedestrianLocationOption", OracleDbType.Decimal).Value = enablePedestrianLocationOption;
                        cmd.Parameters.Add("enableGroupTyping", OracleDbType.Decimal).Value = enableGroupTyping;
                        cmd.Parameters.Add("lastUpdateUserId", OracleDbType.Varchar2).Value = lastUpdateUserId;
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.ExecuteNonQuery();
                    }
                    UpdateCrashReport(conn, hsmvRptNbr);
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
                    using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
                    {
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.ExecuteNonQuery();
                    }
                    UpdateCrashReport(conn, hsmvRptNbr);
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
                using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
                {
                    cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                    ct = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return ct > 0;
        }

        private void UpdateCrashReport(OracleConnection conn, int hsmvRptNbr)
        {
            var cmdText = @"UPDATE fact_crash_evt c
                SET key_bike_ped_crash_type = (
                  SELECT t.id
                  FROM bike_ped_crash_type t
                  INNER JOIN bike_ped_crash_grp g
                    ON g.id = t.crash_grp_id
                    AND g.bike_or_ped = 'P'
                  INNER JOIN pbcat_ped p
                    ON p.crash_type_nbr = t.crash_type_nbr
                    AND p.crash_grp_nbr = g.crash_grp_nbr
                  WHERE p.hsmv_rpt_nbr = c.hsmv_rpt_nbr
                )
                WHERE c.hsmv_rpt_Nbr = :hsmvRptNbr";
            using (var cmd = new OracleCommand(cmdText, conn) { BindByName = true })
            {
                cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
