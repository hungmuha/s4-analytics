--------------------------------------------------------
--  DDL for Table GEOCODE_RESULT
--------------------------------------------------------

  CREATE TABLE "GEOCODE_RESULT"
   (	"OBJECTID" NUMBER(*,0),
	"HSMV_RPT_NBR" NUMBER(10,0),
	"SCORE_NBR" NUMBER(5,0),
	"LOCATION_TYPE_CD" NUMBER(5,0),
	"MATCH_STATUS_CD" NVARCHAR2(1),
	"MATCH_RESULT_CD" NUMBER(5,0),
	"STD_ADDR_TX" NVARCHAR2(150),
	"MATCH_ADDR_TX" NVARCHAR2(150),
	"ADDR_USED_CD" NUMBER(5,0),
	"CITY_USED_CD" NVARCHAR2(1),
	"BYPASS_USED_CD" NVARCHAR2(1),
	"CUSTOM_GEOCODER_USED_CD" NVARCHAR2(1),
	"UNMATCHED_DESC_CD" NUMBER(5,0),
	"MAP_POINT_X" NUMBER(38,12),
	"MAP_POINT_Y" NUMBER(38,12),
	"MAP_POINT_SRID" NVARCHAR2(15),
	"CENTER_LINE_X" NUMBER(38,12),
	"CENTER_LINE_Y" NUMBER(38,12),
	"CENTER_LINE_SRID" NVARCHAR2(15),
	"CRASH_SEG_ID" NUMBER(10,0),
	"NEAREST_INTRSECT_ID" NUMBER(10,0),
	"NEAREST_INTRSECT_OFFSET_FT" NUMBER(10,0),
	"NEAREST_INTRSECT_OFFSET_DIR" NUMBER(5,0),
	"REF_INTRSECT_ID" NUMBER(10,0),
	"REF_INTRSECT_OFFSET_FT" NUMBER(10,0),
	"REF_INTRSECT_OFFSET_DIR" NUMBER(5,0),
	"UPDT_DIR_TRAVEL" NUMBER(5,0),
	"LAST_UPDT_USER_ID" NVARCHAR2(30),
	"LAST_UPDT_DT" DATE,
	"SYM_ANGLE" NUMBER(5,0),
	"SOURCE_FORMAT_CD" NUMBER(5,0),
	"GEOCODE_ENGINE_CD" NUMBER(5,0),
	"METHOD_USED_CD" NUMBER(5,0),
	"ON_NETWORK" NVARCHAR2(1),
	"DOT_ON_SYS" NVARCHAR2(1),
	"KEY_GEOGRAPHY" NUMBER(10,0),
	"CITY_CD" NUMBER(5,0),
	"CNTY_CD" NUMBER(5,0),
	"MAPPED" NVARCHAR2(1),
	"BATCH_NBR" NUMBER(10,0),
	"SHAPE" SDO_GEOMETRY,
	"AUTHOR" NVARCHAR2(150),
	"ST_NM" NVARCHAR2(50),
	"INTRSECT_ST_NM" NVARCHAR2(50),
	"ST_NBR" NVARCHAR2(10),
	"FAIL_MATCH_RSN_CD" NUMBER(5,0),
	"RE_GEOLOCATED_FROM_VER" NVARCHAR2(6),
	"AM_INTRSECT_ID" NUMBER(*,0),
	"AM_INTRSECT_OFFSET_FT" NUMBER(*,0),
	"AM_INTRSECT_OFFSET_DIR" NUMBER(*,0),
	"REL_TO_NETWORK" NVARCHAR2(20)
   ) ;
--------------------------------------------------------
--  DDL for Table INTRSECT
--------------------------------------------------------

  CREATE TABLE "INTRSECT"
   (	"INTERSECTION_ID" NUMBER(10,0),
	"INTERSECTION_NAME" NVARCHAR2(512),
	"INTERSECTION_GEOM_TYPE" NUMBER(5,0),
	"NODE_ORIENTED_ID" VARCHAR2(512),
	"IS_RAMP" NVARCHAR2(1),
	"IS_RNDABOUT" NVARCHAR2(1),
	"CNTY_CD" NUMBER(5,0),
	"CITY_CD" NUMBER(5,0),
	"RD_SYS_ID" NUMBER(5,0),
	"RD_SYS_INTERSTATE" NVARCHAR2(1),
	"RD_SYS_US" NVARCHAR2(1),
	"RD_SYS_STATE" NVARCHAR2(1),
	"RD_SYS_COUNTY" NVARCHAR2(1),
	"RD_SYS_LOCAL" NVARCHAR2(1),
	"RD_SYS_TOLL" NVARCHAR2(1),
	"RD_SYS_FOREST" NVARCHAR2(1),
	"RD_SYS_PRIVATE" NVARCHAR2(1),
	"RD_SYS_PK_LOT" NVARCHAR2(1),
	"RD_SYS_OTHER" NVARCHAR2(1)
   ) ;
--------------------------------------------------------
--  DDL for Table INTRSECT_NODE
--------------------------------------------------------

  CREATE TABLE "INTRSECT_NODE"
   (	"NODE_ID" NUMBER(10,0),
	"INTERSECTION_ID" NUMBER(10,0)
   ) ;
--------------------------------------------------------
--  DDL for Table ST
--------------------------------------------------------

  CREATE TABLE "ST"
   (	"OBJECTID" NUMBER(*,0),
	"LINK_ID" NUMBER(10,0),
	"ST_NAME" NVARCHAR2(240),
	"FEAT_ID" NUMBER(10,0),
	"ST_LANGCD" NVARCHAR2(3),
	"NUM_STNMES" NUMBER(2,0),
	"ST_NM_PREF" NVARCHAR2(6),
	"ST_TYP_BEF" NVARCHAR2(90),
	"ST_NM_BASE" NVARCHAR2(105),
	"ST_NM_SUFF" NVARCHAR2(6),
	"ST_TYP_AFT" NVARCHAR2(90),
	"ST_TYP_ATT" NVARCHAR2(1),
	"ADDR_TYPE" NVARCHAR2(1),
	"L_REFADDR" NVARCHAR2(10),
	"L_NREFADDR" NVARCHAR2(10),
	"L_ADDRSCH" NVARCHAR2(1),
	"L_ADDRFORM" NVARCHAR2(2),
	"R_REFADDR" NVARCHAR2(10),
	"R_NREFADDR" NVARCHAR2(10),
	"R_ADDRSCH" NVARCHAR2(1),
	"R_ADDRFORM" NVARCHAR2(2),
	"REF_IN_ID" NUMBER(10,0),
	"NREF_IN_ID" NUMBER(10,0),
	"N_SHAPEPNT" NUMBER(5,0),
	"FUNC_CLASS" NVARCHAR2(1),
	"SPEED_CAT" NVARCHAR2(1),
	"FR_SPD_LIM" NUMBER(5,0),
	"TO_SPD_LIM" NUMBER(5,0),
	"TO_LANES" NUMBER(2,0),
	"FROM_LANES" NUMBER(2,0),
	"ENH_GEOM" NVARCHAR2(1),
	"LANE_CAT" NVARCHAR2(1),
	"DIVIDER" NVARCHAR2(1),
	"DIR_TRAVEL" NVARCHAR2(1),
	"L_AREA_ID" NUMBER(10,0),
	"R_AREA_ID" NUMBER(10,0),
	"L_POSTCODE" NVARCHAR2(11),
	"R_POSTCODE" NVARCHAR2(11),
	"L_NUMZONES" NUMBER(2,0),
	"R_NUMZONES" NUMBER(2,0),
	"NUM_AD_RNG" NUMBER(2,0),
	"AR_AUTO" NVARCHAR2(1),
	"AR_BUS" NVARCHAR2(1),
	"AR_TAXIS" NVARCHAR2(1),
	"AR_CARPOOL" NVARCHAR2(1),
	"AR_PEDEST" NVARCHAR2(1),
	"AR_TRUCKS" NVARCHAR2(1),
	"AR_TRAFF" NVARCHAR2(1),
	"AR_DELIV" NVARCHAR2(1),
	"AR_EMERVEH" NVARCHAR2(1),
	"AR_MOTOR" NVARCHAR2(1),
	"PAVED" NVARCHAR2(1),
	"PRIVATE" NVARCHAR2(1),
	"FRONTAGE" NVARCHAR2(1),
	"BRIDGE" NVARCHAR2(1),
	"TUNNEL" NVARCHAR2(1),
	"RAMP" NVARCHAR2(1),
	"TOLLWAY" NVARCHAR2(1),
	"POIACCESS" NVARCHAR2(1),
	"CONTRACC" NVARCHAR2(1),
	"ROUNDABOUT" NVARCHAR2(1),
	"INTERINTER" NVARCHAR2(1),
	"UNDEFTRAFF" NVARCHAR2(1),
	"FERRY_TYPE" NVARCHAR2(1),
	"MULTIDIGIT" NVARCHAR2(1),
	"MAXATTR" NVARCHAR2(1),
	"SPECTRFIG" NVARCHAR2(1),
	"INDESCRIB" NVARCHAR2(1),
	"MANOEUVRE" NVARCHAR2(1),
	"DIVIDERLEG" NVARCHAR2(1),
	"INPROCDATA" NVARCHAR2(1),
	"FULL_GEOM" NVARCHAR2(1),
	"URBAN" NVARCHAR2(1),
	"ROUTE_TYPE" NVARCHAR2(1),
	"DIRONSIGN" NVARCHAR2(1),
	"EXPLICATBL" NVARCHAR2(1),
	"NAMEONRDSN" NVARCHAR2(1),
	"POSTALNAME" NVARCHAR2(1),
	"STALENAME" NVARCHAR2(1),
	"VANITYNAME" NVARCHAR2(1),
	"JUNCTIONNM" NVARCHAR2(1),
	"EXITNAME" NVARCHAR2(1),
	"SCENIC_RT" NVARCHAR2(1),
	"SCENIC_NM" NVARCHAR2(1),
	"FOURWHLDR" NVARCHAR2(1),
	"COVERIND" NVARCHAR2(2),
	"PLOT_ROAD" NVARCHAR2(1),
	"REVERSIBLE" NVARCHAR2(1),
	"EXPR_LANE" NVARCHAR2(1),
	"CARPOOLRD" NVARCHAR2(1),
	"PHYS_LANES" NUMBER(2,0),
	"VER_TRANS" NVARCHAR2(1),
	"PUB_ACCESS" NVARCHAR2(1),
	"LOW_MBLTY" NVARCHAR2(1),
	"PRIORITYRD" NVARCHAR2(1),
	"SPD_LM_SRC" NVARCHAR2(2),
	"EXPAND_INC" NVARCHAR2(1),
	"TRANS_AREA" NVARCHAR2(1),
	"DESCRIPT" NVARCHAR2(80),
	"FGDLAQDATE" DATE,
	"SHAPE" SDO_GEOMETRY ,
	"CITY_CD" NUMBER,
	"CNTY_CD" NUMBER
   ) ;
--------------------------------------------------------
--  DDL for Table S_CITATION
--------------------------------------------------------

  CREATE TABLE "S_CITATION"
   (	"OBJECTID" NUMBER(38,0),
	"CITATION_NBR" VARCHAR2(20),
	"SHAPE" SDO_GEOMETRY,
	"MPO_BND_ID" NUMBER(5,0)
   ) ;
--------------------------------------------------------
--  DDL for Table ST_EXT
--------------------------------------------------------

  CREATE TABLE "ST_EXT"
   (	"LINK_ID" NUMBER(10,0),
	"ROADWAY" NVARCHAR2(8),
	"DOT_FUNCLASS" NVARCHAR2(2),
	"DOT_ON_SYS" NUMBER(10,0),
	"FHP_BND_ID" NUMBER(5,0),
	"DOT_BND_ID" NUMBER(5,0),
	"MPO_BND_ID" NUMBER(5,0),
	"CNTY_BND_ID" NUMBER(5,0),
	"CITY_BND_ID" NUMBER(5,0),
	"CENTROID_X" NUMBER(38,12),
	"CENTROID_Y" NUMBER(38,12),
	"RD_SYS_ID" NUMBER(5,0),
	"RD_SYS_INTERSTATE" NVARCHAR2(1),
	"RD_SYS_US" NVARCHAR2(1),
	"RD_SYS_STATE" NVARCHAR2(1),
	"RD_SYS_COUNTY" NVARCHAR2(1),
	"RD_SYS_LOCAL" NVARCHAR2(1),
	"RD_SYS_TOLL" NVARCHAR2(1),
	"RD_SYS_FOREST" NVARCHAR2(1),
	"RD_SYS_PRIVATE" NVARCHAR2(1),
	"RD_SYS_PK_LOT" NVARCHAR2(1),
	"RD_SYS_OTHER" NVARCHAR2(1)
   ) ;
--------------------------------------------------------
--  DDL for Sequence INTRSECT_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "INTRSECT_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 590061 CACHE 20 NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_GEOCODE_RESULT
--------------------------------------------------------

   CREATE SEQUENCE  "SEQ_GEOCODE_RESULT"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 10582639 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence S_CITATION_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "S_CITATION_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 5581038 CACHE 20 NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Index IDX_GEOCODE_RESULT_SEG
--------------------------------------------------------

  CREATE INDEX "IDX_GEOCODE_RESULT_SEG" ON "GEOCODE_RESULT" ("CRASH_SEG_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_REFINTID
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_REFINTID" ON "GEOCODE_RESULT" ("REF_INTRSECT_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_NREFIN
--------------------------------------------------------

  CREATE INDEX "IDX_ST_NREFIN" ON "ST" ("NREF_IN_ID", "LINK_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_REFIN
--------------------------------------------------------

  CREATE INDEX "IDX_ST_REFIN" ON "ST" ("REF_IN_ID", "LINK_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_CNTY
--------------------------------------------------------

  CREATE INDEX "IDX_STEXT_CNTY" ON "ST_EXT" ("CNTY_BND_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_INTERMAN
--------------------------------------------------------

  CREATE INDEX "IDX_ST_INTERMAN" ON "ST" ("INTERINTER", "MANOEUVRE")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_LINK
--------------------------------------------------------

  CREATE UNIQUE INDEX "IDX_ST_LINK" ON "ST" ("LINK_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_LINKFLAGS
--------------------------------------------------------

  CREATE INDEX "IDX_ST_LINKFLAGS" ON "ST" ("LINK_ID", "ROUNDABOUT", "SPECTRFIG", "ST_NAME")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_NEARINTFT
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_NEARINTFT" ON "GEOCODE_RESULT" ("NEAREST_INTRSECT_OFFSET_FT")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_CITY
--------------------------------------------------------

  CREATE INDEX "IDX_ST_CITY" ON "ST" ("CITY_CD")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_NODERAMP
--------------------------------------------------------

  CREATE INDEX "IDX_ST_NODERAMP" ON "ST" ("REF_IN_ID", "NREF_IN_ID", "RAMP", "LINK_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_LINK
--------------------------------------------------------

  CREATE UNIQUE INDEX "IDX_STEXT_LINK" ON "ST_EXT" ("LINK_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_INTRSECTNODE_INTRSECT
--------------------------------------------------------

  CREATE INDEX "IDX_INTRSECTNODE_INTRSECT" ON "INTRSECT_NODE" ("INTERSECTION_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_FHP
--------------------------------------------------------

  CREATE INDEX "IDX_STEXT_FHP" ON "ST_EXT" ("FHP_BND_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_INTRSECTNODE_NODE
--------------------------------------------------------

  CREATE INDEX "IDX_INTRSECTNODE_NODE" ON "INTRSECT_NODE" ("NODE_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_DOT
--------------------------------------------------------

  CREATE INDEX "IDX_STEXT_DOT" ON "ST_EXT" ("DOT_BND_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_REFINTFT
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_REFINTFT" ON "GEOCODE_RESULT" ("REF_INTRSECT_OFFSET_FT")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_UPDTDT
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_UPDTDT" ON "GEOCODE_RESULT" ("LAST_UPDT_DT")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_INTRSECTID
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_INTRSECTID" ON "GEOCODE_RESULT" ("AM_INTRSECT_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_INTRSECTFT
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_INTRSECTFT" ON "GEOCODE_RESULT" ("AM_INTRSECT_OFFSET_FT")
  ;
--------------------------------------------------------
--  DDL for Index IDX_S_CITATION_OID
--------------------------------------------------------

  CREATE UNIQUE INDEX "IDX_S_CITATION_OID" ON "S_CITATION" ("CITATION_NBR")
  ;
--------------------------------------------------------
--  DDL for Index IDX_GEORESULT_NEARINTID
--------------------------------------------------------

  CREATE INDEX "IDX_GEORESULT_NEARINTID" ON "GEOCODE_RESULT" ("NEAREST_INTRSECT_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_MPO
--------------------------------------------------------

  CREATE INDEX "IDX_STEXT_MPO" ON "ST_EXT" ("MPO_BND_ID")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_RAMP
--------------------------------------------------------

  CREATE INDEX "IDX_ST_RAMP" ON "ST" ("RAMP")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_NAME
--------------------------------------------------------

  CREATE INDEX "IDX_ST_NAME" ON "ST" ("ST_NAME")
  ;
--------------------------------------------------------
--  DDL for Index IDX_ST_CNTY
--------------------------------------------------------

  CREATE INDEX "IDX_ST_CNTY" ON "ST" ("CNTY_CD")
  ;
--------------------------------------------------------
--  DDL for Index IDX_STEXT_CITY
--------------------------------------------------------

  CREATE INDEX "IDX_STEXT_CITY" ON "ST_EXT" ("CITY_BND_ID")
  ;
--------------------------------------------------------
--  Constraints for Table GEOCODE_RESULT
--------------------------------------------------------

  ALTER TABLE "GEOCODE_RESULT" ADD CONSTRAINT "GEOCODE_RESULT_PK" PRIMARY KEY ("HSMV_RPT_NBR") ENABLE;

  ALTER TABLE "GEOCODE_RESULT" MODIFY ("OBJECTID" NOT NULL ENABLE);

  ALTER TABLE "GEOCODE_RESULT" MODIFY ("HSMV_RPT_NBR" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table ST
--------------------------------------------------------

  ALTER TABLE "ST" MODIFY ("OBJECTID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table S_CITATION
--------------------------------------------------------

  ALTER TABLE "S_CITATION" MODIFY ("OBJECTID" NOT NULL ENABLE);

  ALTER TABLE "S_CITATION" MODIFY ("CITATION_NBR" NOT NULL ENABLE);

  ALTER TABLE "S_CITATION" ADD PRIMARY KEY ("OBJECTID") ENABLE;
--------------------------------------------------------
--  Constraints for Table INTRSECT
--------------------------------------------------------

  ALTER TABLE "INTRSECT" MODIFY ("INTERSECTION_ID" NOT NULL ENABLE);

  ALTER TABLE "INTRSECT" ADD PRIMARY KEY ("INTERSECTION_ID") ENABLE;
--------------------------------------------------------
--  Constraints for Table INTRSECT_NODE
--------------------------------------------------------

  ALTER TABLE "INTRSECT_NODE" MODIFY ("NODE_ID" NOT NULL ENABLE);

  ALTER TABLE "INTRSECT_NODE" MODIFY ("INTERSECTION_ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table ST_EXT
--------------------------------------------------------

  ALTER TABLE "ST_EXT" MODIFY ("LINK_ID" NOT NULL ENABLE);

--------------------------------------------------------
--  GRANTS
--------------------------------------------------------
GRANT SELECT ON geocode_result TO s4_warehouse_ng;
GRANT SELECT ON intrsect TO s4_warehouse_ng;
GRANT SELECT ON intrsect_node TO s4_warehouse_ng;
GRANT SELECT ON s_citation TO s4_warehouse_ng;
GRANT SELECT ON st TO s4_warehouse_ng;
GRANT SELECT ON st_ext TO s4_warehouse_ng;

--------------------------------------------------------
--  SDO_GEOMETRY setup
--------------------------------------------------------
INSERT INTO user_sdo_geom_metadata
    (TABLE_NAME,
     COLUMN_NAME,
     DIMINFO,
     SRID)
  VALUES (
  'geocode_result',
  'shape',
  SDO_DIM_ARRAY(
    SDO_DIM_ELEMENT('X', 529504, 588392, 0.005),
    SDO_DIM_ELEMENT('Y', 604583, 660486, 0.005)
     ),
  3087   -- SRID
);

INSERT INTO user_sdo_geom_metadata
    (TABLE_NAME,
     COLUMN_NAME,
     DIMINFO,
     SRID)
  VALUES (
  's_citation',
  'shape',
  SDO_DIM_ARRAY(
    SDO_DIM_ELEMENT('X', 529504, 588392, 0.005),
    SDO_DIM_ELEMENT('Y', 604583, 660486, 0.005)
     ),
  3087   -- SRID
);

INSERT INTO user_sdo_geom_metadata
    (TABLE_NAME,
     COLUMN_NAME,
     DIMINFO,
     SRID)
  VALUES (
  'geocode_result',
  'shape_web_mercator',
  SDO_DIM_ARRAY(
    SDO_DIM_ELEMENT('X', -9201989, -9133296, 0.005),
    SDO_DIM_ELEMENT('Y', 3431966, 3495576, 0.005)
     ),
  3857   -- SRID
);

INSERT INTO user_sdo_geom_metadata
    (TABLE_NAME,
     COLUMN_NAME,
     DIMINFO,
     SRID)
  VALUES (
  's_citation',
  'shape_web_mercator',
  SDO_DIM_ARRAY(
    SDO_DIM_ELEMENT('X', -9201989, -9133296, 0.005),
    SDO_DIM_ELEMENT('Y', 3431966, 3495576, 0.005)
     ),
  3857   -- SRID
);

COMMIT;

CREATE INDEX geocode_result_spatial_idx
   ON geocode_result(shape)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX s_citation_spatial_idx
   ON s_citation(shape)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

ALTER TABLE geocode_result
ADD shape_web_mercator SDO_GEOMETRY;

ALTER TABLE s_citation
ADD shape_web_mercator SDO_GEOMETRY;

UPDATE geocode_result
SET shape_web_mercator = sdo_cs.transform(shape, 3857);

UPDATE s_citation
SET shape_web_mercator = sdo_cs.transform(shape, 3857);

COMMIT;

CREATE INDEX geocode_result_web_merc_idx
   ON geocode_result(shape_web_mercator)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX s_citation_web_merc_idx
   ON s_citation(shape_web_mercator)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;
