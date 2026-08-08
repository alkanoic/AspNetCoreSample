#!/bin/bash
set -e

# HTTPS 開発証明書 (ホストが provided の場合にインポート、なければ新規作成)
if [ -f /workspaces/.aspnet/https/NetCoreWebAppOnWslDocker001.pfx ]; then
    dotnet dev-certs https --clean --import /workspaces/.aspnet/https/NetCoreWebAppOnWslDocker001.pfx --password PfxFilePassword
else
    dotnet dev-certs https --clean 2>/dev/null || true
    dotnet dev-certs https
    dotnet dev-certs https --trust 2>/dev/null || true
fi

dotnet tool restore
dotnet restore

# opencode (Ollama Cloud を利用するための TUI/CLI)
# compose.yaml の名前付きボリュームが root 所有でマウントされることがあるため、
# 先に vscode ユーザーの所有権へ修正してから、バイナリ実在確認後にインストールする
sudo chown -R vscode:vscode \
    /home/vscode/.opencode \
    /home/vscode/.config \
    /home/vscode/.cache \
    /home/vscode/.local 2>/dev/null || true
if [ ! -x /home/vscode/.opencode/bin/opencode ]; then
    curl -fsSL https://opencode.ai/install | bash || echo "opencode install failed"
fi

# Rebuild で /home/vscode/.bashrc が初期化されるため、PATH の追記と
# グローバルシンボリックリンクを必ず行う (バイナリが volume に残っている場合のみ)
if [ -x /home/vscode/.opencode/bin/opencode ] && ! command -v opencode >/dev/null 2>&1; then
    if ! grep -q '/home/vscode/.opencode/bin' /home/vscode/.bashrc 2>/dev/null; then
        printf '\n# opencode\nexport PATH=/home/vscode/.opencode/bin:$PATH\n' >> /home/vscode/.bashrc
    fi
    if [ ! -e /usr/local/bin/opencode ]; then
        sudo ln -s /home/vscode/.opencode/bin/opencode /usr/local/bin/opencode 2>/dev/null || true
    fi
fi

# SBOM 生成物 (export-cyclonedx.sh で使用)
npm install -g @cyclonedx/cyclonedx-npm || echo "cyclonedx-npm skipped"

# Nuxt (NuxtSample) は pnpm で依存を管理しているため、グローバルに導入する
npm install -g pnpm || echo "pnpm skipped"

# Nuxt (NuxtSample) のローカル環境設定 (.env) を .env.example から初期化する
if [ -f "src/NuxtSample/.env.example" ] && [ ! -f "src/NuxtSample/.env" ]; then
    cp "src/NuxtSample/.env.example" "src/NuxtSample/.env"
fi

# Agent Skills (skills-lock.json から復元 / npm install 相当)
# コミットされている .agents/skills/ と skills-lock.json から公式スキルを再インストールする。
if [ -f skills-lock.json ]; then
    (npx -y skills@latest experimental_install) || echo "skills (experimental_install) skipped"
fi

# フロント側 (Mvc/vite) の依存関係を非同期で準備
if [ -f src/AspNetCoreSample.Mvc/package.json ]; then
    (cd src/AspNetCoreSample.Mvc && npm ci) || echo "npm ci (Mvc) skipped"
fi
if [ -f e2e/package.json ]; then
    (cd e2e && npm ci) || echo "npm ci (e2e) skipped"
fi