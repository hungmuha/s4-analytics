DROP INDEX crash_evt_id_idx;
DROP INDEX crash_evt_geocode_pt_3857_idx;
DROP INDEX driver_id_idx;
DROP INDEX non_motorist_id_idx;
DROP INDEX pass_id_idx;
DROP INDEX veh_id_idx;
DROP INDEX violation_id_idx;
DROP INDEX citation_id_idx;
DROP INDEX citation_geocode_pt_3857_idx;

CALL s4_sync_crash_evt(); -- 87m
CALL s4_sync_driver();
CALL s4_sync_non_motorist();
CALL s4_sync_pass();
CALL s4_sync_veh();
CALL s4_sync_violation();
CALL s4_sync_citation(); -- 82m

CALL s4_sync_st();
CALL s4_sync_intrsect();

CREATE UNIQUE INDEX crash_evt_id_idx ON
    crash_evt ( "ID" );
CREATE INDEX crash_evt_geocode_pt_3857_idx ON
    crash_evt ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;
CREATE UNIQUE INDEX driver_id_idx ON
    driver ( "ID" );
CREATE UNIQUE INDEX non_motorist_id_idx ON
    non_motorist ( "ID" );
CREATE UNIQUE INDEX pass_id_idx ON
    pass ( "ID" );
CREATE UNIQUE INDEX veh_id_idx ON
    veh ( "ID" );
CREATE UNIQUE INDEX violation_id_idx ON
    violation ( "ID" );
CREATE UNIQUE INDEX citation_id_idx ON
    citation ( "ID" );
CREATE INDEX citation_geocode_pt_3857_idx ON
    citation ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;
