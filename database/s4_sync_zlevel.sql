CREATE OR REPLACE PROCEDURE s4_sync_zlevel
AS
    CURSOR ddl_cur IS SELECT
        'ALTER INDEX ' || index_name || ' UNUSABLE' AS unusable_ddl,
        'ALTER INDEX ' || index_name || ' REBUILD' AS rebuild_ddl
    FROM user_indexes
    WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
    AND index_name NOT LIKE 'SYS_%'
    AND index_name NOT LIKE '%_ID_IDX'
    AND table_name = 'ZLEVEL';
BEGIN
    -- disable indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.unusable_ddl);
    END LOOP;

    -- skip disabled indexes
    EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';

    EXECUTE IMMEDIATE 'TRUNCATE TABLE zlevel';

    INSERT INTO zlevel (
        link_id,
        point_num,
        node_id,
        z_level,
        intrsect,
        dot_shape,
        aligned,
        shape_3087,
        shape_3857
    )
    SELECT
        link_id,
        point_num,
        node_id,
        z_level,
        intrsect,
        dot_shape,
        aligned,
        shape_3087,
        sdo_cs.transform(shape_3087, 3857) AS shape_3857
    FROM v_flat_zlevel@s4_warehouse;

    COMMIT;

    -- rebuild indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.rebuild_ddl);
    END LOOP;
END;
/
