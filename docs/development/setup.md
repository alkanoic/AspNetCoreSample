# ローカル開発環境

## 前提条件

- .NET SDK 10.0
- Node.js 22.x
- pnpm（`corepack enable pnpm`）
- Docker / Docker Compose
- Visual Studio Code（推奨、devcontainer 使用時）

## Dev Container

リポジトリには `.devcontainer/` 設定が含まれており、VS Code の Dev Container 機能で開発環境を自動構築できます。

Dev Container 起動時に以下が自動実行されます。

1. Docker Compose でミドルウェア起動（PostgreSQL、Redis、Keycloak、MySQL、MSSQL、nginx、pgadmin4）
2. .NET SDK, Node.js, pnpm のセットアップ
3. `dotnet tool restore`
4. Git hooks の設定（`git config core.hooksPath .githooks`）
5. Agent Skills の復元（`npx skills experimental_install`）

## ミドルウェア

Docker Compose で以下のサービスが起動します。

| サービス | ポート | 用途 |
| -------- | ------ | ---- |
| PostgreSQL | 5432 | メインデータベース |
| Redis | 6379 | セッションストア / キャッシュ |
| Keycloak | 8080 | 認証サーバー |
| MySQL | 3306 | 追加 DB（sqldef 検証用） |
| MSSQL | 1433 | 追加 DB（sqldef 検証用） |
| pgadmin4 | 8081 | PostgreSQL 管理 UI |
| nginx | 80 | リバースプロキシ |

## Keycloak 設定

- Realm: `Test`
- クライアント: `test-client`
- テストユーザー:
  - `test` / `test`（一般ユーザー）
  - `admin` / `passwd`（管理者）
- 設定ファイル: `keycloak/` ディレクトリ

### トークン取得（動作確認）

```bash
bash keycloak/token.sh
```

## 接続文字列

アプリケーションはコンテナ名でミドルウェアに接続します。

```json
{
  "ConnectionStrings": {
    "Default": "Server=postgresql;Port=5432;Database=sampledb;User Id=sa;Password=pass;",
    "Redis": "redis"
  }
}
```
