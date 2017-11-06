CREATE OR REPLACE VIEW v_flat_zlevel AS
SELECT
    link_id,
    point_num,
    node_id,
    z_level,
    intrsect,
    dot_shape,
    aligned,
    sdo_geometry(sde.st_astext(shape), 3087) AS shape_3087
FROM navteq_2015q1.zlevel
;
