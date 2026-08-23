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

### 前提

- Docker Desktop はポートを Windows の `127.0.0.1`（localhost）にのみ公開するため、Tailscale の IP からは直接届かない。
- そのため Windows 側で `netsh portproxy` による転送が必要。

### Windows 側の設定（管理者 PowerShell）

```powershell
# opencode web (8088)
netsh interface portproxy add v4tov4 listenport=8088 listenaddress=0.0.0.0 connectport=8088 connectaddress=127.0.0.1

# code-server (8089)
netsh interface portproxy add v4tov4 listenport=8089 listenaddress=0.0.0.0 connectport=8089 connectaddress=127.0.0.1

# ファイアウォールで受信を許可
New-NetFirewallRule -DisplayName "opencode 8088" -Direction Inbound -LocalPort 8088 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "code-server 8089" -Direction Inbound -LocalPort 8089 -Protocol TCP -Action Allow
```

### 接続先

| サービス | URL | 認証 |
| -------- | --- | ---- |
| opencode web | `http://<WindowsのTailscale IP>:8088` | ユーザー名 `opencode` / `OPENCODE_SERVER_PASSWORD` |
| code-server | `http://<WindowsのTailscale IP>:8089` | `OPENCODE_SERVER_PASSWORD`（opencode と同じ値） |

- Tailscale IP は Windows 側で `tailscale ip -4` で確認する。
- `netsh portproxy` は Windows 再起動で消えるため、恒久化する場合はスタートアップタスク等に登録する。
