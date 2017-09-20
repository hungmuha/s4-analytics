CREATE OR REPLACE PROCEDURE s4_sync_crash_evt (p_days_back INT DEFAULT NULL)
AS
    v_start_dt DATE;
BEGIN
    v_start_dt := CASE WHEN p_days_back IS NOT NULL THEN TRUNC(SYSDATE - p_days_back) ELSE NULL END;

    IF v_start_dt IS NULL THEN
        EXECUTE IMMEDIATE 'TRUNCATE TABLE crash_evt';
    ELSE
        DELETE FROM crash_evt evt
        WHERE EXISTS (
            SELECT 1
            FROM v_flat_crash_evt@s4_warehouse vce
            WHERE vce.last_updt_dt >= v_start_dt
            AND vce.hsmv_rpt_nbr = evt.hsmv_rpt_nbr
        );
    END IF;

    INSERT INTO crash_evt (
        hsmv_rpt_nbr,
        hsmv_rpt_nbr_trunc,
        key_crash_dt,
        crash_yr,
        crash_mm,
        crash_mo,
        crash_dd,
        crash_day,
        crash_d,
        key_geography,
        dot_district_nm,
        rpc_nm,
        mpo_nm,
        cnty_cd,
        cnty_nm,
        city_cd,
        city_nm,
        key_rptg_agncy,
        rptg_agncy_nm,
        rptg_agncy_short_nm,
        rptg_agncy_type_nm,
        key_rptg_unit,
        rptg_unit_nm,
        rptg_unit_short_nm,
        key_contrib_circum_env1,
        contrib_circum_env1,
        key_contrib_circum_env2,
        contrib_circum_env2,
        key_contrib_circum_env3,
        contrib_circum_env3,
        key_contrib_circum_rd1,
        contrib_circum_rd1,
        key_contrib_circum_rd2,
        contrib_circum_rd2,
        key_contrib_circum_rd3,
        contrib_circum_rd3,
        key_crash_sev,
        crash_sev,
        key_crash_sev_dtl,
        crash_sev_dtl,
        key_crash_type,
        crash_type,
        crash_type_simplified,
        crash_type_dir_tx,
        key_first_he,
        first_he,
        key_first_he_loc,
        first_he_loc,
        key_first_he_rel_to_jct,
        first_he_rel_to_jct,
        key_light_cond,
        light_cond,
        key_loc_in_work_zn,
        loc_in_work_zn,
        key_manner_of_collision,
        manner_of_collision,
        key_notif_by,
        notif_by,
        key_rd_sys_id,
        rd_sys_id,
        is_public_rd,
        key_rd_surf_cond,
        rd_surf_cond,
        key_type_of_intrsect,
        type_of_intrsect,
        key_type_of_shoulder,
        type_of_shoulder,
        key_type_of_work_zn,
        type_of_work_zn,
        key_weather_cond,
        weather_cond,
        key_bike_ped_crash_type,
        key_bike_ped_crash_group,
        bike_or_ped,
        bike_ped_crash_grp_nbr,
        bike_ped_crash_grp_nm,
        bike_ped_crash_type_nbr,
        bike_ped_crash_type_nm,
        crash_tm,
        crash_hh24mi,
        intrsect_st_nm,
        is_alc_rel,
        is_distracted,
        is_drug_rel,
        is_1st_he_within_intrchg,
        is_le_in_work_zn,
        is_pictures_taken,
        is_sch_bus_rel,
        is_within_city_lim,
        is_workers_in_work_zn,
        is_work_zn_rel,
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
        inj_cnt,
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
        inj_incapacitating_cnt,
        inj_none_cnt,
        inj_possible_cnt,
        inj_non_incapacitating_cnt,
        inj_fatal_30_cnt,
        inj_fatal_non_traffic_cnt,
        geo_status_cd,
        form_type_cd,
        form_type_tx,
        agncy_rpt_nbr,
        agncy_rpt_nbr_trunc,
        batch_nbr,
        data_src_cd,
        is_complete,
        is_aggressive,
        rpt_dt,
        notif_tm,
        dispatched_tm,
        arrived_tm,
        cleared_tm,
        img_file_nm,
        img_src_nm,
        codeable,
        crash_seg_id,
        nearest_intrsect_id,
        nearest_intrsect_offset_ft,
        nearest_intrsect_offset_dir,
        ref_intrsect_id,
        ref_intrsect_offset_ft,
        ref_intrsect_offset_dir,
        on_network,
        dot_on_sys,
        mapped,
        sym_angle,
        gc_st_nm,
        gc_intrsect_st_nm,
        gc_key_geography,
        gc_is_within_city_lim,
        gc_dot_district_nm,
        gc_rpc_nm,
        gc_mpo_nm,
        gc_cnty_cd,
        gc_cnty_nm,
        gc_city_cd,
        gc_city_nm,
        gps_pt_4326,
        geocode_pt_3087,
        geocode_pt_3857
    )
    SELECT
        hsmv_rpt_nbr,
        hsmv_rpt_nbr_trunc,
        key_crash_dt,
        crash_yr,
        crash_mm,
        crash_mo,
        crash_dd,
        crash_day,
        crash_d,
        key_geography,
        dot_district_nm,
        rpc_nm,
        mpo_nm,
        cnty_cd,
        cnty_nm,
        city_cd,
        city_nm,
        key_rptg_agncy,
        rptg_agncy_nm,
        rptg_agncy_short_nm,
        rptg_agncy_type_nm,
        key_rptg_unit,
        rptg_unit_nm,
        rptg_unit_short_nm,
        key_contrib_circum_env1,
        contrib_circum_env1,
        key_contrib_circum_env2,
        contrib_circum_env2,
        key_contrib_circum_env3,
        contrib_circum_env3,
        key_contrib_circum_rd1,
        contrib_circum_rd1,
        key_contrib_circum_rd2,
        contrib_circum_rd2,
        key_contrib_circum_rd3,
        contrib_circum_rd3,
        key_crash_sev,
        crash_sev,
        key_crash_sev_dtl,
        crash_sev_dtl,
        key_crash_type,
        crash_type,
        crash_type_simplified,
        crash_type_dir_tx,
        key_first_he,
        first_he,
        key_first_he_loc,
        first_he_loc,
        key_first_he_rel_to_jct,
        first_he_rel_to_jct,
        key_light_cond,
        light_cond,
        key_loc_in_work_zn,
        loc_in_work_zn,
        key_manner_of_collision,
        manner_of_collision,
        key_notif_by,
        notif_by,
        key_rd_sys_id,
        rd_sys_id,
        is_public_rd,
        key_rd_surf_cond,
        rd_surf_cond,
        key_type_of_intrsect,
        type_of_intrsect,
        key_type_of_shoulder,
        type_of_shoulder,
        key_type_of_work_zn,
        type_of_work_zn,
        key_weather_cond,
        weather_cond,
        key_bike_ped_crash_type,
        key_bike_ped_crash_group,
        bike_or_ped,
        bike_ped_crash_grp_nbr,
        bike_ped_crash_grp_nm,
        bike_ped_crash_type_nbr,
        bike_ped_crash_type_nm,
        crash_tm,
        crash_hh24mi,
        intrsect_st_nm,
        is_alc_rel,
        is_distracted,
        is_drug_rel,
        is_1st_he_within_intrchg,
        is_le_in_work_zn,
        is_pictures_taken,
        is_sch_bus_rel,
        is_within_city_lim,
        is_workers_in_work_zn,
        is_work_zn_rel,
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
        inj_cnt,
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
        inj_incapacitating_cnt,
        inj_none_cnt,
        inj_possible_cnt,
        inj_non_incapacitating_cnt,
        inj_fatal_30_cnt,
        inj_fatal_non_traffic_cnt,
        geo_status_cd,
        form_type_cd,
        form_type_tx,
        agncy_rpt_nbr,
        agncy_rpt_nbr_trunc,
        batch_nbr,
        data_src_cd,
        is_complete,
        is_aggressive,
        rpt_dt,
        notif_tm,
        dispatched_tm,
        arrived_tm,
        cleared_tm,
        img_file_nm,
        img_src_nm,
        codeable,
        crash_seg_id,
        nearest_intrsect_id,
        nearest_intrsect_offset_ft,
        nearest_intrsect_offset_dir,
        ref_intrsect_id,
        ref_intrsect_offset_ft,
        ref_intrsect_offset_dir,
        on_network,
        dot_on_sys,
        mapped,
        sym_angle,
        gc_st_nm,
        gc_intrsect_st_nm,
        gc_key_geography,
        gc_is_within_city_lim,
        gc_dot_district_nm,
        gc_rpc_nm,
        gc_mpo_nm,
        gc_cnty_cd,
        gc_cnty_nm,
        gc_city_cd,
        gc_city_nm,
        gps_pt_4326,
        geocode_pt_3087,
        sdo_cs.transform(geocode_pt_3087, 3857) AS geocode_pt_3857
    FROM v_flat_crash_evt@s4_warehouse vce
    WHERE v_start_dt IS NULL
    OR vce.last_updt_dt >= v_start_dt;

    COMMIT;
END;
/
