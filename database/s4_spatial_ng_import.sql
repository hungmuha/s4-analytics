INSERT INTO geocode_result (
  HSMV_RPT_NBR,
  CRASH_SEG_ID,
  NEAREST_INTRSECT_ID,
  NEAREST_INTRSECT_OFFSET_FT,
  NEAREST_INTRSECT_OFFSET_DIR,
  REF_INTRSECT_ID,
  REF_INTRSECT_OFFSET_FT,
  REF_INTRSECT_OFFSET_DIR,
  UPDT_DIR_TRAVEL,
  ON_NETWORK,
  DOT_ON_SYS,
  KEY_GEOGRAPHY,
  CITY_CD,
  CNTY_CD,
  MAPPED,
  SHAPE)
SELECT
  HSMV_RPT_NBR,
  CRASH_SEG_ID,
  NEAREST_INTRSECT_ID,
  NEAREST_INTRSECT_OFFSET_FT,
  NEAREST_INTRSECT_OFFSET_DIR,
  REF_INTRSECT_ID,
  REF_INTRSECT_OFFSET_FT,
  REF_INTRSECT_OFFSET_DIR,
  UPDT_DIR_TRAVEL,
  ON_NETWORK,
  DOT_ON_SYS,
  KEY_GEOGRAPHY,
  CITY_CD,
  CNTY_CD,
  MAPPED,
  SHAPE
FROM v_geocode_result_for_sdo@lime_navteq_2015q1 gr
WHERE EXISTS (
  SELECT 1
  FROM s4_warehouse_ng.fact_crash_evt fce
  WHERE fce.hsmv_rpt_nbr = gr.hsmv_rpt_nbr
);

UPDATE geocode_result
SET shape_merc = sdo_cs.transform(shape, 3857);

INSERT INTO s_citation (
  CITATION_NBR,
  MPO_BND_ID,
  SHAPE)
SELECT
  CITATION_NBR,
  MPO_BND_ID,
  SHAPE
FROM v_st_citation_for_sdo@lime_navteq_2015q1 sc
WHERE EXISTS (
  SELECT 1
  FROM s4_warehouse_ng.citation ci
  WHERE ci.citation_nbr = sc.citation_nbr
);

UPDATE s_citation
SET shape_merc = sdo_cs.transform(shape, 3857);

INSERT INTO st (
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
  CNTY_CD)
SELECT
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
  CNTY_CD
FROM v_st_for_sdo@lime_navteq_2015q1;

INSERT INTO intrsect (
  INTERSECTION_ID,
  INTERSECTION_NAME,
  INTERSECTION_GEOM_TYPE,
  NODE_ORIENTED_ID,
  IS_RAMP,
  IS_RNDABOUT,
  CNTY_CD,
  CITY_CD,
  RD_SYS_ID,
  RD_SYS_INTERSTATE,
  RD_SYS_US,
  RD_SYS_STATE,
  RD_SYS_COUNTY,
  RD_SYS_LOCAL,
  RD_SYS_TOLL,
  RD_SYS_FOREST,
  RD_SYS_PRIVATE,
  RD_SYS_PK_LOT,
  RD_SYS_OTHER)
SELECT
  INTERSECTION_ID,
  INTERSECTION_NAME,
  INTERSECTION_GEOM_TYPE,
  NODE_ORIENTED_ID,
  IS_RAMP,
  IS_RNDABOUT,
  CNTY_CD,
  CITY_CD,
  RD_SYS_ID,
  RD_SYS_INTERSTATE,
  RD_SYS_US,
  RD_SYS_STATE,
  RD_SYS_COUNTY,
  RD_SYS_LOCAL,
  RD_SYS_TOLL,
  RD_SYS_FOREST,
  RD_SYS_PRIVATE,
  RD_SYS_PK_LOT,
  RD_SYS_OTHER
FROM intrsect@lime_navteq_2015q1;

INSERT INTO intrsect_node (
  NODE_ID,
  INTERSECTION_ID)
SELECT
  NODE_ID,
  INTERSECTION_ID
FROM intrsect_node@lime_navteq_2015q1;

INSERT INTO st_ext (
  link_id,
  roadway,
  dot_funclass,
  dot_on_sys,
  fhp_bnd_id,
  dot_bnd_id,
  mpo_bnd_id,
  cnty_bnd_id,
  city_bnd_id,
  centroid,
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
  rd_sys_other)
SELECT
  link_id,
  roadway,
  dot_funclass,
  dot_on_sys,
  fhp_bnd_id,
  dot_bnd_id,
  mpo_bnd_id,
  cnty_bnd_id,
  city_bnd_id,
  centroid,
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
FROM v_st_ext_for_sdo@lime_navteq_2015q1;

COMMIT;
