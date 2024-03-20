# Azureにログイン
az login --use-device-code

# リソースグループの作成
az group create --name MyResourceGroup --location japaneast

# App Serviceプランの作成
az appservice plan create --name MyPlan --resource-group MyResourceGroup --sku F1 --is-linux

cd src/AspNetCoreSample.Mvc

# アプリケーションのデプロイ
az webapp up --name WebSampleApp2024 --resource-group MyResourceGroup --plan MyPlan --os-type Linux --runtime "DOTNETCORE:8.0" --settings WEBSITE_RUN_FROM_PACKAGE=1 DOTNET_VERSION=8.0

az appservice plan create --name MyPlanWebApi --resource-group MyResourceGroup --sku F1 --is-linux --location japanwest

cd src/AspNetCoreSample.WebApi

# アプリケーションのデプロイ
az webapp up --name WebSampleWebApi2024 --resource-group MyResourceGroup --plan MyPlanWebApi --os-type Linux --runtime "DOTNETCORE:8.0" --location japanwest --settings WEBSITE_RUN_FROM_PACKAGE=1 DOTNET_VERSION=8.0
