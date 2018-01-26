DECLARE
    v_sync_crash_id INT := NULL;
    v_sync_citation_id INT := NULL;
BEGIN
    SELECT sync_crash_seq.nextval INTO v_sync_crash_id FROM dual;
    SELECT sync_citation_seq.nextval INTO v_sync_citation_id FROM dual;

    INSERT INTO sync_crash
    SELECT v_sync_crash_id, hsmv_rpt_nbr
    FROM v_flat_crash_evt@s4_warehouse vce;
    COMMIT;

    INSERT INTO sync_citation
    SELECT v_sync_citation_id, citation_nbr
    FROM v_flat_citation@s4_warehouse vci;
    COMMIT;

    s4_sync_crash_evt(v_sync_crash_id);
    s4_sync_driver(v_sync_crash_id);
    s4_sync_non_motorist(v_sync_crash_id);
    s4_sync_pass(v_sync_crash_id);
    s4_sync_veh(v_sync_crash_id);
    s4_sync_violation(v_sync_crash_id);
    s4_sync_citation(v_sync_citation_id);
    s4_sync_ref_tables();

    DELETE FROM sync_crash
    WHERE sync_id = v_sync_crash_id;
    COMMIT;

    DELETE FROM sync_citation
    WHERE sync_id = v_sync_citation_id;
    COMMIT;

    s4_sync_st();
    s4_sync_zlevel();
    s4_sync_intrsect();
    s4_sync_intrsect_node();
END;
/
