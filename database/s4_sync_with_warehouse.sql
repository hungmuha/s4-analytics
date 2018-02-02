CREATE OR REPLACE PROCEDURE s4_sync_with_warehouse (p_start_dt DATE, p_end_dt DATE DEFAULT NULL)
AS
    v_sync_crash_id INT := NULL;
    v_sync_citation_id INT := NULL;
BEGIN
    SELECT sync_crash_seq.nextval INTO v_sync_crash_id FROM dual;
    SELECT sync_citation_seq.nextval INTO v_sync_citation_id FROM dual;

    INSERT INTO sync_crash
    SELECT v_sync_crash_id, hsmv_rpt_nbr
    FROM v_flat_crash_evt@s4_warehouse vce
    WHERE vce.last_updt_dt >= trunc(p_start_dt)
    AND (p_end_dt IS NULL OR vce.last_updt_dt < trunc(p_end_dt + 1));
    COMMIT;

    INSERT INTO sync_citation
    SELECT v_sync_citation_id, citation_nbr
    FROM v_flat_citation@s4_warehouse vci
    WHERE vci.last_updt_dt >= trunc(p_start_dt)
    AND (p_end_dt IS NULL OR vci.last_updt_dt < trunc(p_end_dt + 1));
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
END;
/

CREATE OR REPLACE PROCEDURE s4_sync_with_warehouse_trailing (p_days INT DEFAULT NULL)
AS
    v_start_dt DATE;
BEGIN
    v_start_dt := TRUNC(SYSDATE - p_days);
    s4_sync_with_warehouse(v_start_dt);
END;
/
