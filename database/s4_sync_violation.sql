CREATE OR REPLACE PROCEDURE s4_sync_violation (p_sync_id INT)
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
        AND table_name = 'VIOLATION';
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

            DELETE FROM violation v
            WHERE v.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

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
            WHERE vv.hsmv_rpt_nbr = ws.hsmv_rpt_nbr;

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
