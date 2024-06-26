CREATE TABLE name
(
    id INT NOT NULL
    AUTO_INCREMENT PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE enum_sample
(
    id INT NOT NULL
    AUTO_INCREMENT PRIMARY KEY,
    enum_column INT NOT NULL
);

CREATE TABLE sample_table
(
    id INT NOT NULL PRIMARY KEY,
    target_name varchar(30) NOT NULL,
    target_int INT NULL,
    target_decimal DECIMAL(10, 2) NULL,
    target_date DATE NULL,
    target_bit BIT NULL,
    create_at DATETIME NOT NULL,
    create_user varchar(20) NOT NULL,
    update_at DATETIME NOT NULL,
    update_user varchar(20) NOT NULL
);

CREATE TABLE multi_table
(
    id INT NOT NULL
    COMMENT 'マルチID',
    charid varchar(10) NOT NULL COMMENT 'マルチStringId',
    target_name varchar(30) NOT NULL COMMENT '名前',
    target_int INT NULL COMMENT 'int型',
    target_decimal DECIMAL(10, 2) NULL COMMENT 'decimal型',
    target_date DATE NULL COMMENT '日付型',
    target_bit BIT NULL COMMENT 'Bit型',
    create_at DATETIME NOT NULL COMMENT '作成日時',
    create_user varchar(20) NOT NULL COMMENT '作成ユーザー',
    update_at DATETIME NOT NULL COMMENT '更新日時',
    update_user varchar(20) NOT NULL COMMENT '更新ユーザー',
    PRIMARY KEY(id, charid)
) COMMENT = 'マルチテーブル';

CREATE TABLE parent_table
(
    id INT NOT NULL PRIMARY KEY,
    target_name varchar(30) NOT NULL,
    target_int INT NULL,
    target_decimal DECIMAL(10, 2) NULL,
    target_date DATE NULL,
    target_bit BIT NULL,
    create_at DATETIME NOT NULL,
    create_user varchar(20) NOT NULL,
    update_at DATETIME NOT NULL,
    update_user varchar(20) NOT NULL
);

CREATE TABLE child_table
(
    id INT NOT NULL
    AUTO_INCREMENT PRIMARY KEY,
    parent_id INT NOT NULL,
    child_name VARCHAR(30) NOT NULL,
    child_int INT NULL,
    child_decimal DECIMAL(10, 2) NULL,
    child_date DATE NULL,
    child_bit BIT NULL,
    create_at DATETIME NOT NULL,
    create_user VARCHAR(20) NOT NULL,
    update_at DATETIME NOT NULL,
    update_user VARCHAR(20) NOT NULL,
    CONSTRAINT child_table_fk FOREIGN KEY (parent_id) REFERENCES parent_table (id)
);

CREATE TABLE policies
(
    policy_name VARCHAR(255) NOT NULL,
    PRIMARY KEY(policy_name)
);

CREATE TABLE role_policies
(
    policy_name VARCHAR(255) NOT NULL,
    role_name VARCHAR(255) NOT NULL,
    PRIMARY KEY(policy_name, role_name),
    CONSTRAINT policies_fk FOREIGN KEY (policy_name) REFERENCES policies (policy_name)
);
