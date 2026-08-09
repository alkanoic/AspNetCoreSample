# コード生成

本リポジトリには複数のコード生成アプローチが含まれています。

## CodeGen CLI

`src/CodeGen/` はカスタムコード生成 CLI ツールです。

- テンプレートから MVC アプリケーションのコードを生成
- テンプレート: `Templates/`
- 出力先: `Outputs/`

### 実行

```bash
dotnet run --project src/CodeGen
```

## CodeGen.Result

`src/CodeGen.Result/` は CodeGen CLI によって生成された MVC アプリケーションです。

- 生成物のビルド検証用
- `Controllers/`, `Models/`, `Views/`, `Common/` を含む

## Kiota

Microsoft Kiota を使用した API クライアント生成。

### C# クライアント

```bash
# src/CodeGen.Result.Kiota/csharp/ に生成済み
```

### TypeScript クライアント

```bash
# src/CodeGen.Result.Kiota/typescript/ に生成済み
```

## T4 テンプレート

T4（Text Template Transformation Toolkit）を使用したコード生成。

- `src/T4Execute/` - 実行時テンプレート
- `src/T4Design/` - デザイン時テンプレート
- `SampleGenerator.tt` - サンプルテンプレート

### 実行

```bash
dotnet t4 SampleGenerator.tt --project src/T4Execute
```
