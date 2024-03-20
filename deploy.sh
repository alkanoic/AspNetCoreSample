# Azureにログイン
az login --use-device-code

# リソースグループの作成
az group create --name MyResourceGroup --location japaneast

# App Serviceプランの作成
az appservice plan create --name MyPlan --resource-group MyResourceGroup --sku F1 --is-linux

cd src/AspNetCoreSample.Mvc

# アプリケーションのデプロイ
az webapp up --name WebSampleApp2024 --resource-group MyResourceGroup --plan MyPlan --os-type Linux --runtime "DOTNETCORE:8.0"
