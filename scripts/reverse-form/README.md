# InlineExpander - Windows Forms 機能分析ツール

## 概要

Windows Forms アプリのイベントハンドラを起点に、自作メソッドをインライン展開し、
LLM（Claude API）で画面の機能一覧を自動生成します。

## セットアップ

### 1. dotnet-script のインストール

```bash
dotnet tool install -g dotnet-script
```

### 2. .NET SDK の確認（6.0 以上推奨）

```bash
dotnet --version
```

### 3. MSBuild が使える環境を確認

Visual Studio または Build Tools for Visual Studio がインストールされていること。

---

## 実行方法

### 基本（展開のみ、API呼び出しなし）

```bash
dotnet script InlineExpander.csx -- \
  --solution "../../AspNetCoreSample.sln" \
  --output   "../../analysis"
```

### API呼び出しあり（機能分析まで実行）

```bash
dotnet script InlineExpander.csx -- \
  --solution "../../AspNetCoreSample.sln" \
  --output   "../../analysis" \
  --apikey   "sk-ant-xxxxxxxx"
```

```bash
cd scripts/reverse-form/InlineExpander
dotnet run -- --solution "../../AspNetCoreSample.sln" --output "../../analysis" --apikey ""
```

### オプション一覧

| オプション | 説明 | デフォルト |
|---|---|---|
| `--solution` | .sln ファイルのパス（必須） | - |
| `--apikey` | Claude API キー | 環境変数 ANTHROPIC_API_KEY |
| `--output` | 出力先ディレクトリ | ソリューションと同階層の `form_analysis/` |
| `--depth` | インライン展開の最大再帰深度 | 10 |

---

## 出力ディレクトリ構造

```
form_analysis/
├── expanded/
│   ├── OrderForm/
│   │   ├── btnSave_Click.txt       ← 展開済みコード
│   │   ├── btnDelete_Click.txt
│   │   └── Form_Load.txt
│   └── CustomerForm/
│       └── ...
└── features/
    ├── OrderForm.md                ← 機能分析結果（Markdown）
    └── CustomerForm.md
```

### expanded/{FormName}/{EventName}.txt の例

```
// === OrderForm.btnSave_Click ===
// ファイル: C:\Projects\MyApp\OrderForm.cs

if (!ValidateInput())
{
    MessageBox.Show("入力エラー");
    return;
}
// ▼ OrderService.Save() をインライン展開
{
    // 引数: order = _currentOrder
    var conn = new SqlConnection(_connectionString);
    conn.Open();
    var cmd = new SqlCommand("INSERT INTO Orders ...", conn);
    cmd.ExecuteNonQuery();
} // ▲ Save()
```

### features/{FormName}.md の例（API呼び出しあり）

```markdown
## 画面概要
受注入力画面。新規受注の登録・編集・削除を行う画面です。

## 機能一覧
- **btnSave_Click**: 受注データを保存する
  - 入力バリデーション後、Ordersテーブルに INSERT/UPDATE
- **btnDelete_Click**: 選択中の受注を削除する
  - 確認ダイアログ後、論理削除フラグを更新

## データ操作
- SqlConnection: データベース接続（Orders, OrderDetails テーブル）
- File.WriteAllText: 受注確認書のPDF出力

## 注意点・特記事項
- バリデーションは ValidateInput() メソッドで集約
- 削除は物理削除ではなく論理削除
```

---

## トラブルシューティング

### `MSBuildLocator` が失敗する

Visual Studio または [Build Tools for Visual Studio](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022) をインストールしてください。

### 特定のプロジェクトが読み込めない

`workspace.WorkspaceFailed` の警告を確認してください。NuGet パッケージの復元が必要な場合があります：

```bash
dotnet restore MyApp.sln
```

### API レート制限エラー

`--depth` を下げてトークン数を削減するか、Form ごとに分割実行してください：

```bash
dotnet script InlineExpander.csx -- --solution "..." --depth 5
```

### 展開が深くなりすぎる

`--depth 3` 程度に制限すると、機能把握には十分なケースが多いです。

---

## カスタマイズポイント

### イベントハンドラの検出パターンを追加

`InlineExpander.csx` 内の `eventHandlerPattern` に正規表現で追加：

```csharp
var eventHandlerPattern = new Regex(@"_(Click|Load|...|MyCustomEvent)$");
```

### 分析プロンプトの変更

`BuildAnalysisPrompt()` 関数内のプロンプトを変更することで、
出力フォーマットや分析観点をカスタマイズできます。
