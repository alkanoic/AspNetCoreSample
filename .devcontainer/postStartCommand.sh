#!/bin/bash
set -e

# HTTPS 開発証明書を起動のたびに確認し、不足時に生成する
# (postCreateCommand はコンテナ作成時に一度しか実行されないため、
#  既存コンテナの再起動でも必ず検証する目的で postStart から呼び出す)
if ! dotnet dev-certs https --check >/dev/null 2>&1; then
    dotnet dev-certs https 2>/dev/null || true
fi
