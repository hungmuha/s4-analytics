using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Lib.PBCAT;

namespace S4Analytics.Models
{
    public partial class PbcatRepository
    {

        public PBCATBicyclistInfo FindBicyclist(int hsmvRptNbr)
        {
            PBCATBicyclistInfo info = null;

            var cmdText = @"SELECT bicyclist_dir_cd,
                bicyclist_failed_to_clear_cd, bicyclist_overtaking_cd, bicyclist_position_cd,
                bicyclist_ride_out_cd, bicyclist_turned_merged_cd, crash_location_cd,
                crossing_paths_intrsect_cd, crossing_paths_non_intrsect_cd, head_on_crash_cd,
                initial_approach_paths_cd, intentional_crash_cd, intrsect_circumstances_cd,
                loss_of_control_cd, motorist_drive_out_cd, motorist_overtaking_cd,
                motorist_turned_merged_cd, parallel_paths_cd, right_turn_on_red_cd, turning_error_cd,
                type_traffic_control_cd, unusual_circumstances_cd, group_typing_enabled
                FROM pbcat_bike WHERE hsmv_rpt_nbr = :hsmvRptNbr";
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
                    info = new PBCATBicyclistInfo();
                    info.BicyclistDirCd = ConvertToEnum<BicyclistDirection>(dr.Field<decimal>("bicyclist_dir_cd"));
                    info.BicyclistFailedToClearCd = ConvertToEnum<BicyclistFailedToClear>(dr.Field<decimal>("bicyclist_failed_to_clear_cd"));
                    info.BicyclistOvertakingCd = ConvertToEnum<BicyclistOvertaking>(dr.Field<decimal>("bicyclist_overtaking_cd"));
                    info.BicyclistPositionCd = ConvertToEnum<BicyclistPosition>(dr.Field<decimal>("bicyclist_position_cd"));
                    info.BicyclistRideOutCd = ConvertToEnum<RideOutDriveOutFromLocation>(dr.Field<decimal>("bicyclist_ride_out_cd"));
                    info.BicyclistTurnedMergedCd = ConvertToEnum<BicyclistTurnedOrMerged>(dr.Field<decimal>("bicyclist_turned_merged_cd"));
                    info.CrashLocationCd = ConvertToEnum<CrashLocationBike>(dr.Field<decimal>("crash_location_cd"));
                    info.CrossingPathsIntrsectCd = ConvertToEnum<CrossingPathsIntrsect>(dr.Field<decimal>("crossing_paths_intrsect_cd"));
                    info.CrossingPathsNonIntrsectCd = ConvertToEnum<CrossingPathsNonIntrsect>(dr.Field<decimal>("crossing_paths_non_intrsect_cd"));
                    info.HeadOnCrashCd = ConvertToEnum<HeadOnFault>(dr.Field<decimal>("head_on_crash_cd"));
                    info.InitialApproachPathsCd = ConvertToEnum<InitialApproachPaths>(dr.Field<decimal>("initial_approach_paths_cd"));
                    info.IntentionalCrashCd = ConvertToEnum<IntentionalCrashCausedBy>(dr.Field<decimal>("intentional_crash_cd"));
                    info.IntrsectCircumstancesCd = ConvertToEnum<IntrsectCircumstances>(dr.Field<decimal>("intrsect_circumstances_cd"));
                    info.LossOfControlCd = ConvertToEnum<LossOfControlReason>(dr.Field<decimal>("loss_of_control_cd"));
                    info.MotoristDriveOutCd = ConvertToEnum<RideOutDriveOutFromLocation>(dr.Field<decimal>("motorist_drive_out_cd"));
                    info.MotoristOvertakingCd = ConvertToEnum<MotoristOvertaking>(dr.Field<decimal>("motorist_overtaking_cd"));
                    info.MotoristTurnedMergedCd = ConvertToEnum<MotoristTurnedOrMerged>(dr.Field<decimal>("motorist_turned_merged_cd"));
                    info.ParallelPathsCd = ConvertToEnum<ParallelPathsCircumstances>(dr.Field<decimal>("parallel_paths_cd"));
                    info.RightTurnOnRedCd = ConvertToEnum<TurningRightOnRed>(dr.Field<decimal>("right_turn_on_red_cd"));
                    info.TurningErrorCd = ConvertToEnum<TurningError>(dr.Field<decimal>("turning_error_cd"));
                    info.TypeTrafficControlCd = ConvertToEnum<TypeOfTrafficControl>(dr.Field<decimal>("type_traffic_control_cd"));
                    info.UnusualCircumstancesCd = ConvertToEnum<UnusualCircumstancesBicyclist>(dr.Field<decimal>("unusual_circumstances_cd"));
                    info.EnableGroupTyping = dr.Field<decimal>("group_typing_enabled") != 0;
                }
            }

            return info;
        }

        public void Add(int hsmvRptNbr, PBCATBicyclistInfo bikeInfo, CrashTypeBicyclist crashType)
        {
            // todo: user real user id
            var lastUpdateUserId = "pbcat";

            // convert some values to int
            int crashGroupNbr = 0;
            int crashTypeNbr;
            int crashGroupExpanded;
            int crashTypeExpanded;
            int.TryParse(crashType.CrashGroupNbr, out crashGroupNbr);
            int.TryParse(crashType.CrashTypeNbr, out crashTypeNbr);
            int.TryParse(crashType.CrashGroupExpanded, out crashGroupExpanded);
            int.TryParse(crashType.CrashTypeExpanded, out crashTypeExpanded);
            int enableGroupTyping = bikeInfo.EnableGroupTyping ? 1 : 0;

            var cmdText = @"INSERT INTO pbcat_bike (
                hsmv_rpt_nbr, bicyclist_dir_cd,
                bicyclist_failed_to_clear_cd, bicyclist_overtaking_cd, bicyclist_position_cd,
                bicyclist_ride_out_cd, bicyclist_turned_merged_cd, crash_location_cd,
                crossing_paths_intrsect_cd, crossing_paths_non_intrsect_cd, head_on_crash_cd,
                initial_approach_paths_cd, intentional_crash_cd, intrsect_circumstances_cd,
                loss_of_control_cd, motorist_drive_out_cd, motorist_overtaking_cd,
                motorist_turned_merged_cd, parallel_paths_cd, right_turn_on_red_cd, turning_error_cd,
                type_traffic_control_cd, unusual_circumstances_cd, crash_type_nbr, crash_grp_nbr,
                crash_type_expanded, crash_grp_expanded, group_typing_enabled, last_updt_user_id
            ) VALUES (
                :hsmvRptNbr, :bicyclistDirCd, :bicyclistFailedToClearCd,
                :bicyclistOvertakingCd, :bicyclistPositionCd, :bicyclistRideOutCd,
                :bicyclistTurnedMergedCd, :crashLocationCd, :crossingPathsIntrsectCd,
                :crossingPathsNonIntrsectCd, :headOnCrashCd, :initialApproachPathsCd,
                :intentionalCrashCd, :intrsectCircumstancesCd, :lossOfControlCd,
                :motoristDriveOutCd, :motoristOvertakingCd, :motoristTurnedMergedCd,
                :parallelPathsCd, :rightTurnOnRedCd, :turningErrorCd,
                :typeTrafficControlCd, :unusualCircumstancesCd, :crashTypeNbr,
                :crashGroupNbr, :crashTypeExpanded, :crashGroupExpanded,
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
                        cmd.Parameters.Add("bicyclistDirCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistDirCd;
                        cmd.Parameters.Add("bicyclistFailedToClearCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistFailedToClearCd;
                        cmd.Parameters.Add("bicyclistOvertakingCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistOvertakingCd;
                        cmd.Parameters.Add("bicyclistPositionCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistPositionCd;
                        cmd.Parameters.Add("bicyclistRideOutCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistRideOutCd;
                        cmd.Parameters.Add("bicyclistTurnedMergedCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistTurnedMergedCd;
                        cmd.Parameters.Add("crashLocationCd", OracleDbType.Decimal).Value = bikeInfo.CrashLocationCd;
                        cmd.Parameters.Add("crossingPathsIntrsectCd", OracleDbType.Decimal).Value = bikeInfo.CrossingPathsIntrsectCd;
                        cmd.Parameters.Add("crossingPathsNonIntrsectCd", OracleDbType.Decimal).Value = bikeInfo.CrossingPathsNonIntrsectCd;
                        cmd.Parameters.Add("headOnCrashCd", OracleDbType.Decimal).Value = bikeInfo.HeadOnCrashCd;
                        cmd.Parameters.Add("initialApproachPathsCd", OracleDbType.Decimal).Value = bikeInfo.InitialApproachPathsCd;
                        cmd.Parameters.Add("intentionalCrashCd", OracleDbType.Decimal).Value = bikeInfo.IntentionalCrashCd;
                        cmd.Parameters.Add("intrsectCircumstancesCd", OracleDbType.Decimal).Value = bikeInfo.IntrsectCircumstancesCd;
                        cmd.Parameters.Add("lossOfControlCd", OracleDbType.Decimal).Value = bikeInfo.LossOfControlCd;
                        cmd.Parameters.Add("motoristDriveOutCd", OracleDbType.Decimal).Value = bikeInfo.MotoristDriveOutCd;
                        cmd.Parameters.Add("motoristOvertakingCd", OracleDbType.Decimal).Value = bikeInfo.MotoristOvertakingCd;
                        cmd.Parameters.Add("motoristTurnedMergedCd", OracleDbType.Decimal).Value = bikeInfo.MotoristTurnedMergedCd;
                        cmd.Parameters.Add("parallelPathsCd", OracleDbType.Decimal).Value = bikeInfo.ParallelPathsCd;
                        cmd.Parameters.Add("rightTurnOnRedCd", OracleDbType.Decimal).Value = bikeInfo.RightTurnOnRedCd;
                        cmd.Parameters.Add("turningErrorCd", OracleDbType.Decimal).Value = bikeInfo.TurningErrorCd;
                        cmd.Parameters.Add("typeTrafficControlCd", OracleDbType.Decimal).Value = bikeInfo.TypeTrafficControlCd;
                        cmd.Parameters.Add("unusualCircumstancesCd", OracleDbType.Decimal).Value = bikeInfo.UnusualCircumstancesCd;
                        cmd.Parameters.Add("crashTypeNbr", OracleDbType.Decimal).Value = crashTypeNbr;
                        cmd.Parameters.Add("crashGroupNbr", OracleDbType.Decimal).Value = crashGroupNbr;
                        cmd.Parameters.Add("crashTypeExpanded", OracleDbType.Decimal).Value = crashTypeExpanded;
                        cmd.Parameters.Add("crashGroupExpanded", OracleDbType.Decimal).Value = crashGroupExpanded;
                        cmd.Parameters.Add("enableGroupTyping", OracleDbType.Decimal).Value = enableGroupTyping;
                        cmd.Parameters.Add("lastUpdateUserId", OracleDbType.Varchar2).Value = lastUpdateUserId;
                        cmd.ExecuteNonQuery();
                    }
                    UpdateBicyclistCrashReport(conn, hsmvRptNbr);
                    trans.Commit();
                }
            }
        }

        public void Update(int hsmvRptNbr, PBCATBicyclistInfo bikeInfo, CrashTypeBicyclist crashType)
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
            int enableGroupTyping = bikeInfo.EnableGroupTyping ? 1 : 0;

            var cmdText = @"UPDATE pbcat_bike SET
                bicyclist_dir_cd = :bicyclistDirCd,
                bicyclist_failed_to_clear_cd = :bicyclistFailedToClearCd,
                bicyclist_overtaking_cd = :bicyclistOvertakingCd,
                bicyclist_position_cd = :bicyclistPositionCd,
                bicyclist_ride_out_cd = :bicyclistRideOutCd,
                bicyclist_turned_merged_cd = :bicyclistTurnedMergedCd,
                crash_location_cd = :crashLocationCd,
                crossing_paths_intrsect_cd = :crossingPathsIntrsectCd,
                crossing_paths_non_intrsect_cd = :crossingPathsNonIntrsectCd,
                head_on_crash_cd = :headOnCrashCd,
                initial_approach_paths_cd = :initialApproachPathsCd,
                intentional_crash_cd = :intentionalCrashCd,
                intrsect_circumstances_cd = :intrsectCircumstancesCd,
                loss_of_control_cd = :lossOfControlCd,
                motorist_drive_out_cd = :motoristDriveOutCd,
                motorist_overtaking_cd = :motoristOvertakingCd,
                motorist_turned_merged_cd = :motoristTurnedMergedCd,
                parallel_paths_cd = :parallelPathsCd,
                right_turn_on_red_cd = :rightTurnOnRedCd,
                turning_error_cd = :turningErrorCd,
                type_traffic_control_cd = :typeTrafficControlCd,
                unusual_circumstances_cd = :unusualCircumstancesCd,
                crash_type_nbr = :crashTypeNbr,
                crash_grp_nbr = :crashGroupNbr,
                crash_type_expanded = :crashTypeExpanded,
                crash_grp_expanded = :crashGroupExpanded,
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
                        cmd.Parameters.Add("bicyclistDirCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistDirCd;
                        cmd.Parameters.Add("bicyclistFailedToClearCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistFailedToClearCd;
                        cmd.Parameters.Add("bicyclistOvertakingCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistOvertakingCd;
                        cmd.Parameters.Add("bicyclistPositionCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistPositionCd;
                        cmd.Parameters.Add("bicyclistRideOutCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistRideOutCd;
                        cmd.Parameters.Add("bicyclistTurnedMergedCd", OracleDbType.Decimal).Value = bikeInfo.BicyclistTurnedMergedCd;
                        cmd.Parameters.Add("crashLocationCd", OracleDbType.Decimal).Value = bikeInfo.CrashLocationCd;
                        cmd.Parameters.Add("crossingPathsIntrsectCd", OracleDbType.Decimal).Value = bikeInfo.CrossingPathsIntrsectCd;
                        cmd.Parameters.Add("crossingPathsNonIntrsectCd", OracleDbType.Decimal).Value = bikeInfo.CrossingPathsNonIntrsectCd;
                        cmd.Parameters.Add("headOnCrashCd", OracleDbType.Decimal).Value = bikeInfo.HeadOnCrashCd;
                        cmd.Parameters.Add("initialApproachPathsCd", OracleDbType.Decimal).Value = bikeInfo.InitialApproachPathsCd;
                        cmd.Parameters.Add("intentionalCrashCd", OracleDbType.Decimal).Value = bikeInfo.IntentionalCrashCd;
                        cmd.Parameters.Add("intrsectCircumstancesCd", OracleDbType.Decimal).Value = bikeInfo.IntrsectCircumstancesCd;
                        cmd.Parameters.Add("lossOfControlCd", OracleDbType.Decimal).Value = bikeInfo.LossOfControlCd;
                        cmd.Parameters.Add("motoristDriveOutCd", OracleDbType.Decimal).Value = bikeInfo.MotoristDriveOutCd;
                        cmd.Parameters.Add("motoristOvertakingCd", OracleDbType.Decimal).Value = bikeInfo.MotoristOvertakingCd;
                        cmd.Parameters.Add("motoristTurnedMergedCd", OracleDbType.Decimal).Value = bikeInfo.MotoristTurnedMergedCd;
                        cmd.Parameters.Add("parallelPathsCd", OracleDbType.Decimal).Value = bikeInfo.ParallelPathsCd;
                        cmd.Parameters.Add("rightTurnOnRedCd", OracleDbType.Decimal).Value = bikeInfo.RightTurnOnRedCd;
                        cmd.Parameters.Add("turningErrorCd", OracleDbType.Decimal).Value = bikeInfo.TurningErrorCd;
                        cmd.Parameters.Add("typeTrafficControlCd", OracleDbType.Decimal).Value = bikeInfo.TypeTrafficControlCd;
                        cmd.Parameters.Add("unusualCircumstancesCd", OracleDbType.Decimal).Value = bikeInfo.UnusualCircumstancesCd;
                        cmd.Parameters.Add("crashTypeNbr", OracleDbType.Decimal).Value = crashTypeNbr;
                        cmd.Parameters.Add("crashGroupNbr", OracleDbType.Decimal).Value = crashGroupNbr;
                        cmd.Parameters.Add("crashTypeExpanded", OracleDbType.Decimal).Value = crashTypeExpanded;
                        cmd.Parameters.Add("crashGroupExpanded", OracleDbType.Decimal).Value = crashGroupExpanded;
                        cmd.Parameters.Add("enableGroupTyping", OracleDbType.Decimal).Value = enableGroupTyping;
                        cmd.Parameters.Add("lastUpdateUserId", OracleDbType.Varchar2).Value = lastUpdateUserId;
                        cmd.Parameters.Add("hsmvRptNbr", OracleDbType.Decimal).Value = hsmvRptNbr;
                        cmd.ExecuteNonQuery();
                    }
                    UpdateBicyclistCrashReport(conn, hsmvRptNbr);
                    trans.Commit();
                }
            }
        }

        public CrashTypeBicyclist GetCrashType(PBCATBicyclistInfo bikeInfo)
        {
            var engine = PBCATFactory.GetCrashTypingEngine(
                CrashParticipantType.Bicyclist,
                bikeInfo.EnableGroupTyping);
            var crashType = (CrashTypeBicyclist)engine.GetCrashType(bikeInfo);
            return crashType;
        }

        public void RemoveBicyclist(int hsmvRptNbr)
        {
            var cmdText = "DELETE FROM pbcat_bike WHERE hsmv_rpt_nbr = :hsmvRptNbr";
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
                    UpdateBicyclistCrashReport(conn, hsmvRptNbr);
                    trans.Commit();
                }
            }
        }

        private void UpdateBicyclistCrashReport(OracleConnection conn, int hsmvRptNbr)
        {
            var cmdText = @"UPDATE fact_crash_evt c
                SET key_bike_ped_crash_type = (
                  SELECT t.id
                  FROM bike_ped_crash_type t
                  INNER JOIN bike_ped_crash_grp g
                    ON g.id = t.crash_grp_id
                    AND g.bike_or_ped = 'B'
                  INNER JOIN pbcat_bike p
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
