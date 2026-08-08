---
name: aspnet-run
description: Use when the user wants to start/stop the web apps or infrastructure locally (dotnet run, docker compose, Aspire AppHost, Keycloak, PostgreSQL, Redis, connection strings, ports). Trigger keywords: "起動", "ローカル実行", "dotnet run", "AppHost", "docker compose up", "Keycloak", "接続文字列", "ポート", "swagger".
---

# ローカル起動・環境

AspNetCoreSample のローカル実行手順。ミドルは devcontainer の docker compose で起動済みを前提とする。

## インフラ（devcontainer）

- `.devcontainer/compose.yaml` で MySQL / PostgreSQL / MSSQL / Redis / Keycloak / nginx / pgadmin4 / localstack を起動。
- ホスト名はコンテナ名（例: `postgresql`, `mysql`, `mssql`, `redis`, `keycloak`）で解決。接続文字列は appsettings*.json 参照。
- 初期データ：`.devcontainer/docker/volumes/*/initdb.d/`（001_create_tables.sql / 101_insert_rows.sql）。
- コンテナを明示的に立ち上げる場合：
  ```bash
  docker compose -f .devcontainer/compose.yaml up -d
  ```

## アプリ起動

```bash
dotnet run --project src/AspNetCoreSample.Mvc      # https://localhost:7079
dotnet run --project src/AspNetCoreSample.WebApi    # https://localhost:7036 (Swagger /swagger)
dotnet run --project src/AspNetCoreSample.Grpc      # gRPC（launchSettings 参照）
dotnet run --project src/AspNetCoreSample.AppHost   # Aspire オーケストレータ（Mvc+WebApi+Postgres 一括起動）
```

- **Aspire を使う場合**: `dotnet workload` に Aspire が必要（`Aspire.md` 参照）。`AspNetCoreSample.AppHost` 実行で Mvc / WebApi / Postgres が一括起動される。
- Mvc は Vite フロントをビルド済みにしておくこと（`src/AspNetCoreSample.Mvc` で `npm run build` か `npm run dev`）。

## 設定値

- 接続文字列: `"Default": "Server=postgresql;Username=root;Password=postgres;Database=sample"`, `"Redis": "redis"`（`src/AspNetCoreSample.Mvc/appsettings.json` 他）。
- Keycloak（Mvc/WebApi の appsettings）:
  - realm: `Test`
  - 認証: Mvc は OpenID Connect コードフロー（`http://keycloak:8080/realms/Test`）
  - WebApi は JWT Bearer + Keycloak Admin API 使用（AdminUserName `admin` / AdminPassword `passwd`）
- CORS: WebApi の `CorsOptions.MvcUrl` を `https://localhost:7079` に設定。

## Keycloak 動作確認

```bash
# token 取得（ユーザー: test / パスワード: test）
curl -X POST http://keycloak:8080/realms/Test/protocol/openid-connect/token \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=password&client_id=test-client&client_secret=mA1VxFslWGukos6JquOZcoU7qVUElsmv&username=test&password=test'
```

- その他 `keycloak/curl.sh`, `keycloak/export.sh`, `src/AspNetCoreSample.WebApi/keycloak.sh`。
- realm の初期データは `.devcontainer/docker/volumes/keycloak/data/import/Test-realm.json`。変更時は `keycloak/export.sh` で書き出し。

## 注意

- HTTPS 開発証明書は postCreateCommand で生成/インポートされる。ポート 7079 / 7036 は自己署名のため `ignoreHTTPSErrors` や `--allow-insecure` 相当の対応が必要な場合あり。
- appsettings の接続先は「コンテナ名」なので、ホストマシン（Windows 等）から直接繋ぐ場合は localhost + 公開ポートに置き換えること。