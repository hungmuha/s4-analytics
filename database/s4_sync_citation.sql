CREATE OR REPLACE PROCEDURE s4_sync_citation (p_sync_id INT)
AS
    v_ct INT;
    v_rebuild_indexes BOOLEAN;
    v_i INT := 0;
    CURSOR index_cur IS
        SELECT index_name
        FROM user_indexes
        WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
        AND index_name NOT LIKE 'SYS_%'
        AND index_name NOT LIKE '%_ID_IDX'
        AND table_name = 'CITATION';
BEGIN
    SELECT COUNT(*) INTO v_ct
    FROM sync_citation
    WHERE sync_id = p_sync_id;

    v_rebuild_indexes := v_ct > 5000;

    IF v_rebuild_indexes THEN
        -- disable indexes
        FOR rec IN index_cur LOOP
          dbms_utility.exec_ddl_statement('ALTER INDEX ' || rec.index_name || ' UNUSABLE');
        END LOOP;
        -- skip disabled indexes
        EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';
    END IF;

    BEGIN
        FOR ws IN (
            SELECT citation_nbr
            FROM sync_citation
            WHERE sync_id = p_sync_id
        )
        LOOP
            v_i := v_i + 1;

            DELETE FROM citation ci
            WHERE ci.citation_nbr = ws.citation_nbr;

            INSERT INTO citation (
                citation_nbr,
                citation_nbr_trunc,
                check_digit,
                key_citation_dt,
                citation_yr,
                citation_mm,
                citation_mo,
                citation_dd,
                citation_day,
                key_geography,
                dot_district_nm,
                rpc_nm,
                mpo_nm,
                cnty_cd,
                cnty_nm,
                city_cd,
                city_nm,
                key_agncy,
                agncy_nm,
                agncy_short_nm,
                agncy_type_nm,
                key_driver_age_rng,
                driver_age_rng,
                key_violation,
                sect_nbr,
                sect_nbr_dsp,
                sub_sect_nbr,
                sub_sect_nbr_dsp,
                violation_desc,
                violation_class,
                violation_type,
                offense_dt,
                driver_addr_diff_than_dl,
                driver_addr_city_nm,
                driver_addr_state_cd,
                driver_addr_zip_cd,
                driver_age_nbr,
                driver_race_cd,
                driver_gender_cd,
                driver_height_tx,
                dl_state_cd,
                dl_class_cd,
                dl_expir_yr,
                comm_veh_cd,
                veh_yr,
                veh_make_tx,
                veh_style_tx,
                veh_color_tx,
                hazmat_cd,
                veh_state_cd,
                veh_tag_expir_yr,
                companion_citation_cd,
                violation_loc_tx,
                offset_feet_meas,
                offset_miles_meas,
                offset_dir_cd,
                offset_from_node_id,
                actual_speed_meas,
                posted_speed_meas,
                hwy_4lane_cd,
                hwy_intrstate_cd,
                violation_careless_cd,
                violation_device_cd,
                violation_row_cd,
                violation_lane_cd,
                violation_passing_cd,
                violation_child_restraint_cd,
                violation_dui_cd,
                driver_bal_meas,
                violation_seatbelt_cd,
                violation_equip_cd,
                violation_tag_less_cd,
                violation_tag_more_cd,
                violation_ins_cd,
                violation_expir_dl_cd,
                violation_expir_dl_more_cd,
                violation_no_valid_dl_cd,
                violation_susp_dl_cd,
                oth_comments_tx,
                fl_dl_edit_override_cd,
                state_statute_cd,
                crash_cd,
                prop_dmg_cd,
                prop_dmg_amt,
                inj_cd,
                serious_inj_cd,
                fatal_inj_cd,
                method_of_arrest_cd,
                criminal_appear_reqd_cd,
                infraction_appear_reqd_cd,
                infraction_no_appear_reqd_cd,
                court_dt,
                court_nm,
                court_addr_tx,
                court_city_nm,
                court_state_cd,
                court_zip_cd,
                arrest_delivered_to_tx,
                arrest_delivered_dt,
                ofcr_rank_tx,
                trooper_unit_tx,
                bal_008_or_above_cd,
                dui_refuse_cd,
                dui_lic_surrendered_cd,
                dui_lic_rsn_tx,
                dui_eligible_cd,
                dui_eligible_rsn_tx,
                dui_bar_ofc_tx,
                status_cd,
                aggressive_driver_cd,
                criminal_cd,
                fine_amt,
                issue_arrest_dt,
                ofcr_dlvry_verif_cd,
                due_dt,
                motorcycle_cd,
                veh_16_pass_cd,
                ofcr_re_exam_cd,
                dui_pass_under_18_cd,
                e_citation_cd,
                nm_chg_cd,
                comm_dl_cd,
                violation_sig_red_light_cd,
                violation_workers_present_cd,
                violation_handheld_cd,
                violation_sch_zn_cd,
                perm_reg_cd,
                compliance_dt,
                speed_meas_device_id,
                dl_seize_cd,
                business_cd,
                source_format_cd,
                addr_used_cd,
                gps_pt_4326,
                geocode_pt_3087,
                geocode_pt_3857
            )
            SELECT
                citation_nbr,
                citation_nbr_trunc,
                check_digit,
                key_citation_dt,
                citation_yr,
                citation_mm,
                citation_mo,
                citation_dd,
                citation_day,
                key_geography,
                dot_district_nm,
                rpc_nm,
                mpo_nm,
                cnty_cd,
                cnty_nm,
                city_cd,
                city_nm,
                key_agncy,
                agncy_nm,
                agncy_short_nm,
                agncy_type_nm,
                key_driver_age_rng,
                driver_age_rng,
                key_violation,
                sect_nbr,
                sect_nbr_dsp,
                sub_sect_nbr,
                sub_sect_nbr_dsp,
                violation_desc,
                violation_class,
                violation_type,
                offense_dt,
                driver_addr_diff_than_dl,
                driver_addr_city_nm,
                driver_addr_state_cd,
                driver_addr_zip_cd,
                driver_age_nbr,
                driver_race_cd,
                driver_gender_cd,
                driver_height_tx,
                dl_state_cd,
                dl_class_cd,
                dl_expir_yr,
                comm_veh_cd,
                veh_yr,
                veh_make_tx,
                veh_style_tx,
                veh_color_tx,
                hazmat_cd,
                veh_state_cd,
                veh_tag_expir_yr,
                companion_citation_cd,
                violation_loc_tx,
                offset_feet_meas,
                offset_miles_meas,
                offset_dir_cd,
                offset_from_node_id,
                actual_speed_meas,
                posted_speed_meas,
                hwy_4lane_cd,
                hwy_intrstate_cd,
                violation_careless_cd,
                violation_device_cd,
                violation_row_cd,
                violation_lane_cd,
                violation_passing_cd,
                violation_child_restraint_cd,
                violation_dui_cd,
                driver_bal_meas,
                violation_seatbelt_cd,
                violation_equip_cd,
                violation_tag_less_cd,
                violation_tag_more_cd,
                violation_ins_cd,
                violation_expir_dl_cd,
                violation_expir_dl_more_cd,
                violation_no_valid_dl_cd,
                violation_susp_dl_cd,
                oth_comments_tx,
                fl_dl_edit_override_cd,
                state_statute_cd,
                crash_cd,
                prop_dmg_cd,
                prop_dmg_amt,
                inj_cd,
                serious_inj_cd,
                fatal_inj_cd,
                method_of_arrest_cd,
                criminal_appear_reqd_cd,
                infraction_appear_reqd_cd,
                infraction_no_appear_reqd_cd,
                court_dt,
                court_nm,
                court_addr_tx,
                court_city_nm,
                court_state_cd,
                court_zip_cd,
                arrest_delivered_to_tx,
                arrest_delivered_dt,
                ofcr_rank_tx,
                trooper_unit_tx,
                bal_008_or_above_cd,
                dui_refuse_cd,
                dui_lic_surrendered_cd,
                dui_lic_rsn_tx,
                dui_eligible_cd,
                dui_eligible_rsn_tx,
                dui_bar_ofc_tx,
                status_cd,
                aggressive_driver_cd,
                criminal_cd,
                fine_amt,
                issue_arrest_dt,
                ofcr_dlvry_verif_cd,
                due_dt,
                motorcycle_cd,
                veh_16_pass_cd,
                ofcr_re_exam_cd,
                dui_pass_under_18_cd,
                e_citation_cd,
                nm_chg_cd,
                comm_dl_cd,
                violation_sig_red_light_cd,
                violation_workers_present_cd,
                violation_handheld_cd,
                violation_sch_zn_cd,
                perm_reg_cd,
                compliance_dt,
                speed_meas_device_id,
                dl_seize_cd,
                business_cd,
                source_format_cd,
                addr_used_cd,
                gps_pt_4326,
                geocode_pt_3087,
                sdo_cs.transform(geocode_pt_3087, 3857) AS geocode_pt_3857
            FROM v_flat_citation@s4_warehouse vc
            WHERE vc.citation_nbr = ws.citation_nbr;

            IF MOD(v_i, 10000) = 0 THEN
                COMMIT;
            END IF;
        END LOOP;
    END;

    COMMIT;

    IF v_rebuild_indexes THEN
        -- rebuild indexes
        FOR rec IN index_cur LOOP
          dbms_utility.exec_ddl_statement('ALTER INDEX ' || rec.index_name || ' REBUILD');
        END LOOP;
    END IF;
END;
/
