CREATE OR REPLACE PROCEDURE s4_sync_with_warehouse (p_days_back INT)
AS
BEGIN
    s4_sync_crash_evt(p_days_back);
    s4_sync_driver(p_days_back);
    s4_sync_non_motorist(p_days_back);
    s4_sync_pass(p_days_back);
    s4_sync_veh(p_days_back);
    s4_sync_violation(p_days_back);
    s4_sync_citation(p_days_back);
END;
/
