#!/bin/bash

bash before-e2e-test.sh
npx playwright codegen --device 'iPhone 12'
bash after-e2e-test.sh
