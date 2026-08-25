# AspNetCoreSample

## GitHub Action 設定

Azure Web Apps の構成 → アプリケーション設定。

```bash
WEBSITE_RUN_FROM_PACKAGE=1
DOTNET_VERSION=8.0
```

# dotnet https

```ps1
dotnet dev-certs https -ep .aspnet/https/NetCoreWebAppOnWslDocker001.pfx -p PfxFilePassword
```

# KeycloakTest

Keycloak.AuthServices.Authentication を使用する。

## Keycloak 設定

### Create Realm

Realm name：Test

Realm を「Test」に設定。

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

Windows で動作させる場合は`keycloak`、`127.0.0.1`を hosts に追加すること。

## リモート接続（Tailscale 経由）

devcontainer 内で起動する `opencode web`（8088）と `code-server`（8089）へ、他端末（iPad 等）から Tailscale 経由で接続するための設定。

### 接続先

| サービス | URL | 認証 |
| -------- | --- | ---- |
| opencode web | `http://<WSL2のTailscale IP>:8088` | ユーザー名 `opencode` / `OPENCODE_SERVER_PASSWORD` |
| code-server | `http://<WSL2のTailscale IP>:8089` | `OPENCODE_SERVER_PASSWORD`（opencode と同じ値） |

- WSL2 の Tailscale IP は WSL2 側で `tailscale ip -4` で確認する。
- Docker がポートを `0.0.0.0` で公開しているため、WSL2 の Tailscale IP に直接接続できる（`netsh portproxy` は不要）。
