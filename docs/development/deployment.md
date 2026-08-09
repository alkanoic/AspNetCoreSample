# Azure デプロイ

## デプロイ先

| アプリケーション | Azure リソース | 種類 |
| ---------------- | -------------- | ---- |
| MVC | `WebSampleApp2024` | Azure Web Apps (Linux) |
| Web API | `WebSampleWebApi2024` | Azure Web Apps (Linux) |
| NuxtSample | Static Web App | Azure Static Web Apps |
| Spring Boot | `WebSampleSpring2024` | Azure Web Apps (Linux) |

## 手動デプロイ

### MVC / Web API

```bash
# deploy.sh を使用
bash deploy.sh
```

`deploy.sh` は `az webapp up` で MVC と Web API をデプロイします。

### 個別デプロイ

```bash
# MVC
dotnet publish src/AspNetCoreSample.Mvc -c Release -o published --framework net10.0
az webapp deploy --resource-group <rg> --name WebSampleApp2024 --src-path published

# Web API
dotnet publish src/AspNetCoreSample.WebApi -c Release -o published --framework net10.0
az webapp deploy --resource-group <rg> --name WebSampleWebApi2024 --src-path published
```

## アプリ設定

Azure Web Apps に必要なアプリケーション設定は次のとおりです。

| 設定キー | 値 | 説明 |
| -------- | -- | ---- |
| `WEBSITE_RUN_FROM_PACKAGE` | `1` | パッケージから実行 |
| `DOTNET_VERSION` | `10.0` | .NET バージョン |
| `ASPNETCORE_ENVIRONMENT` | `Production` | 環境 |
| `ConnectionStrings__Default` | (接続文字列) | PostgreSQL 接続 |
| `ConnectionStrings__Redis` | (接続文字列) | Redis 接続 |
| `KeycloakOptions__Authority` | (Keycloak URL) | 認証サーバー |
| `KeycloakOptions__ClientId` | `test-client` | クライアント ID |
| `KeycloakOptions__ClientSecret` | (シークレット) | クライアントシークレット |

## CI/CD 自動デプロイ

`main` ブランチへの push で自動デプロイされます。詳細は [CI/CD パイプライン](ci-cd.md) を参照してください。
