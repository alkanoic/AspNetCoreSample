#!/bin/bash
set -e

# HTTPS 開発証明書を起動のたびに確認し、不足時に生成する
# (postCreateCommand はコンテナ作成時に一度しか実行されないため、
#  既存コンテナの再起動でも必ず検証する目的で postStart から呼び出す)
if ! dotnet dev-certs https --check >/dev/null 2>&1; then
    dotnet dev-certs https 2>/dev/null || true
fi

# opencode web を起動する (Tailscale 経由で他端末から接続するため)
# postStartCommand は非対話シェルで実行され PATH が通っていないことがあるため、
# 絶対パスで起動する。既に起動済みなら何もしない。
# プロジェクトを /workspaces/AspNetCoreSample に固定するため cwd を明示する。
OPENCODE_BIN="/home/vscode/.opencode/bin/opencode"
if [ -x "$OPENCODE_BIN" ] && ! pgrep -f "opencode web" >/dev/null 2>&1; then
    # setsid + </dev/null で完全にデタッチする (シェルの fd を保持させない)
    (cd /workspaces/AspNetCoreSample && setsid "$OPENCODE_BIN" web --hostname 0.0.0.0 --port 8088 </dev/null >/tmp/opencode-web.log 2>&1 &)
fi

# code-server (ブラウザで動く VS Code) を起動する
# パスワードは opencode と同じ OPENCODE_SERVER_PASSWORD を使う (未設定なら自動生成)
# VSCODE_IPC_HOOK_CLI が設定されていると既存 VSCode に接続して終了するため解除する
if command -v code-server >/dev/null 2>&1 && ! pgrep -f "code-server --bind-addr" >/dev/null 2>&1; then
    # パスワードを config.yaml に反映する (--password フラグは使えないため)
    # config.yaml が無い場合は先に生成する
    CODE_SERVER_CONFIG="$HOME/.config/code-server/config.yaml"
    if [ -n "$OPENCODE_SERVER_PASSWORD" ]; then
        mkdir -p "$HOME/.config/code-server"
        if [ ! -f "$CODE_SERVER_CONFIG" ]; then
            printf 'bind-addr: 127.0.0.1:8080\nauth: password\npassword: %s\ncert: false\n' "$OPENCODE_SERVER_PASSWORD" > "$CODE_SERVER_CONFIG"
        else
            sed -i "s/^password:.*/password: $OPENCODE_SERVER_PASSWORD/" "$CODE_SERVER_CONFIG"
        fi
    fi
    env -u VSCODE_IPC_HOOK_CLI setsid --fork code-server --bind-addr 0.0.0.0:8089 --auth password /workspaces/AspNetCoreSample </dev/null >/tmp/code-server.log 2>&1
fi
