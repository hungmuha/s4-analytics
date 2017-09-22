CREATE OR REPLACE PROCEDURE s4_sync_ref_tables
AS
BEGIN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE cnty_city';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_agncy';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_crash_attrs';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_date';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_driver_attrs';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_geography';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_harmful_evt';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_nm_attrs';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_pass_attrs';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_veh_attrs';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE dim_violation';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE bike_ped_crash_type';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE bike_ped_crash_grp';

    INSERT INTO cnty_city (
        "ID",
        cnty_cd,
        city_cd,
        cnty_nm,
        cnty_navteq_nm,
        city_nm,
        city_navteq_nm,
        navteq_area_id,
        courthouse_tx
    )
    SELECT
        "ID",
        cnty_cd,
        city_cd,
        cnty_nm,
        cnty_navteq_nm,
        city_nm,
        city_navteq_nm,
        navteq_area_id,
        courthouse_tx
    FROM cnty_city@s4_warehouse;

    INSERT INTO dim_agncy (
        "ID",
        parent_agncy_id,
        agncy_nbr,
        agncy_nm,
        agncy_short_nm,
        agncy_type_nm,
        hierarchy_lvl_nbr,
        is_workflow_lvl_cd,
        org_unit_type_nm,
        extent_min_x,
        extent_min_y,
        extent_max_x,
        extent_max_y,
        obsolete_cd,
        vendor_nm,
        ori_nbr
    )
    SELECT
        "ID",
        parent_agncy_id,
        agncy_nbr,
        agncy_nm,
        agncy_short_nm,
        agncy_type_nm,
        hierarchy_lvl_nbr,
        is_workflow_lvl_cd,
        org_unit_type_nm,
        extent_min_x,
        extent_min_y,
        extent_max_x,
        extent_max_y,
        obsolete_cd,
        vendor_nm,
        ori_nbr
    FROM dim_agncy@s4_warehouse;

    INSERT INTO dim_crash_attrs (
        "ID",
        crash_attr_nm,
        crash_attr_cd,
        crash_attr_tx
    )
    SELECT
        "ID",
        crash_attr_nm,
        crash_attr_cd,
        crash_attr_tx
    FROM dim_crash_attrs@s4_warehouse;

    INSERT INTO dim_date (
        evt_dt,
        evt_yr,
        evt_mm,
        evt_mo,
        evt_dd,
        evt_day,
        evt_d
    )
    SELECT
        crash_dt AS evt_dt,
        crash_yr AS evt_yr,
        crash_mm AS evt_mm,
        crash_mo AS evt_mo,
        crash_dd AS evt_dd,
        crash_day AS evt_day,
        CAST(TO_CHAR(crash_dt, 'D') AS INTEGER) AS evt_d
    FROM dim_date@s4_warehouse;

    INSERT INTO dim_driver_attrs (
        "ID",
        driver_attr_nm,
        driver_attr_cd,
        driver_attr_tx
    )
    SELECT
        "ID",
        driver_attr_nm,
        driver_attr_cd,
        driver_attr_tx
    FROM dim_driver_attrs@s4_warehouse;

    INSERT INTO dim_geography (
        "ID",
        state_nm,
        dot_district_nm,
        rpc_nm,
        mpo_nm,
        cnty_cd,
        cnty_nm,
        city_cd,
        city_nm
    )
    SELECT
        "ID",
        state_nm,
        dot_district_nm,
        rpc_nm,
        mpo_nm,
        cnty_cd,
        cnty_nm,
        city_cd,
        city_nm
    FROM dim_geography@s4_warehouse;

    INSERT INTO dim_harmful_evt (
        "ID",
        harmful_evt_type_nm,
        harmful_evt_cd,
        harmful_evt_tx
    )
    SELECT
        "ID",
        harmful_evt_type_nm,
        harmful_evt_cd,
        harmful_evt_tx
    FROM dim_harmful_evt@s4_warehouse;

    INSERT INTO dim_nm_attrs (
        "ID",
        nm_attr_nm,
        nm_attr_cd,
        nm_attr_tx
    )
    SELECT
        "ID",
        nm_attr_nm,
        nm_attr_cd,
        nm_attr_tx
    FROM dim_nm_attrs@s4_warehouse;

    INSERT INTO dim_pass_attrs (
        "ID",
        pass_attr_nm,
        pass_attr_cd,
        pass_attr_tx
    )
    SELECT
        "ID",
        pass_attr_nm,
        pass_attr_cd,
        pass_attr_tx
    FROM dim_pass_attrs@s4_warehouse;

    INSERT INTO dim_veh_attrs (
        "ID",
        veh_attr_nm,
        veh_attr_cd,
        veh_attr_tx
    )
    SELECT
        "ID",
        veh_attr_nm,
        veh_attr_cd,
        veh_attr_tx
    FROM dim_veh_attrs@s4_warehouse;

    INSERT INTO dim_violation (
        "ID",
        sect_nbr,
        sect_nbr_dsp,
        sub_sect_nbr,
        sub_sect_nbr_dsp,
        violation_cd,
        violation_desc,
        violation_class,
        violation_type
    )
    SELECT
        "ID",
        sect_nbr,
        sect_nbr_dsp,
        sub_sect_nbr,
        sub_sect_nbr_dsp,
        violation_cd,
        violation_desc,
        violation_class,
        violation_type
    FROM dim_violation@s4_warehouse;

    INSERT INTO bike_ped_crash_grp (
        "ID",
        bike_or_ped,
        crash_grp_nbr,
        crash_grp_nm
    )
    SELECT
        "ID",
        bike_or_ped,
        crash_grp_nbr,
        crash_grp_nm
    FROM bike_ped_crash_grp@s4_warehouse;

    INSERT INTO bike_ped_crash_type (
        "ID",
        crash_grp_id,
        crash_type_nbr,
        crash_type_nm
    )
    SELECT
        "ID",
        crash_grp_id,
        crash_type_nbr,
        crash_type_nm
    FROM bike_ped_crash_type@s4_warehouse;

    COMMIT;
END;
/
