CREATE OR REPLACE PROCEDURE s4_sync_with_warehouse (p_start_dt DATE DEFAULT NULL, p_end_dt DATE DEFAULT NULL)
AS
    v_sync_id INT := NULL;
BEGIN
    IF p_start_dt IS NOT NULL OR p_end_dt IS NOT NULL THEN
        SELECT warehouse_sync_seq.nextval INTO v_sync_id FROM dual;

        INSERT INTO warehouse_sync
        SELECT v_sync_id, hsmv_rpt_nbr
        FROM v_flat_crash_evt@s4_warehouse vce
        WHERE (p_start_dt IS NULL OR vce.last_updt_dt >= trunc(p_start_dt))
        AND (p_end_dt IS NULL OR vce.last_updt_dt < trunc(p_end_dt + 1));

        COMMIT;
    END IF;

    s4_sync_crash_evt(v_sync_id);
    /* s4_sync_driver(v_sync_id);
    s4_sync_non_motorist(v_sync_id);
    s4_sync_pass(v_sync_id);
    s4_sync_veh(v_sync_id);
    s4_sync_violation(v_sync_id);
    s4_sync_citation(v_sync_id); */
    s4_sync_ref_tables();
END;
/
