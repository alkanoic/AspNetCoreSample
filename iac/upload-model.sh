#!/bin/bash
# Hugging Face からモデルを取得し、検証してから S3 へアップロードする。
# 使い方: MODEL_S3_URI=s3://<bucket>/models/Ternary-Bonsai-2-27B-PQ2_0.gguf ./upload-model.sh
set -euo pipefail

MODEL_REPO="${MODEL_REPO:-prism-ml/Ternary-Bonsai-2-27B-gguf}"
MODEL_FILE="${MODEL_FILE:-Ternary-Bonsai-2-27B-PQ2_0.gguf}"
MODEL_REVISION="${MODEL_REVISION:-b072e1d3b35a0a630cece372c2127528e0994386}"
MODEL_SHA256="${MODEL_SHA256:-3907dc1658db1f78a9826bf8d5bcb8dc65db0d466388937af57f2294fae62ec1}"
MODEL_S3_URI="${MODEL_S3_URI:?MODEL_S3_URI を指定してください（例: s3://<bucket>/models/Ternary-Bonsai-2-27B-PQ2_0.gguf）}"
WORK_DIR="${WORK_DIR:-./.model-work}"

command -v curl >/dev/null || { echo "curl が必要です" >&2; exit 1; }
if [ "${SKIP_UPLOAD:-0}" != "1" ]; then
  command -v aws >/dev/null || { echo "aws CLI が必要です" >&2; exit 1; }
fi

mkdir -p "$WORK_DIR"
LOCAL_PATH="$WORK_DIR/$MODEL_FILE"

if [ ! -s "$LOCAL_PATH" ]; then
  echo "Downloading ${MODEL_REPO}@${MODEL_REVISION} ..."
  curl --fail --location --retry 3 \
    --output "$LOCAL_PATH" \
    "https://huggingface.co/${MODEL_REPO}/resolve/${MODEL_REVISION}/${MODEL_FILE}"
fi

echo "${MODEL_SHA256}  ${LOCAL_PATH}" | sha256sum -c -

if [ "${SKIP_UPLOAD:-0}" = "1" ]; then
  echo "SKIP_UPLOAD=1 のため S3 アップロードを省略しました: ${LOCAL_PATH}"
  exit 0
fi

echo "Uploading to ${MODEL_S3_URI} ..."
aws s3 cp "$LOCAL_PATH" "$MODEL_S3_URI"

echo "Verifying upload ..."
aws s3 ls "$MODEL_S3_URI"

echo "Done: ${MODEL_S3_URI}"
