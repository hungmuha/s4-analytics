CREATE OR REPLACE VIEW v_flat_veh AS
SELECT
    veh.hsmv_rpt_nbr,
    floor(veh.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    veh.veh_nbr,
    veh.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    veh.key_geography,
    dg.dot_district_nm,
    dg.rpc_nm,
    dg.mpo_nm,
    dg.cnty_cd,
    dg.cnty_nm,
    dg.city_cd,
    dg.city_nm,
    veh.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type_nm,
    veh.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    veh.key_ar_of_init_impact,
    vaii.veh_attr_tx AS ar_of_init_impact,
    veh.key_body_type,
    vbt.veh_attr_tx AS body_type,
    veh.key_cargo_body_type,
    vcbt.veh_attr_tx AS cargo_body_type,
    veh.key_cmv_config,
    vcc.veh_attr_tx AS cmv_config,
    veh.key_comm_non_comm,
    vcnc.veh_attr_tx AS comm_non_comm,
    veh.key_dmg_extent,
    vde.veh_attr_tx AS dmg_extent,
    veh.key_dir_before,
    vdb.veh_attr_tx AS dir_before,
    veh.key_gvwr_gcwr,
    vgg.veh_attr_tx AS gvwr_gcwr,
    veh.key_he1,
    dhe1.harmful_evt_tx AS he1,
    veh.key_he2,
    dhe2.harmful_evt_tx AS he2,
    veh.key_he3,
    dhe3.harmful_evt_tx AS he3,
    veh.key_he4,
    dhe4.harmful_evt_tx AS he4,
    veh.key_maneuver_action,
    vma.veh_attr_tx AS maneuver_action,
    veh.key_most_dmg_ar,
    vmda.veh_attr_tx AS most_dmg_ar,
    veh.key_most_he,
    mhe.harmful_evt_tx AS most_he,
    veh.key_rd_align,
    vra.veh_attr_tx AS rd_align,
    veh.key_rd_grade,
    vrg.veh_attr_tx AS rd_grade,
    veh.key_special_func,
    vsf.veh_attr_tx AS special_func,
    veh.key_traffic_ctl,
    vtc.veh_attr_tx AS traffic_ctl,
    veh.key_trafficway,
    vtw.veh_attr_tx AS trafficway,
    veh.key_veh_defect1,
    vvd1.veh_attr_tx AS veh_defect1,
    veh.key_veh_defect2,
    vvd2.veh_attr_tx AS veh_defect2,
    veh.key_veh_type,
    vvt.veh_attr_tx AS veh_type,
    veh.key_wrecker_sel,
    vws.veh_attr_tx AS wrecker_sel,
    veh.est_speed,
    veh.is_comm_veh,
    veh.is_emerg_veh,
    veh.is_hazmat_released,
    veh.is_hit_and_run,
    veh.is_owner_a_business,
    veh.is_perm_reg,
    veh.is_towed_due_to_dmg,
    veh.motor_carrier_city,
    veh.motor_carrier_state,
    veh.motor_carrier_zip,
    veh.placard_hazmat_class,
    veh.posted_speed,
    veh.reg_state,
    veh.tot_lanes,
    veh.traveling_on_st,
    veh.veh_color,
    veh.veh_make,
    veh.veh_model,
    veh.veh_owner_city,
    veh.veh_owner_state,
    veh.veh_owner_zip,
    veh.veh_style,
    veh.veh_yr,
    veh.moped_cnt,
    veh.motorcycle_cnt,
    veh.pass_cnt,
    veh.trailer_cnt,
    veh.fatality_cnt,
    veh.fatality_unrestrained_cnt,
    veh.inj_cnt,
    veh.inj_unrestrained_cnt,
    veh.citation_cnt,
    veh.citation_amt,
    veh.prop_dmg_cnt,
    veh.prop_dmg_amt,
    veh.veh_dmg_cnt,
    veh.veh_dmg_amt,
    veh.tot_dmg_amt,
    veh.inj_incapacitating_cnt,
    veh.batch_nbr,
    veh.inj_none_cnt,
    veh.inj_possible_cnt,
    veh.inj_non_incapacitating_cnt,
    veh.inj_fatal_30_cnt,
    veh.inj_fatal_non_traffic_cnt,
    fce.last_updt_dt
FROM v_fact_all_veh veh
INNER JOIN fact_crash_evt fce ON fce.hsmv_rpt_nbr = veh.hsmv_rpt_nbr
LEFT JOIN dim_date dd ON veh.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg ON veh.key_geography = dg.ID
LEFT JOIN dim_agncy da ON veh.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON veh.key_rptg_unit = dau.ID
LEFT JOIN v_veh_area_on_veh vaii ON veh.key_ar_of_init_impact = vaii.ID
LEFT JOIN v_veh_body_type vbt ON veh.key_body_type = vbt.ID
LEFT JOIN v_veh_cargo_body_type vcbt ON veh.key_cargo_body_type = vcbt.ID
LEFT JOIN v_veh_cmv_config vcc ON veh.key_cmv_config = vcc.ID
LEFT JOIN v_veh_comm_non_comm vcnc ON veh.key_comm_non_comm = vcnc.ID
LEFT JOIN v_veh_dmg_ext vde ON veh.key_dmg_extent = vde.ID
LEFT JOIN v_veh_dir_before vdb ON veh.key_dir_before = vdb.ID
LEFT JOIN v_veh_gvwr vgg ON veh.key_gvwr_gcwr = vgg.ID
LEFT JOIN dim_harmful_evt dhe1 ON veh.key_he1 = dhe1.ID
LEFT JOIN dim_harmful_evt dhe2 ON veh.key_he2 = dhe2.ID
LEFT JOIN dim_harmful_evt dhe3 ON veh.key_he3 = dhe3.ID
LEFT JOIN dim_harmful_evt dhe4 ON veh.key_he4 = dhe4.ID
LEFT JOIN v_veh_maneuver vma ON veh.key_maneuver_action = vma.ID
LEFT JOIN v_veh_area_on_veh vmda ON veh.key_most_dmg_ar = vmda.ID
LEFT JOIN dim_harmful_evt mhe ON veh.key_most_he = mhe.ID
LEFT JOIN v_veh_rd_align vra ON veh.key_rd_align = vra.ID
LEFT JOIN v_veh_rd_grade vrg ON veh.key_rd_grade = vrg.ID
LEFT JOIN v_veh_special_func vsf ON veh.key_special_func = vsf.ID
LEFT JOIN v_veh_traffic_ctl vtc ON veh.key_traffic_ctl = vtc.ID
LEFT JOIN v_veh_trafficway vtw ON veh.key_trafficway = vtw.ID
LEFT JOIN v_veh_defect vvd1 ON veh.key_veh_defect1 = vvd1.ID
LEFT JOIN v_veh_defect vvd2 ON veh.key_veh_defect2 = vvd2.ID
LEFT JOIN v_veh_type vvt ON veh.key_veh_type = vvt.ID
LEFT JOIN v_veh_wrecker_sel vws ON veh.key_wrecker_sel = vws.ID
;
