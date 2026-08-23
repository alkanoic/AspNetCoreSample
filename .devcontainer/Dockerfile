# syntax=docker/dockerfile:1
# # 開発コンテナ (マルチサービス: ASP.NET Core + Nuxt + Spring Boot + dbs)
ARG VARIANT="10.0-noble"
FROM mcr.microsoft.com/devcontainers/dotnet:${VARIANT}

ARG TARGETARCH

# 基本 CLI と DB クライアント
RUN apt-get update && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        gnupg \
        iputils-ping \
        mysql-client \
        postgresql-client \
        protobuf-compiler \
        python3 \
        python3-pip \
        python3-venv \
        wget \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# trivy (vulnerability scanner) - signed 方式でインストール
RUN wget -q -O - https://aquasecurity.github.io/trivy-repo/deb/public.key | gpg --dearmor -o /usr/share/keyrings/trivy.gpg \
    && echo "deb [signed-by=/usr/share/keyrings/trivy.gpg] https://aquasecurity.github.io/trivy-repo/deb $(lsb_release -sc) main" \
        > /etc/apt/sources.list.d/trivy.list \
    && apt-get update \
    && apt-get install -y trivy

# protoc-gen-grpc-web (マルチアーキ)
RUN GRPC_WEB_ARCH="$(case "${TARGETARCH:-amd64}" in arm64) echo aarch64;; *) echo x86_64;; esac)" \
    && wget -q "https://github.com/grpc/grpc-web/releases/download/1.5.0/protoc-gen-grpc-web-1.5.0-linux-${GRPC_WEB_ARCH}" \
    && mv "protoc-gen-grpc-web-1.5.0-linux-${GRPC_WEB_ARCH}" /usr/local/bin/protoc-gen-grpc-web \
    && chmod +x /usr/local/bin/protoc-gen-grpc-web

# grpcurl (マルチアーキ対応)
RUN GRPCURL_ARCH="$(case "${TARGETARCH:-amd64}" in arm64) echo arm64;; *) echo x86_64;; esac)" \
    && wget -q "https://github.com/fullstorydev/grpcurl/releases/download/v1.8.9/grpcurl_1.8.9_linux_${GRPCURL_ARCH}.tar.gz" \
    && tar -xzf "grpcurl_1.8.9_linux_${GRPCURL_ARCH}.tar.gz" -C /usr/local/bin grpcurl \
    && rm "grpcurl_1.8.9_linux_${GRPCURL_ARCH}.tar.gz"

# sqldef (psqldef)
ARG SQLDEF_VERSION="v1.0.1"
RUN PSQLDEF_ARCH="$(case "${TARGETARCH:-amd64}" in arm64) echo arm64;; *) echo amd64;; esac)" \
    && wget -q "https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/psqldef_linux_${PSQLDEF_ARCH}.tar.gz" \
    && tar -xzf "psqldef_linux_${PSQLDEF_ARCH}.tar.gz" -C /usr/local/bin \
    && rm "psqldef_linux_${PSQLDEF_ARCH}.tar.gz"

# mysqldef / mssqldef (必要な場合にコメントを外す)
# RUN curl -fsSOL "https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/mysqldef_linux_${PSQLDEF_ARCH}.tar.gz" \
#     && tar -xzf "mysqldef_linux_${PSQLDEF_ARCH}.tar.gz" -C /usr/local/bin \
#     && rm "mysqldef_linux_${PSQLDEF_ARCH}.tar.gz"
# RUN curl -fsSOL "https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/mssqldef_linux_${PSQLDEF_ARCH}.tar.gz" \
#     && tar -xzf "mssqldef_linux_${PSQLDEF_ARCH}.tar.gz" -C /usr/local/bin \
#     && rm "mssqldef_linux_${PSQLDEF_ARCH}.tar.gz"

# opencode web がブラウザ自動起動に使う xdg-open の no-op 版を用意する
# (ヘッドレス環境では xdg-open が無く、起動時にエラーになるため)
RUN printf '#!/bin/sh\nexit 0\n' > /usr/local/bin/xdg-open \
    && chmod +x /usr/local/bin/xdg-open

# code-server (ブラウザで動く VS Code) をインストールする
# (Tailscale 経由で他端末からソースコードを閲覧・編集するため)
RUN curl -fsSL https://code-server.dev/install.sh | sh

# Node / Java / Azure CLI / Playwright は devcontainer.json の features で導入