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
- [ ] **平文シークレットのリポジトリコミット**
  `src/AspNetCoreSample.Mvc/appsettings.json` / `src/AspNetCoreSample.WebApi/appsettings.json` の DB パスワード /
  Keycloak `ClientSecret` / `AdminPassword`。**devcontainer のローカル用途では許容**。
  非デモ環境では user-secrets / Key Vault へ移行すること。

## S2: 正確性バグ

- [ ] **ログ属性で `.WithProperty("Arguments")` が二重発行**
  `src/AspNetCoreSample.Mvc/Logging/LoggingAttribute.cs` と WebApi 側の複製。前一項を後一項が同じキーで上書きする。

## S3: API 設計・エラー処理

- [ ] **Keycloak / DB 障害が 400 で `ex.Message`（内部メッセージ）を含む**
  `Controllers/KeycloakController.cs` の `catch (InvalidDataException ex)` が `WebApiFailResponse(ex)` として
  生の Keycloak 応答を返す。500 側は固定メッセージに化済み（`WebApiFailResponse("Internal server error")`）。
  400 側も固定メッセージ化し、詳細はログのみへ。
- [ ] **HTTP メソッドの意味が誤り**
  `KeycloakController.cs` の読み取り系（`FetchUser` / `FetchClient` / `FetchClientRoles` /
  `FetchUserRoleMappings` / `FetchUserClientRoles`）が `[HttpPost]`。また DELETE 系がリクエストボディを利用する。
  `[HttpGet]` + パス / クエリパラメータ化。

## S4: テスト・生成系

- [ ] **Testcontainers が全 Fixture で未 Dispose**
  `tests/AspNetCoreSample.WebApi.Test/DbFixture.cs` / `KeycloakFixture.cs` / `WebApplicationFactoryFixture.cs`
  が `ValueTask.CompletedTask` を返却し、Ryuk までコンテナが残る。
- [ ] **`CreateHost` で `builder.Build()` の二重呼出 + 未破棄 Kestrel**
  `tests/AspNetCoreSample.WebApi.Test/WebApplicationFactoryFixture.cs`（Mvc 側にも複製）。
  ホスト / ポート漏洩、flaky の原因。

## S5: 整理・お掃除

- [ ] **未使用の DI 注入を掃除**
  コンストラクタに注入しているが未使用の `ILogger<T>` / `IHttpClientFactory`。
  `QrCodeController` と `Console.WriteLine` の NLog 化は対応済み。
- [ ] **重複規定の共通化**
  Htmx / Vue / VueComponent コントローラのビューモデル構築が逐語複製。
  `GetAvailablePort` も WebApi テスト / Mvc テストに同一実装がある（共通ヘルパへ集約候補）。
