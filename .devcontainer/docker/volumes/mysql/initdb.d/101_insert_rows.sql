INSERT INTO name (name) VALUES ('太郎'),('花子'),('令和');

INSERT INTO enum_sample (enum_column) VALUES (0),(1),(2);

INSERT INTO sample_table(
    id, target_name, target_int, target_decimal, target_date, target_bit
    , create_at, create_user, update_at, update_user
)VALUES(
    1, 'table_name', 123, 123.45, SYSDATE(), 1
    , SYSDATE(), 'create_user', SYSDATE(), 'update_user'
);

INSERT INTO multi_table(
    id, charid, target_name, target_int, target_decimal, target_date, target_bit
    , create_at, create_user, update_at, update_user
)VALUES(
    1, '001', 'table_name', 123, 123.45, SYSDATE(), 1
    , SYSDATE(), 'create_user', SYSDATE(), 'update_user'
);
