#!/bin/bash

dotnet restore
dotnet dev-certs https --clean --import /workspaces/.aspnet/https/NetCoreWebAppOnWslDocker001.pfx --password PfxFilePassword
