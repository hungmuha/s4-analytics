CREATE OR REPLACE PROCEDURE s4_sync_st
AS
    CURSOR ddl_cur IS SELECT
        'ALTER INDEX ' || index_name || ' UNUSABLE' AS unusable_ddl,
        'ALTER INDEX ' || index_name || ' REBUILD' AS rebuild_ddl
    FROM user_indexes
    WHERE index_type IN ('NORMAL', 'DOMAIN', 'BITMAP')
    AND index_name NOT LIKE 'SYS_%'
    AND index_name NOT LIKE '%_ID_IDX'
    AND table_name = 'ST';
BEGIN
    -- disable indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.unusable_ddl);
    END LOOP;

    -- skip disabled indexes
    EXECUTE IMMEDIATE 'ALTER SESSION SET skip_unusable_indexes=TRUE';

    EXECUTE IMMEDIATE 'TRUNCATE TABLE st';

    INSERT INTO st (
        link_id,
        st_name,
        st_nm_pref,
        st_typ_bef,
        st_nm_base,
        st_nm_suff,
        st_typ_aft,
        st_typ_att,
        ref_in_id,
        nref_in_id,
        dironsign,
        city_cd,
        cnty_cd,
        roadway,
        dot_funclass,
        dot_on_sys,
        fhp_bnd_id,
        dot_bnd_id,
        mpo_bnd_id,
        cnty_bnd_id,
        city_bnd_id,
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
        rd_sys_other,
        centroid_3087,
        shape_3087,
        shape_3857
    )
    SELECT
        link_id,
        st_name,
        st_nm_pref,
        st_typ_bef,
        st_nm_base,
        st_nm_suff,
        st_typ_aft,
        st_typ_att,
        ref_in_id,
        nref_in_id,
        dironsign,
        city_cd,
        cnty_cd,
        roadway,
        dot_funclass,
        dot_on_sys,
        fhp_bnd_id,
        dot_bnd_id,
        mpo_bnd_id,
        cnty_bnd_id,
        city_bnd_id,
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
        rd_sys_other,
        centroid_3087,
        shape_3087,
        sdo_cs.transform(shape_3087, 3857) AS shape_3857
    FROM v_flat_st@s4_warehouse;

    COMMIT;

    -- rebuild indexes
    FOR rec IN ddl_cur LOOP
      dbms_utility.exec_ddl_statement(rec.rebuild_ddl);
    END LOOP;
END;
/
