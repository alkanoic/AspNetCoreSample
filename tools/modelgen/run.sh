#!/usr/bin/env bash
# modelgen ラッパー。リポジトリルートを基準に実行する。
# 使い方: tools/modelgen/run.sh check --strict / generate / graph
set -euo pipefail

TOOL_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$TOOL_DIR/../.." && pwd)"

if [ -n "${PYTHON:-}" ]; then
  PY="$PYTHON"
elif [ -x "$ROOT/.venv/bin/python" ] \
  && "$ROOT/.venv/bin/python" -c 'import yaml, jsonschema' >/dev/null 2>&1; then
  PY="$ROOT/.venv/bin/python"
elif command -v python3 >/dev/null 2>&1; then
  PY=python3
else
  PY=python
fi

cd "$ROOT"
exec env PYTHONPATH="$TOOL_DIR${PYTHONPATH:+:$PYTHONPATH}" "$PY" -m modelgen "$@"
