#!/bin/bash

bash recreate-dockers.sh
npx playwright test --headed name-db.spec.ts
