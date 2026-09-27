#!/bin/bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

dotnet build "$script_dir/AspNetCoreSample.Spa.Test.csproj"
pwsh "$script_dir/bin/Debug/net10.0/playwright.ps1" install firefox --with-deps
