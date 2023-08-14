#!/bin/bash

bash before-e2e-test.sh
npx playwright test --reporter=line,html,allure-playwright
bash after-e2e-test.sh
