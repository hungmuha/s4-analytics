CREATE OR REPLACE PROCEDURE s4_sync_zlevel
AS
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE zlevel';

    INSERT INTO zlevel (
        link_id,
        point_num,
        node_id,
        z_level,
        intrsect,
        dot_shape,
        aligned,
        shape_3087
    )
    SELECT
        link_id,
        point_num,
        node_id,
        z_level,
        intrsect,
        dot_shape,
        aligned,
        shape_3087
    FROM v_flat_zlevel@s4_warehouse;

    COMMIT;
END;
/
