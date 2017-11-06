CREATE OR REPLACE PROCEDURE s4_sync_intrsect
AS
    CURSOR ddl_cur IS SELECT
        'ALTER INDEX ' || index_name || ' UNUSABLE' AS unusable_ddl,
        'ALTER INDEX ' || index_name || ' REBUILD' AS rebuild_ddl
    FROM user_indexes
    WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
    AND index_name NOT LIKE 'SYS_%'
    AND index_name NOT LIKE '%_ID_IDX'
    AND table_name = 'INTRSECT';
BEGIN
    -- disable indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.unusable_ddl);
    END LOOP;

    -- skip disabled indexes
    EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';

    EXECUTE IMMEDIATE 'TRUNCATE TABLE intrsect';

    INSERT INTO intrsect (
        intersection_id,
        intersection_name,
        is_ramp,
        is_rndabout,
        cnty_cd,
        city_cd,
        rd_sys_id,
        rd_sys_interstate,
        rd_sys_us,
        rd_sys_state,
        rd_sys_county,
        rd_sys_local,
        rd_sys_toll,
        rd_sys_forest,
        rd_sys_private,
        rd_sys_pk_lot,
        rd_sys_other,
        geom_type,
        centroid_3087,
        shape_3087,
        shape_3857
    )
    SELECT
        intersection_id,
        intersection_name,
        is_ramp,
        is_rndabout,
        cnty_cd,
        city_cd,
        rd_sys_id,
        rd_sys_interstate,
        rd_sys_us,
        rd_sys_state,
        rd_sys_county,
        rd_sys_local,
        rd_sys_toll,
        rd_sys_forest,
        rd_sys_private,
        rd_sys_pk_lot,
        rd_sys_other,
        geom_type,
        centroid_3087,
        shape_3087,
        sdo_cs.transform(shape_3087, 3857) AS shape_3857
    FROM v_flat_intrsect@s4_warehouse;

    COMMIT;

    -- rebuild indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.rebuild_ddl);
    END LOOP;
END;
/
