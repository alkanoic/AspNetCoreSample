# MySQL
export MYSQL_PWD=mysql
MYSQL_HOST=mysql
MYSQL_DB=sample
MYSQL_USER=root
MYSQL_PORT=3306

# 現在のスキーマ情報を出力
mysqldef -u ${MYSQL_USER} -h ${MYSQL_HOST} -P ${MYSQL_PORT} --export ${MYSQL_DB}
# 現在のスキーマに変更した場合のSQL syntax errorが出るため以下のスクリプトに変換
# cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | \
#   mysqldef -u ${MYSQL_USER} -h ${MYSQL_HOST} -P ${MYSQL_PORT} --dry-run ${MYSQL_DB}

# A5M2で作成されたDDLのテーブルコメントの) COMMENT '' の部分を COMMENT = ''に変換する
cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | sed -E "s/^\) COMMENT/\) COMMENT =/Ig" | \
    mysqldef -u ${MYSQL_USER} -h ${MYSQL_HOST} -P ${MYSQL_PORT} --dry-run ${MYSQL_DB}

# 実際のDBに反映
# cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | \
#   mysqldef -u ${MYSQL_USER} -h ${MYSQL_HOST} -P ${MYSQL_PORT} ${MYSQL_DB}

# dry-runしたものをmysqlコマンドで実行する方法
cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | sed -E "s/COMMENT\s*'[^']*'//Ig" | \
    mysqldef -u ${MYSQL_USER} -h ${MYSQL_HOST} -P ${MYSQL_PORT} --dry-run ${MYSQL_DB} | \
    awk '/DROP FOREIGN KEY/ {matched = matched $0 "\n";next}{others = others $0 "\n"}END{printf "%s%s", matched, others}' | \
    sed '/COMMENT = ;/d' | sed '/-- dry run --/d' | \
    mysql -u ${MYSQL_USER} -p${MYSQL_PWD} -h ${MYSQL_HOST} ${MYSQL_DB}

# PostgreSQL
export PGPASSWORD=postgres\
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
