CREATE OR REPLACE VIEW v_crash_contrib_circum_env (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Contributing Circumstance Environment';

CREATE OR REPLACE VIEW v_crash_contrib_circum_road (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Contributing Circumstance Road';

CREATE OR REPLACE VIEW v_crash_first_he_loc (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'First Harmful Event Location';

CREATE OR REPLACE VIEW v_crash_first_he_rel_to_jct (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'First Harmful Event Relation to Junction';

CREATE OR REPLACE VIEW v_crash_light_cond (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Light Condition';

CREATE OR REPLACE VIEW v_crash_loc_in_work_zn (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Location in Work Zone';

CREATE OR REPLACE VIEW v_crash_manner_of_collision (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Manner of Collision/Impact';

CREATE OR REPLACE VIEW v_crash_notif_by (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Notified By';

CREATE OR REPLACE VIEW v_crash_road_surf_cond (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Roadway Surface Condition';

CREATE OR REPLACE VIEW v_crash_road_sys_id (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Road System Identifier';

CREATE OR REPLACE VIEW v_crash_sev (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Crash Severity';

CREATE OR REPLACE VIEW v_crash_sev_dtl (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Crash Severity Detailed';

CREATE OR REPLACE VIEW v_crash_type (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Crash Type';

CREATE OR REPLACE VIEW v_bike_ped_crash_type AS
    SELECT
        g."ID" AS crash_grp_id,
        t."ID" AS crash_type_id,
        g.bike_or_ped,
        g.crash_grp_nbr,
        g.crash_grp_nm,
        t.crash_type_nbr,
        t.crash_type_nm
    FROM
        bike_ped_crash_grp g
        INNER JOIN bike_ped_crash_type t ON g."ID" = t.crash_grp_id
WITH READ ONLY;

CREATE OR REPLACE VIEW v_crash_type_simplified (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        DECODE(
            crash_attr_tx,
            'Right Angle',
            'Angle',
            'Animal',
            'Animal',
            'Bicycle',
            'Bicycle',
            'Off Road',
            'Off Road',
            'Head On',
            'Head On',
            'Left Entering',
            'Left Turn',
            'Left Leaving',
            'Left Turn',
            'Left Rear',
            'Left Turn',
            'Right/Left',
            'Right Turn',
            'Right/Through',
            'Right Turn',
            'Right/U-Turn',
            'Right Turn',
            'Backed Into',
            'Other',
            'Other',
            'Other',
            'Parked Vehicle',
            'Other',
            'Single Vehicle',
            'Other',
            'Pedestrian',
            'Pedestrian',
            'Rear End',
            'Rear End',
            'Rollover',
            'Rollover',
            'Opposing Sideswipe',
            'Sideswipe',
            'Same Direction Sideswipe',
            'Sideswipe',
            'Unknown'
        )
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Crash Type';

CREATE OR REPLACE VIEW v_crash_type_of_intrsect (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Type of Intersection';

CREATE OR REPLACE VIEW v_crash_type_of_shoulder (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Type of Shoulder';

CREATE OR REPLACE VIEW v_crash_type_of_work_zn (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Type of Work Zone';

CREATE OR REPLACE VIEW v_crash_weather_cond (
    "ID",
    crash_attr_cd,
    crash_attr_tx
) AS
    SELECT
        "ID",
        crash_attr_cd,
        crash_attr_tx
    FROM
        dim_crash_attrs
    WHERE
        crash_attr_nm = 'Weather Condition';

CREATE OR REPLACE VIEW v_driver_action (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Action';

CREATE OR REPLACE VIEW v_driver_age_rng (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Age Range';

CREATE OR REPLACE VIEW v_driver_airbag_deployed (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Air Bag Deployed';

CREATE OR REPLACE VIEW v_driver_alc_test_result (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Alcohol Test Result';

CREATE OR REPLACE VIEW v_driver_alc_test_type (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Alcohol Test Type';

CREATE OR REPLACE VIEW v_driver_alc_tested (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Alcohol Tested';

CREATE OR REPLACE VIEW v_driver_cond_at_tm_of_crash (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Condition at Time of Crash';

CREATE OR REPLACE VIEW v_driver_distracted_by (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Distracted By';

CREATE OR REPLACE VIEW v_driver_dl_endorsements (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'DL Required Endorsements';

CREATE OR REPLACE VIEW v_driver_dl_type (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Driver License Type';

CREATE OR REPLACE VIEW v_driver_drug_test_result (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Drug Test Result';

CREATE OR REPLACE VIEW v_driver_drug_test_type (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Drug Test Type';

CREATE OR REPLACE VIEW v_driver_drug_tested (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Drug Tested';

CREATE OR REPLACE VIEW v_driver_ejection (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Ejection';

CREATE OR REPLACE VIEW v_driver_gender (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Gender';

CREATE OR REPLACE VIEW v_driver_helmet_use (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Helmet Use';

CREATE OR REPLACE VIEW v_driver_inj_sev (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Injury Severity';

CREATE OR REPLACE VIEW v_driver_restraint_sys (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Restraint System';

CREATE OR REPLACE VIEW v_driver_src_of_trans (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Source of Transport';

CREATE OR REPLACE VIEW v_driver_veh_body_type (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd AS driver_attr_cd,
        veh_attr_tx AS driver_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Body Type';

CREATE OR REPLACE VIEW v_driver_vision_obstruction (
    "ID",
    driver_attr_cd,
    driver_attr_tx
) AS
    SELECT
        "ID",
        driver_attr_cd,
        driver_attr_tx
    FROM
        dim_driver_attrs
    WHERE
        driver_attr_nm = 'Vision Obstruction';

CREATE OR REPLACE VIEW v_nm_action_circum (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Action/Circumstance';

CREATE OR REPLACE VIEW v_nm_action_prior (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Action Prior to Crash';

CREATE OR REPLACE VIEW v_nm_age_rng (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Age Range';

CREATE OR REPLACE VIEW v_nm_alc_test_result (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Alcohol Test Result';

CREATE OR REPLACE VIEW v_nm_alc_test_type (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Alcohol Test Type';

CREATE OR REPLACE VIEW v_nm_alc_tested (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Alcohol Tested';

CREATE OR REPLACE VIEW v_nm_desc (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Description';

CREATE OR REPLACE VIEW v_nm_drug_test_result (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Drug Test Result';

CREATE OR REPLACE VIEW v_nm_drug_test_type (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Drug Test Type';

CREATE OR REPLACE VIEW v_nm_drug_tested (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Drug Tested';

CREATE OR REPLACE VIEW v_nm_gender (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Gender';

CREATE OR REPLACE VIEW v_nm_inj_sev (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Injury Severity';

CREATE OR REPLACE VIEW v_nm_loc_at_tm_of_crash (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Location at Time of Crash';

CREATE OR REPLACE VIEW v_nm_safety_eq (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Safety Equipment';

CREATE OR REPLACE VIEW v_nm_src_of_trans (
    "ID",
    nm_attr_cd,
    nm_attr_tx
) AS
    SELECT
        "ID",
        nm_attr_cd,
        nm_attr_tx
    FROM
        dim_nm_attrs
    WHERE
        nm_attr_nm = 'Source of Transport';

CREATE OR REPLACE VIEW v_pass_age_rng (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Age Range';

CREATE OR REPLACE VIEW v_pass_airbag_deployed (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Air Bag Deployed';

CREATE OR REPLACE VIEW v_pass_ejection (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Ejection';

CREATE OR REPLACE VIEW v_pass_gender (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Gender';

CREATE OR REPLACE VIEW v_pass_helmet_use (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Helmet Use';

CREATE OR REPLACE VIEW v_pass_inj_sev (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Injury Severity';

CREATE OR REPLACE VIEW v_pass_restraint_sys (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Restraint System';

CREATE OR REPLACE VIEW v_pass_seating_other (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Seating Position Other';

CREATE OR REPLACE VIEW v_pass_seating_row (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Seating Position Row';

CREATE OR REPLACE VIEW v_pass_seating_seat (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Seating Position Seat';

CREATE OR REPLACE VIEW v_pass_src_of_trans (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        pass_attr_cd,
        pass_attr_tx
    FROM
        dim_pass_attrs
    WHERE
        pass_attr_nm = 'Source of Transport';

CREATE OR REPLACE VIEW v_pass_veh_body_type (
    "ID",
    pass_attr_cd,
    pass_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd AS pass_attr_cd,
        veh_attr_tx AS pass_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Body Type';

CREATE OR REPLACE VIEW v_veh_area_on_veh (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Area on Vehicle';

CREATE OR REPLACE VIEW v_veh_body_type (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Body Type';

CREATE OR REPLACE VIEW v_veh_cargo_body_type (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Cargo Body Type';

CREATE OR REPLACE VIEW v_veh_cmv_config (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'CMV Configuration';

CREATE OR REPLACE VIEW v_veh_comm_non_comm (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Commercial/Non-Commercial';

CREATE OR REPLACE VIEW v_veh_defect (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Vehicle Defect';

CREATE OR REPLACE VIEW v_veh_dir_before (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Direction Before Crash';

CREATE OR REPLACE VIEW v_veh_dmg_ext (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Damage Extent';

CREATE OR REPLACE VIEW v_veh_gvwr (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'GVWR/GCWR';

CREATE OR REPLACE VIEW v_veh_maneuver (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Maneuver Action';

CREATE OR REPLACE VIEW v_veh_rd_align (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Roadway Alignment';

CREATE OR REPLACE VIEW v_veh_rd_grade (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Roadway Grade';

CREATE OR REPLACE VIEW v_veh_special_func (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Special Function';

CREATE OR REPLACE VIEW v_veh_traffic_ctl (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Traffic Control Device';

CREATE OR REPLACE VIEW v_veh_trafficway (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Trafficway';

CREATE OR REPLACE VIEW v_veh_type (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Vehicle Type';

CREATE OR REPLACE VIEW v_veh_wrecker_sel (
    "ID",
    veh_attr_cd,
    veh_attr_tx
) AS
    SELECT
        "ID",
        veh_attr_cd,
        veh_attr_tx
    FROM
        dim_veh_attrs
    WHERE
        veh_attr_nm = 'Wrecker Selection Method';
