#!/bin/bash
set -euo pipefail

: "${MODEL_S3_URI:?MODEL_S3_URI is not set}"
MODEL_PATH="${MODEL_PATH:-/models/Ternary-Bonsai-2-27B-PQ2_0.gguf}"

if [ ! -s "$MODEL_PATH" ]; then
  mkdir -p "$(dirname "$MODEL_PATH")"
  echo "Downloading model from ${MODEL_S3_URI} ..."
  aws s3 cp "$MODEL_S3_URI" "$MODEL_PATH"
fi

if [ -n "${MODEL_SHA256:-}" ]; then
  echo "${MODEL_SHA256}  ${MODEL_PATH}" | sha256sum -c -
fi

exec /opt/llama/llama-server "$@"
