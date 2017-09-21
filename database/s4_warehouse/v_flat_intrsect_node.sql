CREATE OR REPLACE VIEW v_flat_intrsect_node AS
SELECT
    node_id,
    intersection_id
FROM navteq_2015q1.intrsect_node;
