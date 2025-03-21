# https://hub.docker.com/_/microsoft-dotnet
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/AspNetCoreSample.Mvc/AspNetCoreSample.Mvc.csproj ./src/AspNetCoreSample.Mvc/
COPY src/AspNetCoreSample.ServiceDefaults/AspNetCoreSample.ServiceDefaults.csproj ./src/AspNetCoreSample.ServiceDefaults/
COPY src/AspNetCoreSample.DataModel/AspNetCoreSample.DataModel.csproj ./src/AspNetCoreSample.DataModel/
WORKDIR /source/src/AspNetCoreSample.Mvc
RUN dotnet restore

# copy everything else and build app
WORKDIR /source
COPY src/AspNetCoreSample.Mvc/. ./src/AspNetCoreSample.Mvc/
COPY src/AspNetCoreSample.ServiceDefaults/. ./src/AspNetCoreSample.ServiceDefaults/
COPY src/AspNetCoreSample.DataModel/. ./src/AspNetCoreSample.DataModel/
WORKDIR /source/src/AspNetCoreSample.Mvc
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AspNetCoreSample.Mvc.dll"]
