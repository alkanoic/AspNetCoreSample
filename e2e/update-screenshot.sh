#!/bin/bash

bash before-e2e-test.sh
npx playwright test --update-snapshots
bash after-e2e-test.sh
