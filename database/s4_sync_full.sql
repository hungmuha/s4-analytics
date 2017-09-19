DROP INDEX crash_evt_id_idx;
DROP INDEX crash_evt_geocode_pt_3857_idx;

CALL s4_sync_crash_evt(); -- 87m

CREATE UNIQUE INDEX crash_evt_id_idx ON
    crash_evt ( "ID" );
CREATE INDEX crash_evt_geocode_pt_3857_idx ON
    crash_evt ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CALL s4_sync_driver();
CALL s4_sync_non_motorist();
CALL s4_sync_pass();
CALL s4_sync_veh();
CALL s4_sync_violation();

DROP INDEX citation_id_idx;
DROP INDEX citation_geocode_pt_3857_idx;

CALL s4_sync_citation(); -- 82m

CREATE UNIQUE INDEX citation_id_idx ON
    citation ( "ID" );
CREATE INDEX citation_geocode_pt_3857_idx ON
    citation ( geocode_pt_3857 )
        INDEXTYPE IS mdsys.spatial_index;

CALL s4_sync_st();
CALL s4_sync_intrsect();
