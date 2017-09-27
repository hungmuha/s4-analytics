CREATE OR REPLACE PROCEDURE s4_sync_violation (p_days_back INT DEFAULT NULL)
AS
    v_start_dt DATE;
    v_rebuild_indexes BOOLEAN;
    CURSOR index_cur IS
        SELECT index_name
        FROM user_indexes
        WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
        AND index_name NOT LIKE 'SYS_%'
        AND index_name NOT LIKE '%_ID_IDX'
        AND table_name = 'VIOLATION';
BEGIN
    v_start_dt := CASE WHEN p_days_back IS NOT NULL THEN TRUNC(SYSDATE - p_days_back) ELSE NULL END;
    v_rebuild_indexes := p_days_back IS NULL OR p_days_back > 30;

    IF v_rebuild_indexes THEN
        -- disable indexes
        FOR rec IN index_cur LOOP
          dbms_utility.exec_ddl_statement('ALTER INDEX ' || rec.index_name || ' UNUSABLE');
        END LOOP;
        -- skip disabled indexes
        EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';
    END IF;

    IF v_start_dt IS NULL THEN
        EXECUTE IMMEDIATE 'TRUNCATE TABLE violation';
    ELSE
        DELETE FROM violation vi
        WHERE EXISTS (
            SELECT 1
            FROM v_flat_violation@s4_warehouse vv
            WHERE vv.last_updt_dt >= v_start_dt
            AND vv.hsmv_rpt_nbr = vi.hsmv_rpt_nbr
        );
    END IF;

    INSERT INTO violation (
        hsmv_rpt_nbr,
        hsmv_rpt_nbr_trunc,
        person_nbr,
        citation_nbr,
        citation_nbr_trunc,
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
        fl_statute_nbr,
        charge,
        batch_nbr
    )
    SELECT
        hsmv_rpt_nbr,
        hsmv_rpt_nbr_trunc,
        person_nbr,
        citation_nbr,
        citation_nbr_trunc,
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
        fl_statute_nbr,
        charge,
        batch_nbr
    FROM v_flat_violation@s4_warehouse vv
    WHERE v_start_dt IS NULL
    OR vv.last_updt_dt >= v_start_dt;

    COMMIT;

    IF v_rebuild_indexes THEN
        -- rebuild indexes
        FOR rec IN index_cur LOOP
          dbms_utility.exec_ddl_statement('ALTER INDEX ' || rec.index_name || ' REBUILD');
        END LOOP;
    END IF;
END;
/
