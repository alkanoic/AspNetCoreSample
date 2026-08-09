# AspNetCoreSample

.NET / TypeScript / Java を混載した ASP.NET Core のサンドボックス・サンプルリポジトリです。技術検証のために多数の機能がデモ的に同居しています。

## 技術スタック

| カテゴリ | 技術 |
| -------- | ---- |
| バックエンド | .NET 10 (ASP.NET Core MVC / Web API / gRPC) |
| フロントエンド | Vue 3 (Nuxt 4), Lit, TypeScript, Vite, Bootstrap |
| データベース | PostgreSQL (EF Core / Npgsql) |
| キャッシュ | Redis |
| 認証 | Keycloak (OIDC / JWT) |
| オーケストレーション | .NET Aspire |
| テスト | xunit v3, Testcontainers, Playwright, Verify |
| CI/CD | GitHub Actions → Azure Web Apps / Static Web Apps |
| 監視 | OpenTelemetry, NLog |

## クイックスタート

```bash
# ツールの復元
dotnet tool restore

# ビルド
dotnet build AspNetCoreSample.sln

# フォーマットチェック
dotnet format

# MVC アプリ起動
dotnet run --project src/AspNetCoreSample.Mvc
# → https://localhost:7079

# Web API 起動
dotnet run --project src/AspNetCoreSample.WebApi
# → https://localhost:7036 (Swagger: /swagger)

# Aspire オーケストレータ起動
dotnet run --project src/AspNetCoreSample.AppHost
```

## ディレクトリ構成

| パス | 内容 |
| ---- | ---- |
| `src/AspNetCoreSample.Mvc/` | MVC+Razor、Vue/Lit/Htmx/SignalR/QrCode/Vite/Bootstrap/Blazor コンポーネント等のデモ実装 |
| `src/AspNetCoreSample.WebApi/` | REST API（NSwag / Keycloak 認証・管理 API・CORS） |
| `src/AspNetCoreSample.Grpc/` | gRPC サービス（Reflection 有効） |
| `src/AspNetCoreSample.ServiceDefaults/` | Aspire 共有設定（OpenTelemetry / リカバリ / Service Discovery） |
| `src/AspNetCoreSample.AppHost/` | Aspire オーケストレータ |
| `src/AspNetCoreSample.DataModel/` | EF Core モデル（PostgreSQL 向け `SampleContext`） |
| `src/AspNetCoreSample.Templates/` | ライブラリ保持（scaffold 用参照先） |
| `src/CodeGen/` | コード生成 CLI |
| `src/NuxtSample/` | Nuxt 4 フロントエンド（pnpm 使用） |
| `src/typescript/` | lit/vue-webcomponent/openapi-typescript/quicktype |
| `src/SpringBoot.Reports/` | Java（JasperReports） |
| `tests/` | xunit、Testcontainers、Playwright（C#） |
| `e2e/` | Node 版 Playwright E2E |
