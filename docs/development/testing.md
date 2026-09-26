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
| `tests/AspNetCoreSample.Spa.Test` | 統合テスト | SPA と WebApi の連携検証（Playwright + Nuxt dev サーバー必要） |
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

### Spa.Test

```bash
# Playwright のインストール（初回のみ）
bash tests/AspNetCoreSample.Spa.Test/install-playwright.sh

dotnet test tests/AspNetCoreSample.Spa.Test
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

## Testcontainers を使ったテストの作成手順

まず WebApi で「依存コンテナの起動 + アプリの起動 + HTTP での検証」を作り、発展形として同じ構成を MVC に適用し、Playwright によるブラウザ検証を組み合わせます。応用形として SPA と WebApi の連携検証もあります。いずれも Docker が必要です。

| 段階 | 実装例 | 検証方法 |
| ---- | ------ | -------- |
| 基本 | `tests/AspNetCoreSample.WebApi.Test` | `HttpClient` で API を呼ぶ |
| 発展 | `tests/AspNetCoreSample.Mvc.Test` | Playwright（C#）で画面を操作する |
| 応用 | `tests/AspNetCoreSample.Spa.Test` | Playwright（C#）で SPA を操作する |

### 1. WebApi: Testcontainers とアプリを起動する

#### 1-1. テストプロジェクトを準備する

`tests/AspNetCoreSample.WebApi.Test/AspNetCoreSample.WebApi.Test.csproj` と同様に、次のパッケージとプロジェクト参照を追加します。

- `xunit.v3` / `xunit.runner.visualstudio` / `Microsoft.NET.Test.Sdk`（`OutputType=Exe`、`<Using Include="Xunit" />`）
- `Microsoft.AspNetCore.Mvc.Testing`（`WebApplicationFactory`）
- `Testcontainers.PostgreSql` / `Testcontainers.Keycloak`
- テスト対象の `src/AspNetCoreSample.WebApi` へのプロジェクト参照

コンテナに渡す初期化 SQL と Keycloak の realm 定義は `tests/testcontainer/` で共有し、テスト出力へコピーします。

```xml
<ItemGroup>
  <None Include="../testcontainer/migrate/*" LinkBase="migrate"
    CopyToOutputDirectory="PreserveNewest" />
  <None Include="../testcontainer/Test-realm.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

`WebApplicationFactory<Program>` からエントリポイントを参照できるよう、アプリ側の `Program.cs` 末尾に `public partial class Program { }` を定義しています。

#### 1-2. フィクスチャでコンテナを定義する

`WebApplicationFactory<TEntryPoint>` を継承し、`IAsyncLifetime` を実装したフィクスチャを作ります（実装: `tests/AspNetCoreSample.WebApi.Test/WebApplicationFactoryFixture.cs`）。コンストラクターではコンテナを組み立てるだけで、起動はしません。

```csharp
public sealed class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    private readonly PostgreSqlContainer _postgresqlContainer;
    private readonly IContainer _keycloakContainer;

    public WebApplicationFactoryFixture()
    {
        _postgresqlContainer = new PostgreSqlBuilder("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .Build();

        _keycloakContainer = new ContainerBuilder("quay.io/keycloak/keycloak:latest")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithEnvironment("KEYCLOAK_ADMIN", "admin")
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "passwd")
            .WithPortBinding(KeycloakBuilder.KeycloakPort, true)
            .WithPortBinding(KeycloakBuilder.KeycloakHealthPort, true)
            .WithCommand("start-dev")
            .WithCommand("--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health/ready").ForPort(KeycloakBuilder.KeycloakHealthPort)))
            .Build();
    }

    public string DbConnectionString => _postgresqlContainer.GetConnectionString();

    public string KeycloakBaseAddress => new UriBuilder(Uri.UriSchemeHttp, _keycloakContainer.Hostname,
        _keycloakContainer.GetMappedPublicPort(KeycloakBuilder.KeycloakPort)).ToString();
}
```

- ホスト側のポートは `WithPortBinding(port, true)` で動的に割り当てる（固定ポートは並列実行や開発用 docker compose と衝突する）
- 接続先はコンテナ名ではなく、`Hostname` と `GetMappedPublicPort()`（PostgreSQL は `GetConnectionString()`）から取得する

#### 1-3. コンテナを起動・破棄する

xunit v3 は `IAsyncLifetime.InitializeAsync()` をテストより先に呼びます。独立したコンテナは並行起動して待ち時間を短縮します。

```csharp
async ValueTask IAsyncLifetime.InitializeAsync()
{
    await Task.WhenAll(_keycloakContainer.StartAsync(), _postgresqlContainer.StartAsync());
}

public override async ValueTask DisposeAsync()
{
    await _keycloakContainer.DisposeAsync();
    await _postgresqlContainer.DisposeAsync();
    await base.DisposeAsync();
}
```

#### 1-4. アプリの設定をコンテナへ向ける

`ConfigureWebHost` をオーバーライドし、アプリの接続文字列や Keycloak の URL を起動済みのコンテナへ差し替えます。環境変数による上書きはほかのテストに影響するため、`UseSetting`（メモリー上の設定）を使います。

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.UseUrls("https://127.0.0.1:0");

    builder.UseSetting("ConnectionStrings:Default", DbConnectionString);
    builder.UseSetting("KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString());
    builder.UseSetting("KeycloakOptions:TokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/protocol/openid-connect/token").ToString());
}
```

`RevokeTokenEndpoint` / `AdminTokenEndpoint` / `AdminBaseAddress` も同様に設定します。`ConfigureWebHost` はアプリを初めて起動するとき（`CreateClient()` の初回呼び出し時）に実行されます。この時点ではコンテナが起動済みなので、マッピング後のポートを参照できます。

#### 1-5. Kestrel で実ポートにも公開する（任意）

`WebApplicationFactory` の既定はメモリー上の `TestServer` で、実ポートは開きません。SignalR の実接続や、後述の Playwright のように外部プロセスからアクセスする場合は、`CreateHost` をオーバーライドして Kestrel ホストも起動し、実際の URL を `HostUrl` として公開します。

```csharp
public string HostUrl { get; private set; } = "";

protected override IHost CreateHost(IHostBuilder builder)
{
    var dummyHost = builder.Build();
    dummyHost.StartAsync().GetAwaiter().GetResult();

    builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
    _kestrelHost = builder.Build();
    _kestrelHost.Start();

    HostUrl = ResolveHostUrl(_kestrelHost);

    return dummyHost;
}
```

`ResolveHostUrl` は `IServer` の `IServerAddressesFeature` から `https://` のアドレスを取得します。バインドの確定を待つリトライ処理を含めた実装は、既存のフィクスチャを参照してください。`DisposeAsync()` では、コンテナより先に `_kestrelHost` を停止します。

#### 1-6. テストを書く

テストクラスは `IClassFixture<WebApplicationFactoryFixture<Program>>` でフィクスチャを受け取ります。コンテナとアプリは、テストクラス内の全テストで共有されます（実装例: `WeatherForecastTest/WeatherForecastControllerTest.cs`、`SelectTest/DbAccessWebApiSelectTest.cs`）。

```csharp
public sealed class DbAccessWebApiSelectTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;
    private readonly HttpClient _httpClient;

    public DbAccessWebApiSelectTest(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    public void Dispose() => _httpClient.Dispose();

    [Fact]
    public async Task GetDbAccessReturnsOk()
    {
        var response = await _httpClient.GetAsync(
            new Uri(new Uri(_factory.HostUrl), "api/dbaccess"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- `TestContext.Current.CancellationToken` を渡し、タイムアウトや中断時に処理を止める
- レスポンスの内容全体は、`Verify(result, settings)` のスナップショットで確認できる（`VerifySettingsFixture` を `[Collection]` で共有）

### 2. 発展: MVC で Playwright と組み合わせる

MVC では、WebApi と同じフィクスチャ構成でアプリを Kestrel 上に起動し、Playwright で操作するブラウザから `HostUrl` にアクセスします。ブラウザは別プロセスで動くため、1-5 の Kestrel 起動が必須です。

#### 2-1. テストプロジェクトを準備する

WebApi のテストプロジェクト構成に `Microsoft.Playwright` を追加し、`src/AspNetCoreSample.Mvc` を参照します（`tests/AspNetCoreSample.Mvc.Test/AspNetCoreSample.Mvc.Test.csproj`）。初回はビルド後にブラウザをインストールします。

```bash
cd tests/AspNetCoreSample.Mvc.Test
dotnet build
bash install-playwright.sh   # pwsh bin/Debug/net10.0/playwright.ps1 install --with-deps
```

#### 2-2. MVC 用に設定とサービスを差し替える

`tests/AspNetCoreSample.Mvc.Test/WebApplicationFactoryFixture.cs` のコンテナ定義、起動処理、`CreateHost` は WebApi 版とほぼ同じです。`ConfigureWebHost` では、MVC の OIDC ログインに必要な設定を上書きし、テストで用意しない外部依存をサービスごと差し替えます。

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.UseUrls("https://127.0.0.1:0");

    builder.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"ConnectionStrings:Default", DbConnectionString},
            {"KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString()},
            {"KeycloakOptions:MetadataAddress", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/.well-known/openid-configuration").ToString()},
        }).Build());

    builder.ConfigureServices(services =>
    {
        services.RemoveAll<IDistributedCache>();
        services.AddDistributedMemoryCache();
        services.AddRazorComponents().AddInteractiveServerComponents();
    });
}
```

Redis のコンテナは起動しないため、`IDistributedCache` をメモリー実装へ差し替えています。このように、検証対象外の依存はすべてをコンテナで再現せず、DI でメモリー実装に差し替えます。

#### 2-3. コンテナを全テストクラスで共有する

ブラウザテストはクラスが増えやすく、`IClassFixture` のままではクラスごとに PostgreSQL と Keycloak が起動します。`ICollectionFixture` でフィクスチャを 1 つにまとめ、起動を 1 回に抑えます（`MvcTestFixtures.cs`）。

```csharp
[CollectionDefinition(nameof(MvcTestFixtures))]
public sealed class MvcTestFixtures : ICollectionFixture<WebApplicationFactoryFixture<Program>>
{
}
```

各テストクラスには `[Collection(nameof(MvcTestFixtures))]` を付けます。同じコレクションのテストは直列に実行されます。

#### 2-4. Playwright でテストを書く

コンストラクターで `CreateDefaultClient()` を呼んでアプリ（Kestrel ホスト）を起動し、`HostUrl` をブラウザで開きます（実装例: `MvcInProcessTest.cs`）。

```csharp
[Collection(nameof(MvcTestFixtures))]
public sealed class MvcInProcessTest
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Fact]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        await using var context = await browser.NewContextAsync(PlaywrightSettings.DefaultBrowserNewContextOptions());
        PlaywrightSettings.SetDefaultBrowserContext(context);

        await PlaywrightRetry.RunAsync(async () =>
        {
            var page = await context.NewPageAsync();
            try
            {
                await page.GotoAsync(_factory.HostUrl);
                await page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
                await page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
                await page.GetByRole(AriaRole.Textbox, new() { Name = "パスワード" }).FillAsync("admin");
                await Task.WhenAll(
                    page.GetByRole(AriaRole.Button, new() { Name = "サインイン" }).ClickAsync(),
                    page.WaitForURLAsync($"{_factory.HostUrl}/Auth"));

                Assert.Contains("Auth Page", await page.TitleAsync());
            }
            finally
            {
                await page.CloseAsync();
            }
        });
    }
}
```

補助クラスの役割は次のとおりです。

| クラス | 役割 |
| ------ | ---- |
| `PlaywrightSettings` | 証明書エラーを無視する起動オプション（`--ignore-certificate-errors`）、`ja-JP` ロケール、既定タイムアウト 60 秒を設定する |
| `PlaywrightRetry` | CI でコンテナのネットワーク構築と並行し、Chromium が一時的に失敗する場合に再試行する |

ブラウザを使わずに HTTP ステータスだけを確認するテスト（`MvcApiTest.cs`）も、同じコレクションフィクスチャを共有できます。

### 3. SPA と WebApi を組み合わせる

Nuxt 4（`src/NuxtSample`）と WebApi を C# の Playwright で検証する構成です。実装は `tests/AspNetCoreSample.Spa.Test` にあり、WebApi.Test や Mvc.Test と同じフィクスチャの仕組みを使います。C# のフィクスチャは別プロセスから共有できないため、SPA（Nuxt dev サーバー）の寿命も同じフィクスチャで管理します。

#### 3-1. テストプロジェクトを準備する

`tests/AspNetCoreSample.Mvc.Test` と同じパッケージ（`xunit.v3` / `Microsoft.AspNetCore.Mvc.Testing` / `Microsoft.Playwright` / `Testcontainers.PostgreSql` / `Testcontainers.Keycloak`）を使います。テスト対象の `src/AspNetCoreSample.WebApi` を参照し、コンテナに渡す初期データ（`tests/testcontainer/migrate/`、`tests/testcontainer/Test-realm.json`）のコピー設定も同様にします。初回はビルド後にブラウザをインストールします。

```bash
dotnet build tests/AspNetCoreSample.Spa.Test
bash tests/AspNetCoreSample.Spa.Test/install-playwright.sh
```

#### 3-2. WebAPI と SPA をまとめて起動する

`SpaTestFixture`（`IAsyncLifetime`）が、WebApi.Test と同じ `WebApplicationFactoryFixture<Program>` で PostgreSQL・Keycloak・Kestrel を起動します。`CreateDefaultClient()` で `HostUrl` を確定させた後、子プロセスで `pnpm dev --port 3000 --host 127.0.0.1` を起動します。

```csharp
startInfo.Environment["API_BASE_URL"] = WebApiUrl;
startInfo.Environment["NUXT_PUBLIC_KEYCLOAK_URL"] = _webFactory.KeycloakBaseAddress;
```

`nuxt.config.ts` の `runtimeConfig.public.apiBaseUrl` は `API_BASE_URL`、Keycloak の URL は `NUXT_PUBLIC_KEYCLOAK_URL` を参照するため、テスト用の公開 URL に合わせてから SPA を起動します。`/login` が応答するまで待ってからテストを始めます。`DisposeAsync()` では SPA のプロセスツリーを停止してからコンテナを破棄します。子プロセスの標準出力は `bin/*/spa-logs/nuxt.log` に保存します。

#### 3-3. SPA を操作して検証する

`SpaWebApiTest.cs` は、トークン発行と認可付き API の直接呼び出し、ログイン画面からの認証と認可付き API の画面操作を検証します。ブラウザ操作は Mvc.Test と同じ `PlaywrightSettings` / `PlaywrightRetry` を使います。ただし `Channel = "chromium"` でフル版の Chromium を起動します。headless shell は環境によって `NewPageAsync` 時にクラッシュすることがあります。

```bash
dotnet test tests/AspNetCoreSample.Spa.Test
```

#### 3-4. 接続先の注意点

別オリジンで使う場合は、WebApi 側の CORS で SPA の実際のオリジンを許可してください（現在の `Program.cs` は `http://localhost:3000` などの固定オリジンのみ許可しています）。Keycloak 側の `spa-client` にも SPA のリダイレクト URI と Web Origin を登録します。現在の `tests/testcontainer/Test-realm.json` は `http://localhost:3000` 固定です。認証のリダイレクト先には、ブラウザから到達できる Keycloak の URL を使います。コンテナ内のホスト名（例: `keycloak`）は、ブラウザの実行環境から解決できるとは限りません。

テスト用 URL を決める際は、**ブラウザ・WebApi・Keycloak のそれぞれからどこに接続するか**を分けて確認してください。特に WebApi から Keycloak のメタデータを取得する URL と、SPA から認証画面へ遷移する URL が異なる環境では、OIDC の issuer との整合性も検証する必要があります。まず WebApi 単体テストで DB / 認証の動作を確認し、その後に SPA からの通信とログインを追加すると切り分けやすくなります。

### 注意点

- テストが異常終了して残ったコンテナは、Testcontainers の Resource Reaper（Ryuk）が削除する
- `migrate/` の SQL はコンテナの初回起動時だけ実行される（データを変更するテストは実行順に依存させない）
- イメージは `latest` タグのため、初回や更新時は pull に時間がかかる
- CI（`main.yml` の `webapi_test` / `mvc_test`）も同じ方法で実行する
