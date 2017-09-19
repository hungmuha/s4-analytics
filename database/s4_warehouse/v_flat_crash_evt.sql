CREATE OR REPLACE VIEW v_flat_crash_evt AS
SELECT
    fce.hsmv_rpt_nbr,
    floor(fce.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    fce.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    fce.key_geography,
    dg1.dot_district_nm,
    dg1.rpc_nm,
    dg1.mpo_nm,
    dg1.cnty_cd,
    dg1.cnty_nm,
    dg1.city_cd,
    dg1.city_nm,
    fce.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type_nm,
    fce.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    fce.key_contrib_circum_env1,
    cce1.crash_attr_tx AS contrib_circum_env1,
    fce.key_contrib_circum_env2,
    cce2.crash_attr_tx AS contrib_circum_env2,
    fce.key_contrib_circum_env3,
    cce3.crash_attr_tx AS contrib_circum_env3,
    fce.key_contrib_circum_rd1,
    ccr3.crash_attr_tx AS contrib_circum_rd1,
    fce.key_contrib_circum_rd2,
    ccr3.crash_attr_tx AS contrib_circum_rd2,
    fce.key_contrib_circum_rd3,
    ccr3.crash_attr_tx AS contrib_circum_rd3,
    fce.key_crash_sev,
    vcs.crash_attr_tx AS crash_sev,
    fce.key_crash_sev_dtl,
    vcsd.crash_attr_tx AS crash_sev_dtl,
    fce.key_crash_type,
    vct.crash_attr_tx AS crash_type,
    vcts.crash_attr_tx AS crash_type_simplified,
    fce.crash_type_dir_tx,
    fce.key_1st_he AS key_first_he,
    dhe.harmful_evt_tx AS first_he,
    fce.key_1st_he_loc AS key_first_he_loc,
    vfhel.crash_attr_tx AS first_he_loc,
    fce.key_1st_he_rel_to_jct AS key_first_he_rel_to_jct,
    vfhej.crash_attr_tx AS first_he_rel_to_jct,
    fce.key_light_cond,
    vclc.crash_attr_tx AS light_cond,
    fce.key_loc_in_work_zn,
    vlwz.crash_attr_tx AS loc_in_work_zn,
    fce.key_manner_of_collision,
    vmc.crash_attr_tx AS manner_of_collision,
    fce.key_notif_by,
    vnb.crash_attr_tx AS notif_by,
    fce.key_rd_sys_id,
    vcrsi.crash_attr_tx AS rd_sys_id,
    fce.key_rd_surf_cond,
    vcrs.crash_attr_tx AS rd_surf_cond,
    fce.key_type_of_intrsect,
    vcti.crash_attr_tx AS type_of_intrsect,
    fce.key_type_of_shoulder,
    vctsh.crash_attr_tx AS type_of_shoulder,
    fce.key_type_of_work_zn,
    vctwz.crash_attr_tx AS type_of_work_zn,
    fce.key_weather_cond,
    vcwc.crash_attr_tx AS weather_cond,
    fce.key_bike_ped_crash_type,
    vbpct.crash_grp_id AS key_bike_ped_crash_group,
    vbpct.bike_or_ped AS bike_ped_bike_or_ped,
    vbpct.crash_grp_nbr AS bike_ped_crash_grp_nbr,
    vbpct.crash_grp_nm AS bike_ped_crash_grp_nm,
    vbpct.crash_type_nbr AS bike_ped_crash_type_nbr,
    vbpct.crash_type_nm AS bike_ped_crash_type_nm,
    fce.crash_tm,
    fce.intrsect_st_nm,
    fce.is_alc_rel,
    fce.is_distracted,
    fce.is_drug_rel,
    fce.is_1st_he_within_intrchg,
    fce.is_geolocated,
    fce.is_le_in_work_zn,
    fce.is_pictures_taken,
    fce.is_sch_bus_rel,
    fce.is_within_city_lim,
    fce.is_workers_in_work_zn,
    fce.is_work_zn_rel,
    fce.milepost_nbr,
    fce.offset_dir,
    fce.offset_ft,
    fce.rptg_ofcr_rank,
    fce.st_nm,
    fce.st_nbr,
    fce.veh_cnt,
    fce.moped_cnt,
    fce.motorcycle_cnt,
    fce.nm_cnt,
    fce.pass_cnt,
    fce.trailer_cnt,
    fce.bike_cnt,
    fce.ped_cnt,
    fce.fatality_cnt,
    fce.inj_cnt,
    fce.citation_cnt,
    fce.citation_amt,
    fce.prop_dmg_cnt,
    fce.prop_dmg_amt,
    fce.veh_dmg_cnt,
    fce.veh_dmg_amt,
    fce.tot_dmg_amt,
    fce.trans_by_ems_cnt,
    fce.trans_by_le_cnt,
    fce.trans_by_oth_cnt,
    fce.inj_incapacitating_cnt,
    fce.inj_none_cnt,
    fce.inj_possible_cnt,
    fce.inj_non_incapacitating_cnt,
    fce.inj_fatal_30_cnt,
    fce.inj_fatal_non_traffic_cnt,
    fce.geo_status_cd,
    fce.form_type_cd,
    fce.agncy_rpt_nbr,
    fce.batch_nbr,
    fce.data_src_cd,
    fce.is_complete,
    fce.is_aggressive,
    fce.rpt_dt,
    fce.notif_tm,
    fce.dispatched_tm,
    fce.arrived_tm,
    fce.cleared_tm,
    CASE WHEN fce.img_ext_tx IS NOT NULL THEN fce.hsmv_rpt_nbr || fce.img_ext_tx ELSE NULL END AS img_file_nm,
    fce.img_src_nm,
    fce.codeable,
    gcr.crash_seg_id,
    gcr.nearest_intrsect_id,
    gcr.nearest_intrsect_offset_ft,
    gcr.nearest_intrsect_offset_dir,
    gcr.ref_intrsect_id,
    gcr.ref_intrsect_offset_ft,
    gcr.ref_intrsect_offset_dir,
    gcr.on_network,
    gcr.dot_on_sys,
    gcr.mapped,
    gcr.key_geography AS gc_key_geography,
    dg2.dot_district_nm AS gc_dot_district_nm,
    dg2.rpc_nm AS gc_rpc_nm,
    dg2.mpo_nm AS gc_mpo_nm,
    dg2.cnty_cd AS gc_cnty_cd,
    dg2.cnty_nm AS gc_cnty_nm,
    dg2.city_cd AS gc_city_cd,
    dg2.city_nm AS gc_city_nm,
    CASE
        WHEN fce.lng IS NULL OR fce.lat IS NULL THEN NULL
        ELSE sdo_geometry(2001, 4326, mdsys.sdo_point_type(fce.lng, fce.lat, NULL), NULL, NULL)
    END AS gps_pt_4326,
    CASE
        WHEN gcr.shape IS NULL THEN NULL
        ELSE sdo_geometry(sde.st_astext(gcr.shape), 3087)
    END AS geocode_pt_3087,
    CASE
        WHEN fce.last_updt_dt IS NULL THEN gcr.last_updt_dt
        WHEN gcr.last_updt_dt IS NULL THEN fce.last_updt_dt
        ELSE greatest(fce.last_updt_dt, gcr.last_updt_dt)
    END AS last_updt_dt -- see https://community.oracle.com/thread/958617
FROM fact_crash_evt fce
LEFT JOIN navteq_2015q1.geocode_result gcr ON gcr.hsmv_rpt_nbr = fce.hsmv_rpt_nbr
LEFT JOIN dim_date dd ON fce.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg1 ON fce.key_geography = dg1.ID
LEFT JOIN dim_geography dg2 ON gcr.key_geography = dg2.ID
LEFT JOIN dim_agncy da ON fce.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON fce.key_rptg_unit = dau.ID
LEFT JOIN v_crash_contrib_circum_env cce1 ON fce.key_contrib_circum_env1 = cce1.ID
LEFT JOIN v_crash_contrib_circum_env cce2 ON fce.key_contrib_circum_env2 = cce2.ID
LEFT JOIN v_crash_contrib_circum_env cce3 ON fce.key_contrib_circum_env3 = cce3.ID
LEFT JOIN v_crash_contrib_circum_road ccr1 ON fce.key_contrib_circum_rd1 = ccr1.ID
LEFT JOIN v_crash_contrib_circum_road ccr2 ON fce.key_contrib_circum_rd2 = ccr2.ID
LEFT JOIN v_crash_contrib_circum_road ccr3 ON fce.key_contrib_circum_rd3 = ccr3.ID
LEFT JOIN v_crash_sev vcs ON fce.key_crash_sev = vcs.ID
LEFT JOIN v_crash_sev_dtl vcsd ON fce.key_crash_sev_dtl = vcsd.ID
LEFT JOIN v_crash_light_cond vclc ON fce.key_light_cond = vclc.ID
LEFT JOIN v_crash_type_simplified vcts ON fce.key_crash_type = vcts.ID
LEFT JOIN v_crash_type vct ON fce.key_crash_type = vct.ID
LEFT JOIN dim_harmful_evt dhe ON fce.key_1st_he = dhe.ID
LEFT JOIN v_crash_first_he_loc vfhel ON fce.key_1st_he_loc = vfhel.ID
LEFT JOIN v_crash_first_he_rel_to_jct vfhej ON fce.key_1st_he_rel_to_jct = vfhej.ID
LEFT JOIN v_crash_loc_in_work_zn vlwz ON fce.key_loc_in_work_zn = vlwz.ID
LEFT JOIN v_crash_manner_of_collision vmc ON fce.key_manner_of_collision = vmc.ID
LEFT JOIN v_crash_notif_by vnb ON fce.key_notif_by = vnb.ID
LEFT JOIN v_crash_road_sys_id vcrsi ON fce.key_rd_sys_id = vcrsi.ID
LEFT JOIN v_crash_road_surf_cond vcrs ON fce.key_rd_surf_cond = vcrs.ID
LEFT JOIN v_crash_type_of_intrsect vcti ON fce.key_type_of_intrsect = vcti.ID
LEFT JOIN v_crash_type_of_shoulder vctsh ON fce.key_type_of_shoulder = vctsh.ID
LEFT JOIN v_crash_type_of_work_zn vctwz ON fce.key_type_of_work_zn = vctwz.ID
LEFT JOIN v_crash_weather_cond vcwc ON fce.key_weather_cond = vcwc.ID
LEFT JOIN v_bike_ped_crash_type vbpct ON fce.key_bike_ped_crash_type = vbpct.crash_type_id
;
