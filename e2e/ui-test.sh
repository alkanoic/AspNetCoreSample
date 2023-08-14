#!/bin/bash

bash recreate-dockers.sh
npx playwright test --ui
