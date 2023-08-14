#!/bin/bash

bash before-e2e-test.sh
npx playwright test --config playwright.production.config.ts
npx playwright test --config playwright.develop.config.ts
bash after-e2e-test.sh
