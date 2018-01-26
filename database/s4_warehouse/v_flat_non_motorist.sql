CREATE OR REPLACE VIEW v_flat_non_motorist AS
SELECT
    nm.hsmv_rpt_nbr,
    floor(nm.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    nm.person_nbr,
    nm.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    nm.key_geography,
    dg.dot_district_nm,
    dg.rpc_nm,
    dg.mpo_nm,
    dg.cnty_cd,
    dg.cnty_nm,
    dg.city_cd,
    dg.city_nm,
    nm.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type_nm,
    nm.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    nm.key_action_prior,
    vap.nm_attr_tx AS action_prior,
    nm.key_action_circum1,
    vac1.nm_attr_tx AS action_circum1,
    nm.key_action_circum2,
    vac2.nm_attr_tx AS action_circum2,
    nm.key_age_rng,
    var.nm_attr_tx AS age_rng,
    nm.key_alc_test_result,
    vatr.nm_attr_tx AS alc_test_result,
    nm.key_alc_test_type,
    vatt.nm_attr_tx AS alc_test_type,
    nm.key_alc_tested,
    vat.nm_attr_tx AS alc_tested,
    nm.key_desc,
    vd.nm_attr_tx AS "DESC",
    nm.key_drug_test_result,
    vdtr.nm_attr_tx AS drug_test_result,
    nm.key_drug_test_type,
    vdtt.nm_attr_tx AS drug_test_type,
    nm.key_drug_tested,
    vdt.nm_attr_tx AS drug_tested,
    nm.key_gender,
    vg.nm_attr_tx AS gender,
    nm.key_inj_sev,
    vis.nm_attr_tx AS inj_sev,
    nm.key_loc_at_tm_of_crash,
    vlc.nm_attr_tx AS loc_at_tm_of_crash,
    nm.key_safety_eq1,
    vse1.nm_attr_tx AS safety_eq1,
    nm.key_safety_eq2,
    vse2.nm_attr_tx AS safety_eq2,
    nm.key_src_of_trans,
    vst.nm_attr_tx AS src_of_trans,
    nm.addr_city,
    nm.addr_state,
    nm.addr_zip,
    nm.bac,
    nm.dl_state,
    DECODE(nm.is_alc_use_suspected, '1', 'Y', '0', 'N', NULL) AS is_alc_use_suspected,
    DECODE(nm.is_drug_use_suspected, '1', 'Y', '0', 'N', NULL) AS is_drug_use_suspected,
    nm.bike_cnt,
    nm.ped_cnt,
    nm.fatality_cnt,
    nm.inj_cnt,
    nm.citation_cnt,
    nm.citation_amt,
    nm.prop_dmg_cnt,
    nm.prop_dmg_amt,
    nm.inj_incapacitating_cnt,
    nm.batch_nbr
FROM fact_non_motorist nm
LEFT JOIN dim_date dd ON nm.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg ON nm.key_geography = dg.ID
LEFT JOIN dim_agncy da ON nm.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON nm.key_rptg_unit = dau.ID
LEFT JOIN v_nm_action_prior vap ON nm.key_action_prior = vap.ID
LEFT JOIN v_nm_action_circum vac1 ON nm.key_action_circum1 = vac1.ID
LEFT JOIN v_nm_action_circum vac2 ON nm.key_action_circum2 = vac2.ID
LEFT JOIN v_nm_age_rng var ON nm.key_age_rng = var.ID
LEFT JOIN v_nm_alc_test_result vatr ON nm.key_alc_test_result = vatr.ID
LEFT JOIN v_nm_alc_test_type vatt ON nm.key_alc_test_type = vatt.ID
LEFT JOIN v_nm_alc_tested vat ON nm.key_alc_tested = vat.ID
LEFT JOIN v_nm_desc vd ON nm.key_desc = vd.ID
LEFT JOIN v_nm_drug_test_result vdtr ON nm.key_drug_test_result = vdtr.ID
LEFT JOIN v_nm_drug_test_type vdtt ON nm.key_drug_test_type = vdtt.ID
LEFT JOIN v_nm_drug_tested vdt ON nm.key_drug_tested = vdt.ID
LEFT JOIN v_nm_gender vg ON nm.key_gender = vg.ID
LEFT JOIN v_nm_inj_sev vis ON nm.key_inj_sev = vis.ID
LEFT JOIN v_nm_loc_at_tm_of_crash vlc ON nm.key_loc_at_tm_of_crash = vlc.ID
LEFT JOIN v_nm_safety_eq vse1 ON nm.key_safety_eq1 = vse1.ID
LEFT JOIN v_nm_safety_eq vse2 ON nm.key_safety_eq2 = vse2.ID
LEFT JOIN v_nm_src_of_trans vst ON nm.key_src_of_trans = vst.ID
;
