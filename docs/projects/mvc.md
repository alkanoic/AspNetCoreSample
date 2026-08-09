# MVC アプリケーション

`AspNetCoreSample.Mvc` は ASP.NET Core MVC + Razor ビューをベースに、多様なフロントエンド技術のデモを実装したアプリケーションです。

## 起動

```bash
dotnet run --project src/AspNetCoreSample.Mvc
# https://localhost:7079
```

## コントローラー一覧

| コントローラー | パス | 説明 |
| -------------- | ---- | ---- |
| `HomeController` | `/Home` | トップページ、エラー表示 |
| `AuthController` | `/Auth` | Keycloak 認証情報表示（`[Authorize]`） |
| `VueController` | `/Vue` | Vue.js デモ |
| `HtmxController` | `/Htmx` | HTMX デモ |
| `HtmxApiController` | `/HtmxApi` | HTMX API エンドポイント |
| `LitController` | `/Lit` | Lit Web Components デモ |
| `ChatController` | `/Chat` | SignalR チャットデモ |
| `PushController` | `/Push` | Web Push API（VAPID） |
| `QrCodeController` | `/QrCode` | QR コードリーダーデモ |
| `QrCodeNotificationController` | `/QrCodeNotification` | QR コード通知 |
| `BootstrapController` | `/Bootstrap` | Bootstrap UI デモ |
| `ComponentController` | `/Component` | View Components デモ |
| `FluentController` | `/Fluent` | FluentValidation デモ |
| `JQueryController` | `/JQuery` | jQuery デモ |
| `MapController` | `/Map` | 地図デモ |
| `NameController` | `/Name` | Name CRUD デモ |
| `SessionController` | `/Session` | セッション状態デモ |
| `ViteController` | `/Vite` | Vite 統合デモ |
| `VueComponentController` | `/VueComponent` | Vue Web Component デモ |

## 認証

Keycloak OIDC 認証を使用します。

- スキーム: Cookie + OpenID Connect
- 認可コードフロー
- スコープ: `openid`, `profile`

### 設定 (`appsettings.json`)

```json
{
  "KeycloakOptions": {
    "Authority": "http://keycloak:8080/realms/Test",
    "MetadataAddress": "http://keycloak:8080/realms/Test/.well-known/openid-configuration",
    "ClientId": "test-client",
    "ClientSecret": "********"
  }
}
```

## セッション管理

Redis をバッキングストアとする分散セッションを使用します。

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SampleInstance";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(20);
});
```

## Web Push

VAPID プロトコルを使用した Web Push 通知をサポートします。

- `PushController` がサブスクリプション管理を担当
- 起動時に VAPID キーペアを自動生成

## フロントエンド資産

- **Vite**: `npm run publish` でビルド、`wwwroot/` に出力
- **WebOptimizer**: JS/CSS のミニファイ
- **Bootstrap**: UI フレームワーク
- **Vue.js / Lit / HTMX**: SPA ライクなインタラクション

## 依存サービス

| サービス | 接続先 |
| -------- | ------ |
| PostgreSQL | `Server=postgresql` |
| Redis | `redis` |
| Keycloak | `http://keycloak:8080` |
| Web API | `https://localhost:7035` |
