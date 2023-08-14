#!/bin/bash

bash recreate-dockers.sh
npx playwright test --update-snapshots
