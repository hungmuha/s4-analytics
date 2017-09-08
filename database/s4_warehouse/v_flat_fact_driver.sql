-- CREATE VIEW v_flat_fact_driver AS
SELECT
    fd.hsmv_rpt_nbr,
    floor(fd.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    fd.veh_nbr,
    fd.person_nbr,
    fd.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    fd.key_geography,
    dg.dot_district_nm,
    dg.rpc_nm,
    dg.mpo_nm,
    dg.cnty_cd,
    dg.cnty_nm,
    dg.city_cd,
    dg.city_nm,
    fd.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type,
    fd.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    fd.key_action1,
    vda1.driver_attr_tx AS action1,
    fd.key_action2,
    vda2.driver_attr_tx AS action2,
    fd.key_action3,
    vda3.driver_attr_tx AS action3,
    fd.key_action4,
    vda4.driver_attr_tx AS action4,
    fd.key_age_rng,
    var.driver_attr_tx AS age_rng,
    fd.key_airbag_deployed,
    vad.driver_attr_tx AS airbag_deployed,
    fd.key_alc_test_result,
    vatr.driver_attr_tx AS alc_test_result,
    fd.key_alc_test_type,
    vatt.driver_attr_tx AS alc_test_type,
    fd.key_alc_tested,
    vat.driver_attr_tx AS alc_tested,
    fd.key_cond_at_tm_of_crash,
    vcc.driver_attr_tx AS cond_at_tm_of_crash,
    fd.key_distracted_by,
    vdb.driver_attr_tx AS distracted_by,
    fd.key_dl_endorsements,
    vdle.driver_attr_tx AS dl_endorsements,
    fd.key_dl_type,
    vdlt.driver_attr_tx AS dl_type,
    fd.key_drug_test_result,
    vdtr.driver_attr_tx AS drug_test_result,
    fd.key_drug_test_type,
    vdtt.driver_attr_tx AS drug_test_type,
    fd.key_drug_tested,
    vdt.driver_attr_tx AS drug_tested,
    fd.key_ejection,
    ve.driver_attr_tx AS ejection,
    fd.key_gender,
    vg.driver_attr_tx AS gender,
    fd.key_helmet_use,
    vhu.driver_attr_tx AS helmet_use,
    fd.key_inj_sev,
    vis.driver_attr_tx AS inj_sev,
    fd.key_restraint_sys,
    vrs.driver_attr_tx AS restraint_sys,
    fd.key_src_of_trans,
    vst.driver_attr_tx AS src_of_trans,
    fd.key_veh_body_type,
    vvbt.driver_attr_tx AS veh_body_type,
    fd.key_vision_obstruction,
    vvo.driver_attr_tx AS vision_obstruction,
    fd.addr_city,
    fd.addr_state,
    fd.addr_zip,
    fd.bac,
    fd.dl_state,
    fd.ins_co,
    fd.is_alc_use_suspected,
    fd.is_distracted,
    fd.is_drug_use_suspected,
    fd.is_using_eye_protection,
    fd.is_re_exam_recommended,
    fd.fatality_cnt,
    fd.fatality_unrestrained_cnt,
    fd.inj_cnt,
    fd.inj_unrestrained_cnt,
    fd.citation_cnt,
    fd.citation_amt,
    fd.prop_dmg_cnt,
    fd.prop_dmg_amt,
    fd.inj_incapacitating_cnt,
    fd.batch_nbr
FROM fact_driver fd
LEFT JOIN dim_date dd ON fd.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg ON fd.key_geography = dg.ID
LEFT JOIN dim_agncy da ON fd.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON fd.key_rptg_unit = dau.ID
LEFT JOIN v_driver_action vda1 ON fd.key_action1 = vda1.ID
LEFT JOIN v_driver_action vda2 ON fd.key_action2 = vda2.ID
LEFT JOIN v_driver_action vda3 ON fd.key_action3 = vda3.ID
LEFT JOIN v_driver_action vda4 ON fd.key_action4 = vda4.ID
LEFT JOIN v_driver_age_rng var ON fd.key_age_rng = var.ID
LEFT JOIN v_driver_airbag_deployed vad ON fd.key_airbag_deployed = vad.ID
LEFT JOIN v_driver_alc_test_result vatr ON fd.key_alc_test_result = vatr.ID
LEFT JOIN v_driver_alc_test_type vatt ON fd.key_alc_test_type = vatt.ID
LEFT JOIN v_driver_alc_tested vat ON fd.key_alc_tested = vat.ID
LEFT JOIN v_driver_cond_at_tm_of_crash vcc ON fd.key_cond_at_tm_of_crash = vcc.ID
LEFT JOIN v_driver_distracted_by vdb ON fd.key_distracted_by = vdb.ID
LEFT JOIN v_driver_dl_endorsements vdle ON fd.key_dl_endorsements = vdle.ID
LEFT JOIN v_driver_dl_type vdlt ON fd.key_dl_type = vdlt.ID
LEFT JOIN v_driver_drug_test_result vdtr ON fd.key_drug_test_result = vdtr.ID
LEFT JOIN v_driver_drug_test_type vdtt ON fd.key_drug_test_type = vdtt.ID
LEFT JOIN v_driver_drug_tested vdt ON fd.key_drug_tested = vdt.ID
LEFT JOIN v_driver_ejection ve ON fd.key_ejection = ve.ID
LEFT JOIN v_driver_gender vg ON fd.key_gender = vg.ID
LEFT JOIN v_driver_helmet_use vhu ON fd.key_helmet_use = vhu.ID
LEFT JOIN v_driver_inj_sev vis ON fd.key_inj_sev = vis.ID
LEFT JOIN v_driver_restraint_sys vrs ON fd.key_restraint_sys = vrs.ID
LEFT JOIN v_driver_src_of_trans vst ON fd.key_src_of_trans = vst.ID
LEFT JOIN v_driver_veh_body_type vvbt ON fd.key_veh_body_type = vvbt.ID
LEFT JOIN v_driver_vision_obstruction vvo ON fd.key_vision_obstruction = vvo.ID
;
