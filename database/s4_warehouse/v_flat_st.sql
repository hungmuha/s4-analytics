CREATE OR REPLACE VIEW v_flat_st AS
SELECT
  st.link_id,
  st.st_name,
  st.st_nm_pref,
  st.st_typ_bef,
  st.st_nm_base,
  st.st_nm_suff,
  st.st_typ_aft,
  st.st_typ_att,
  st.ref_in_id,
  st.nref_in_id,
  st.dironsign,
  st.city_cd,
  st.cnty_cd,
  ste.roadway,
  ste.dot_funclass,
  ste.dot_on_sys,
  ste.fhp_bnd_id,
  ste.dot_bnd_id,
  ste.mpo_bnd_id,
  ste.cnty_bnd_id,
  ste.city_bnd_id,
  ste.rd_sys_id,
  ste.rd_sys_interstate,
  ste.rd_sys_us,
  ste.rd_sys_state,
  ste.rd_sys_county,
  ste.rd_sys_local,
  ste.rd_sys_toll,
  ste.rd_sys_forest,
  ste.rd_sys_private,
  ste.rd_sys_pk_lot,
  ste.rd_sys_other,
  CASE
    WHEN centroid_x IS NULL OR centroid_y IS NULL THEN NULL
    ELSE SDO_GEOMETRY(2001, 3087, mdsys.sdo_point_type(centroid_x, centroid_y, NULL), NULL, NULL)
  END AS centroid_3087,
  CASE WHEN shape IS NULL THEN NULL ELSE SDO_GEOMETRY(sde.st_astext(shape), 3087) END AS shape_3087
FROM navteq_2015q1.st st
LEFT JOIN navteq_2015q1.st_ext ste ON ste.link_id = st.link_id
;
