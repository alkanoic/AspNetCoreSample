CREATE TABLE name (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE enum_sample (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    enum_column INT NOT NULL
);

CREATE TABLE sample_table (
    id INT NOT NULL PRIMARY KEY,
    target_name varchar(30) NOT NULL,
    target_int INT NULL,
    target_decimal DECIMAL(10,2) NULL,
    target_date DATE NULL,
    target_bit BIT NULL,
    create_at DATETIME NOT NULL,
    create_user varchar(20) NOT NULL,
    update_at DATETIME NOT NULL,
    update_user varchar(20) NOT NULL
);

CREATE TABLE multi_table (
    id INT NOT NULL,
    charid varchar(10) NOT NULL,
    target_name varchar(30) NOT NULL,
    target_int INT NULL,
    target_decimal DECIMAL(10,2) NULL,
    target_date DATE NULL,
    target_bit BIT NULL,
    create_at DATETIME NOT NULL,
    create_user varchar(20) NOT NULL,
    update_at DATETIME NOT NULL,
    update_user varchar(20) NOT NULL,
    PRIMARY KEY(id, charid)
);
