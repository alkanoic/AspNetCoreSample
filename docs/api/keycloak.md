# Keycloak 管理 API

`KeycloakController` が提供する Keycloak 管理 API です。管理者権限が必要です。

## ベースパス

```
/api/Keycloak
```

## エンドポイント

### ユーザー管理

#### ユーザー作成

```
POST /api/Keycloak/createUser
```

**リクエスト**:

```json
{
  "userName": "newuser",
  "email": "newuser@example.com",
  "firstName": "Taro",
  "lastName": "Yamada",
  "password": "password123",
  "enabled": true
}
```

#### ユーザー更新

```
POST /api/Keycloak/updateUser
```

#### ユーザー削除

```
POST /api/Keycloak/deleteUser
```

**リクエスト**:

```json
{
  "userId": "user-uuid"
}
```

#### ユーザー取得

```
POST /api/Keycloak/fetchUser
```

#### パスワード変更

```
POST /api/Keycloak/changePassword
```

#### パスワードリセット（メール）

```
POST /api/Keycloak/resetPasswordByEmail
```

### ロール管理

#### ロール一覧取得

```
POST /api/Keycloak/fetchRoles
```

#### ユーザーロールマッピング取得

```
POST /api/Keycloak/fetchUserRoleMappings
```

#### ユーザーロール追加

```
POST /api/Keycloak/addUserRoleMappings
```

#### ユーザーロール削除

```
POST /api/Keycloak/deleteUserRoleMappings
```

### クライアント管理

#### クライアント一覧取得

```
POST /api/Keycloak/fetchClients
```

#### クライアント取得

```
POST /api/Keycloak/fetchClient
```

#### クライアントロール取得

```
POST /api/Keycloak/fetchClientRoles
```

#### ユーザークライアントロール取得

```
POST /api/Keycloak/fetchUserClientRoles
```

## サービス実装

`IKeycloakService` / `KeycloakService` (`src/AspNetCoreSample.WebApi/Services/Keycloak/Admin/KeycloakService.cs`) が実際の Keycloak Admin REST API との通信を担当します。

### 設定

```json
{
  "KeycloakOptions": {
    "AdminBaseAddress": "http://keycloak:8080",
    "AdminUserName": "admin",
    "AdminPassword": "admin",
    "TargetRealmName": "Test"
  }
}
```
