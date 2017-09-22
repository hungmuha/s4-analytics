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
END;
/
