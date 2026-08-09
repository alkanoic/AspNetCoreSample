---
name: dotnet-build-test
description: Use when the user asks to build the .NET solution or run tests (dotnet build, dotnet test, dotnet format, xunit). Covers the repo's xunit v3 + Testcontainers + Verify snapshot testing conventions for tests/DbContainer.Test, AspNetCoreSample.WebApi.Test, AspNetCoreSample.Mvc.Test, AspNetCoreSample.Mvc.Container.Test. Trigger keywords: "ビルド", "テスト実行", "dotnet test", "dotnet build", "testを追加", "Verify", "Testcontainers", "xunit".
---

# .NET ビルド & テスト

AspNetCoreSample の .NET 10 ビルド・テスト手順。

## ビルド

```bash
dotnet tool restore                # まず必ず実行（ツール: dotnet-ef / dotnet-t4 / dotnet-format / kiota / CycloneDX）
dotnet build AspNetCoreSample.sln
dotnet format                    # editorconfig 準拠。差分確認のみなら `dotnet format --verify-no-changes`
```

- ソリューションに含まれるのは Web 系 + 生成系 + 一部テストのみ。以下は**ソリューション外**（個別ビルド）：
  `src/AspNetCoreSample.Mvc.Container.Test`（Docker イメージを使うテスト）、`src/CodeGen.Result.Kiota/`（kiota 生成物）、`src/localstack/`、`src/SpringBoot.Reports/`（Maven）、`e2e/`（npm）。
- `Directory.Build.props` で Roslynator + `AnalysisLevel=latest-Recommended` が有効。**コンパイル警告・アナライザー警告を残さない**こと（編集後必ず build を通す）。

## テストプロジェクト一覧

| プロジェクト | 実行 | 前提 |
| ------------ | ---- | ---- |
| `tests/DbContainer.Test` | `dotnet test tests/DbContainer.Test` | Docker（Postgres コンテナ） |
| `tests/AspNetCoreSample.WebApi.Test` | `dotnet test tests/AspNetCoreSample.WebApi.Test` | Docker（Testcontainers で **Postgres + Keycloak** 起動） |
| `tests/AspNetCoreSample.Mvc.Test` | `dotnet test tests/AspNetCoreSample.Mvc.Test` | Playwright（`pwsh bin/Release/net10.0/playwright.ps1 install --with-deps` 実行済み） |
| `tests/AspNetCoreSample.Mvc.Container.Test` | `dotnet test tests/AspNetCoreSample.Mvc.Container.Test` | Docker ビルド + 証明書（`bash create_certificate.sh`）+ Playwright |

CI 相当の全実行は `main.yml` の各テストジョブ（`dbcontainer_test` / `webapi_test` / `mvc_test` / `mvc_container_test`）を参照（`dotnet build -c Release -f net10.0` → `playwright install` → `dotnet test --no-build`）。

## xunit v3 の規約

- xunit.v3（`xunit.v3` パッケージ、`<OutputType>Exe</OutputType>`、`<Using Include="Xunit" />` で global using）。
- **非同期テストは `async ValueTask` を返す**（`async void` / `Task` にしない）。キャンセルトークンを適切に回す。
- フィクスチャパターン：`tests/AspNetCoreSample.WebApi.Test/WebApplicationFactoryFixture.cs`、`DbFixture.cs`、`KeycloakFixture.cs`（ソリューション配下の実例を参考に、`IClassFixture<T>` / `IAsyncLifetime` を使う）。
- `xunit.runner.json` が各テストプロジェクトにある（並列化等の設定確認）。

## Verify（差分スナップショット）

- `tests/AspNetCoreSample.WebApi.Test` のみ使用（`Verify.XunitV3`）。
- 検証済み出力は `SelectTest/snapshots/*.verified.txt`。**スナップショットを書き換えたら必ず当該テストを自己検証**すること。
- 差分更新は Verify 規約（`*.received.txt` を目視確認 → `verified` へ昇格、または Verify の再検証ツール）。

## Testcontainers

- `Testcontainers.PostgreSql` / `Testcontainers.Keycloak`。マイグレデータは `tests/testcontainer/migrate/`、realm は `tests/testcontainer/Test-realm.json` を `<None Include="../testcontainer/..." />` でリンクしてコピー。
- ポートは動的確保。ホスト名はコンテナ名ではなく localhost。docker エンジン必須。

## Playwright（C# 版）

- `Microsoft.Playwright` を使用したブラウザ検証。実行前に：
  ```bash
  dotnet build
  pwsh bin/Release/net10.0/playwright.ps1 install --with-deps
  ```
- `PlaywrightSettings.cs`（BaseUrl 等）を参照。

## 注意
- ログは NLog（`Web.AspnetCore`）。テストでもログが `logs/` に出力されることがあるので `.gitignore` 対象を把握。
- MySQL 系の検証は `tests/DbContainer.Test`（Postgres）のみ。MySQL 対応は `CodeGen.Result` で Pomelo を使用する別経路。
