INSERT INTO bike_ped_crash_grp
SELECT * FROM bike_ped_crash_grp@lime_s4_warehouse;

INSERT INTO bike_ped_crash_type
SELECT * FROM bike_ped_crash_type@lime_s4_warehouse;

INSERT INTO cnty_city
SELECT * FROM cnty_city@lime_s4_warehouse;

INSERT INTO dim_agncy
SELECT * FROM dim_agncy@lime_s4_warehouse;

INSERT INTO dim_crash_attrs
SELECT * FROM dim_crash_attrs@lime_s4_warehouse;

INSERT INTO dim_date
SELECT * FROM dim_date@lime_s4_warehouse;

INSERT INTO dim_driver_attrs
SELECT * FROM dim_driver_attrs@lime_s4_warehouse;

INSERT INTO dim_geography
SELECT * FROM dim_geography@lime_s4_warehouse;

INSERT INTO dim_harmful_evt
SELECT * FROM dim_harmful_evt@lime_s4_warehouse;

INSERT INTO dim_nm_attrs
SELECT * FROM dim_nm_attrs@lime_s4_warehouse;

INSERT INTO dim_pass_attrs
SELECT * FROM dim_pass_attrs@lime_s4_warehouse;

INSERT INTO dim_veh_attrs
SELECT * FROM dim_veh_attrs@lime_s4_warehouse;

INSERT INTO dim_violation
SELECT * FROM dim_violation@lime_s4_warehouse;

/* for some weird reason we get this error on OTH_COMMENTS_TX if we include it:
   SQL Error: ORA-12899: value too large for column "S4_WAREHOUSE_NG"."CITATION"."OTH_COMMENTS_TX" (actual: 91, maximum: 90) */
INSERT INTO citation (CITATION_NBR, CHECK_DIGIT, KEY_DATE, KEY_GEOGRAPHY, KEY_AGNCY, KEY_DRIVER_AGE_RNG, CNTY_NBR, JURISDICTION_NBR, CITY_NM, AGNCY_TYPE_CD, AGNCY_NM, OFFENSE_DT, DRIVER_ADDR_DIFF_THAN_DL, DRIVER_ADDR_CITY_NM, DRIVER_ADDR_STATE_CD, DRIVER_ADDR_ZIP_CD, DRIVER_AGE_NBR, DRIVER_RACE_CD, DRIVER_GENDER_CD, DRIVER_HEIGHT_TX, DL_STATE_CD, DL_CLASS_CD, DL_EXPIR_YR, COMM_VEH_CD, VEH_YR, VEH_MAKE_TX, VEH_STYLE_TX, VEH_COLOR_TX, HAZMAT_CD, VEH_STATE_CD, VEH_TAG_EXPIR_YR, COMPANION_CITATION_CD, VIOLATION_LOC_TX, OFFSET_FEET_MEAS, OFFSET_MILES_MEAS, OFFSET_DIR_CD, OFFSET_FROM_NODE_ID, ACTUAL_SPEED_MEAS, POSTED_SPEED_MEAS, HWY_4LANE_CD, HWY_INTRSTATE_CD, VIOLATION_CARELESS_CD, VIOLATION_DEVICE_CD, VIOLATION_ROW_CD, VIOLATION_LANE_CD, VIOLATION_PASSING_CD, VIOLATION_CHILD_RESTRAINT_CD, VIOLATION_DUI_CD, DRIVER_BAL_MEAS, VIOLATION_SEATBELT_CD, VIOLATION_EQUIP_CD, VIOLATION_TAG_LESS_CD, VIOLATION_TAG_MORE_CD, VIOLATION_INS_CD, VIOLATION_EXPIR_DL_CD, VIOLATION_EXPIR_DL_MORE_CD, VIOLATION_NO_VALID_DL_CD, VIOLATION_SUSP_DL_CD, VIOLATION_CD, FL_DL_EDIT_OVERRIDE_CD, STATE_STATUTE_CD, SECT_NBR, SUB_SECT_NBR, CRASH_CD, PROP_DMG_CD, PROP_DMG_AMT, INJ_CD, SERIOUS_INJ_CD, FATAL_INJ_CD, METHOD_OF_ARREST_CD, CRIMINAL_APPEAR_REQD_CD, INFRACTION_APPEAR_REQD_CD, INFRACTION_NO_APPEAR_REQD_CD, COURT_DT, COURT_NM, COURT_ADDR_TX, COURT_CITY_NM, COURT_STATE_CD, COURT_ZIP_CD, ARREST_DELIVERED_TO_TX, ARREST_DELIVERED_DT, OFCR_RANK_TX, TROOPER_UNIT_TX, BAL_008_OR_ABOVE_CD, DUI_REFUSE_CD, DUI_LIC_SURRENDERED_CD, DUI_LIC_RSN_TX, DUI_ELIGIBLE_CD, DUI_ELIGIBLE_RSN_TX, DUI_BAR_OFC_TX, STATUS_CD, AGGRESSIVE_DRIVER_CD, CRIMINAL_CD, FINE_AMT, ISSUE_ARREST_DT, OFCR_DLVRY_VERIF_CD, DUE_DT, MOTORCYCLE_CD, VEH_16_PASS_CD, OFCR_RE_EXAM_CD, DUI_PASS_UNDER_18_CD, E_CITATION_CD, NM_CHG_CD, COMM_DL_CD, GPS_LAT, GPS_LNG, NAVTEQ_PT_X, NAVTEQ_PT_Y, VIOLATION_SIG_RED_LIGHT_CD, VIOLATION_WORKERS_PRESENT_CD, VIOLATION_HANDHELD_CD, VIOLATION_SCH_ZN_CD, AGENCY_ID, PERM_REG_CD, COMPLIANCE_DT, SPEED_MEAS_DEVICE_ID, DL_SEIZE_CD, BUSINESS_CD, SOURCE_FORMAT_CD, ADDR_USED_CD, KEY_VIOLATION)
SELECT CITATION_NBR, CHECK_DIGIT, KEY_DATE, KEY_GEOGRAPHY, KEY_AGNCY, KEY_DRIVER_AGE_RNG, CNTY_NBR, JURISDICTION_NBR, CITY_NM, AGNCY_TYPE_CD, AGNCY_NM, OFFENSE_DT, DRIVER_ADDR_DIFF_THAN_DL, DRIVER_ADDR_CITY_NM, DRIVER_ADDR_STATE_CD, DRIVER_ADDR_ZIP_CD, DRIVER_AGE_NBR, DRIVER_RACE_CD, DRIVER_GENDER_CD, DRIVER_HEIGHT_TX, DL_STATE_CD, DL_CLASS_CD, DL_EXPIR_YR, COMM_VEH_CD, VEH_YR, VEH_MAKE_TX, VEH_STYLE_TX, VEH_COLOR_TX, HAZMAT_CD, VEH_STATE_CD, VEH_TAG_EXPIR_YR, COMPANION_CITATION_CD, VIOLATION_LOC_TX, OFFSET_FEET_MEAS, OFFSET_MILES_MEAS, OFFSET_DIR_CD, OFFSET_FROM_NODE_ID, ACTUAL_SPEED_MEAS, POSTED_SPEED_MEAS, HWY_4LANE_CD, HWY_INTRSTATE_CD, VIOLATION_CARELESS_CD, VIOLATION_DEVICE_CD, VIOLATION_ROW_CD, VIOLATION_LANE_CD, VIOLATION_PASSING_CD, VIOLATION_CHILD_RESTRAINT_CD, VIOLATION_DUI_CD, DRIVER_BAL_MEAS, VIOLATION_SEATBELT_CD, VIOLATION_EQUIP_CD, VIOLATION_TAG_LESS_CD, VIOLATION_TAG_MORE_CD, VIOLATION_INS_CD, VIOLATION_EXPIR_DL_CD, VIOLATION_EXPIR_DL_MORE_CD, VIOLATION_NO_VALID_DL_CD, VIOLATION_SUSP_DL_CD, VIOLATION_CD, FL_DL_EDIT_OVERRIDE_CD, STATE_STATUTE_CD, SECT_NBR, SUB_SECT_NBR, CRASH_CD, PROP_DMG_CD, PROP_DMG_AMT, INJ_CD, SERIOUS_INJ_CD, FATAL_INJ_CD, METHOD_OF_ARREST_CD, CRIMINAL_APPEAR_REQD_CD, INFRACTION_APPEAR_REQD_CD, INFRACTION_NO_APPEAR_REQD_CD, COURT_DT, COURT_NM, COURT_ADDR_TX, COURT_CITY_NM, COURT_STATE_CD, COURT_ZIP_CD, ARREST_DELIVERED_TO_TX, ARREST_DELIVERED_DT, OFCR_RANK_TX, TROOPER_UNIT_TX, BAL_008_OR_ABOVE_CD, DUI_REFUSE_CD, DUI_LIC_SURRENDERED_CD, DUI_LIC_RSN_TX, DUI_ELIGIBLE_CD, DUI_ELIGIBLE_RSN_TX, DUI_BAR_OFC_TX, STATUS_CD, AGGRESSIVE_DRIVER_CD, CRIMINAL_CD, FINE_AMT, ISSUE_ARREST_DT, OFCR_DLVRY_VERIF_CD, DUE_DT, MOTORCYCLE_CD, VEH_16_PASS_CD, OFCR_RE_EXAM_CD, DUI_PASS_UNDER_18_CD, E_CITATION_CD, NM_CHG_CD, COMM_DL_CD, GPS_LAT, GPS_LNG, NAVTEQ_PT_X, NAVTEQ_PT_Y, VIOLATION_SIG_RED_LIGHT_CD, VIOLATION_WORKERS_PRESENT_CD, VIOLATION_HANDHELD_CD, VIOLATION_SCH_ZN_CD, AGENCY_ID, PERM_REG_CD, COMPLIANCE_DT, SPEED_MEAS_DEVICE_ID, DL_SEIZE_CD, BUSINESS_CD, SOURCE_FORMAT_CD, ADDR_USED_CD, KEY_VIOLATION
FROM citation@lime_s4_warehouse
WHERE key_date >= SYSDATE - 90;

INSERT INTO fact_crash_evt (
  hsmv_rpt_nbr,
  key_contrib_circum_env1,
  key_contrib_circum_env2,
  key_contrib_circum_env3,
  key_contrib_circum_rd1,
  key_contrib_circum_rd2,
  key_contrib_circum_rd3,
  key_crash_dt,
  key_crash_sev,
  key_crash_type,
  key_1st_he,
  key_1st_he_loc,
  key_1st_he_rel_to_jct,
  key_geography,
  key_light_cond,
  key_loc_in_work_zn,
  key_manner_of_collision,
  key_notif_by,
  key_rptg_agncy,
  key_rptg_unit,
  key_rd_sys_id,
  key_rd_surf_cond,
  key_type_of_intrsect,
  key_type_of_shoulder,
  key_type_of_work_zn,
  key_weather_cond,
  crash_tm,
  intrsect_st_nm,
  is_alc_rel,
  is_distracted,
  is_drug_rel,
  is_1st_he_within_intrchg,
  is_geolocated,
  is_le_in_work_zn,
  is_pictures_taken,
  is_sch_bus_rel,
  is_within_city_lim,
  is_workers_in_work_zn,
  is_work_zn_rel,
  lat,
  lng,
  milepost_nbr,
  offset_dir,
  offset_ft,
  rptg_ofcr_rank,
  st_nm,
  st_nbr,
  veh_cnt,
  moped_cnt,
  motorcycle_cnt,
  nm_cnt,
  pass_cnt,
  trailer_cnt,
  bike_cnt,
  ped_cnt,
  fatality_cnt,
  fatality_unrestrained_cnt,
  inj_cnt,
  inj_unrestrained_cnt,
  citation_cnt,
  citation_amt,
  prop_dmg_cnt,
  prop_dmg_amt,
  veh_dmg_cnt,
  veh_dmg_amt,
  tot_dmg_amt,
  trans_by_ems_cnt,
  trans_by_le_cnt,
  trans_by_oth_cnt,
  geo_status_cd,
  form_type_cd,
  agncy_rpt_nbr,
  inj_incapacitating_cnt,
  batch_nbr,
  data_src_cd,
  is_complete,
  is_aggressive,
  crash_type_dir_tx,
  rpt_dt,
  notif_tm,
  dispatched_tm,
  arrived_tm,
  cleared_tm,
  img_ext_tx,
  img_src_nm,
  codeable,
  inj_none_cnt,
  inj_possible_cnt,
  inj_non_incapacitating_cnt,
  inj_fatal_30_cnt,
  inj_fatal_non_traffic_cnt,
  key_bike_ped_crash_type,
  key_crash_sev_dtl)
SELECT
  hsmv_rpt_nbr,
  key_contrib_circum_env1,
  key_contrib_circum_env2,
  key_contrib_circum_env3,
  key_contrib_circum_rd1,
  key_contrib_circum_rd2,
  key_contrib_circum_rd3,
  key_crash_dt,
  key_crash_sev,
  key_crash_type,
  key_1st_he,
  key_1st_he_loc,
  key_1st_he_rel_to_jct,
  key_geography,
  key_light_cond,
  key_loc_in_work_zn,
  key_manner_of_collision,
  key_notif_by,
  key_rptg_agncy,
  key_rptg_unit,
  key_rd_sys_id,
  key_rd_surf_cond,
  key_type_of_intrsect,
  key_type_of_shoulder,
  key_type_of_work_zn,
  key_weather_cond,
  crash_tm,
  intrsect_st_nm,
  is_alc_rel,
  is_distracted,
  is_drug_rel,
  is_1st_he_within_intrchg,
  is_geolocated,
  is_le_in_work_zn,
  is_pictures_taken,
  is_sch_bus_rel,
  is_within_city_lim,
  is_workers_in_work_zn,
  is_work_zn_rel,
  lat,
  lng,
  milepost_nbr,
  offset_dir,
  offset_ft,
  rptg_ofcr_rank,
  st_nm,
  st_nbr,
  veh_cnt,
  moped_cnt,
  motorcycle_cnt,
  nm_cnt,
  pass_cnt,
  trailer_cnt,
  bike_cnt,
  ped_cnt,
  fatality_cnt,
  fatality_unrestrained_cnt,
  inj_cnt,
  inj_unrestrained_cnt,
  citation_cnt,
  citation_amt,
  prop_dmg_cnt,
  prop_dmg_amt,
  veh_dmg_cnt,
  veh_dmg_amt,
  tot_dmg_amt,
  trans_by_ems_cnt,
  trans_by_le_cnt,
  trans_by_oth_cnt,
  geo_status_cd,
  form_type_cd,
  agncy_rpt_nbr,
  inj_incapacitating_cnt,
  batch_nbr,
  data_src_cd,
  is_complete,
  is_aggressive,
  crash_type_dir_tx,
  rpt_dt,
  notif_tm,
  dispatched_tm,
  arrived_tm,
  cleared_tm,
  img_ext_tx,
  img_src_nm,
  codeable,
  inj_none_cnt,
  inj_possible_cnt,
  inj_non_incapacitating_cnt,
  inj_fatal_30_cnt,
  inj_fatal_non_traffic_cnt,
  key_bike_ped_crash_type,
  key_crash_sev_dtl
FROM fact_crash_evt@lime_s4_warehouse
WHERE key_crash_dt >= SYSDATE - 90;

INSERT INTO hsmv_codeable
SELECT * FROM hsmv_codeable@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_comm_veh
SELECT * FROM fact_comm_veh@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_driver
SELECT * FROM fact_driver@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_non_motorist
SELECT * FROM fact_non_motorist@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_pass
SELECT * FROM fact_pass@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_veh
SELECT * FROM fact_veh@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO fact_violation
SELECT * FROM fact_violation@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO pbcat_bike
SELECT * FROM pbcat_bike@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

INSERT INTO pbcat_ped
SELECT * FROM pbcat_ped@lime_s4_warehouse t1
WHERE EXISTS (
  SELECT 1 FROM fact_crash_evt t2
  WHERE t2.hsmv_rpt_nbr = t1.hsmv_rpt_nbr
);

COMMIT;
