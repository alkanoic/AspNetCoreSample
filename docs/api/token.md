# トークン API

`TokenController` が提供する Keycloak トークン操作 API です。

## ベースパス

```
/api/Token
```

## エンドポイント

### トークン認証

```
POST /api/Token/auth
```

**リクエスト**:

```json
{
  "userName": "test",
  "password": "test"
}
```

**レスポンス**:

```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "eyJhbGciOi...",
  "expiresIn": 300,
  "refreshExpiresIn": 1800,
  "tokenType": "Bearer"
}
```

### トークンリフレッシュ

```
POST /api/Token/refresh
```

**リクエスト**:

```json
{
  "refreshToken": "eyJhbGciOi..."
}
```

### トークン失効

```
POST /api/Token/revoke
```

**リクエスト**:

```json
{
  "refreshToken": "eyJhbGciOi..."
}
```

## サービス実装

`ITokenService` / `TokenService` (`src/AspNetCoreSample.WebApi/Services/Keycloak/Token/TokenService.cs`) が Keycloak のトークンエンドポイントとの通信を担当します。

### 設定

```json
{
  "KeycloakOptions": {
    "TokenEndpoint": "http://keycloak:8080/realms/Test/protocol/openid-connect/token",
    "RevokeTokenEndpoint": "http://keycloak:8080/realms/Test/protocol/openid-connect/revoke",
    "ClientId": "test-client",
    "ClientSecret": "********"
  }
}
```

## 動作確認スクリプト

```bash
# トークン取得の動作確認
bash src/AspNetCoreSample.WebApi/keycloak.sh
```
