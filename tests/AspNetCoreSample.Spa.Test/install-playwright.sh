#!/bin/bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

dotnet build "$script_dir/AspNetCoreSample.Spa.Test.csproj"

if command -v google-chrome >/dev/null 2>&1; then
    google-chrome --version
elif command -v google-chrome-stable >/dev/null 2>&1; then
    google-chrome-stable --version
else
    pwsh "$script_dir/bin/Debug/net10.0/playwright.ps1" install chrome --with-deps
fi
