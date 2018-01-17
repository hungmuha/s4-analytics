/*
DROP DATABASE LINK s4_warehouse.geoplan.ufl.edu;
DROP TABLE cnty_city;
DROP TABLE dim_agncy;
DROP TABLE dim_crash_attrs;
DROP TABLE dim_date;
DROP TABLE dim_driver_attrs;
DROP TABLE dim_geography;
DROP TABLE dim_harmful_evt;
DROP TABLE dim_nm_attrs;
DROP TABLE dim_pass_attrs;
DROP TABLE dim_veh_attrs;
DROP TABLE dim_violation;
DROP TABLE bike_ped_crash_type;
DROP TABLE bike_ped_crash_grp;
DROP TABLE s4_coord_sys;
DROP TABLE intrsect_node;
DROP TABLE intrsect;
DROP TABLE zlevel;
DROP TABLE st;
DROP TABLE crash_evt;
DROP TABLE driver;
DROP TABLE non_motorist;
DROP TABLE pass;
DROP TABLE veh;
DROP TABLE violation;
DROP TABLE citation;
DROP PROCEDURE s4_register_sdo_geom;
DROP PROCEDURE s4_unregister_sdo_geom;
DELETE FROM user_sdo_geom_metadata;
COMMIT;
*/

CREATE DATABASE LINK s4_warehouse.geoplan.ufl.edu
CONNECT TO s4_warehouse IDENTIFIED BY "heathcote-autogeddon"
USING 'lime';

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

CREATE TABLE crash_evt (
    "ID"                          NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr                  NUMBER(*,0),
    hsmv_rpt_nbr_trunc            VARCHAR2(9),
    key_crash_dt                  DATE,
    crash_yr                      NUMBER(*,0),
    crash_mm                      NUMBER(*,0),
    crash_mo                      VARCHAR2(9),
    crash_dd                      NUMBER(*,0),
    crash_day                     VARCHAR2(9),
    crash_d                       NUMBER(*,0),
    key_geography                 NUMBER(*,0),
    dot_district_nm               VARCHAR2(15),
    rpc_nm                        VARCHAR2(30),
    mpo_nm                        VARCHAR2(35),
    cnty_cd                       NUMBER(*,0),
    cnty_nm                       VARCHAR2(15),
    city_cd                       NUMBER(*,0),
    city_nm                       VARCHAR2(35),
    key_rptg_agncy                NUMBER(*,0),
    rptg_agncy_nm                 VARCHAR2(60),
    rptg_agncy_short_nm           VARCHAR2(40),
    rptg_agncy_type_nm            VARCHAR2(30),
    key_rptg_unit                 NUMBER(*,0),
    rptg_unit_nm                  VARCHAR2(60),
    rptg_unit_short_nm            VARCHAR2(40),
    key_contrib_circum_env1       NUMBER(*,0),
    contrib_circum_env1           VARCHAR2(60),
    key_contrib_circum_env2       NUMBER(*,0),
    contrib_circum_env2           VARCHAR2(60),
    key_contrib_circum_env3       NUMBER(*,0),
    contrib_circum_env3           VARCHAR2(60),
    key_contrib_circum_rd1        NUMBER(*,0),
    contrib_circum_rd1            VARCHAR2(60),
    key_contrib_circum_rd2        NUMBER(*,0),
    contrib_circum_rd2            VARCHAR2(60),
    key_contrib_circum_rd3        NUMBER(*,0),
    contrib_circum_rd3            VARCHAR2(60),
    key_crash_sev                 NUMBER(*,0),
    crash_sev                     VARCHAR2(60),
    key_crash_sev_dtl             NUMBER(*,0),
    crash_sev_dtl                 VARCHAR2(60),
    key_crash_type                NUMBER(*,0),
    crash_type                    VARCHAR2(60),
    crash_type_simplified         VARCHAR2(60),
    crash_type_dir_tx             VARCHAR2(2),
    key_first_he                  NUMBER(*,0),
    first_he                      VARCHAR2(60),
    key_first_he_loc              NUMBER(*,0),
    first_he_loc                  VARCHAR2(60),
    key_first_he_rel_to_jct       NUMBER(*,0),
    first_he_rel_to_jct           VARCHAR2(60),
    key_light_cond                NUMBER(*,0),
    light_cond                    VARCHAR2(60),
    key_loc_in_work_zn            NUMBER(*,0),
    loc_in_work_zn                VARCHAR2(60),
    key_manner_of_collision       NUMBER(*,0),
    manner_of_collision           VARCHAR2(60),
    key_notif_by                  NUMBER(*,0),
    notif_by                      VARCHAR2(60),
    key_rd_sys_id                 NUMBER(*,0),
    rd_sys_id                     VARCHAR2(60),
    is_public_rd                  VARCHAR2(1),
    key_rd_surf_cond              NUMBER(*,0),
    rd_surf_cond                  VARCHAR2(60),
    key_type_of_intrsect          NUMBER(*,0),
    type_of_intrsect              VARCHAR2(60),
    key_type_of_shoulder          NUMBER(*,0),
    type_of_shoulder              VARCHAR2(60),
    key_type_of_work_zn           NUMBER(*,0),
    type_of_work_zn               VARCHAR2(60),
    key_weather_cond              NUMBER(*,0),
    weather_cond                  VARCHAR2(60),
    key_bike_ped_crash_type       NUMBER(*,0),
    key_bike_ped_crash_group      NUMBER(*,0),
    bike_or_ped                   VARCHAR2(1),
    bike_ped_crash_grp_nbr        NUMBER(*,0),
    bike_ped_crash_grp_nm         VARCHAR2(100),
    bike_ped_crash_type_nbr       NUMBER(*,0),
    bike_ped_crash_type_nm        VARCHAR2(100),
    crash_tm                      DATE,
    crash_hh24mi                  NUMBER(*,0),
    crash_hh24                    NUMBER(*,0),
    crash_hh_am                   VARCHAR2(5),
    intrsect_st_nm                VARCHAR2(80),
    is_alc_rel                    VARCHAR2(1),
    is_distracted                 VARCHAR2(1),
    is_drug_rel                   VARCHAR2(1),
    is_1st_he_within_intrchg      VARCHAR2(1),
    --is_geolocated                 VARCHAR2(1),
    is_le_in_work_zn              VARCHAR2(1),
    is_pictures_taken             VARCHAR2(1),
    is_sch_bus_rel                VARCHAR2(1),
    is_within_city_lim            VARCHAR2(1),
    is_workers_in_work_zn         VARCHAR2(1),
    is_work_zn_rel                VARCHAR2(1),
    --lat                          FLOAT(126),
    --lng                          FLOAT(126),
    milepost_nbr                  NUMBER(*,0),
    offset_dir                    VARCHAR2(5),
    offset_ft                     NUMBER(*,0),
    rptg_ofcr_rank                VARCHAR2(20),
    st_nm                         VARCHAR2(80),
    st_nbr                        VARCHAR2(20),
    veh_cnt                       NUMBER(*,0),
    comm_veh_cnt                  NUMBER(*,0),
    moped_cnt                     NUMBER(*,0),
    motorcycle_cnt                NUMBER(*,0),
    nm_cnt                        NUMBER(*,0),
    pass_cnt                      NUMBER(*,0),
    trailer_cnt                   NUMBER(*,0),
    bike_cnt                      NUMBER(*,0),
    ped_cnt                       NUMBER(*,0),
    fatality_cnt                  NUMBER(*,0),
    inj_cnt                       NUMBER(*,0),
    citation_cnt                  NUMBER(*,0),
    citation_amt                  NUMBER(*,0),
    prop_dmg_cnt                  NUMBER(*,0),
    prop_dmg_amt                  NUMBER(*,0),
    veh_dmg_cnt                   NUMBER(*,0),
    veh_dmg_amt                   NUMBER(*,0),
    tot_dmg_amt                   NUMBER(*,0),
    trans_by_ems_cnt              NUMBER(*,0),
    trans_by_le_cnt               NUMBER(*,0),
    trans_by_oth_cnt              NUMBER(*,0),
    inj_incapacitating_cnt        NUMBER(*,0),
    inj_none_cnt                  NUMBER(*,0),
    inj_possible_cnt              NUMBER(*,0),
    inj_non_incapacitating_cnt    NUMBER(*,0),
    inj_fatal_30_cnt              NUMBER(*,0),
    inj_fatal_non_traffic_cnt     NUMBER(*,0),
    geo_status_cd                 NUMBER(*,0),
    form_type_cd                  VARCHAR2(1),
    form_type_tx                  VARCHAR2(5),
    agncy_rpt_nbr                 VARCHAR2(25),
    agncy_rpt_nbr_trunc           VARCHAR2(8),
    batch_nbr                     NUMBER(*,0),
    data_src_cd                   NUMBER(10,0),
    is_complete                   VARCHAR2(1),
    is_aggressive                 VARCHAR2(1),
    rpt_dt                        DATE,
    notif_tm                      DATE,
    dispatched_tm                 DATE,
    arrived_tm                    DATE,
    cleared_tm                    DATE,
    img_file_nm                   VARCHAR2(15),
    img_src_nm                    VARCHAR2(20),
    codeable                      VARCHAR2(1),
    crash_seg_id                  NUMBER(*,0),
    nearest_intrsect_id           NUMBER(*,0),
    nearest_intrsect_offset_ft    NUMBER(10,0),
    nearest_intrsect_offset_dir   NUMBER(5,0),
    ref_intrsect_id               NUMBER(*,0),
    ref_intrsect_offset_ft        NUMBER(10,0),
    ref_intrsect_offset_dir       NUMBER(5,0),
    on_network                    VARCHAR2(1),
    dot_on_sys                    VARCHAR2(1),
    mapped                        VARCHAR2(1),
    sym_angle                     NUMBER(5,0),
    gc_st_nm                      VARCHAR2(50),
    gc_intrsect_st_nm             VARCHAR2(50),
    gc_key_geography              NUMBER(*,0),
    gc_is_within_city_lim         VARCHAR2(1),
    gc_dot_district_nm            VARCHAR2(15),
    gc_rpc_nm                     VARCHAR2(30),
    gc_mpo_nm                     VARCHAR2(35),
    gc_cnty_cd                    NUMBER(*,0),
    gc_cnty_nm                    VARCHAR2(15),
    gc_city_cd                    NUMBER(*,0),
    gc_city_nm                    VARCHAR2(35),
    is_cmv_involved               CHAR(1),
    is_intrsect_related           CHAR(1),
    is_lane_departure             CHAR(1),
    day_or_night                  VARCHAR2(20),
    last_updt_dt                  DATE,
    hsmv_orig_load_dt             DATE,
    hsmv_orig_load_dt_diff        NUMBER(*,0),
    gps_pt_4326                   SDO_GEOMETRY,
    geocode_pt_3087               SDO_GEOMETRY,
    geocode_pt_3857               SDO_GEOMETRY,
    PRIMARY KEY ( hsmv_rpt_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX crash_evt_id_idx ON
    crash_evt ( "ID" );

CREATE INDEX crash_evt_yr_mm ON crash_evt (crash_yr, crash_mm);
CREATE INDEX crash_evt_yr_mm_dd ON crash_evt (crash_yr, crash_mm, crash_dd);
CREATE INDEX crash_evt_dt ON crash_evt (key_crash_dt);

CREATE BITMAP INDEX crash_evt_cnty_cd ON crash_evt (cnty_cd);
CREATE INDEX crash_evt_geography ON crash_evt (key_geography);
CREATE INDEX crash_evt_rptg_agncy ON crash_evt (key_rptg_agncy);
CREATE BITMAP INDEX crash_evt_crash_sev ON crash_evt (key_crash_sev);
CREATE BITMAP INDEX crash_evt_drug_rel ON crash_evt (is_drug_rel);
CREATE BITMAP INDEX crash_evt_alc_rel ON crash_evt (is_alc_rel);
CREATE BITMAP INDEX crash_evt_bike_cnt ON crash_evt (bike_cnt);
CREATE BITMAP INDEX crash_evt_ped_cnt ON crash_evt (ped_cnt);
CREATE BITMAP INDEX crash_evt_first_he ON crash_evt (key_first_he);
CREATE BITMAP INDEX crash_evt_comm_veh_cnt ON crash_evt (comm_veh_cnt);
CREATE BITMAP INDEX crash_evt_codeable ON crash_evt (codeable);
CREATE BITMAP INDEX crash_evt_form_type_cd ON crash_evt (form_type_cd);

CALL s4_register_sdo_geom('crash_evt','gps_pt_4326',4326);
CALL s4_register_sdo_geom('crash_evt','geocode_pt_3087',3087);
CALL s4_register_sdo_geom('crash_evt','geocode_pt_3857',3857);

CREATE INDEX crash_evt_geocode_pt_3857_idx ON
    crash_evt ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CREATE TABLE driver (
    "ID"                        NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr                NUMBER(*,0),
    hsmv_rpt_nbr_trunc          VARCHAR2(9),
    veh_nbr                     NUMBER(*,0),
    person_nbr                  NUMBER(*,0),
    key_crash_dt                DATE,
    crash_yr                    NUMBER(*,0),
    crash_mm                    NUMBER(*,0),
    crash_mo                    VARCHAR2(9),
    crash_dd                    NUMBER(*,0),
    crash_day                   VARCHAR2(9),
    key_geography               NUMBER(*,0),
    dot_district_nm             VARCHAR2(15),
    rpc_nm                      VARCHAR2(30),
    mpo_nm                      VARCHAR2(35),
    cnty_cd                     NUMBER(*,0),
    cnty_nm                     VARCHAR2(15),
    city_cd                     NUMBER(*,0),
    city_nm                     VARCHAR2(35),
    key_rptg_agncy              NUMBER(*,0),
    rptg_agncy_nm               VARCHAR2(60),
    rptg_agncy_short_nm         VARCHAR2(40),
    rptg_agncy_type_nm          VARCHAR2(30),
    key_rptg_unit               NUMBER(*,0),
    rptg_unit_nm                VARCHAR2(60),
    rptg_unit_short_nm          VARCHAR2(40),
    key_action1                 NUMBER(*,0),
    action1                     VARCHAR2(100),
    key_action2                 NUMBER(*,0),
    action2                     VARCHAR2(100),
    key_action3                 NUMBER(*,0),
    action3                     VARCHAR2(100),
    key_action4                 NUMBER(*,0),
    action4                     VARCHAR2(100),
    key_age_rng                 NUMBER(*,0),
    age_rng                     VARCHAR2(100),
    key_airbag_deployed         NUMBER(*,0),
    airbag_deployed             VARCHAR2(100),
    key_alc_test_result         NUMBER(*,0),
    alc_test_result             VARCHAR2(100),
    key_alc_test_type           NUMBER(*,0),
    alc_test_type               VARCHAR2(100),
    key_alc_tested              NUMBER(*,0),
    alc_tested                  VARCHAR2(100),
    key_cond_at_tm_of_crash     NUMBER(*,0),
    cond_at_tm_of_crash         VARCHAR2(100),
    key_distracted_by           NUMBER(*,0),
    distracted_by               VARCHAR2(100),
    key_dl_endorsements         NUMBER(*,0),
    dl_endorsements             VARCHAR2(100),
    key_dl_type                 NUMBER(*,0),
    dl_type                     VARCHAR2(100),
    key_drug_test_result        NUMBER(*,0),
    drug_test_result            VARCHAR2(100),
    key_drug_test_type          NUMBER(*,0),
    drug_test_type              VARCHAR2(100),
    key_drug_tested             NUMBER(*,0),
    drug_tested                 VARCHAR2(100),
    key_ejection                NUMBER(*,0),
    ejection                    VARCHAR2(100),
    key_gender                  NUMBER(*,0),
    gender                      VARCHAR2(100),
    key_helmet_use              NUMBER(*,0),
    helmet_use                  VARCHAR2(100),
    key_inj_sev                 NUMBER(*,0),
    inj_sev                     VARCHAR2(100),
    key_restraint_sys           NUMBER(*,0),
    restraint_sys               VARCHAR2(100),
    key_src_of_trans            NUMBER(*,0),
    src_of_trans                VARCHAR2(100),
    key_veh_body_type           NUMBER(*,0),
    veh_body_type               VARCHAR2(80),
    key_vision_obstruction      NUMBER(*,0),
    vision_obstruction          VARCHAR2(100),
    addr_city                   VARCHAR2(40),
    addr_state                  VARCHAR2(2),
    addr_zip                    VARCHAR2(10),
    bac                         NUMBER(4,3),
    dl_state                    VARCHAR2(2),
    ins_co                      VARCHAR2(60),
    is_alc_use_suspected        VARCHAR2(1),
    is_distracted               VARCHAR2(1),
    is_drug_use_suspected       VARCHAR2(1),
    is_using_eye_protection     VARCHAR2(1),
    is_re_exam_recommended      VARCHAR2(1),
    fatality_cnt                NUMBER(*,0),
    fatality_unrestrained_cnt   NUMBER(*,0),
    inj_cnt                     NUMBER(*,0),
    inj_unrestrained_cnt        NUMBER(*,0),
    citation_cnt                NUMBER(*,0),
    citation_amt                NUMBER(*,0),
    prop_dmg_cnt                NUMBER(*,0),
    prop_dmg_amt                NUMBER(*,0),
    inj_incapacitating_cnt      NUMBER(*,0),
    batch_nbr                   NUMBER(*,0),
    PRIMARY KEY ( hsmv_rpt_nbr,veh_nbr,person_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX driver_id_idx ON
    driver ( "ID" );

CREATE TABLE non_motorist (
    "ID"                     NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr             NUMBER(*,0),
    hsmv_rpt_nbr_trunc       VARCHAR2(9),
    person_nbr               NUMBER(*,0),
    key_crash_dt             DATE,
    crash_yr                 NUMBER(*,0),
    crash_mm                 NUMBER(*,0),
    crash_mo                 VARCHAR2(9),
    crash_dd                 NUMBER(*,0),
    crash_day                VARCHAR2(9),
    key_geography            NUMBER(*,0),
    dot_district_nm          VARCHAR2(15),
    rpc_nm                   VARCHAR2(30),
    mpo_nm                   VARCHAR2(35),
    cnty_cd                  NUMBER(*,0),
    cnty_nm                  VARCHAR2(15),
    city_cd                  NUMBER(*,0),
    city_nm                  VARCHAR2(35),
    key_rptg_agncy           NUMBER(*,0),
    rptg_agncy_nm            VARCHAR2(60),
    rptg_agncy_short_nm      VARCHAR2(40),
    rptg_agncy_type_nm       VARCHAR2(30),
    key_rptg_unit            NUMBER(*,0),
    rptg_unit_nm             VARCHAR2(60),
    rptg_unit_short_nm       VARCHAR2(40),
    key_action_prior         NUMBER(*,0),
    action_prior             VARCHAR2(100),
    key_action_circum1       NUMBER(*,0),
    action_circum1           VARCHAR2(100),
    key_action_circum2       NUMBER(*,0),
    action_circum2           VARCHAR2(100),
    key_age_rng              NUMBER(*,0),
    age_rng                  VARCHAR2(100),
    key_alc_test_result      NUMBER(*,0),
    alc_test_result          VARCHAR2(100),
    key_alc_test_type        NUMBER(*,0),
    alc_test_type            VARCHAR2(100),
    key_alc_tested           NUMBER(*,0),
    alc_tested               VARCHAR2(100),
    key_desc                 NUMBER(*,0),
    "DESC"                   VARCHAR2(100),
    key_drug_test_result     NUMBER(*,0),
    drug_test_result         VARCHAR2(100),
    key_drug_test_type       NUMBER(*,0),
    drug_test_type           VARCHAR2(100),
    key_drug_tested          NUMBER(*,0),
    drug_tested              VARCHAR2(100),
    key_gender               NUMBER(*,0),
    gender                   VARCHAR2(100),
    key_inj_sev              NUMBER(*,0),
    inj_sev                  VARCHAR2(100),
    key_loc_at_tm_of_crash   NUMBER(*,0),
    loc_at_tm_of_crash       VARCHAR2(100),
    key_safety_eq1           NUMBER(*,0),
    safety_eq1               VARCHAR2(100),
    key_safety_eq2           NUMBER(*,0),
    safety_eq2               VARCHAR2(100),
    key_src_of_trans         NUMBER(*,0),
    src_of_trans             VARCHAR2(100),
    addr_city                VARCHAR2(40),
    addr_state               VARCHAR2(2),
    addr_zip                 VARCHAR2(10),
    bac                      NUMBER(4,3),
    dl_state                 VARCHAR2(2),
    is_alc_use_suspected     VARCHAR2(1),
    is_drug_use_suspected    VARCHAR2(1),
    bike_cnt                 NUMBER(*,0),
    ped_cnt                  NUMBER(*,0),
    fatality_cnt             NUMBER(*,0),
    inj_cnt                  NUMBER(*,0),
    citation_cnt             NUMBER(*,0),
    citation_amt             NUMBER(*,0),
    prop_dmg_cnt             NUMBER(*,0),
    prop_dmg_amt             NUMBER(*,0),
    inj_incapacitating_cnt   NUMBER(*,0),
    batch_nbr                NUMBER(*,0),
    PRIMARY KEY ( hsmv_rpt_nbr,person_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX non_motorist_id_idx ON
    non_motorist ( "ID" );

CREATE TABLE pass (
    "ID"                        NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr                NUMBER(*,0),
    hsmv_rpt_nbr_trunc          VARCHAR2(9),
    veh_nbr                     NUMBER(*,0),
    person_nbr                  NUMBER(*,0),
    key_crash_dt                DATE,
    crash_yr                    NUMBER(*,0),
    crash_mm                    NUMBER(*,0),
    crash_mo                    VARCHAR2(9),
    crash_dd                    NUMBER(*,0),
    crash_day                   VARCHAR2(9),
    key_geography               NUMBER(*,0),
    dot_district_nm             VARCHAR2(15),
    rpc_nm                      VARCHAR2(30),
    mpo_nm                      VARCHAR2(35),
    cnty_cd                     NUMBER(*,0),
    cnty_nm                     VARCHAR2(15),
    city_cd                     NUMBER(*,0),
    city_nm                     VARCHAR2(35),
    key_rptg_agncy              NUMBER(*,0),
    rptg_agncy_nm               VARCHAR2(60),
    rptg_agncy_short_nm         VARCHAR2(40),
    rptg_agncy_type_nm          VARCHAR2(30),
    key_rptg_unit               NUMBER(*,0),
    rptg_unit_nm                VARCHAR2(60),
    rptg_unit_short_nm          VARCHAR2(40),
    key_age_rng                 NUMBER(*,0),
    age_rng                     VARCHAR2(60),
    key_airbag_deployed         NUMBER(*,0),
    airbag_deployed             VARCHAR2(60),
    key_ejection                NUMBER(*,0),
    ejection                    VARCHAR2(60),
    key_gender                  NUMBER(*,0),
    gender                      VARCHAR2(60),
    key_helmet_use              NUMBER(*,0),
    helmet_use                  VARCHAR2(60),
    key_inj_sev                 NUMBER(*,0),
    inj_sev                     VARCHAR2(60),
    key_restraint_sys           NUMBER(*,0),
    restraint_sys               VARCHAR2(60),
    key_seating_other           NUMBER(*,0),
    seating_other               VARCHAR2(60),
    key_seating_row             NUMBER(*,0),
    seating_row                 VARCHAR2(60),
    key_seating_seat            NUMBER(*,0),
    seating_seat                VARCHAR2(60),
    key_src_of_trans            NUMBER(*,0),
    src_of_trans                VARCHAR2(60),
    key_veh_body_type           NUMBER(*,0),
    veh_body_type               VARCHAR2(80),
    addr_city                   VARCHAR2(40),
    addr_state                  VARCHAR2(2),
    addr_zip                    VARCHAR2(10),
    dl_state                    VARCHAR2(2),
    is_using_eye_protection     VARCHAR2(1),
    fatality_cnt                NUMBER(*,0),
    fatality_unrestrained_cnt   NUMBER(*,0),
    inj_cnt                     NUMBER(*,0),
    inj_unrestrained_cnt        NUMBER(*,0),
    citation_cnt                NUMBER(*,0),
    citation_amt                NUMBER(*,0),
    prop_dmg_cnt                NUMBER(*,0),
    batch_nbr                   NUMBER(*,0),
    inj_incapacitating_cnt      NUMBER(*,0),
    prop_dmg_amt                NUMBER(*,0),
    PRIMARY KEY ( hsmv_rpt_nbr,veh_nbr,person_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX pass_id_idx ON
    pass ( "ID" );

CREATE TABLE veh (
    "ID"                         NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr                 NUMBER(*,0),
    hsmv_rpt_nbr_trunc           VARCHAR2(9),
    veh_nbr                      NUMBER(*,0),
    key_crash_dt                 DATE,
    crash_yr                     NUMBER(*,0),
    crash_mm                     NUMBER(*,0),
    crash_mo                     VARCHAR2(9),
    crash_dd                     NUMBER(*,0),
    crash_day                    VARCHAR2(9),
    key_geography                NUMBER(*,0),
    dot_district_nm              VARCHAR2(15),
    rpc_nm                       VARCHAR2(30),
    mpo_nm                       VARCHAR2(35),
    cnty_cd                      NUMBER(*,0),
    cnty_nm                      VARCHAR2(15),
    city_cd                      NUMBER(*,0),
    city_nm                      VARCHAR2(35),
    key_rptg_agncy               NUMBER(*,0),
    rptg_agncy_nm                VARCHAR2(60),
    rptg_agncy_short_nm          VARCHAR2(40),
    rptg_agncy_type_nm           VARCHAR2(30),
    key_rptg_unit                NUMBER(*,0),
    rptg_unit_nm                 VARCHAR2(60),
    rptg_unit_short_nm           VARCHAR2(40),
    key_ar_of_init_impact        NUMBER(*,0),
    ar_of_init_impact            VARCHAR2(80),
    key_body_type                NUMBER(*,0),
    body_type                    VARCHAR2(80),
    key_cargo_body_type          NUMBER(*,0),
    cargo_body_type              VARCHAR2(80),
    key_cmv_config               NUMBER(*,0),
    cmv_config                   VARCHAR2(80),
    key_comm_non_comm            NUMBER(*,0),
    comm_non_comm                VARCHAR2(80),
    key_dmg_extent               NUMBER(*,0),
    dmg_extent                   VARCHAR2(80),
    key_dir_before               NUMBER(*,0),
    dir_before                   VARCHAR2(80),
    key_gvwr_gcwr                NUMBER(*,0),
    gvwr_gcwr                    VARCHAR2(80),
    key_he1                      NUMBER(*,0),
    he1                          VARCHAR2(60),
    key_he2                      NUMBER(*,0),
    he2                          VARCHAR2(60),
    key_he3                      NUMBER(*,0),
    he3                          VARCHAR2(60),
    key_he4                      NUMBER(*,0),
    he4                          VARCHAR2(60),
    key_maneuver_action          NUMBER(*,0),
    maneuver_action              VARCHAR2(80),
    key_most_dmg_ar              NUMBER(*,0),
    most_dmg_ar                  VARCHAR2(80),
    key_most_he                  NUMBER(*,0),
    most_he                      VARCHAR2(60),
    key_rd_align                 NUMBER(*,0),
    rd_align                     VARCHAR2(80),
    key_rd_grade                 NUMBER(*,0),
    rd_grade                     VARCHAR2(80),
    key_special_func             NUMBER(*,0),
    special_func                 VARCHAR2(80),
    key_traffic_ctl              NUMBER(*,0),
    traffic_ctl                  VARCHAR2(80),
    key_trafficway               NUMBER(*,0),
    trafficway                   VARCHAR2(80),
    key_veh_defect1              NUMBER(*,0),
    veh_defect1                  VARCHAR2(80),
    key_veh_defect2              NUMBER(*,0),
    veh_defect2                  VARCHAR2(80),
    key_veh_type                 NUMBER(*,0),
    veh_type                     VARCHAR2(80),
    key_wrecker_sel              NUMBER(*,0),
    wrecker_sel                  VARCHAR2(80),
    est_speed                    NUMBER(*,0),
    is_comm_veh                  VARCHAR2(1),
    is_emerg_veh                 VARCHAR2(1),
    is_hazmat_released           VARCHAR2(1),
    is_hit_and_run               VARCHAR2(1),
    is_owner_a_business          VARCHAR2(1),
    is_perm_reg                  VARCHAR2(1),
    is_towed_due_to_dmg          VARCHAR2(1),
    motor_carrier_city           VARCHAR2(40),
    motor_carrier_state          VARCHAR2(2),
    motor_carrier_zip            VARCHAR2(10),
    placard_hazmat_class         NUMBER(*,0),
    posted_speed                 NUMBER(*,0),
    reg_state                    VARCHAR2(2),
    tot_lanes                    NUMBER(*,0),
    traveling_on_st              VARCHAR2(50),
    veh_color                    VARCHAR2(12),
    veh_make                     VARCHAR2(5),
    veh_model                    VARCHAR2(12),
    veh_owner_city               VARCHAR2(40),
    veh_owner_state              VARCHAR2(2),
    veh_owner_zip                VARCHAR2(10),
    veh_style                    VARCHAR2(12),
    veh_yr                       NUMBER(*,0),
    moped_cnt                    NUMBER(*,0),
    motorcycle_cnt               NUMBER(*,0),
    pass_cnt                     NUMBER(*,0),
    trailer_cnt                  NUMBER(*,0),
    fatality_cnt                 NUMBER(*,0),
    fatality_unrestrained_cnt    NUMBER(*,0),
    inj_cnt                      NUMBER(*,0),
    inj_unrestrained_cnt         NUMBER(*,0),
    citation_cnt                 NUMBER(*,0),
    citation_amt                 NUMBER(*,0),
    prop_dmg_cnt                 NUMBER(*,0),
    prop_dmg_amt                 NUMBER(*,0),
    veh_dmg_cnt                  NUMBER(*,0),
    veh_dmg_amt                  NUMBER(*,0),
    tot_dmg_amt                  NUMBER(*,0),
    inj_incapacitating_cnt       NUMBER(*,0),
    batch_nbr                    NUMBER(*,0),
    inj_none_cnt                 NUMBER(*,0),
    inj_possible_cnt             NUMBER(*,0),
    inj_non_incapacitating_cnt   NUMBER(*,0),
    inj_fatal_30_cnt             NUMBER(*,0),
    inj_fatal_non_traffic_cnt    NUMBER(*,0),
    PRIMARY KEY ( hsmv_rpt_nbr,veh_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX veh_id_idx ON
    veh ( "ID" );

CREATE TABLE violation (
    "ID"                  NUMBER
        GENERATED ALWAYS AS IDENTITY,
    hsmv_rpt_nbr          NUMBER(*,0),
    hsmv_rpt_nbr_trunc    VARCHAR2(9),
    person_nbr            NUMBER(*,0),
    citation_nbr          VARCHAR2(8),
    citation_nbr_trunc    VARCHAR2(8),
    key_crash_dt          DATE,
    crash_yr              NUMBER(*,0),
    crash_mm              NUMBER(*,0),
    crash_mo              VARCHAR2(9),
    crash_dd              NUMBER(*,0),
    crash_day             VARCHAR2(9),
    key_geography         NUMBER(*,0),
    dot_district_nm       VARCHAR2(15),
    rpc_nm                VARCHAR2(30),
    mpo_nm                VARCHAR2(35),
    cnty_cd               NUMBER(*,0),
    cnty_nm               VARCHAR2(15),
    city_cd               NUMBER(*,0),
    city_nm               VARCHAR2(35),
    key_rptg_agncy        NUMBER(*,0),
    rptg_agncy_nm         VARCHAR2(60),
    rptg_agncy_short_nm   VARCHAR2(40),
    rptg_agncy_type_nm    VARCHAR2(30),
    key_rptg_unit         NUMBER(*,0),
    rptg_unit_nm          VARCHAR2(60),
    rptg_unit_short_nm    VARCHAR2(40),
    fl_statute_nbr        VARCHAR2(30),
    charge                VARCHAR2(128),
    batch_nbr             NUMBER(*,0),
    PRIMARY KEY ( hsmv_rpt_nbr,person_nbr,citation_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX violation_id_idx ON
    violation ( "ID" );

CREATE TABLE citation (
    "ID"                           NUMBER
        GENERATED ALWAYS AS IDENTITY,
    citation_nbr                   VARCHAR2(20),
    citation_nbr_trunc             VARCHAR2(8),
    check_digit                    VARCHAR2(1),
    key_citation_dt                DATE,
    citation_yr                    NUMBER(*,0),
    citation_mm                    NUMBER(*,0),
    citation_mo                    VARCHAR2(9),
    citation_dd                    NUMBER(*,0),
    citation_day                   VARCHAR2(9),
    key_geography                  NUMBER(*,0),
    dot_district_nm                VARCHAR2(15),
    rpc_nm                         VARCHAR2(30),
    mpo_nm                         VARCHAR2(35),
    cnty_cd                        NUMBER(*,0),
    cnty_nm                        VARCHAR2(15),
    city_cd                        NUMBER(*,0),
    city_nm                        VARCHAR2(35),
    key_agncy                      NUMBER(*,0),
    agncy_nm                       VARCHAR2(60),
    agncy_short_nm                 VARCHAR2(40),
    agncy_type_nm                  VARCHAR2(30),
    key_driver_age_rng             NUMBER(*,0),
    driver_age_rng                 VARCHAR2(100),
    key_violation                  NUMBER(*,0),
    sect_nbr                       VARCHAR2(11),
    sect_nbr_dsp                   VARCHAR2(11),
    sub_sect_nbr                   VARCHAR2(11),
    sub_sect_nbr_dsp               VARCHAR2(15),
    violation_cd                   VARCHAR2(3),
    violation_desc                 VARCHAR2(256),
    violation_class                VARCHAR2(10),
    violation_type                 VARCHAR2(50),
    --cnty_nbr                       VARCHAR2(2),
    --jurisdiction_nbr               VARCHAR2(2),
    --city_nm                        VARCHAR2(20),
    --agncy_type_cd                  VARCHAR2(1),
    --agncy_nm                       VARCHAR2(20),
    offense_dt                     DATE,
    driver_addr_diff_than_dl       VARCHAR2(1),
    driver_addr_city_nm            VARCHAR2(35),
    driver_addr_state_cd           VARCHAR2(2),
    driver_addr_zip_cd             VARCHAR2(10),
    driver_age_nbr                 NUMBER(5,0),
    driver_race_cd                 VARCHAR2(1),
    driver_gender_cd               VARCHAR2(1),
    driver_height_tx               VARCHAR2(4),
    dl_state_cd                    VARCHAR2(2),
    dl_class_cd                    VARCHAR2(1),
    dl_expir_yr                    NUMBER(4,0),
    comm_veh_cd                    VARCHAR2(1),
    veh_yr                         NUMBER(4,0),
    veh_make_tx                    VARCHAR2(15),
    veh_style_tx                   VARCHAR2(10),
    veh_color_tx                   VARCHAR2(7),
    hazmat_cd                      VARCHAR2(1),
    veh_state_cd                   VARCHAR2(2),
    veh_tag_expir_yr               NUMBER(4,0),
    companion_citation_cd          VARCHAR2(1),
    violation_loc_tx               VARCHAR2(40),
    offset_feet_meas               NUMBER(9,2),
    offset_miles_meas              NUMBER(5,2),
    offset_dir_cd                  VARCHAR2(1),
    offset_from_node_id            NUMBER(10,0),
    actual_speed_meas              NUMBER(5,0),
    posted_speed_meas              NUMBER(5,0),
    hwy_4lane_cd                   VARCHAR2(1),
    hwy_intrstate_cd               VARCHAR2(1),
    violation_careless_cd          VARCHAR2(1),
    violation_device_cd            VARCHAR2(1),
    violation_row_cd               VARCHAR2(1),
    violation_lane_cd              VARCHAR2(1),
    violation_passing_cd           VARCHAR2(1),
    violation_child_restraint_cd   VARCHAR2(1),
    violation_dui_cd               VARCHAR2(1),
    driver_bal_meas                NUMBER(4,3),
    violation_seatbelt_cd          VARCHAR2(1),
    violation_equip_cd             VARCHAR2(1),
    violation_tag_less_cd          VARCHAR2(1),
    violation_tag_more_cd          VARCHAR2(1),
    violation_ins_cd               VARCHAR2(1),
    violation_expir_dl_cd          VARCHAR2(1),
    violation_expir_dl_more_cd     VARCHAR2(1),
    violation_no_valid_dl_cd       VARCHAR2(1),
    violation_susp_dl_cd           VARCHAR2(1),
    oth_comments_tx                VARCHAR2(100),
    --violation_cd                   NUMBER(3,0),
    fl_dl_edit_override_cd         VARCHAR2(1),
    state_statute_cd               VARCHAR2(1),
    --sect_nbr                       VARCHAR2(11),
    --sub_sect_nbr                   VARCHAR2(11),
    crash_cd                       VARCHAR2(1),
    prop_dmg_cd                    VARCHAR2(1),
    prop_dmg_amt                   NUMBER(10,0),
    inj_cd                         VARCHAR2(1),
    serious_inj_cd                 VARCHAR2(1),
    fatal_inj_cd                   VARCHAR2(1),
    method_of_arrest_cd            NUMBER(2,0),
    criminal_appear_reqd_cd        VARCHAR2(1),
    infraction_appear_reqd_cd      VARCHAR2(1),
    infraction_no_appear_reqd_cd   VARCHAR2(1),
    court_dt                       DATE,
    court_nm                       VARCHAR2(30),
    court_addr_tx                  VARCHAR2(50),
    court_city_nm                  VARCHAR2(20),
    court_state_cd                 VARCHAR2(2),
    court_zip_cd                   VARCHAR2(10),
    arrest_delivered_to_tx         VARCHAR2(22),
    arrest_delivered_dt            DATE,
    ofcr_rank_tx                   VARCHAR2(10),
    trooper_unit_tx                VARCHAR2(15),
    bal_008_or_above_cd            VARCHAR2(1),
    dui_refuse_cd                  VARCHAR2(1),
    dui_lic_surrendered_cd         VARCHAR2(1),
    dui_lic_rsn_tx                 VARCHAR2(20),
    dui_eligible_cd                VARCHAR2(1),
    dui_eligible_rsn_tx            VARCHAR2(20),
    dui_bar_ofc_tx                 VARCHAR2(20),
    status_cd                      VARCHAR2(1),
    aggressive_driver_cd           VARCHAR2(1),
    criminal_cd                    VARCHAR2(1),
    fine_amt                       NUMBER(12,2),
    issue_arrest_dt                DATE,
    ofcr_dlvry_verif_cd            VARCHAR2(1),
    due_dt                         DATE,
    motorcycle_cd                  VARCHAR2(1),
    veh_16_pass_cd                 VARCHAR2(1),
    ofcr_re_exam_cd                VARCHAR2(1),
    dui_pass_under_18_cd           VARCHAR2(1),
    e_citation_cd                  VARCHAR2(1),
    nm_chg_cd                      VARCHAR2(1),
    comm_dl_cd                     VARCHAR2(1),
    --gps_lat                        FLOAT(126),
    --gps_lng                        FLOAT(126),
    --navteq_pt_x                    FLOAT(126),
    --navteq_pt_y                    FLOAT(126),
    violation_sig_red_light_cd     VARCHAR2(1),
    violation_workers_present_cd   VARCHAR2(1),
    violation_handheld_cd          VARCHAR2(1),
    violation_sch_zn_cd            VARCHAR2(1),
    --agency_id                      VARCHAR2(5),
    perm_reg_cd                    VARCHAR2(1),
    compliance_dt                  DATE,
    speed_meas_device_id           VARCHAR2(20),
    dl_seize_cd                    VARCHAR2(1),
    business_cd                    VARCHAR2(1),
    source_format_cd               NUMBER(5,0),
    addr_used_cd                   NUMBER(5,0),
    gps_pt_4326                    SDO_GEOMETRY,
    geocode_pt_3087                SDO_GEOMETRY,
    geocode_pt_3857                SDO_GEOMETRY,
    PRIMARY KEY ( citation_nbr )
        USING INDEX enable
);

CREATE UNIQUE INDEX citation_id_idx ON
    citation ( "ID" );

CALL s4_register_sdo_geom('citation','gps_pt_4326',4326);
CALL s4_register_sdo_geom('citation','geocode_pt_3087',3087);
CALL s4_register_sdo_geom('citation','geocode_pt_3857',3857);

CREATE INDEX citation_geocode_pt_3857_idx ON
    citation ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CREATE TABLE st (
    link_id             NUMBER(10,0),
    st_name             VARCHAR2(240),
    st_nm_pref          VARCHAR2(6),
    st_typ_bef          VARCHAR2(90),
    st_nm_base          VARCHAR2(105),
    st_nm_suff          VARCHAR2(6),
    st_typ_aft          VARCHAR2(90),
    st_typ_att          VARCHAR2(1),
    ref_in_id           NUMBER(10,0),
    nref_in_id          NUMBER(10,0),
    dironsign           VARCHAR2(1),
    city_cd             NUMBER,
    cnty_cd             NUMBER,
    roadway             VARCHAR2(8),
    dot_funclass        VARCHAR2(2),
    dot_on_sys          NUMBER(10,0),
    fhp_bnd_id          NUMBER(5,0),
    dot_bnd_id          NUMBER(5,0),
    mpo_bnd_id          NUMBER(5,0),
    cnty_bnd_id         NUMBER(5,0),
    city_bnd_id         NUMBER(5,0),
    rd_sys_id           NUMBER(5,0),
    rd_sys_interstate   VARCHAR2(1),
    rd_sys_us           VARCHAR2(1),
    rd_sys_state        VARCHAR2(1),
    rd_sys_county       VARCHAR2(1),
    rd_sys_local        VARCHAR2(1),
    rd_sys_toll         VARCHAR2(1),
    rd_sys_forest       VARCHAR2(1),
    rd_sys_private      VARCHAR2(1),
    rd_sys_pk_lot       VARCHAR2(1),
    rd_sys_other        VARCHAR2(1),
    centroid_3087       SDO_GEOMETRY,
    shape_3087          SDO_GEOMETRY,
    shape_3857          SDO_GEOMETRY,
    PRIMARY KEY ( link_id )
        USING INDEX enable
);

CALL s4_register_sdo_geom('st','centroid_3087',3087);
CALL s4_register_sdo_geom('st','shape_3087',3087);
CALL s4_register_sdo_geom('st','shape_3857',3857);

CREATE INDEX st_shape_3857_idx ON
    st ( shape_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CREATE TABLE zlevel (
    link_id                 NUMBER(10,0),
    point_num               NUMBER(4,0),
    node_id                 NUMBER(10,0),
    z_level                 NUMBER(2,0),
    intrsect                VARCHAR2(1),
    dot_shape               NUMBER(2,0),
    aligned                 VARCHAR2(1),
    shape_3087              SDO_GEOMETRY,
    shape_3857              SDO_GEOMETRY
);

CALL s4_register_sdo_geom('zlevel','shape_3087',3087);
CALL s4_register_sdo_geom('zlevel','shape_3857',3857);

CREATE INDEX zlevel_shape_3857_idx ON
    zlevel ( shape_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CREATE TABLE intrsect (
    intersection_id          NUMBER(10,0),
    intersection_name        VARCHAR2(512),
    is_ramp                  VARCHAR2(1),
    is_rndabout              VARCHAR2(1),
    cnty_cd                  NUMBER(5,0),
    city_cd                  NUMBER(5,0),
    rd_sys_id                NUMBER(5,0),
    rd_sys_interstate        VARCHAR2(1),
    rd_sys_us                VARCHAR2(1),
    rd_sys_state             VARCHAR2(1),
    rd_sys_county            VARCHAR2(1),
    rd_sys_local             VARCHAR2(1),
    rd_sys_toll              VARCHAR2(1),
    rd_sys_forest            VARCHAR2(1),
    rd_sys_private           VARCHAR2(1),
    rd_sys_pk_lot            VARCHAR2(1),
    rd_sys_other             VARCHAR2(1),
    geom_type                VARCHAR2(10),
    centroid_3087            SDO_GEOMETRY,
    shape_3087               SDO_GEOMETRY,
    shape_3857               SDO_GEOMETRY,
    PRIMARY KEY ( intersection_id )
        USING INDEX enable
);

CALL s4_register_sdo_geom('intrsect','centroid_3087',3087);
CALL s4_register_sdo_geom('intrsect','shape_3087',3087);
CALL s4_register_sdo_geom('intrsect','shape_3857',3857);

CREATE INDEX intrsect_shape_3857_idx ON
    intrsect ( shape_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CREATE TABLE intrsect_node (
    node_id             NUMBER(10,0),
    intersection_id     NUMBER(10,0)
);

CREATE TABLE cnty_city (
    "ID"             NUMBER(*,0),
    cnty_cd          NUMBER(*,0),
    city_cd          NUMBER(*,0),
    cnty_nm          VARCHAR2(15),
    cnty_navteq_nm   VARCHAR2(15),
    city_nm          VARCHAR2(35),
    city_navteq_nm   VARCHAR2(35),
    navteq_area_id   NUMBER(*,0),
    courthouse_tx    VARCHAR2(255),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_agncy (
    "ID"                 NUMBER(*,0),
    parent_agncy_id      NUMBER(10,0),
    agncy_nbr            VARCHAR2(7),
    agncy_nm             VARCHAR2(60),
    agncy_short_nm       VARCHAR2(40),
    agncy_type_nm        VARCHAR2(30),
    hierarchy_lvl_nbr    NUMBER(*,0),
    is_workflow_lvl_cd   NUMBER(*,0),
    org_unit_type_nm     VARCHAR2(60),
    extent_min_x         NUMBER,
    extent_min_y         NUMBER,
    extent_max_x         NUMBER,
    extent_max_y         NUMBER,
    obsolete_cd          VARCHAR2(1),
    vendor_nm            VARCHAR2(40),
    ori_nbr              VARCHAR2(9),
    trooper_unit_tx      VARCHAR2(1),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE INDEX fk_dim_agncy_parent_agncy ON
    dim_agncy ( parent_agncy_id );

CREATE TABLE dim_crash_attrs (
    "ID"            NUMBER(*,0),
    crash_attr_nm   VARCHAR2(40),
    crash_attr_cd   NUMBER(*,0),
    crash_attr_tx   VARCHAR2(60),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_date (
    evt_dt    DATE,
    evt_yr    NUMBER(*,0),
    evt_mm    NUMBER(*,0),
    evt_mo    VARCHAR2(9),
    evt_dd    NUMBER(*,0),
    evt_day   VARCHAR2(9),
    evt_d     NUMBER(1,0),
    prev_yr_dt_align_day_of_wk  DATE,
    prev_yr_dt_align_day_of_mo  DATE,
    PRIMARY KEY ( evt_dt )
        USING INDEX enable
);

CREATE TABLE dim_driver_attrs (
    "ID"             NUMBER(*,0),
    driver_attr_nm   VARCHAR2(40),
    driver_attr_cd   NUMBER(*,0),
    driver_attr_tx   VARCHAR2(100),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_geography (
    "ID"              NUMBER(*,0),
    state_nm          VARCHAR2(30),
    dot_district_nm   VARCHAR2(15),
    rpc_nm            VARCHAR2(30),
    mpo_nm            VARCHAR2(35),
    cnty_cd           NUMBER(*,0),
    cnty_nm           VARCHAR2(15),
    city_cd           NUMBER(*,0),
    city_nm           VARCHAR2(35),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_harmful_evt (
    "ID"                  NUMBER(*,0),
    harmful_evt_type_nm   VARCHAR2(60),
    harmful_evt_cd        NUMBER(*,0),
    harmful_evt_tx        VARCHAR2(60),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_nm_attrs (
    "ID"         NUMBER(*,0),
    nm_attr_nm   VARCHAR2(40),
    nm_attr_cd   NUMBER(*,0),
    nm_attr_tx   VARCHAR2(100),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_pass_attrs (
    "ID"           NUMBER(*,0),
    pass_attr_nm   VARCHAR2(40),
    pass_attr_cd   NUMBER(*,0),
    pass_attr_tx   VARCHAR2(60),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_veh_attrs (
    "ID"          NUMBER(*,0),
    veh_attr_nm   VARCHAR2(40),
    veh_attr_cd   NUMBER(*,0),
    veh_attr_tx   VARCHAR2(80),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE dim_violation (
    "ID"               NUMBER(*,0),
    sect_nbr           VARCHAR2(11),
    sect_nbr_dsp       VARCHAR2(11),
    sub_sect_nbr       VARCHAR2(11),
    sub_sect_nbr_dsp   VARCHAR2(15),
    violation_cd       VARCHAR2(3),
    violation_desc     VARCHAR2(256),
    violation_class    VARCHAR2(10),
    violation_type     VARCHAR2(50),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE bike_ped_crash_grp (
    "ID"            NUMBER,
    bike_or_ped     VARCHAR2(1),
    crash_grp_nbr   NUMBER,
    crash_grp_nm    VARCHAR2(100),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);

CREATE TABLE bike_ped_crash_type (
    "ID"             NUMBER,
    crash_grp_id     NUMBER,
    crash_type_nbr   NUMBER,
    crash_type_nm    VARCHAR2(100),
    PRIMARY KEY ( "ID" )
        USING INDEX enable
);
