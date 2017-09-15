CREATE OR REPLACE PROCEDURE s4_sync_citation (p_days_back INT DEFAULT NULL)
AS
    v_start_dt DATE;
BEGIN
    v_start_dt := CASE WHEN p_days_back IS NOT NULL THEN TRUNC(SYSDATE - p_days_back) ELSE NULL END;

    IF v_start_dt IS NULL THEN
        EXECUTE IMMEDIATE 'TRUNCATE TABLE citation';
    ELSE
        DELETE FROM citation ci
        WHERE EXISTS (
            SELECT 1
            FROM v_flat_citation@s4_warehouse vc
            WHERE vc.last_updt_dt >= v_start_dt
            AND vc.hsmv_rpt_nbr = ci.hsmv_rpt_nbr
        );
    END IF;

    INSERT INTO citation (

    )
    SELECT

    FROM v_flat_citation@s4_warehouse vc
    WHERE v_start_dt IS NULL
    OR vc.last_updt_dt >= v_start_dt;

    COMMIT;
END;
/
