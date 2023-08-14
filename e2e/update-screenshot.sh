#!/bin/bash

bash before-e2e-test.sh
npx playwright test --config playwright.production.config.ts --update-snapshots
npx playwright test --config playwright.develop.config.ts --update-snapshots
bash after-e2e-test.sh
