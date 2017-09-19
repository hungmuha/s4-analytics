CREATE OR REPLACE PROCEDURE s4_sync_non_motorist (p_days_back INT DEFAULT NULL)
AS
    v_start_dt DATE;
BEGIN
    v_start_dt := CASE WHEN p_days_back IS NOT NULL THEN TRUNC(SYSDATE - p_days_back) ELSE NULL END;

    IF v_start_dt IS NULL THEN
        EXECUTE IMMEDIATE 'TRUNCATE TABLE non_motorist';
    ELSE
        DELETE FROM non_motorist nm
        WHERE EXISTS (
            SELECT 1
            FROM v_flat_non_motorist@s4_warehouse vnm
            WHERE vnm.last_updt_dt >= v_start_dt
            AND vnm.hsmv_rpt_nbr = nm.hsmv_rpt_nbr
        );
    END IF;

    INSERT INTO non_motorist (
        hsmv_rpt_nbr,
        hsmv_rpt_nbr_trunc,
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
        key_action_prior,
        action_prior,
        key_action_circum1,
        action_circum1,
        key_action_circum2,
        action_circum2,
        key_age_rng,
        age_rng,
        key_alc_test_result,
        alc_test_result,
        key_alc_test_type,
        alc_test_type,
        key_alc_tested,
        alc_tested,
        key_desc,
        "DESC",
        key_drug_test_result,
        drug_test_result,
        key_drug_test_type,
        drug_test_type,
        key_drug_tested,
        drug_tested,
        key_gender,
        gender,
        key_inj_sev,
        inj_sev,
        key_loc_at_tm_of_crash,
        loc_at_tm_of_crash,
        key_safety_eq1,
        safety_eq1,
        key_safety_eq2,
        safety_eq2,
        key_src_of_trans,
        src_of_trans,
        addr_city,
        addr_state,
        addr_zip,
        bac,
        dl_state,
        is_alc_use_suspected,
        is_drug_use_suspected,
        bike_cnt,
        ped_cnt,
        fatality_cnt,
        inj_cnt,
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
        key_action_prior,
        action_prior,
        key_action_circum1,
        action_circum1,
        key_action_circum2,
        action_circum2,
        key_age_rng,
        age_rng,
        key_alc_test_result,
        alc_test_result,
        key_alc_test_type,
        alc_test_type,
        key_alc_tested,
        alc_tested,
        key_desc,
        "DESC",
        key_drug_test_result,
        drug_test_result,
        key_drug_test_type,
        drug_test_type,
        key_drug_tested,
        drug_tested,
        key_gender,
        gender,
        key_inj_sev,
        inj_sev,
        key_loc_at_tm_of_crash,
        loc_at_tm_of_crash,
        key_safety_eq1,
        safety_eq1,
        key_safety_eq2,
        safety_eq2,
        key_src_of_trans,
        src_of_trans,
        addr_city,
        addr_state,
        addr_zip,
        bac,
        dl_state,
        is_alc_use_suspected,
        is_drug_use_suspected,
        bike_cnt,
        ped_cnt,
        fatality_cnt,
        inj_cnt,
        citation_cnt,
        citation_amt,
        prop_dmg_cnt,
        prop_dmg_amt,
        inj_incapacitating_cnt,
        batch_nbr
    FROM v_flat_non_motorist@s4_warehouse vnm
    WHERE v_start_dt IS NULL
    OR vnm.last_updt_dt >= v_start_dt;

    COMMIT;
END;
/
