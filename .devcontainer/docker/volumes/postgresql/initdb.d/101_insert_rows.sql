INSERT INTO name (name) VALUES ('太郎'),('花子'),('令和');

INSERT INTO enum_sample (enum_column) VALUES (0),(1),(2);

INSERT INTO sample_table(
    id, target_name, target_int, target_decimal, target_date, target_bit
    , create_at, create_user, update_at, update_user
)VALUES(
    1, 'table_name', 123, 123.45, NOW(), TRUE
    , NOW(), 'create_user', NOW(), 'update_user'
);

INSERT INTO multi_table(
    id, charid, target_name, target_int, target_decimal, target_date, target_bit
    , create_at, create_user, update_at, update_user
)VALUES
(1, '001', 'table_name', 123, 123.45, NOW(), TRUE, NOW(), 'create_user', NOW(), 'update_user'),
(2, '002', 'table_name2', 223, 223.45, NOW(), FALSE, NOW(), 'create_user2', NOW(), 'update_user2'),
(3, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(4, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(5, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(6, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(7, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(8, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(9, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(10, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(11, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(12, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(13, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3'),
(14, '003', 'table_name', 323, 323.45, NOW(), TRUE, NOW(), 'create_user3', NOW(), 'update_user3');

INSERT INTO policies(
    policy_name
)VALUES
('Admin'),('User')
;

INSERT INTO role_policies(
    policy_name, role_name
)VALUES
('Admin', 'admin'),
('User', 'admin'),
('User', 'user')
;
