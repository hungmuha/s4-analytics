CREATE OR REPLACE PROCEDURE s4_sync_intrsect_node
AS
    CURSOR ddl_cur IS SELECT
        'ALTER INDEX ' || index_name || ' UNUSABLE' AS unusable_ddl,
        'ALTER INDEX ' || index_name || ' REBUILD' AS rebuild_ddl
    FROM user_indexes
    WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
    AND index_name NOT LIKE 'SYS_%'
    AND index_name NOT LIKE '%_ID_IDX'
    AND table_name = 'INTRSECT_NODE';
BEGIN
    -- disable indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.unusable_ddl);
    END LOOP;

    -- skip disabled indexes
    EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';

    EXECUTE IMMEDIATE 'TRUNCATE TABLE intrsect_node';

    INSERT INTO intrsect_node (
        node_id,
        intersection_id
    )
    SELECT
        node_id,
        intersection_id
    FROM v_flat_intrsect_node@s4_warehouse;

    COMMIT;

    -- rebuild indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.rebuild_ddl);
    END LOOP;
END;
/
