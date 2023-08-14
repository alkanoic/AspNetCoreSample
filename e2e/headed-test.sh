#!/bin/bash

bash before-e2e-test.sh
npx playwright test --headed name-db.spec.ts
bash after-e2e-test.sh
