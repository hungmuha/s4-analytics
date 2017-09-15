CREATE OR REPLACE PROCEDURE s4_sync_driver (p_days_back INT DEFAULT NULL)
AS
    v_start_dt DATE;
BEGIN
    v_start_dt := CASE WHEN p_days_back IS NOT NULL THEN TRUNC(SYSDATE - p_days_back) ELSE NULL END;

    IF v_start_dt IS NULL THEN
        EXECUTE IMMEDIATE 'TRUNCATE TABLE driver';
    ELSE
        DELETE FROM driver dr
        WHERE EXISTS (
            SELECT 1
            FROM v_flat_driver@s4_warehouse vd
            WHERE vd.last_updt_dt >= v_start_dt
            AND vd.hsmv_rpt_nbr = dr.hsmv_rpt_nbr
        );
    END IF;

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
        rptg_agncy_type,
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
        rptg_agncy_type,
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
    WHERE v_start_dt IS NULL
    OR vd.last_updt_dt >= v_start_dt;

    COMMIT;
END;
/
