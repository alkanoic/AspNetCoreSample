# データモデル

`AspNetCoreSample.DataModel` は EF Core を使用した PostgreSQL 向けデータアクセス層です。

## DbContext

`SampleContext` (`src/AspNetCoreSample.DataModel/Models/SampleContext.cs`) がデータベースコンテキストです。

### 接続

```csharp
builder.Services.AddDbContext<SampleContext>(
    options => options.UseNpgsql(connectionString));
```

接続文字列は `appsettings.json` の `ConnectionStrings:Default` から取得します。

```json
{
  "ConnectionStrings": {
    "Default": "Server=postgresql;Port=5432;Database=sampledb;User Id=sa;Password=pass;"
  }
}
```

## エンティティ

### SampleTable

メインのサンプルテーブル。多様なデータ型のカラムを持ちます。

| カラム | 型 | 説明 |
| ------ | -- | ---- |
| `Id` | `int` | 主キー（自動採番） |
| `Name` | `string` | 名前 |
| `IntValue` | `int` | 整数値 |
| `DecimalValue` | `decimal` | 小数値 |
| `DateValue` | `DateTime` | 日付 |
| `BitValue` | `bool` | 真偽値 |
| `CreatedAt` | `DateTime` | 作成日時 |
| `UpdatedAt` | `DateTime` | 更新日時 |

### MultiTable

複合主キーを持つテーブル。

| カラム | 型 | 説明 |
| ------ | -- | ---- |
| `Id` | `int` | 複合主キー 1 |
| `CharId` | `string` | 複合主キー 2 |
| `Name` | `string` | 名前 |

### Name

簡易な名前エンティティ。CRUD デモで使用。

| カラム | 型 | 説明 |
| ------ | -- | ---- |
| `Id` | `int` | 主キー |
| `FirstName` | `string` | 名 |
| `LastName` | `string` | 姓 |

### EnumSample

Enum カラムの使用例を示すエンティティ。

### ParentTable / ChildTable

1対多の親子関係を示すエンティティ。

- `ParentTable` が親
- `ChildTable` が子（`ParentTableId` 外部キー）

### Policy / RolePolicy

動的認可ポリシーを管理するエンティティ。

- `Policy`: ポリシー名を保持
- `RolePolicy`: ロールとポリシーのマッピング（複合キー: `RoleName` + `PolicyName`）

## マイグレーション

EF Core のマイグレーションは `migrate/` ディレクトリで管理されています。

```bash
# マイグレーションの適用
dotnet ef database update --project src/AspNetCoreSample.DataModel
```
