DROP INDEX crash_evt_id_idx;
DROP INDEX crash_evt_gps_pt_4326_idx;
DROP INDEX crash_evt_geocode_pt_3087_idx;

CALL s4_sync_crash_evt(); -- 1.5h

CREATE UNIQUE INDEX crash_evt_id_idx ON
    crash_evt ( "ID" );
CREATE INDEX crash_evt_gps_pt_4326_idx ON
    crash_evt ( gps_pt_4326 )
        INDEXTYPE IS mdsys.spatial_index;
CREATE INDEX crash_evt_geocode_pt_3087_idx ON
    crash_evt ( geocode_pt_3087 )
        INDEXTYPE IS mdsys.spatial_index;

CALL s4_sync_driver();
CALL s4_sync_non_motorist();
CALL s4_sync_pass();
CALL s4_sync_veh();
CALL s4_sync_violation();

DROP UNIQUE INDEX citation_id_idx;
DROP INDEX citation_gps_pt_4326_idx;
DROP INDEX citation_geocode_pt_3087_idx;

CALL s4_sync_citation();

CREATE UNIQUE INDEX citation_id_idx ON
    citation ( "ID" );
CREATE INDEX citation_gps_pt_4326_idx ON
    citation ( gps_pt_4326 )
        INDEXTYPE IS mdsys.spatial_index;
CREATE INDEX citation_geocode_pt_3087_idx ON
    citation ( geocode_pt_3087 )
        INDEXTYPE IS mdsys.spatial_index;