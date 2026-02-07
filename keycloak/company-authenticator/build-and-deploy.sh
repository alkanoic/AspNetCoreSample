#!/usr/bin/env bash
set -euo pipefail
# Build company-authenticator and copy produced JAR into devcontainer Keycloak providers dir
HERE="$(cd "$(dirname "$0")" && pwd)"
ROOT="$(cd "$HERE/../.." && pwd)"

cd "$HERE"
mvn -DskipTests package

mkdir -p "$ROOT/.devcontainer/docker/volumes/keycloak/providers"
JAR="$HERE/target/company-authenticator-1.0.0.jar"
if [ ! -f "$JAR" ]; then
  echo "Expected jar not found: $JAR"
  exit 2
fi
if cp "$JAR" "$ROOT/.devcontainer/docker/volumes/keycloak/providers/" 2>/dev/null; then
  echo "Deployed $(basename "$JAR") to .devcontainer/docker/volumes/keycloak/providers/"
else
  echo "Regular copy failed, trying with sudo..."
  if command -v sudo >/dev/null 2>&1 && sudo cp "$JAR" "$ROOT/.devcontainer/docker/volumes/keycloak/providers/"; then
    echo "Deployed with sudo. Adjusting ownership..."
    sudo chown $(id -u):$(id -g) "$ROOT/.devcontainer/docker/volumes/keycloak/providers/$(basename "$JAR")" || true
    echo "Deployed $(basename "$JAR") to .devcontainer/docker/volumes/keycloak/providers/ (via sudo)"
  else
    echo "Failed to copy jar to providers directory due to permission error." >&2
    echo "Please run the script with sufficient permissions or adjust the directory ownership:" >&2
    echo "  sudo mkdir -p $ROOT/.devcontainer/docker/volumes/keycloak/providers && sudo chown $(id -u):$(id -g) $ROOT/.devcontainer/docker/volumes/keycloak/providers" >&2
    exit 3
  fi
fi
