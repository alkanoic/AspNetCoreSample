#!/bin/bash

bash before-e2e-test.sh
npx playwright test --ui
bash after-e2e-test.sh
