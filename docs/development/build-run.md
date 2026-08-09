# ビルドと実行

## ツールの復元

```bash
dotnet tool restore
```

インストールされるツールは次のとおりです。

- `dotnet-ef` - Entity Framework Core CLI
- `dotnet-t4` - T4 テンプレート実行
- `dotnet-format` - コードフォーマッタ
- `kiota` - API クライアント生成
- `CycloneDX` - SBOM 生成

## ビルド

```bash
# ソリューション全体のビルド
dotnet build AspNetCoreSample.sln

# 特定プロジェクトのビルド
dotnet build src/AspNetCoreSample.Mvc
```

### アナライザー

`Directory.Build.props` で以下のアナライザーが有効です。

- Roslynator.Analyzers 4.7.0
- `EnableNETAnalyzers` + `AnalysisLevel=latest-Recommended`

ビルド時に警告が出た場合は修正が必要です。

## フォーマット

```bash
# フォーマットチェック
dotnet format AspNetCoreSample.sln --verify-no-changes

# フォーマット適用
dotnet format AspNetCoreSample.sln
```

`.editorconfig` に準拠したコードスタイルが適用されます。

## アプリケーション起動

### MVC

```bash
dotnet run --project src/AspNetCoreSample.Mvc
# https://localhost:7079
```

### Web API

```bash
dotnet run --project src/AspNetCoreSample.WebApi
# https://localhost:7036
# Swagger: https://localhost:7036/swagger
```

### gRPC

```bash
dotnet run --project src/AspNetCoreSample.Grpc
```

### Aspire オーケストレータ

```bash
dotnet run --project src/AspNetCoreSample.AppHost
```

MVC、Web API、PostgreSQL を一括起動します。

### NuxtSample

```bash
cd src/NuxtSample
pnpm install
pnpm run dev
# http://localhost:3000
```

## Docker ビルド

```bash
# MVC アプリの Docker イメージビルド
bash docker-build.sh

# または
docker build -t aspnetcore-sample-mvc .
```

マルチステージビルドの構成は次のとおりです。

1. `mcr.microsoft.com/dotnet/sdk:10.0` でビルド
2. `mcr.microsoft.com/dotnet/aspnet:10.0-alpine` でランタイム
