# AspNetCoreSample

# KeycloakTest

Keycloak.AuthServices.Authentication を使用する。

## Keycloak 設定

### Create Realm

Realm name：Test

Realm を「Test」に設定

### Create User

Users -> Add user

Username：test

test ユーザを選択。
Credentials タブで新しいパスワードを追加。
Password を test として設定。

同様に user と admin を作成しておく。

### Create Client

- Client type：OpenID Connect
- Client ID：test-client
- Valid redirect URIs：\*

### Create Role

- Realm roles
- Create Role
- admin and user

| ユーザー名 | ロール |
| ---------- | ------ |
| test       |        |
| user       | user   |
| admin      | admin  |

### Hosts

Windows で動作させる場合は`keycloak`、`127.0.0.1`を hosts に追加すること
