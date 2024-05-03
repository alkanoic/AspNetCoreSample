# 動作方法

```sh
cd src/AspNetCoreSample.Mvc
bash create_certificate.sh
cd ../../tests/AspNetCoreSample.Mvc.Test
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
sudo pwsh bin/Debug/net8.0/playwright.ps1 install-deps
dotnet test
```
