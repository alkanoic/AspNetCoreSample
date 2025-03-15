#!bin/bash

targetDir=../AspNetCoreSample.Templates
assemblyPath=${targetDir}/bin/Debug/net9.0/AspNetCoreSample.Templates.dll

dotnet build
dotnet run -- --ap $assemblyPath --op ./output.cs

# dotnet build $targetDir
# dotnet t4 -o output.cs -P targetPath=$assemblyPath ./sample.tt
