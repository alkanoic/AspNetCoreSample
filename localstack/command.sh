docker compose up -d

sudo apt install -y zip unzip
aws configure
# test test ap-northeast-1 json

dotnet tool restore
dotnet new -i Amazon.Lambda.Templates
dotnet new sln
dotnet new lambda.EmptyFunction --name MyLambdaFunction
dotnet sln add MyLambdaFunction/src/MyLambdaFunction/

# Lambdaのデプロイ

cd MyLambdaFunction/src/MyLambdaFunction
dotnet lambda package
aws --endpoint-url=http://localhost:4566 --region ap-northeast-1 lambda create-function --function-name MyCsharpFunction --runtime dotnet6 --role arn:aws:iam::000000000000:role/irrelevant --handler MyLambdaFunction::MyLambdaFunction.Function::FunctionHandler --zip-file fileb://./bin/Release/net6.0/MyLambdaFunction.zip
aws --endpoint-url=http://localhost:4566 --region ap-northeast-1 lambda update-function-code --function-name MyCsharpFunction --zip-file fileb://./bin/Release/net6.0/MyLambdaFunction.zip

# API Gatewayの設定

aws --endpoint-url=http://localhost:4566 apigateway create-rest-api --name 'MyAPIGateway' --region ap-northeast-1
export REST_API_ID=w0vvmltydi
aws --endpoint-url=http://localhost:4566 apigateway get-resources --rest-api-id $REST_API_ID
export ROOT_RESOURCE_ID=p5mb20kn4d
aws --endpoint-url=http://localhost:4566 apigateway create-resource --rest-api-id $REST_API_ID --parent-id $ROOT_RESOURCE_ID --path-part 'myresource'
export RESOURCE_ID=dqo1otkkab
aws --endpoint-url=http://localhost:4566 apigateway put-method --rest-api-id $REST_API_ID --resource-id $RESOURCE_ID --http-method POST --authorization-type NONE
aws --endpoint-url=http://localhost:4566 apigateway put-integration --rest-api-id $REST_API_ID --resource-id $RESOURCE_ID --http-method POST --type AWS_PROXY --integration-http-method POST --uri arn:aws:apigateway:ap-northeast-1:lambda:path/2015-03-31/functions/arn:aws:lambda:ap-northeast-1:000000000000:function:MyCsharpFunction/invocations
aws --endpoint-url=http://localhost:4566 apigateway create-deployment --rest-api-id $REST_API_ID --stage-name test

# テスト実行

aws --endpoint-url=http://localhost:4566 lambda invoke --function-name MyCsharpFunction --payload '{"input": "abc"}' output.json
aws --endpoint-url=http://localhost:4566 lambda invoke --function-name MyCsharpFunction --payload file://mydata.json output.json

curl -X POST "http://localhost:4566/restapis/$REST_API_ID/test/_user_request_/myresource" -H "Content-Type: application/json" -d '{"input": "abc"}'
