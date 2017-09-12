CREATE VIEW v_flat_pass AS
SELECT
    fp.hsmv_rpt_nbr,
    floor(fp.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    fp.person_nbr,
    fp.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    fp.key_geography,
    dg.dot_district_nm,
    dg.rpc_nm,
    dg.mpo_nm,
    dg.cnty_cd,
    dg.cnty_nm,
    dg.city_cd,
    dg.city_nm,
    fp.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type,
    fp.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    fp.key_age_rng,
    var.pass_attr_tx AS age_rng,
    fp.key_airbag_deployed,
    vad.pass_attr_tx AS airbag_deployed,
    fp.key_ejection,
    ve.pass_attr_tx AS ejection,
    fp.key_gender,
    vg.pass_attr_tx AS gender,
    fp.key_helmet_use,
    vhu.pass_attr_tx AS helmet_use,
    fp.key_inj_sev,
    vis.pass_attr_tx AS inj_sev,
    fp.key_restraint_sys,
    vrs.pass_attr_tx AS restraint_sys,
    fp.key_seating_other,
    vso.pass_attr_tx AS seating_other,
    fp.key_seating_row,
    vsr.pass_attr_tx AS seating_row,
    fp.key_seating_seat,
    vss.pass_attr_tx AS seating_seat,
    fp.key_src_of_trans,
    vst.pass_attr_tx AS src_of_trans,
    fp.key_veh_body_type,
    vvbt.pass_attr_tx AS veh_body_type,
    fp.addr_city,
    fp.addr_state,
    fp.addr_zip,
    fp.dl_state,
    fp.is_using_eye_protection,
    fp.fatality_cnt,
    fp.fatality_unrestrained_cnt,
    fp.inj_cnt,
    fp.inj_unrestrained_cnt,
    fp.citation_cnt,
    fp.citation_amt,
    fp.prop_dmg_cnt,
    fp.batch_nbr,
    fp.inj_incapacitating_cnt,
    fp.prop_dmg_amt
FROM fact_pass fp
LEFT JOIN dim_date dd ON fp.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg ON fp.key_geography = dg.ID
LEFT JOIN dim_agncy da ON fp.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON fp.key_rptg_unit = dau.ID
LEFT JOIN v_pass_age_rng var ON fp.key_age_rng = var.ID
LEFT JOIN v_pass_airbag_deployed vad ON fp.key_airbag_deployed = vad.ID
LEFT JOIN v_pass_ejection ve ON fp.key_ejection = ve.ID
LEFT JOIN v_pass_gender vg ON fp.key_gender = vg.ID
LEFT JOIN v_pass_helmet_use vhu ON fp.key_helmet_use = vhu.ID
LEFT JOIN v_pass_inj_sev vis ON fp.key_inj_sev = vis.ID
LEFT JOIN v_pass_restraint_sys vrs ON fp.key_restraint_sys = vrs.ID
LEFT JOIN v_pass_seating_other vso ON fp.key_seating_other = vso.ID
LEFT JOIN v_pass_seating_row vsr ON fp.key_seating_row = vsr.ID
LEFT JOIN v_pass_seating_seat vss ON fp.key_seating_seat = vss.ID
LEFT JOIN v_pass_src_of_trans vst ON fp.key_src_of_trans = vst.ID
LEFT JOIN v_pass_veh_body_type vvbt ON fp.key_veh_body_type = vvbt.ID
;
