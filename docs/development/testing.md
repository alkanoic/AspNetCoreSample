# テスト

## テストフレームワーク

- **xunit v3**（`xunit.v3`, `OutputType=Exe`）
- **Testcontainers** - PostgreSQL / Keycloak の統合テスト
- **Playwright** - ブラウザ E2E テスト
- **Verify** - スナップショットテスト

## テストプロジェクト

| プロジェクト | 種類 | 説明 |
| ------------ | ---- | ---- |
| `tests/DbContainer.Test` | 統合テスト | PostgreSQL コンテナを使用した DB テスト |
| `tests/AspNetCoreSample.WebApi.Test` | 統合テスト | WebApi の全テスト（Testcontainers で PostgreSQL + Keycloak 起動） |
| `tests/AspNetCoreSample.Mvc.Test` | 統合テスト | Mvc 実行時検証（Playwright 必要） |
| `tests/AspNetCoreSample.Mvc.Container.Test` | コンテナテスト | Docker ビルド + Mvc テスト |
| `e2e/` | E2E テスト | Node 版 Playwright（Prisma, Allure） |

## テスト実行

### DbContainer.Test

```bash
dotnet test tests/DbContainer.Test
```

### WebApi.Test

```bash
dotnet test tests/AspNetCoreSample.WebApi.Test
```

Verify スナップショットテストを含みます。`.verified.txt` ファイルを変更した場合はテストを再実行して差分を確認してください。

### Mvc.Test

```bash
# Playwright のインストール（初回のみ）
bash tests/AspNetCoreSample.Mvc.Test/install-playwright.sh

dotnet test tests/AspNetCoreSample.Mvc.Test
```

### Mvc.Container.Test

```bash
# 証明書の作成
bash tests/AspNetCoreSample.Mvc.Container.Test/create_certificate.sh

# Playwright のインストール
bash tests/AspNetCoreSample.Mvc.Container.Test/install-playwright.sh

dotnet test tests/AspNetCoreSample.Mvc.Container.Test
```

### E2E テスト

```bash
cd e2e

# 依存関係のインストール
bash install-deps.sh

# テスト実行
bash execute-test.sh

# ヘッドレスモード
bash headed-test.sh

# UI モード
bash ui-test.sh

# スクリーンショット更新
bash update-screenshot.sh

# Allure レポート生成
bash allure-create.sh
```

## テストの書き方

### xunit v3

```csharp
public class MyTest
{
    [Fact]
    public async ValueTask TestSomething()
    {
        // テストコード
    }
}
```

- `ValueTask` を返す
- `global using Xunit` が有効

### Verify スナップショット

```csharp
[Fact]
public async ValueTask VerifyResponse()
{
    var result = await GetApiResponse();
    await Verify(result);
}
```

### Testcontainers

```csharp
var postgres = new PostgreSqlBuilder()
    .WithDatabase("testdb")
    .WithUsername("test")
    .WithPassword("test")
    .Build();
await postgres.StartAsync();
```
