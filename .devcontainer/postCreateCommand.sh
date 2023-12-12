#!/bin/bash

dotnet dev-certs https --clean --import /workspaces/.aspnet/https/NetCoreWebAppOnWslDocker001.pfx --password PfxFilePassword
dotnet tool restore
dotnet restore
