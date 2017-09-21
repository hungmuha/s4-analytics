CREATE OR REPLACE PROCEDURE s4_sync_intrsect_node
AS
BEGIN
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
END;
/
