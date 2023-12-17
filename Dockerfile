# https://hub.docker.com/_/microsoft-dotnet
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/AspNetCoreSample.Mvc/AspNetCoreSample.Mvc.csproj ./src/AspNetCoreSample.Mvc/
COPY src/AspNetCoreSample.ServiceDefaults/AspNetCoreSample.ServiceDefaults.csproj ./src/AspNetCoreSample.ServiceDefaults/
WORKDIR /source/src/AspNetCoreSample.Mvc
RUN dotnet restore

# copy everything else and build app
WORKDIR /source
COPY src/AspNetCoreSample.Mvc/. ./src/AspNetCoreSample.Mvc/
COPY src/AspNetCoreSample.ServiceDefaults/. ./src/AspNetCoreSample.ServiceDefaults/
WORKDIR /source/src/AspNetCoreSample.Mvc
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0.0-alpine3.18
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AspNetCoreSample.Mvc.dll"]
