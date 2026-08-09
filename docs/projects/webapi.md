# Web API

`AspNetCoreSample.WebApi` は RESTful API を提供する ASP.NET Core Web API アプリケーションです。

## 起動

```bash
dotnet run --project src/AspNetCoreSample.WebApi
# https://localhost:7036
# Swagger UI: https://localhost:7036/swagger
# ReDoc: https://localhost:7036/api-docs
```

## コントローラー一覧

| コントローラー | パス | 説明 |
| -------------- | ---- | ---- |
| `KeycloakController` | `/api/Keycloak` | Keycloak 管理 API（ユーザー CRUD、ロール、クライアント、パスワード管理） |
| `TokenController` | `/api/Token` | Keycloak トークン認証・リフレッシュ・失効 |
| `TokenTestController` | `/api/TokenTest` | トークンテストエンドポイント |
| `DbAccessController` | `/api/DbAccess` | EF Core CRUD（`Name` エンティティ） |
| `SimpleController` | `/api/Simple` | 簡易 I/O、多言語対応デモ |
| `ValidationController` | `/api/Validation` | FluentValidation デモ |
| `WeatherForecastController` | `/api/WeatherForecast` | 標準 Weather Forecast API |
| `QrCodeController` | `/api/QrCode` | SignalR QR コードリレー |

## 認証・認可

### JWT Bearer 認証

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = keycloakOptions.Authority;
    options.Audience = keycloakOptions.Audience;
    if (builder.Environment.IsDevelopment())
    {
        // ローカル（Keycloak が http://keycloak）でのみ HTTPS メタデータ検証をスキップする
        options.RequireHttpsMetadata = false;
    }
});
```

### 動的認可ポリシー

`CustomAuthorizationPolicyProvider` が DB の `Policy` / `RolePolicy` テーブルからポリシーを動的に解決します。

- `PolicyService` が定期的にポリシーキャッシュをリフレッシュ
- リフレッシュ間隔は `PolicyOptions:RefreshPolicyTimeSpan` で設定（デフォルト 1 分）

## CORS

以下のオリジンからのクロスオリジンリクエストを許可します。

- `http://localhost:5173`（Vite dev server）
- `https://localhost:7079`（MVC アプリ）
- `http://localhost:3000`（Nuxt dev server）
- `CorsOptions:MvcUrl`（設定値）

## 多言語対応

`Accept-Language` ヘッダー、クエリ文字列、Cookie でロケールを切り替えます。

- サポート言語: `ja`, `en`
- デフォルト: `ja`
- リソースファイル: `Resources/`

## OpenAPI

NSwag を使用して OpenAPI ドキュメントを生成します。

- Swagger UI: `/swagger`
- ReDoc: `/api-docs`
- JWT 認証スキームを含むセキュリティ定義
- `Accept-Language` ヘッダーパラメータのカスタムプロセッサ

## SignalR Hub

`QrCodeHub` (`/qrcodeHub`) が QR コードデータのリアルタイムリレーを担当します。

## 依存サービス

| サービス | 接続先 |
| -------- | ------ |
| PostgreSQL | `Server=postgresql` |
| Keycloak | `http://keycloak:8080` |
