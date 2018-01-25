CREATE OR REPLACE PROCEDURE s4_sync_driver (p_sync_id INT)
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
        AND table_name = 'DRIVER';
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

            DELETE FROM driver d
            WHERE d.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

            INSERT INTO driver (
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
                key_action1,
                action1,
                key_action2,
                action2,
                key_action3,
                action3,
                key_action4,
                action4,
                key_age_rng,
                age_rng,
                key_airbag_deployed,
                airbag_deployed,
                key_alc_test_result,
                alc_test_result,
                key_alc_test_type,
                alc_test_type,
                key_alc_tested,
                alc_tested,
                key_cond_at_tm_of_crash,
                cond_at_tm_of_crash,
                key_distracted_by,
                distracted_by,
                key_dl_endorsements,
                dl_endorsements,
                key_dl_type,
                dl_type,
                key_drug_test_result,
                drug_test_result,
                key_drug_test_type,
                drug_test_type,
                key_drug_tested,
                drug_tested,
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
                key_src_of_trans,
                src_of_trans,
                key_veh_body_type,
                veh_body_type,
                key_vision_obstruction,
                vision_obstruction,
                addr_city,
                addr_state,
                addr_zip,
                bac,
                dl_state,
                ins_co,
                is_alc_use_suspected,
                is_distracted,
                is_drug_use_suspected,
                is_using_eye_protection,
                is_re_exam_recommended,
                fatality_cnt,
                fatality_unrestrained_cnt,
                inj_cnt,
                inj_unrestrained_cnt,
                citation_cnt,
                citation_amt,
                prop_dmg_cnt,
                prop_dmg_amt,
                inj_incapacitating_cnt,
                batch_nbr
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
                key_action1,
                action1,
                key_action2,
                action2,
                key_action3,
                action3,
                key_action4,
                action4,
                key_age_rng,
                age_rng,
                key_airbag_deployed,
                airbag_deployed,
                key_alc_test_result,
                alc_test_result,
                key_alc_test_type,
                alc_test_type,
                key_alc_tested,
                alc_tested,
                key_cond_at_tm_of_crash,
                cond_at_tm_of_crash,
                key_distracted_by,
                distracted_by,
                key_dl_endorsements,
                dl_endorsements,
                key_dl_type,
                dl_type,
                key_drug_test_result,
                drug_test_result,
                key_drug_test_type,
                drug_test_type,
                key_drug_tested,
                drug_tested,
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
                key_src_of_trans,
                src_of_trans,
                key_veh_body_type,
                veh_body_type,
                key_vision_obstruction,
                vision_obstruction,
                addr_city,
                addr_state,
                addr_zip,
                bac,
                dl_state,
                ins_co,
                is_alc_use_suspected,
                is_distracted,
                is_drug_use_suspected,
                is_using_eye_protection,
                is_re_exam_recommended,
                fatality_cnt,
                fatality_unrestrained_cnt,
                inj_cnt,
                inj_unrestrained_cnt,
                citation_cnt,
                citation_amt,
                prop_dmg_cnt,
                prop_dmg_amt,
                inj_incapacitating_cnt,
                batch_nbr
            FROM v_flat_driver@s4_warehouse vd
            WHERE vd.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

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
