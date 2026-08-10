# TODO（ソースコード解析による改善候補）

全プロジェクト（Mvc / WebApi / Grpc / DataModel / CodeGen / tests）を静的解析した結果の改善候補一覧。
重要度順に整理し、**対応済みの項目は削除して残項目のみを記載**する。サンプルデモ用リポジトリの趣旨上、全てを直す必要はない。

## S1: セキュリティ（重要）

- [ ] **Mvc: PushSubscriptionStore が static で全ユーザー共有**
  `src/AspNetCoreSample.Mvc/Controllers/PushController.cs`。購読を UserId キーで保持するが static フィールドのため
  並行アクセスで競合し、再起動で全購読が失われる。`[Authorize]` / `[ValidateAntiForgeryToken]` と
  Trigger 例外の logger 化は対応済み。永続ストア（DB / Redis）へ移行するかデモ前提を明示する。
- [ ] **WebApi: KeycloakService の admin 資格情報フォールバック**
  `src/AspNetCoreSample.WebApi/Services/Keycloak/Admin/KeycloakService.cs`。ロール一覧・ロール付与系が
  Authorization ヘッダなし時に `Options` の admin 資格情報で代理アクセスする。
  コントローラは `[Authorize]` 付きになったが、任意のログインユーザーが admin 操作へ昇格しうる。
  フォールバックを廃止し、明示的な管理トークンのみで操作する。
- [ ] **Mvc / WebApi: `RequireHttpsMetadata = false` が無条件設定**
  `Program.cs` で `IsDevelopment()` ゲートを外して HTTP で OIDC/JWT メタデータを取得・検証スキップしている。
  Authority も `http://keycloak`。Keycloak が HTTPS 化されたら Development 限定に戻す。
- [ ] **平文シークレットのリポジトリコミット**
  `src/AspNetCoreSample.Mvc/appsettings.json` / `src/AspNetCoreSample.WebApi/appsettings.json` の DB パスワード /
  Keycloak `ClientSecret` / `AdminPassword`。**devcontainer のローカル用途では許容**。
  非デモ環境では user-secrets / Key Vault へ移行すること。

## S3: API 設計・エラー処理

- [ ] **HTTP メソッドの意味が誤り（DELETE 系のリクエストボディ）**
  `KeycloakController.cs` の `DeleteUserRoleMapping` / `DeleteUserClientRoleMapping` が
  リクエストボディでロール一覧を受け取っている。読み取り系の POST→GET と単純な DELETE の
  パスパラメータ化は対応済み。

## S4: テスト・生成系

- [x] **`WebApplicationFactoryFixture` の `CreateHost` 二重 `Build()` + 未破棄 Kestrel**
  `tests/AspNetCoreSample.Mvc.Test` / `tests/AspNetCoreSample.WebApi.Test` の `WebApplicationFactoryFixture.cs`。
  ポート 0 + `IServerAddressesFeature` による動的ポート割り当て、`DisposeAsync` での
  Kestrel ホスト・コンテナ破棄、`LoggingAttribute` の `ObjectDisposedException` 対策を実施済み。
