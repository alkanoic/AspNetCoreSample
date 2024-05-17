export MYSQL_PWD=mysql
# 現在のスキーマ情報を出力
# cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | mysqldef -u root -h mysql -P 3306 --export sample
# 現在のスキーマに変更した場合のSQL
cat .devcontainer/docker/volumes/mysql/initdb.d/0*.sql | mysqldef -u root -h mysql -P 3306 --dry-run sample
