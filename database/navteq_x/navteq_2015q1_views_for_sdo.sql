CREATE OR REPLACE VIEW v_geocode_result_for_sdo
AS SELECT
  hsmv_rpt_nbr,
  crash_seg_id,
  nearest_intrsect_id,
  nearest_intrsect_offset_ft,
  nearest_intrsect_offset_dir,
  ref_intrsect_id,
  ref_intrsect_offset_ft,
  ref_intrsect_offset_dir,
  updt_dir_travel,
  on_network,
  dot_on_sys,
  key_geography,
  city_cd,
  cnty_cd,
  mapped,
  CASE WHEN shape IS NULL THEN NULL ELSE sdo_geometry(sde.st_astext(shape), 3087) END AS shape
FROM geocode_result;

CREATE OR REPLACE VIEW v_st_citation_for_sdo
AS SELECT
  objectid,
  citation_nbr,
  CASE WHEN shape IS NULL THEN NULL ELSE sdo_geometry(sde.st_astext(shape), 3087) END AS shape,
  mpo_bnd_id
FROM st_citation;

CREATE OR REPLACE VIEW v_st_for_sdo
AS SELECT
  LINK_ID,
  ST_NAME,
  ST_NM_PREF,
  ST_TYP_BEF,
  ST_NM_BASE,
  ST_NM_SUFF,
  ST_TYP_AFT,
  ST_TYP_ATT,
  REF_IN_ID,
  NREF_IN_ID,
  DIRONSIGN,
  CITY_CD,
  CNTY_CD,
  CASE WHEN shape IS NULL THEN NULL ELSE sdo_geometry(sde.st_astext(shape), 3087) END AS shape
FROM st;

CREATE OR REPLACE VIEW v_st_ext_for_sdo
AS SELECT
  link_id,
  roadway,
  dot_funclass,
  dot_on_sys,
  fhp_bnd_id,
  dot_bnd_id,
  mpo_bnd_id,
  cnty_bnd_id,
  city_bnd_id,
  CASE
    WHEN centroid_x IS NULL OR centroid_y IS NULL THEN NULL
    ELSE sdo_geometry(2001, 3087, mdsys.sdo_point_type(centroid_x, centroid_y, NULL), NULL, NULL)
  END AS centroid,
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
  rd_sys_other
FROM st_ext;
