CREATE TABLE name (
    id SERIAL NOT NULL PRIMARY KEY,
    name TEXT NOT NULL
);

-- Project Name : sample project
-- Date/Time    : 2022/11/19 21:28:43
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

-- ユーザー権限
--* RestoreFromTempTable
create table UserRoles (
  UserId varchar(100) not null
  , RoleId varchar(10) not null
  , constraint UserRoles_PKC primary key (UserId,RoleId)
) ;

-- 権限
--* RestoreFromTempTable
create table Roles (
  RoleId varchar(10) not null
  , RoleName varchar(100) not null
  , constraint Roles_PKC primary key (RoleId)
) ;

-- ユーザー
--* RestoreFromTempTable
create table Users (
  UserId varchar(100) not null
  , UserName varchar(100) not null
  , UserStatus int not null
  , constraint Users_PKC primary key (UserId)
) ;

alter table UserRoles
  add constraint UserRoles_FK1 foreign key (RoleId) references Roles(RoleId);

alter table UserRoles
  add constraint UserRoles_FK2 foreign key (UserId) references Users(UserId);

comment on table UserRoles is 'ユーザー権限';
comment on column UserRoles.UserId is 'ユーザーID';
comment on column UserRoles.RoleId is '権限ID';

comment on table Roles is '権限';
comment on column Roles.RoleId is '権限ID';
comment on column Roles.RoleName is '権限名';

comment on table Users is 'ユーザー';
comment on column Users.UserId is 'ユーザーID';
comment on column Users.UserName is 'ユーザー名';
comment on column Users.UserStatus is 'ユーザーステータス';

