# MySQL
export MYSQL_PWD=mysql
# 現在のスキーマ情報を出力
mysqldef -u root -h mysql -P 3306 --export sample
# 現在のスキーマに変更した場合のSQL
cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | \
    mysqldef -u root -h mysql -P 3306 --dry-run sample

# PostgreSQL
export PGPASSWORD=postgres
# 現在のスキーマ情報を出力
psqldef --user root --host postgresql --port 5432 --export sample
# 現在のスキーマに変更した場合のSQL
cat .devcontainer/docker/volumes/postgresql/initdb.d/0*.sql | \
    grep -v "comment" | \
    psqldef --user root --host postgresql --port 5432 --dry-run sample

# MSSQL
export MSSQL_PWD=mssql_PASS1
# 現在のスキーマ情報を出力
mssqldef --user sa --host mssql --port 1433 --export sample
# 現在のスキーマに変更した場合のSQL
cat .devcontainer/docker/volumes/mssql/initdb.d/0*.sql | \
    grep -v "CREATE DATABASE" | \
    grep -v "USE" | \
    mssqldef --user sa --host mssql --port 1433 --dry-run sample
