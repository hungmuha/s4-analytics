CREATE OR REPLACE VIEW v_rpt_crash_attr AS
WITH counties AS (
    SELECT cnty_cd, cnty_nm, ROWNUM AS seq
    FROM (
        SELECT cnty_cd, cnty_nm
        FROM dim_geography
        WHERE city_cd = 0
        ORDER BY CASE WHEN cnty_nm = 'Unknown' THEN 'Z' ELSE cnty_nm END
    )
)
SELECT
    "ID",
    hsmv_rpt_nbr,
    nvl(crash_day, 'Unknown') AS crash_day,
    crash_d AS crash_day_sort,
    nvl(crash_hh_am, 'Unknown') AS crash_hh_am,
    crash_hh24 AS crash_hh_am_sort,
    nvl(weather_cond, 'Unknown') AS weather_cond,
    key_weather_cond AS weather_cond_sort,
    nvl(light_cond, 'Unknown') AS light_cond,
    nvl(key_light_cond, 107) AS light_cond_sort,
    crash_type,
    CASE WHEN crash_type = 'Unknown' THEN 999 ELSE key_crash_type END AS crash_type_sort,
    crash_sev_dtl,
    key_crash_sev_dtl AS crash_sev_dtl_sort,
    nvl(rd_surf_cond, 'Unknown') AS rd_surf_cond,
    nvl(key_rd_surf_cond, 168) AS rd_surf_cond_sort,
    nvl(first_he, 'Unknown') AS first_he,
    key_first_he AS first_he_sort,
    nvl(counties.cnty_nm, 'Unknown') AS cnty_nm,
    nvl(counties.seq, 68) AS cnty_nm_sort
FROM crash_evt
LEFT OUTER JOIN counties
    ON counties.cnty_cd = crash_evt.cnty_cd
;
