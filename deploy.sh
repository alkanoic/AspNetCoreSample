# Azureにログイン
az login --use-device-code

RG_NAME=MyResourceGroup
MVC_PLAN_NAME=MyPlan
MVC_APP_NAME=WebSampleApp2024
WEBAPI_PLAN_NAME=MyPlanWebApi
WEBAPI_APP_NAME=WebSampleWebApi2024

# リソースグループの作成
az group create --name $RG_NAME --location japaneast

# for Mvc
# App Serviceプランの作成
az appservice plan create --name $MVC_PLAN_NAME --resource-group $RG_NAME --sku F1 --is-linux

cd src/AspNetCoreSample.Mvc

# アプリケーションのデプロイ
az webapp up --name $MVC_APP_NAME --resource-group $RG_NAME --plan $MVC_PLAN_NAME --os-type Linux --runtime "DOTNETCORE:8.0" --settings WEBSITE_RUN_FROM_PACKAGE=1 DOTNET_VERSION=8.0
az webapp config appsettings set \
  --name $MVC_APP_NAME \
  --resource-group $RG_NAME \
  --settings WEBSITE_RUN_FROM_PACKAGE=1 DOTNET_VERSION=8.0 WebApiOption__WebApiBaseUrl=https://websamplewebapi2024.azurewebsites.net

# for WebAPI
az appservice plan create --name $WEBAPI_PLAN_NAME --resource-group $RG_NAME --sku F1 --is-linux --location japanwest

cd ../../src/AspNetCoreSample.WebApi

# アプリケーションのデプロイ
az webapp up --name $WEBAPI_APP_NAME --resource-group $RG_NAME --plan $WEBAPI_PLAN_NAME --os-type Linux --runtime "DOTNETCORE:8.0" --location japanwest
az webapp config appsettings set \
  --name $WEBAPI_APP_NAME \
  --resource-group $RG_NAME \
  --settings WEBSITE_RUN_FROM_PACKAGE=1 DOTNET_VERSION=8.0
