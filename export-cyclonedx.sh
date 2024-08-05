dotnet CycloneDX AspNetCoreSample.sln --json --filename dotnet.json --output sbom
trivy sbom sbom/dotnet.json --format json --output sbom/dotnet-result.json

cyclonedx-npm --output-format json --output-file sbom/AspNetCoreSample.Mvc_npm.json src/AspNetCoreSample.Mvc/package.json
trivy sbom sbom/AspNetCoreSample.Mvc_npm.json --format json --output sbom/AspNetCoreSample.Mvc_npm-result.json

cyclonedx-npm --output-format json --output-file sbom/typescript_lit-component_npm.json src/typescript/lit-component/package.json
trivy sbom sbom/typescript_lit-component_npm.json --format json --output sbom/typescript_lit-component_npm-result.json

# javaは個別でビルドすると作成
# mvn package
trivy sbom sbom/SpringBoot.Report.json --format json --output sbom/SpringBoot.Report-result.json
