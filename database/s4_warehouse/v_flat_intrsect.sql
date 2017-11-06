CREATE OR REPLACE VIEW v_flat_intrsect AS
SELECT
    nt.intersection_id,
    nt.intersection_name,
    nt.is_ramp,
    nt.is_rndabout,
    nt.cnty_cd,
    nt.city_cd,
    nt.rd_sys_id,
    nt.rd_sys_interstate,
    nt.rd_sys_us,
    nt.rd_sys_state,
    nt.rd_sys_county,
    nt.rd_sys_local,
    nt.rd_sys_toll,
    nt.rd_sys_forest,
    nt.rd_sys_private,
    nt.rd_sys_pk_lot,
    nt.rd_sys_other,
    shp.geom_type,
    CASE
        WHEN centroid_x IS NULL OR centroid_y IS NULL THEN NULL
        ELSE SDO_GEOMETRY(2001, 3087, mdsys.sdo_point_type(centroid_x, centroid_y, NULL), NULL, NULL)
    END AS centroid_3087,
    CASE WHEN shape IS NULL THEN NULL ELSE SDO_GEOMETRY(sde.st_astext(shape), 3087) END AS shape_3087
FROM navteq_2015q1.intrsect nt
INNER JOIN (
    SELECT 'point' AS geom_type, intersection_id, centroid_x, centroid_y, shape FROM navteq_2015q1.intrsect_pt
    UNION ALL
    SELECT 'line' AS geom_type, intersection_id, centroid_x, centroid_y, shape FROM navteq_2015q1.intrsect_ln
    UNION ALL
    SELECT 'polygon' AS geom_type, intersection_id, centroid_x, centroid_y, shape FROM navteq_2015q1.intrsect_poly
) shp ON shp.intersection_id = nt.intersection_id
;
