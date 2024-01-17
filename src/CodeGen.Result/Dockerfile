FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# install Keploy
# RUN apt-get update && apt-get install -y curl
RUN apk add curl
# Download the ca.crt file
RUN curl -o ca.crt https://raw.githubusercontent.com/keploy/keploy/main/pkg/proxy/asset/ca.crt
RUN curl -o setup_ca.sh https://raw.githubusercontent.com/keploy/keploy/main/pkg/proxy/asset/setup_ca.sh

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=build-env /App/ca.crt .
COPY --from=build-env /App/setup_ca.sh .
RUN apk update && apk add bash

# Give execute permission to the setup_ca.sh script
RUN chmod +x setup_ca.sh

# ENTRYPOINT ["dotnet", "CodeGen.Result.dll"]
CMD ["/bin/bash", "-c", "source ./setup_ca.sh && dotnet CodeGen.Result.dll"]
