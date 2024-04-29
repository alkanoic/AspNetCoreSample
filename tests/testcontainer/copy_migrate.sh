#!/bin/bash

mkdir -p migrate
cp -r -f ../../.devcontainer/docker/volumes/mysql/initdb.d/* migrate/
cp -f ../../.devcontainer/docker/volumes/mysql/my.cnf my.cnf
cp -f ../../.devcontainer/docker/volumes/keycloak/data/import/Test-realm.json Test-realm.json
