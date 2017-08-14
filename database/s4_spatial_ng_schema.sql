/*
DROP TABLE geocode_result;
DROP TABLE intrsect;
DROP TABLE intrsect_node;
DROP TABLE s_citation;
DROP TABLE st;
DROP TABLE st_ext;
DROP TABLE s4_coord_sys;
DROP PROCEDURE s4_register_sdo_geom;
DROP PROCEDURE s4_unregister_sdo_geom;
DELETE FROM user_sdo_geom_metadata;
COMMIT;
*/

CREATE TABLE s4_coord_sys (
  srid NUMBER NOT NULL UNIQUE,
	min_x NUMBER NOT NULL,
	max_x NUMBER NOT NULL,
	min_y NUMBER NOT NULL,
	max_y NUMBER NOT NULL,
	tol NUMBER NOT NULL
);

INSERT INTO s4_coord_sys
(srid, min_x, max_x, min_y, max_y, tol)
VALUES (3087, 14908, 803741, 26231, 811063, 0.005);

INSERT INTO s4_coord_sys
(srid, min_x, max_x, min_y, max_y, tol)
VALUES (3857, -9772889, -8878794, 2775768, 3664717, 0.005);

INSERT INTO s4_coord_sys
(srid, min_x, max_x, min_y, max_y, tol)
VALUES (4326, -87.80, -79.76, 24.19, 31.25, 0.005);

COMMIT;

CREATE OR REPLACE PROCEDURE s4_register_sdo_geom (
	p_table_name VARCHAR2,
	p_column_name VARCHAR2,
	p_srid NUMBER
) AS
BEGIN
	INSERT INTO user_sdo_geom_metadata
	(table_name, column_name, diminfo, srid)
	SELECT
		p_table_name,
		p_column_name,
		sdo_dim_array(
			sdo_dim_element('X', min_x, max_x, tol),
			sdo_dim_element('Y', min_y, max_y, tol)
			),
		srid
	FROM s4_coord_sys
	WHERE srid = p_srid;
	COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE s4_unregister_sdo_geom (
	p_table_name VARCHAR2,
	p_column_name VARCHAR2
) AS
BEGIN
	DELETE FROM user_sdo_geom_metadata
	WHERE table_name = UPPER(p_table_name)
  AND column_name = UPPER(p_column_name);
	COMMIT;
END;
/

CREATE TABLE "GEOCODE_RESULT"
	(	"HSMV_RPT_NBR" NUMBER(10,0) NOT NULL,
"CRASH_SEG_ID" NUMBER(10,0),
"NEAREST_INTRSECT_ID" NUMBER(10,0),
"NEAREST_INTRSECT_OFFSET_FT" NUMBER(10,0),
"NEAREST_INTRSECT_OFFSET_DIR" NUMBER(5,0),
"REF_INTRSECT_ID" NUMBER(10,0),
"REF_INTRSECT_OFFSET_FT" NUMBER(10,0),
"REF_INTRSECT_OFFSET_DIR" NUMBER(5,0),
"UPDT_DIR_TRAVEL" NUMBER(5,0),
"ON_NETWORK" NVARCHAR2(1),
"DOT_ON_SYS" NVARCHAR2(1),
"KEY_GEOGRAPHY" NUMBER(10,0),
"CITY_CD" NUMBER(5,0),
"CNTY_CD" NUMBER(5,0),
"MAPPED" NVARCHAR2(1),
"SHAPE" SDO_GEOMETRY,
"SHAPE_MERC" SDO_GEOMETRY,
CONSTRAINT "GEOCODE_RESULT_PK" PRIMARY KEY ("HSMV_RPT_NBR")
	);

CREATE OR REPLACE TRIGGER geocode_result_merc
BEFORE INSERT OR UPDATE OF shape ON geocode_result
FOR EACH ROW
BEGIN
  IF NVL(sdo_geom.relate(:NEW.shape, 'EQUAL', :OLD.shape, 0.005), 'FALSE') = 'FALSE' THEN
    :NEW.shape_merc := sdo_cs.transform(:NEW.shape, 8307);
  END IF;
END;
/
ALTER TRIGGER geocode_result_merc ENABLE;

CALL s4_register_sdo_geom('geocode_result', 'shape', 3087);
CALL s4_register_sdo_geom('geocode_result', 'shape_merc', 3857);

CREATE INDEX geocode_result_spatial_idx
   ON geocode_result(shape)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX geocode_result_merc_idx
   ON geocode_result(shape_merc)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX "IDX_GEOCODE_RESULT_SEG" ON "GEOCODE_RESULT" ("CRASH_SEG_ID");
CREATE INDEX "IDX_GEORESULT_REFINTID" ON "GEOCODE_RESULT" ("REF_INTRSECT_ID");
CREATE INDEX "IDX_GEORESULT_NEARINTFT" ON "GEOCODE_RESULT" ("NEAREST_INTRSECT_OFFSET_FT");
CREATE INDEX "IDX_GEORESULT_REFINTFT" ON "GEOCODE_RESULT" ("REF_INTRSECT_OFFSET_FT");
CREATE INDEX "IDX_GEORESULT_NEARINTID" ON "GEOCODE_RESULT" ("NEAREST_INTRSECT_ID");

CREATE TABLE "INTRSECT"
	(	"INTERSECTION_ID" NUMBER(10,0) NOT NULL,
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
"RD_SYS_OTHER" NVARCHAR2(1),
CONSTRAINT intrsect_pk PRIMARY KEY ("INTERSECTION_ID")
	);

CREATE TABLE "INTRSECT_NODE"
	(	"NODE_ID" NUMBER(10,0) NOT NULL,
"INTERSECTION_ID" NUMBER(10,0) NOT NULL
	);

CREATE INDEX "IDX_INTRSECTNODE_INTRSECT" ON "INTRSECT_NODE" ("INTERSECTION_ID");
CREATE INDEX "IDX_INTRSECTNODE_NODE" ON "INTRSECT_NODE" ("NODE_ID");

CREATE TABLE "ST"
	(	"LINK_ID" NUMBER(10,0),
"ST_NAME" NVARCHAR2(240),
"ST_NM_PREF" NVARCHAR2(6),
"ST_TYP_BEF" NVARCHAR2(90),
"ST_NM_BASE" NVARCHAR2(105),
"ST_NM_SUFF" NVARCHAR2(6),
"ST_TYP_AFT" NVARCHAR2(90),
"ST_TYP_ATT" NVARCHAR2(1),
"REF_IN_ID" NUMBER(10,0),
"NREF_IN_ID" NUMBER(10,0),
"DIRONSIGN" NVARCHAR2(1),
"CITY_CD" NUMBER,
"CNTY_CD" NUMBER,
"SHAPE" SDO_GEOMETRY,
"SHAPE_MERC" SDO_GEOMETRY,
CONSTRAINT st_pk PRIMARY KEY ("LINK_ID")
	);

CREATE OR REPLACE TRIGGER st_merc
BEFORE INSERT OR UPDATE OF shape ON st
FOR EACH ROW
BEGIN
  IF NVL(sdo_geom.relate(:NEW.shape, 'EQUAL', :OLD.shape, 0.005), 'FALSE') = 'FALSE' THEN
    :NEW.shape_merc := sdo_cs.transform(:NEW.shape, 8307);
  END IF;
END;
/
ALTER TRIGGER st_merc ENABLE;

CALL s4_register_sdo_geom('st', 'shape', 3087);
CALL s4_register_sdo_geom('st', 'shape_merc', 3857);

CREATE INDEX st_spatial_idx
   ON st(shape)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX st_merc_idx
   ON st(shape_merc)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX "IDX_ST_NREFIN" ON "ST" ("NREF_IN_ID", "LINK_ID");
CREATE INDEX "IDX_ST_REFIN" ON "ST" ("REF_IN_ID", "LINK_ID");
CREATE INDEX "IDX_ST_CITY" ON "ST" ("CITY_CD");
CREATE INDEX "IDX_ST_NAME" ON "ST" ("ST_NAME");
CREATE INDEX "IDX_ST_CNTY" ON "ST" ("CNTY_CD");

CREATE TABLE "ST_EXT"
	(	"LINK_ID" NUMBER(10,0) NOT NULL,
"ROADWAY" NVARCHAR2(8),
"DOT_FUNCLASS" NVARCHAR2(2),
"DOT_ON_SYS" NUMBER(10,0),
"FHP_BND_ID" NUMBER(5,0),
"DOT_BND_ID" NUMBER(5,0),
"MPO_BND_ID" NUMBER(5,0),
"CNTY_BND_ID" NUMBER(5,0),
"CITY_BND_ID" NUMBER(5,0),
"CENTROID" SDO_GEOMETRY,
"CENTROID_MERC" SDO_GEOMETRY,
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
"RD_SYS_OTHER" NVARCHAR2(1),
CONSTRAINT st_ext_pk PRIMARY KEY ("LINK_ID")
	);

CREATE OR REPLACE TRIGGER st_ext_merc
BEFORE INSERT OR UPDATE OF centroid ON st_ext
FOR EACH ROW
BEGIN
  IF NVL(sdo_geom.relate(:NEW.centroid, 'EQUAL', :OLD.centroid, 0.005), 'FALSE') = 'FALSE' THEN
    :NEW.centroid_merc := sdo_cs.transform(:NEW.centroid, 8307);
  END IF;
END;
/
ALTER TRIGGER st_ext_merc ENABLE;

CALL s4_register_sdo_geom('st_ext', 'centroid', 3087);
CALL s4_register_sdo_geom('st_ext', 'centroid_merc', 3857);

CREATE INDEX "IDX_STEXT_FHP" ON "ST_EXT" ("FHP_BND_ID");
CREATE INDEX "IDX_STEXT_DOT" ON "ST_EXT" ("DOT_BND_ID");
CREATE INDEX "IDX_STEXT_MPO" ON "ST_EXT" ("MPO_BND_ID");
CREATE INDEX "IDX_STEXT_CITY" ON "ST_EXT" ("CITY_BND_ID");
CREATE INDEX "IDX_STEXT_CNTY" ON "ST_EXT" ("CNTY_BND_ID");

CREATE TABLE "S_CITATION"
	(	"CITATION_NBR" VARCHAR2(20) NOT NULL,
"MPO_BND_ID" NUMBER(5,0),
"SHAPE" SDO_GEOMETRY,
"SHAPE_MERC" SDO_GEOMETRY,
CONSTRAINT citation_pk PRIMARY KEY ("CITATION_NBR")
	);

CREATE OR REPLACE TRIGGER s_citation_merc
BEFORE INSERT OR UPDATE OF shape ON s_citation
FOR EACH ROW
BEGIN
  IF NVL(sdo_geom.relate(:NEW.shape, 'EQUAL', :OLD.shape, 0.005), 'FALSE') = 'FALSE' THEN
    :NEW.shape_merc := sdo_cs.transform(:NEW.shape, 8307);
  END IF;
END;
/
ALTER TRIGGER s_citation_merc ENABLE;

CALL s4_register_sdo_geom('s_citation', 'shape', 3087);
CALL s4_register_sdo_geom('s_citation', 'shape_merc', 3857);

CREATE INDEX s_citation_spatial_idx
   ON s_citation(shape)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

CREATE INDEX s_citation_merc_idx
   ON s_citation(shape_merc)
   INDEXTYPE IS MDSYS.SPATIAL_INDEX;

GRANT SELECT ON geocode_result TO s4_warehouse_ng;
GRANT SELECT ON intrsect TO s4_warehouse_ng;
GRANT SELECT ON intrsect_node TO s4_warehouse_ng;
GRANT SELECT ON s_citation TO s4_warehouse_ng;
GRANT SELECT ON st TO s4_warehouse_ng;
GRANT SELECT ON st_ext TO s4_warehouse_ng;