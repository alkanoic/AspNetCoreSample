#!/bin/bash

npx allure generate ./allure-results --clean
npx allure open ./allure-report --port 10001
