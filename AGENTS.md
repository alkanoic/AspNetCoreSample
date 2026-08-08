# AGENTS.md - AspNetCoreSample

.NET / TypeScript / Java を混載した ASP.NET Core のサンドボックス・サンプルリポジトリ。
技術検証のために多数の機能が「デモ的に」同居している。**既存パターンに合わせて追加・修正するのを基本**とし、既存のデモを壊さないこと。

## 概要

- .NET 9（net9.0）ベース。`Directory.Build.props` で Roslynator + latest-Recommended アナライザー有効。
- 本番アプリは **Mvc**（画面）と **WebApi**（REST）の 2 本立て。Azure Web Apps にデプロイ。
- Aspire（AppHost / ServiceDefaults）でローカルオーケストレーション可。
- devcontainer 内で MySQL / PostgreSQL / MSSQL / Redis / Keycloak / nginx / pgadmin4 を docker compose 起動して開発する。

## ディレクトリ構成

| パス | 内容 |
| ---- | ---- |
| `src/AspNetCoreSample.Mvc/` | MVC+Razor、Vue/Lit/Htmx/SignalR/QrCode/Vite/Bootstrap/Blazor コンポーネント等のデモ実装 |
| `src/AspNetCoreSample.WebApi/` | REST API（NSwag / Keycloak 認証・管理 API・CORS / gRPC は別） |
| `src/AspNetCoreSample.Grpc/` | gRPC サービス（Reflection 有効、`Protos/greet.proto`） |
| `src/AspNetCoreSample.ServiceDefaults/` | Aspire 共有設定（OpenTelemetry / リカバリ / Service Discovery） |
| `src/AspNetCoreSample.AppHost/` | Aspire オーケストレータ |
| `src/AspNetCoreSample.DataModel/` | EF Core モデル（PostgreSQL 向け `SampleContext`） |
| `src/AspNetCoreSample.Templates/` | ライブラリ保持（scaffold 用参照先） |
| `src/CodeGen/`, `CodeGen.Result/`, `CodeGen.Result.Kiota/`, `T4Execute/`, `T4Design/` | コード生成（CLI / 生成物の配置検証 / T4） |
| `src/NuxtSample/`, `src/typescript/` | Nuxt 3、lit/vue-webcomponent/openapi-typescript/quicktype |
| `src/SpringBoot.Reports/` | Java（JasperReports） |
| `tests/` | xunit、Testcontainers、Playwright（C#） |
| `e2e/` | Node 版 Playwright E2E（develop/production）、Prisma、Allure |
| `.github/workflows/` | Azure デプロイ（main.yml）とテスト（test.yml） |
| `.devcontainer/`, `keycloak/`, `localstack/`, `sbom/` | 開発環境・Keycloak 設定・Lambda 局所テスト・CycloneDX SBOM |
| `.agents/skills/`, `skills-lock.json` | Agent Skills（自製 + 公式を混在）。`.agents/skills/` は Claude/Cursor/Codex/opencode 共通の汎用フォルダ、`skills-lock.json` で公式スキルのバージョン管理 |

## ビルド

```bash
dotnet tool restore          # まず必ず（dotnet-ef / dotnet-t4 / dotnet-format / kiota / CycloneDX）
dotnet build AspNetCoreSample.sln
dotnet format                # フォーマットチェック・適用（editorconfig 準拠）
```

- ソリューションに含まれるのは Web 系 + 生成系 + 一部テストのみ。`src/AspNetCoreSample.Mvc.Container.Test`、`src/CodeGen.Result.Kiota/`、`src/localstack/`、`src/SpringBoot.Reports/`、`e2e/` はソリューション外。
- Git フック等により push/commit でフルテストが走る（CI 参照）。

## テスト

```bash
dotnet test tests/DbContainer.Test                      # Postgres コンテナのみ
dotnet test tests/AspNetCoreSample.WebApi.Test            # WebApi 全テスト（Testcontainers で Postgres + Keycloak 起動）
dotnet test tests/AspNetCoreSample.Mvc.Test               # Mvc 実行時検証（Playwright 必要）
dotnet test tests/AspNetCoreSample.Mvc.Container.Test     # Docker ビルド + Mvc テスト
```

- xunit v3（`xunit.v3`、`OutputType=Exe`、global `Using Xunit`）。`ValueTask` 使用。
- `Microsoft.AspNetCore.Mvc.Testing`（`WebApplicationFactory`）+ `Testcontainers.PostgreSql` / `Testcontainers.Keycloak` での統合テスト。
- WebApi テストでは **Verify（差分スナップショット）**使用。`.verified.txt` を変えたら `dotnet test`（自己検証すること）。差分更新は Verify の規約に従う。
- ブラウザ系は `install-playwright.sh` で `dotnet build ... && playwright.ps1 install --with-deps` から実行。

## ローカル起動

```bash
# ミドル（DB/Redis/Keycloak）は devcontainer の docker compose で起動済み想定
dotnet run --project src/AspNetCoreSample.Mvc      # https://localhost:7079
dotnet run --project src/AspNetCoreSample.WebApi    # https://localhost:7036 (swagger /swagger)
dotnet run --project src/AspNetCoreSample.Grpc      # gRPC (ポートは launchSettings 参照)
dotnet run --project src/AspNetCoreSample.AppHost   # Aspire オーケストレータ（Mvc+WebApi+Postgres を一括起動）
```

- 設定値は appsettings*.json（Keycloak / CORS / Policy）参照。接続文字列は `Server=postgresql`, `Redis=redis` 等コンテナ名指定。
- Keycloak realm `Test`（test/test / admin/passwd）、初期ログ存在は `README.md` 参照。

## 主要スクリプト（リポジトリルート）

| スクリプト | 概要 |
| ---------- | ---- |
| `deploy.sh` | Azure へ `az webapp up` で MVC / WebAPI デプロイ |
| `export-cyclonedx.sh` | `dotnet CycloneDX` + trivy で SBOM 出力 |
| `sqldef.sh` | mysqldef / psqldef / mssqldef による now schema 差分適用 |
| `keycloak/*.sh`, `src/AspNetCoreSample.WebApi/keycloak.sh` | Keycloak token 取得などの動作確認 |
| `localstack/command.sh` | LocalStack の S3/CF 動作確認 |
| `src/CodeGen/*.sh`, `e2e/*.sh` | 各生成・E2E ワークフロー（各 SKILL.md 参照） |

## 監理・フォーマット・規約

- **Analizers strict**：`Directory.Build.props` で `EnableNETAnalyzers` + `AnalysisLevel=latest-Recommended`、Roslynator。編集後 `dotnet build` で警告を潰すこと。
- **`.editorconfig`**：コードスタイル定義。コメントは日本語前提（既存の日本語コメントを維持）。
- **ログ**：NLog（`Web.AspnetCore` / `Host` で `UseNLog`、Fody の `LoggingAttribute` でメソッドログ）。
- **検証**：`FluentValidation` で `AddValidatorsFromAssemblyContaining` + クライアント側アダプタ。
- **DI への static アクセス**：`Logging/ServiceProviderAccessor.cs`（`Initialize(app.Services)`）とする。

## コミット規約（Conventional Commits / 日本語）

- `git commit` は **Conventional Commits** 形式で行う。コミットメッセージは**本文を日本語**で書く。
- 形式: `<type>(<scope>): <日本語の要約>`
- type は以下を使う（feat / fix 以外は将来のメジャーリリースでまとめて整理すること）。
  - `feat:` 新機能
  - `fix:` 不具合修正
  - `refactor:` 仕様を変えないコード整理
  - `docs:` ドキュメント（README / コメント / 設定説明）
  - `chore:` ビルド・ツール・依存の更新など
  - `test:` テストの追加・修正（コードの振る舞いは不変）
- scope は対象ディレクトリ・プロジェクト（例: `mvc` / `webapi` / `worker` / `e2e` / `pipeline` / `i18n`）。不必要な場合は省略可。
- 要約は簡潔に。必要なら本文（`-` 箇条書き）で詳細を追記。「〜の」で終わる名詞的な要約より、動詞で「〜する」/「〜できる」まで書く。
- 例: `feat(webapi): Keycloak のトークン更新 API を追加する`、`fix(mvc): QrCode 通知の再送処理を修正する`、`chore: .NET 9 へ依存を更新する`
- コミット時は `git diff` / `git status` で対象を確認し、意図しないファイルを含めないこと。生成物（SBOM・playwright-report・CodeGen 生成物）等は `--no-verify` でない限りフックに注意する。

## 生成物の扱い
- `src/CodeGen/Outputs/**`, `src/CodeGen.Result/**`, `src/CodeGen.Result.Kiota/**` はテスト生成です。テンプレート変更時は再生成して `Outputs` をビルド可能に保つ。
- 自動生成・生成テンプレート・SBOM・playwright-report は手で編集しない。

## エージェントスキル（opencode 用）

- **自製スキル**（`.agents/skills/`）: このリポジトリ固有のワークフロー。`dotnet-build-test` / `aspnet-run` / `playwright-e2e` / `frontend-ts`。opencode は `.agents/skills` を汎用フォルダとして検知する。
- **公式スキル**（`.agents/skills/`）: `npx skills add`（skills.sh）で導入。
  | スキル | 提供元 | 用途 |
  | ------ | ------ | ---- |
  | dotnet-webapi / configuring-opentelemetry-dotnet / convert-blazor-server-to-webapp / minimal-api-file-upload | dotnet/skills（.NET チーム公式）の **dotnet-aspnetcore プラグイン** | ASP.NET Core（Web API・OTel・Blazor・Minimal API） |
  | optimizing-ef-core-queries / create-datadriven-aspnetcore | dotnet/skills（.NET チーム公式）の dotnet-data プラグイン | EF Core・CRUD 生成 |
  | authoring-github-workflows | dotnet/skills（.NET チーム公式） | workflow YAML 検証 |
  | playwright-cli | microsoft/playwright-cli（Playwright 公式） | Playwright CLI によるブラウザ操作・テスト |
- **管理コマンド**（npm ライク）: 追加 `npx skills add <repo> --skill <name> -a opencode`、一覧 `npx skills list`、復元 `npx skills experimental_install`（`skills-lock.json` から）。ここで導入した公式セットは `skills-lock.json` に記録され、**devcontainer の `postCreateCommand.sh` で自動復元される**。ロックファイルは手動編集しない。
- **汎用フォルダ**: `.agents/skills/` は Claude Code / Cursor / Codex / opencode など複数エージェントが読み込む共通フォルダ。リポジトリ固有スキルもここに置くことで全エージェントから利用できる。
- **設定ファイルベースの代替**: 外部スキルは `opencode.json` の `skills.urls` / `skills.paths` でも表記できる（URL/パスで解決）。本リポジトリはファイルコピー方式（`.agents/skills/`）を採用。

## CI / Azure

- `main.yml`：push で Mvc（npm ci + vite publish）+ WebApi を `Release/net9.0` で publish → Azure Web Apps へデプロイ。Nuxt は SWA、Spring (Java) もデプロイ。
- `test.yml`：全てのテストプロジェクトを CI で実行（コンテナ系は Testcontainers、Docker が必要）。
- アプリ設定の前提：`WEBSITE_RUN_FROM_PACKAGE=1`、`DOTNET_VERSION=9.0`。