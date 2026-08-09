# TODO（ソースコード解析による改善候補）

全プロジェクト（Mvc / WebApi / Grpc / DataModel / CodeGen / tests）を静的解析した結果の改善候補一覧。
重要度順に整理。サンプルデモ用リポジトリの趣旨上、全てを直す必要はないが、**S1 は早急対応推奨**。

## S1: セキュリティ（最重要）

- [ ] **WebApi: KeycloakController に認可が無く匿名で管理者 API が操作可能**
  `src/AspNetCoreSample.WebApi/Controllers/KeycloakController.cs:11` に `[Authorize]` が無い。
  加えて `Services/Keycloak/Admin/KeycloakService.cs:89-92` が Authorization ヘッダなし時に
   ハードコードされた admin 資格情報でトークンを自動取得するため、誰でもユーザー作成・
  パスワード削除・ロール付与のフル管理者 API を叩ける。認可の追加とトークンスフォールバックの廃止。
- [ ] **Mvc: Push サブスクリプションが static フィールドで全ユーザー共有**
  `src/AspNetCoreSample.Mvc/Controllers/PushController.cs:39-48`。並行アクセスで競合し他人が
  上書き可能。POST に `[ValidateAntiForgeryToken]` と `[Authorize]` が無い。
  また `Trigger` GET が全例外を `Debug.WriteLine(ex)` で握りつぶし（リリースビルドで死にコード）。
- [ ] **Mvc: VAPID 鍵を起動ごとに再生成**
  `src/AspNetCoreSample.Mvc/Program.cs:73-78` で `GenerateVapidKeys()` が毎回新しい鍵を生成。
  再起動・スケールアウトで全 Push 購読が失効する。既存の `Options/VapidOption.cs` の設定が
  bind されておらず事実上未使用。鍵は秘密情報として Secret 管理に載せること。
- [ ] **全 plaintext シークレットのリポジトリコミット**
  `src/AspNetCoreSample.Mvc/appsettings.json:10-17`、`src/AspNetCoreSample.WebApi/appsettings.json:18-20`。
  DB パスワード / Keycloak `ClientSecret` / `AdminPassword` が平文で git 履歴にも残っている。
  非デモ環境では user-secrets / Key Vault へ。
- [ ] **Mvc: TLS 証明書検証の全無効化**
  `src/AspNetCoreSample.Mvc/Components/MyComponent.razor:33-44`。
  `ServerCertificateCustomValidationCallback` が常に `true`（MITM 受け入れ）。さらに HttpClient を毎リクエスト new。
- [ ] **Mvc / WebApi: `RequireHttpsMetadata = false` が無条件設定**
  `Program.cs:69` / `WebApi/Program.cs:62-63`。`IsDevelopment()` ゲートを外して HTTP で
  OIDC/JWT メタデータを取得・検証スキップしている。Authority も `http://keycloak`。
- [ ] **WebApi: Swagger / Keycloak 管理 API が本番で無条件公開**
  `src/AspNetCoreSample.WebApi/Program.cs:131-134`。Development ガードがコメントアウト済み。

## S2: 正確性バグ

- [ ] **CodeGen: DataTables 検索の Contains 誤り**
  `src/CodeGen.Result/Common/QueryableExtension.cs:242`（`GetContainsMethod`）。
  `int.TryParse` でゲートしており、数値以外の文字列検索が黙って落ちる。
- [ ] **CodeGen: `GetEqualMethod` が型不一致で 500**
  `QueryableExtension.cs:288-303`。bool / string プロパティに `Expression.Equal(property, int)` を組み、
  数値検索時に `InvalidOperationException`。`GetNotEqualMethod` は Contains を `Not` し「NOT CONTAINS」になる誤り。
- [ ] **CodeGen: ThenBy / ThenByDescending が OrderBy になっている**
  `QueryableExtension.cs:22-25`。マルチカラムソートで先頭ソートキーが破棄される。
- [ ] **Mvc: FluentValidation ルールが逆**
  `src/AspNetCoreSample.Mvc/Models/FluentViewModel.cs:29`。`No.LessThan(0)` は負数のみ許可しており
  メッセージ「Noは0以上を入力してください」と矛盾。`GreaterThanOrEqualTo(0)` の誤り。
- [ ] **WebApi: 同一エンティティを 2 回 Add**
  `Controllers/DbAccessController.cs:44-45`。`Add(name)` + `Names.Add(name)` の二重登録。
  レスポンスも `Ok(100)` のマジックナンバー（`CreatedAtAction` 等へ）。
- [ ] **Mvc/WebApi: ログ属性で `.WithProperty("Arguments")` が二重発行**
  `src/AspNetCoreSample.Mvc/Logging/LoggingAttribute.cs:146-147` と WebApi 側複製。前一項が後一項で上書きされキーが重複。
- [ ] **WebApi: `IDistributedCache` を同期 I/O で使用**
  `Controllers/DbAccessController.cs`（Mvc の Session デモも同様）。スレッドブロック。AsyncApi 化。

## S3: API 設計・エラー処理

- [ ] **WebApi: 全例外の 400 への無差別変換 + 内部メッセージ漏洩**
  `Controllers/KeycloakController.cs:55-72`（DB / Keycloak 障害も 400 化、`ex.Message` に生の Keycloak 応答が混入）。
  `DbAccessController.cs:51`、`ValidationController.cs:31` も `StatusCode(500, ex.Message)`。
- [ ] **WebApi: ルートタイポ "FtechUserRoleMappings"**
  `Controllers/KeycloakController.cs:285`（`u` 抜け）。OpenAPI 契約として公開される。
- [ ] **WebApi: HTTP メソッドの意味が誤り**
  `KeycloakController.cs:83,102,269` の平安読み取りが `[HttpPost]`。DELETE がリクエストボディを利用。
- [ ] **WebApi: 管理 URL に未エンコードの username を挿入**
  `Services/Keycloak/Admin/KeycloakService.cs:94`。`Uri.EscapeDataString` 未使用でクエリ改変・パラメータ注入の恐れ。
- [ ] **WebApi: CancellationToken 皆無**
  `HttpClient` / EF の async 呼び出し全てにキャンセルなし。リクエスト中断が反映されずスレッドプール圧迫。
- [ ] **WebApi: CORS ポリシー不整合**
  `Program.cs:94`。`AllowAnyHeader()` が二重。平読のクレデンシャル混在。

## S4: テスト・生成系（リソース / flaky リスク）

- [ ] **テスト: Testcontainers が全 Fixture で未 Dispose**
  `tests/AspNetCoreSample.WebApi.Test/DbFixture.cs:32-35` や `KeycloakFixture.cs:41-44` が
  `ValueTask.CompletedTask` を返却。Ryuk までコンテナが残る。`WebApplicationFactoryFixture` は破棄処理無し。
- [ ] **テスト: `CreateHost` で `builder.Build()` の二重呼出 + 未破棄 Kestrel**
  `WebApplicationFactoryFixture.cs:78-86`（Mvc 側にも複製）。ホスト/ポート漏洩、flaky の原因。
- [ ] **テスト: ランダムポートの check-then-use race**
  `GetAvailablePort.cs:8-10`。予約なしに見つかるので同時起動で `address already in use`。
- [ ] **テスト: `MvcDbFixture` が全例外を握りつぶし**（`Mvc.Container.Test/MvcDbFixture.cs:84-95`）
- [ ] **CodeGen: Create ビューの validation が常に `Id` にバインド**
  `src/CodeGen/TemplateControl.cs:210`。`asp-validation-for="Id"` 固定でフィールド毎に正しく出ない。
- [ ] **CodeGen: 生成コントローラで同期 `Any()`**（`TemplateControl.cs:167` 等）。非同期アクション内で同期 DB 往札。
- [ ] **CodeGen: `authorize` フラグが不完全実装**
   datatables / mvc テンプレートに `__Authorize__` プレースホルダが無く、オプションが効かない。

## S5: 整理・お掃除

- [ ] **全てのコントローラで `ILogger<T>` / `IHttpClientFactory` が未使用**
  注入フィールドがデッドコード（例: `src/AspNetCoreSample.Mvc/Controllers/QrCodeController.cs:9-14`）。
  使うか削除。
- [ ] **`Console.WriteLine` の NLog 回避**
  `Controllers/LitController.cs:26-27`。パスワードを含む echo も含まれる。`_logger` へ。
- [ ] **ビュー内のタイポ / 破棄文言**
  `Views/JQuery/index.cshtml:3`（タイトル "Vue Page"）、`Views/Vue/Index.cshtml:34`（`using System.Text.Encodings.Web;` が可視表示）。
- [ ] **インライナー script への生 JSON 埋め込み**
  `Views/Shared/_Layout.cshtml:103` の `@Html.Raw(JsonSerializer.Serialize(...))`。`</script>` 文脈脱出のリスク。
  JavaScriptEncoder 付きの `Json` ヘルパを使用。
- [ ] **スペル・名前空間のタイポ**
  `WebApi/Simple/CusomPolisyProvider.cs`（`CusomPolisy`）、`ValidationController.cs:8`（`WebApiSample.Controllers`）。
- [ ] **重複規定の共通化**
  Htmx / Vue / VueComponent コントローラのビューモデル構築が逐語複製。`GetAvailablePort` も WebApi / Mvc / Test で同一。
- [ ] **DataTables 応答の recordsTotal / recordsFiltered 誤り**
  `CodeGen.Result/Controllers/CustomController.cs:81-82`。`recordsTotal` は非フィルタ合計であるべき。
  同期 `Count()` + 条件ごとの述語再合成（O(N²) 的な SQL 膨張）もある。