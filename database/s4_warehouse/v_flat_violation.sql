CREATE OR REPLACE VIEW v_flat_violation AS
SELECT
    fv.hsmv_rpt_nbr,
    floor(fv.hsmv_rpt_nbr / 100000) || 'XXXXX' AS hsmv_rpt_nbr_trunc,
    fv.person_nbr,
    fv.citation_nbr,
    substr(fv.citation_nbr, 1, 3) || 'X-XXX' AS citation_nbr_trunc,
    fv.key_crash_dt,
    dd.crash_yr,
    dd.crash_mm,
    dd.crash_mo,
    dd.crash_dd,
    dd.crash_day,
    fv.key_geography,
    dg.dot_district_nm,
    dg.rpc_nm,
    dg.mpo_nm,
    dg.cnty_cd,
    dg.cnty_nm,
    dg.city_cd,
    dg.city_nm,
    fv.key_rptg_agncy,
    da.agncy_nm AS rptg_agncy_nm,
    da.agncy_short_nm AS rptg_agncy_short_nm,
    da.agncy_type_nm AS rptg_agncy_type_nm,
    fv.key_rptg_unit,
    dau.agncy_nm AS rptg_unit_nm,
    dau.agncy_short_nm AS rptg_unit_short_nm,
    fv.fl_statute_nbr,
    fv.charge,
    fv.batch_nbr
FROM fact_violation fv
LEFT JOIN dim_date dd ON fv.key_crash_dt = dd.crash_dt
LEFT JOIN dim_geography dg ON fv.key_geography = dg.ID
LEFT JOIN dim_agncy da ON fv.key_rptg_agncy = da.ID
LEFT JOIN dim_agncy dau ON fv.key_rptg_unit = dau.ID
;
