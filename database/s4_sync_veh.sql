CREATE OR REPLACE PROCEDURE s4_sync_veh (p_sync_id INT)
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
        AND table_name = 'VEH';
BEGIN
    SELECT COUNT(*) INTO v_ct
    FROM sync_crash
    WHERE sync_id = p_sync_id;

    v_rebuild_indexes := v_ct > 2500;

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
            SELECT hsmv_rpt_nbr
            FROM sync_crash
            WHERE sync_id = p_sync_id
        )
        LOOP
            v_i := v_i + 1;

            DELETE FROM veh
            WHERE veh.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

            INSERT INTO veh (
                hsmv_rpt_nbr,
                hsmv_rpt_nbr_trunc,
                veh_nbr,
                key_crash_dt,
                crash_yr,
                crash_mm,
                crash_mo,
                crash_dd,
                crash_day,
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
                key_ar_of_init_impact,
                ar_of_init_impact,
                key_body_type,
                body_type,
                key_cargo_body_type,
                cargo_body_type,
                key_cmv_config,
                cmv_config,
                key_comm_non_comm,
                comm_non_comm,
                key_dmg_extent,
                dmg_extent,
                key_dir_before,
                dir_before,
                key_gvwr_gcwr,
                gvwr_gcwr,
                key_he1,
                he1,
                key_he2,
                he2,
                key_he3,
                he3,
                key_he4,
                he4,
                key_maneuver_action,
                maneuver_action,
                key_most_dmg_ar,
                most_dmg_ar,
                key_most_he,
                most_he,
                key_rd_align,
                rd_align,
                key_rd_grade,
                rd_grade,
                key_special_func,
                special_func,
                key_traffic_ctl,
                traffic_ctl,
                key_trafficway,
                trafficway,
                key_veh_defect1,
                veh_defect1,
                key_veh_defect2,
                veh_defect2,
                key_veh_type,
                veh_type,
                key_wrecker_sel,
                wrecker_sel,
                est_speed,
                is_comm_veh,
                is_emerg_veh,
                is_hazmat_released,
                is_hit_and_run,
                is_owner_a_business,
                is_perm_reg,
                is_towed_due_to_dmg,
                motor_carrier_city,
                motor_carrier_state,
                motor_carrier_zip,
                placard_hazmat_class,
                posted_speed,
                reg_state,
                tot_lanes,
                traveling_on_st,
                veh_color,
                veh_make,
                veh_model,
                veh_owner_city,
                veh_owner_state,
                veh_owner_zip,
                veh_style,
                veh_yr,
                moped_cnt,
                motorcycle_cnt,
                pass_cnt,
                trailer_cnt,
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
                inj_incapacitating_cnt,
                batch_nbr,
                inj_none_cnt,
                inj_possible_cnt,
                inj_non_incapacitating_cnt,
                inj_fatal_30_cnt,
                inj_fatal_non_traffic_cnt
            )
            SELECT
                hsmv_rpt_nbr,
                hsmv_rpt_nbr_trunc,
                veh_nbr,
                key_crash_dt,
                crash_yr,
                crash_mm,
                crash_mo,
                crash_dd,
                crash_day,
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
                key_ar_of_init_impact,
                ar_of_init_impact,
                key_body_type,
                body_type,
                key_cargo_body_type,
                cargo_body_type,
                key_cmv_config,
                cmv_config,
                key_comm_non_comm,
                comm_non_comm,
                key_dmg_extent,
                dmg_extent,
                key_dir_before,
                dir_before,
                key_gvwr_gcwr,
                gvwr_gcwr,
                key_he1,
                he1,
                key_he2,
                he2,
                key_he3,
                he3,
                key_he4,
                he4,
                key_maneuver_action,
                maneuver_action,
                key_most_dmg_ar,
                most_dmg_ar,
                key_most_he,
                most_he,
                key_rd_align,
                rd_align,
                key_rd_grade,
                rd_grade,
                key_special_func,
                special_func,
                key_traffic_ctl,
                traffic_ctl,
                key_trafficway,
                trafficway,
                key_veh_defect1,
                veh_defect1,
                key_veh_defect2,
                veh_defect2,
                key_veh_type,
                veh_type,
                key_wrecker_sel,
                wrecker_sel,
                est_speed,
                is_comm_veh,
                is_emerg_veh,
                is_hazmat_released,
                is_hit_and_run,
                is_owner_a_business,
                is_perm_reg,
                is_towed_due_to_dmg,
                motor_carrier_city,
                motor_carrier_state,
                motor_carrier_zip,
                placard_hazmat_class,
                posted_speed,
                reg_state,
                tot_lanes,
                traveling_on_st,
                veh_color,
                veh_make,
                veh_model,
                veh_owner_city,
                veh_owner_state,
                veh_owner_zip,
                veh_style,
                veh_yr,
                moped_cnt,
                motorcycle_cnt,
                pass_cnt,
                trailer_cnt,
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
                inj_incapacitating_cnt,
                batch_nbr,
                inj_none_cnt,
                inj_possible_cnt,
                inj_non_incapacitating_cnt,
                inj_fatal_30_cnt,
                inj_fatal_non_traffic_cnt
            FROM v_flat_veh@s4_warehouse vv
            WHERE vv.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

            IF MOD(v_i, 1000) = 0 THEN
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
