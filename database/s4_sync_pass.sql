CREATE OR REPLACE PROCEDURE s4_sync_pass (p_sync_id INT)
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
        AND table_name = 'PASS';
BEGIN
    SELECT COUNT(*) INTO v_ct
    FROM sync_crash
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
            SELECT hsmv_rpt_nbr
            FROM sync_crash
            WHERE sync_id = p_sync_id
        )
        LOOP
            v_i := v_i + 1;

            DELETE FROM pass p
            WHERE p.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

            INSERT INTO pass (
                hsmv_rpt_nbr,
                hsmv_rpt_nbr_trunc,
                veh_nbr,
                person_nbr,
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
                key_age_rng,
                age_rng,
                key_airbag_deployed,
                airbag_deployed,
                key_ejection,
                ejection,
                key_gender,
                gender,
                key_helmet_use,
                helmet_use,
                key_inj_sev,
                inj_sev,
                key_restraint_sys,
                restraint_sys,
                key_seating_other,
                seating_other,
                key_seating_row,
                seating_row,
                key_seating_seat,
                seating_seat,
                key_src_of_trans,
                src_of_trans,
                key_veh_body_type,
                veh_body_type,
                addr_city,
                addr_state,
                addr_zip,
                dl_state,
                is_using_eye_protection,
                fatality_cnt,
                fatality_unrestrained_cnt,
                inj_cnt,
                inj_unrestrained_cnt,
                citation_cnt,
                citation_amt,
                prop_dmg_cnt,
                batch_nbr,
                inj_incapacitating_cnt,
                prop_dmg_amt
            )
            SELECT
                hsmv_rpt_nbr,
                hsmv_rpt_nbr_trunc,
                veh_nbr,
                person_nbr,
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
                key_age_rng,
                age_rng,
                key_airbag_deployed,
                airbag_deployed,
                key_ejection,
                ejection,
                key_gender,
                gender,
                key_helmet_use,
                helmet_use,
                key_inj_sev,
                inj_sev,
                key_restraint_sys,
                restraint_sys,
                key_seating_other,
                seating_other,
                key_seating_row,
                seating_row,
                key_seating_seat,
                seating_seat,
                key_src_of_trans,
                src_of_trans,
                key_veh_body_type,
                veh_body_type,
                addr_city,
                addr_state,
                addr_zip,
                dl_state,
                is_using_eye_protection,
                fatality_cnt,
                fatality_unrestrained_cnt,
                inj_cnt,
                inj_unrestrained_cnt,
                citation_cnt,
                citation_amt,
                prop_dmg_cnt,
                batch_nbr,
                inj_incapacitating_cnt,
                prop_dmg_amt
            FROM v_flat_pass@s4_warehouse vp
            WHERE vp.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

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
