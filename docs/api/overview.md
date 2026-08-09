# Web API 概要

`AspNetCoreSample.WebApi` は以下のエンドポイントを提供します。

## ベース URL

- 開発環境: `https://localhost:7036`
- 本番環境: `https://WebSampleWebApi2024.azurewebsites.net`

## OpenAPI ドキュメント

- Swagger UI: `/swagger`
- ReDoc: `/api-docs`
- OpenAPI JSON: `/swagger/v1/swagger.json`

## 認証

全 API エンドポイントは JWT Bearer 認証で保護されています（一部の公開エンドポイントを除く）。

```
Authorization: Bearer <token>
```

トークンは Keycloak から取得します。

## エンドポイント一覧

| メソッド | パス | コントローラー | 認証 |
| ------- | ---- | ------------- | ---- |
| GET/POST | `/api/Keycloak/*` | KeycloakController | 管理者 |
| POST | `/api/Token/auth` | TokenController | 不要 |
| POST | `/api/Token/refresh` | TokenController | 不要 |
| POST | `/api/Token/revoke` | TokenController | 不要 |
| GET/POST/PUT/DELETE | `/api/DbAccess/*` | DbAccessController | 要 |
| GET/POST | `/api/Simple/*` | SimpleController | 一部不要 |
| POST | `/api/Validation` | ValidationController | 不要 |
| GET | `/api/WeatherForecast` | WeatherForecastController | 不要 |
| POST | `/api/QrCode` | QrCodeController | 不要 |

## エラーレスポンス

エラー時は `WebApiFailResponse` 形式で返却されます。

```json
{
  "message": "エラーメッセージ",
  "errors": {
    "field1": ["エラー詳細1"],
    "field2": ["エラー詳細2"]
  }
}
```

## 多言語対応

`Accept-Language` ヘッダーでレスポンス言語を切り替えられます。

- `ja` - 日本語（デフォルト）
- `en` - 英語
