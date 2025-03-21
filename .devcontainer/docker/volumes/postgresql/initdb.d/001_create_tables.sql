-- Project Name : noname
-- Date/Time    : 2025/03/13 22:40:05
-- Author       : naoto
-- RDBMS Type   : PostgreSQL
-- Application  : A5:SQL Mk-2

/*
  << 注意！！ >>
  BackupToTempTable, RestoreFromTempTable疑似命令が付加されています。
  これにより、drop table, create table 後もデータが残ります。
  この機能は一時的に $$TableName のような一時テーブルを作成します。
  この機能は A5:SQL Mk-2でのみ有効であることに注意してください。
*/

-- child_table
-- * RestoreFromTempTable
CREATE TABLE child_table (
  id serial NOT NULL
  , parent_id int NOT NULL
  , child_name varchar(30) NOT NULL
  , child_int int
  , child_decimal decimal(10,2)
  , child_date date
  , child_bit boolean
  , create_at timestamp NOT NULL
  , create_user varchar(20) NOT NULL
  , update_at timestamp NOT NULL
  , update_user varchar(20) NOT NULL
  , CONSTRAINT child_table_PKC PRIMARY KEY (id)
) ;

CREATE INDEX child_table_fk
  ON child_table(parent_id);

-- enum_sample
-- * RestoreFromTempTable
CREATE TABLE enum_sample (
  id serial NOT NULL
  , enum_column int NOT NULL
  , CONSTRAINT enum_sample_PKC PRIMARY KEY (id)
) ;

-- マルチテーブル
-- * RestoreFromTempTable
CREATE TABLE multi_table (
  id int NOT NULL
  , charid varchar(10) NOT NULL
  , target_name varchar(30) NOT NULL
  , target_int int
  , target_decimal decimal(10,2)
  , target_date date
  , target_bit boolean
  , create_at timestamp NOT NULL
  , create_user varchar(20) NOT NULL
  , update_at timestamp NOT NULL
  , update_user varchar(20) NOT NULL
  , CONSTRAINT multi_table_PKC PRIMARY KEY (id,charid)
) ;

CREATE INDEX idx_target_name
  ON multi_table(target_name);

-- name
-- * RestoreFromTempTable
CREATE TABLE name (
  id serial NOT NULL
  , name text NOT NULL
  , CONSTRAINT name_PKC PRIMARY KEY (id)
) ;

-- parent_table
-- * RestoreFromTempTable
CREATE TABLE parent_table (
  id int NOT NULL
  , target_name varchar(30) NOT NULL
  , target_int int
  , target_decimal decimal(10,2)
  , target_date date
  , target_bit boolean
  , create_at timestamp NOT NULL
  , create_user varchar(20) NOT NULL
  , update_at timestamp NOT NULL
  , update_user varchar(20) NOT NULL
  , CONSTRAINT parent_table_PKC PRIMARY KEY (id)
) ;

-- policies
-- * RestoreFromTempTable
CREATE TABLE policies (
  policy_name varchar(255) NOT NULL
  , CONSTRAINT policies_PKC PRIMARY KEY (policy_name)
) ;

-- role_policies
-- * RestoreFromTempTable
CREATE TABLE role_policies (
  role_name varchar(255) NOT NULL
  , policy_name varchar(255) NOT NULL
  , CONSTRAINT role_policies_PKC PRIMARY KEY (role_name,policy_name)
) ;

CREATE INDEX policies_fk
  ON role_policies(policy_name);

-- sample_table
-- * RestoreFromTempTable
CREATE TABLE sample_table (
  id int NOT NULL
  , target_name varchar(30) NOT NULL
  , target_int int
  , target_decimal decimal(10,2)
  , target_date date
  , target_bit boolean
  , create_at timestamp NOT NULL
  , create_user varchar(20) NOT NULL
  , update_at timestamp NOT NULL
  , update_user varchar(20) NOT NULL
  , CONSTRAINT sample_table_PKC PRIMARY KEY (id)
) ;

ALTER TABLE child_table
  ADD CONSTRAINT child_table_FK1 FOREIGN KEY (parent_id) REFERENCES parent_table(id);

ALTER TABLE role_policies
  ADD CONSTRAINT role_policies_FK1 FOREIGN KEY (policy_name) REFERENCES policies(policy_name);

COMMENT ON TABLE child_table IS 'child_table';
COMMENT ON COLUMN child_table.id IS '子テーブルId';
COMMENT ON COLUMN child_table.parent_id IS '親テーブルid';
COMMENT ON COLUMN child_table.child_name IS '子テーブル要素名';
COMMENT ON COLUMN child_table.child_int IS '子テーブルint型';
COMMENT ON COLUMN child_table.child_decimal IS '子テーブルdeimal型';
COMMENT ON COLUMN child_table.child_date IS '子テーブルDate型';
COMMENT ON COLUMN child_table.child_bit IS '子テーブルbit型';
COMMENT ON COLUMN child_table.create_at IS '作成日時';
COMMENT ON COLUMN child_table.create_user IS '作成ユーザー';
COMMENT ON COLUMN child_table.update_at IS '更新日時';
COMMENT ON COLUMN child_table.update_user IS '更新ユーザー';

COMMENT ON TABLE enum_sample IS 'enum_sample';
COMMENT ON COLUMN enum_sample.id IS 'id';
COMMENT ON COLUMN enum_sample.enum_column IS 'Enum列';

COMMENT ON TABLE multi_table IS 'マルチテーブル';
COMMENT ON COLUMN multi_table.id IS 'マルチID';
COMMENT ON COLUMN multi_table.charid IS 'マルチStringId';
COMMENT ON COLUMN multi_table.target_name IS '名前';
COMMENT ON COLUMN multi_table.target_int IS 'int型';
COMMENT ON COLUMN multi_table.target_decimal IS 'decimal型';
COMMENT ON COLUMN multi_table.target_date IS '日付型';
COMMENT ON COLUMN multi_table.target_bit IS 'Bit型';
COMMENT ON COLUMN multi_table.create_at IS '作成日時';
COMMENT ON COLUMN multi_table.create_user IS '作成ユーザー';
COMMENT ON COLUMN multi_table.update_at IS '更新日時';
COMMENT ON COLUMN multi_table.update_user IS '更新ユーザー';

COMMENT ON TABLE name IS 'name';
COMMENT ON COLUMN name.id IS 'id';
COMMENT ON COLUMN name.name IS '名前列';

COMMENT ON TABLE parent_table IS 'parent_table';
COMMENT ON COLUMN parent_table.id IS '親テーブルid';
COMMENT ON COLUMN parent_table.target_name IS '親テーブル要素名';
COMMENT ON COLUMN parent_table.target_int IS '親テーブルint型';
COMMENT ON COLUMN parent_table.target_decimal IS '親テーブルdecimal型';
COMMENT ON COLUMN parent_table.target_date IS '親テーブルdate型';
COMMENT ON COLUMN parent_table.target_bit IS '親テーブルbit型';
COMMENT ON COLUMN parent_table.create_at IS '作成日時';
COMMENT ON COLUMN parent_table.create_user IS '作成ユーザー';
COMMENT ON COLUMN parent_table.update_at IS '更新日時';
COMMENT ON COLUMN parent_table.update_user IS '更新ユーザー';

COMMENT ON TABLE policies IS 'policies';
COMMENT ON COLUMN policies.policy_name IS 'ポリシー名';

COMMENT ON TABLE role_policies IS 'role_policies';
COMMENT ON COLUMN role_policies.role_name IS 'ロール名';
COMMENT ON COLUMN role_policies.policy_name IS 'ポリシー名';

COMMENT ON TABLE sample_table IS 'sample_table';
COMMENT ON COLUMN sample_table.id IS 'id';
COMMENT ON COLUMN sample_table.target_name IS 'target_name';
COMMENT ON COLUMN sample_table.target_int IS 'target_int';
COMMENT ON COLUMN sample_table.target_decimal IS 'target_decimal';
COMMENT ON COLUMN sample_table.target_date IS 'target_date';
COMMENT ON COLUMN sample_table.target_bit IS 'target_bit';
COMMENT ON COLUMN sample_table.create_at IS 'create_at';
COMMENT ON COLUMN sample_table.create_user IS 'create_user';
COMMENT ON COLUMN sample_table.update_at IS 'update_at';
COMMENT ON COLUMN sample_table.update_user IS 'update_user';

