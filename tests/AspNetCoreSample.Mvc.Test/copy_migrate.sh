#!/bin/bash

mkdir -p migrate
cp -r -f ../../.devcontainer/docker/volumes/mysql/initdb.d/* migrate/
cp -f  ../../.devcontainer/docker/volumes/mysql/my.cnf my.cnf
