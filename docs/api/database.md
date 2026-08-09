# データベース API

`DbAccessController` が提供する EF Core を使用した CRUD API です。`Name` エンティティを操作します。

## ベースパス

```
/api/DbAccess
```

## エンティティ

```csharp
public class Name
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

## エンドポイント

### 一覧取得

```
GET /api/DbAccess
```

**レスポンス**:

```json
[
  {
    "id": 1,
    "firstName": "Taro",
    "lastName": "Yamada"
  }
]
```

### 詳細取得

```
GET /api/DbAccess/{id}
```

### 作成

```
POST /api/DbAccess
```

**リクエスト**:

```json
{
  "firstName": "Hanako",
  "lastName": "Suzuki"
}
```

### 更新

```
PUT /api/DbAccess/{id}
```

### 削除

```
DELETE /api/DbAccess/{id}
```

## データベース

PostgreSQL の `Name` テーブルにマッピングされます。`SampleContext` を通じてアクセスします。

## 認証

全エンドポイントで JWT Bearer 認証が必要です。
