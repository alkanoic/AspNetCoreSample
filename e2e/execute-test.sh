#!/bin/bash

bash recreate-dockers.sh
npx playwright test --reporter=line,html,allure-playwright
