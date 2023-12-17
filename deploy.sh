# Azureにログイン
az login --use-device-code

# リソースグループの作成
az group create --name MyResourceGroup --location japaneast

# App Serviceプランの作成
az appservice plan create --name MyPlan --resource-group MyResourceGroup --sku F1 --is-linux

# Web Appの作成
az webapp create --name WebSampleApp2023 --resource-group MyResourceGroup --plan MyPlan --runtime "DOTNET|6.0"

cd src/AspNetCoreSample.Mvc

# アプリケーションのデプロイ
az webapp up --name WebSampleApp2023 --resource-group MyResourceGroup --os-type Linux --runtime "DOTNET|6.0"
