# 動作方法

```sh
cd src/AspNetCoreSample.Mvc
bash create_certificate.sh
cd ../../tests/AspNetCoreSample.Mvc.Test
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
sudo pwsh bin/Debug/net10.0/playwright.ps1 install-deps
dotnet test
```

注意: TUnit 1.5.80を使用していますが、Playwrightは引き続き手動でインストールする必要があります。
