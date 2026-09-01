# Playwright の使い方（起動中の Chrome への接続）

`e2e/` の Playwright は通常、Playwright が管理するブラウザ（`chromium` / `chrome` / `msedge` など）を自動起動してテストを実行する。
ここでは、**すでに起動している Chrome に接続して**テスト・codegen を実行する方法を、実行環境（Windows / WSL）と Chrome の起動場所の組み合わせごとに説明する。

## 前提

- 接続先の Chrome は **リモートデバッグポートを開いた状態**で起動しておく必要がある。
- 接続には Playwright の `connectOverCDP`（Chrome DevTools Protocol）を使う。
- 接続先 Chrome は、テスト対象のプロファイル（ログイン状態・拡張機能など）をそのまま使えるのが利点。

---

## 1. Windows で実行し、Windows で起動中の Chrome に接続する

### 1-1. Chrome をデバッグポート付きで起動する（ショートカット）

1. デスクトップなどで右クリック → **新規作成 → ショートカット**
2. 「項目の場所」に以下を指定:

```
"C:\Program Files\Google\Chrome\Application\chrome.exe" --remote-debugging-port=9222 --user-data-dir="%TEMP%\chrome-debug-profile"
```

3. 名前を付けて完了。以後このショートカットから起動すれば、常にポート 9222 が開いた状態になる。
4. 初回起動時は Chrome のサインイン画面が表示される。テスト用プロファイルなので Google アカウントにログインせず、**「ログアウト状態を保持する」**を選択する。

- 既存の Chrome が起動中だとポート指定が無視されるため、`--user-data-dir` で別プロファイルを指定する。
- 既存のショートカットを流用する場合は、プロパティの「リンク先」末尾に引数を追記する形でも OK。
- 起動後、`http://localhost:9222/json/version` にアクセスして `webSocketDebuggerUrl` が返れば成功。

### 1-2. Playwright から接続する

```ts
import { test, expect, chromium } from "@playwright/test";

test("起動中の Chrome に接続する", async () => {
  const browser = await chromium.connectOverCDP("http://localhost:9222");
  const context = browser.contexts()[0];
  const page = context.pages()[0] ?? (await context.newPage());

  await page.goto("https://localhost:7079/");
  await expect(page).toHaveTitle(/AspNetCoreSample/);
});
```

- `connectOverCDP` は既存のブラウザコンテキストを再利用する。新しいタブが欲しい場合は `context.newPage()` を使う。

### 1-3. テストを実行する

```bash
# Windows → Windows の Chrome
npx playwright test --config playwright.connect.config.ts
```

- 接続テストは単一の Chrome インスタンスに接続するため、専用設定 `playwright.connect.config.ts`（単一プロジェクト・並列なし）で実行する。

---

## 2. WSL で実行し、Windows で起動中の Chrome に接続する

WSL 内で Playwright を実行しつつ、**Windows 側で起動している Chrome** に接続する。

### 2-1. Windows 側で Chrome をデバッグポート付きで起動する

```powershell
& "C:\Program Files\Google\Chrome\Application\chrome.exe" `
  --remote-debugging-port=9222 `
  --user-data-dir="$env:TEMP\chrome-debug-profile"
```

### 2-2. WSL から Windows の Chrome に接続する

WSL2 には **localhost 転送**が備わっており、WSL2 内の `localhost` は Windows の `127.0.0.1` に届く。
そのため `localhost:9222` に直接接続すれば、`Host` ヘッダーが `localhost` のまま Chrome に届き、Chrome 152 の DNS リバインディング対策を通過できる。

```ts
const browser = await chromium.connectOverCDP("http://localhost:9222");
```

- Chrome 152 以降は `--remote-debugging-address=0.0.0.0` が廃止され、デバッグポートは常に `127.0.0.1` にバインドされる。しかし WSL2 の localhost 転送を使えば、接続元がループバックに見えるため問題にならない。
- `netsh portproxy` は不要（むしろ `Host` ヘッダーを書き換えないため Chrome 152 では失敗する）。

### 2-3. テストを実行する

```bash
# WSL → Windows の Chrome
npx playwright test --config playwright.connect.config.ts
```

- 接続テストは単一の Chrome インスタンスに接続するため、専用設定 `playwright.connect.config.ts`（単一プロジェクト・並列なし）で実行する。

### 2-4. localhost 転送が効かない場合（Tailscale 等）

Tailscale などの VPN が入っていると、WSL2 の localhost 転送が無効になり `localhost:9222` が届かないことがある。
その場合は「4. devcontainer 内で実行し、Windows で起動中の Chrome に接続する」と同じく、**Windows 直上にリバースプロキシを立てる**方式を使う。

```powershell
# Windows 側でリバースプロキシを起動（詳細はセクション 4-2 参照）
node chrome-proxy.js
```

```bash
# WSL からは Windows ホストの IP を指定
WIN_HOST=$(ip route show default | awk '{print $3}')
CDP_ENDPOINT=http://$WIN_HOST:9222 npx playwright test --config playwright.connect.config.ts
```

---

## 3. WSL で実行し、WSL で起動中の Chrome に接続する

WSL 内に Chrome（Linux 版）をインストールし、WSL 内で起動した Chrome に接続する。

### 3-1. WSL に Chrome をインストールする

```bash
# Google Chrome のリポジトリを追加してインストール
wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | sudo gpg --dearmor -o /usr/share/keyrings/google-chrome.gpg
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main" | sudo tee /etc/apt/sources.list.d/google-chrome.list
sudo apt-get update
sudo apt-get install -y google-chrome-stable
```

### 3-2. Chrome をデバッグポート付きで起動する

```bash
google-chrome \
  --remote-debugging-port=9222 \
  --user-data-dir=/tmp/chrome-debug-profile \
  --no-sandbox \
  --headless=new &
```

- WSL 上で GUI を表示するには WSLg（Windows 11 の既定）または X サーバーが必要。ヘッドレスで動かす場合は `--headless=new` を付ける。
- `--no-sandbox` は root 実行時などに必要になる場合がある。

### 3-3. Playwright から接続する

```ts
const browser = await chromium.connectOverCDP("http://localhost:9222");
```

- WSL 内の `localhost` は WSL 自身を指すため、そのまま接続できる。

### 3-4. Linux（WSL）上で Windows と同じ表示になるように依存関係をインストールする

Linux 上で Playwright のブラウザを起動すると、**フォントや描画ライブラリが不足して Windows と表示が異なる**（文字化け・豆腐・レイアウト崩れ）ことがある。
スクリーンショットのスナップショット比較を安定させるため、以下の依存関係を入れる。

#### 3-4-1. Playwright のブラウザ依存ライブラリ

```bash
# Playwright が管理するブラウザの依存を一括インストール
npx playwright install --with-deps
```

- `--with-deps` は `apt-get` で必要な共有ライブラリ（`libnss3` / `libatk` / `libgbm` など）を自動導入する。

#### 3-4-2. 日本語フォント（Windows 相当の表示にするため）

Windows の既定フォント（MS ゴシック / メイリオ / Yu Gothic）に近い表示にするため、日本語フォントを導入する。

```bash
sudo apt-get update
sudo apt-get install -y \
  fonts-noto-cjk \
  fonts-ipafont-gothic \
  fonts-ipafont-mincho \
  fonts-vlgothic \
  fonts-takao
```

- `fonts-noto-cjk` は CJK 統合フォントで、日本語・中国語・韓国語をカバーする。
- メイリオ相当の見た目に近づけたい場合は `fonts-ipafont-gothic`（IPA ゴシック）や `fonts-vlgothic`（VL ゴシック）が有効。

#### 3-4-3. その他の描画関連

```bash
sudo apt-get install -y \
  fonts-liberation \
  fonts-dejavu \
  fontconfig
```

- `fonts-liberation` は Arial / Times New Roman などのメトリクス互換フォントで、Windows の標準フォントに近い幅になる。
- `fontconfig` はフォント解決の設定。導入後 `fc-cache -fv` でキャッシュを更新する。

#### 3-4-4. フォントキャッシュの更新

```bash
fc-cache -fv
```

#### 3-4-5. 確認

```bash
# 日本語フォントが認識されているか確認
fc-list :lang=ja
```

---

## 4. devcontainer 内で実行し、Windows で起動中の Chrome に接続する

devcontainer はヘッドレスの Linux コンテナ（Ubuntu noble ベース）で、GUI を持たない。
そのため「devcontainer 内で Chrome を起動して表示する」のではなく、**Windows ホスト側で起動している Chrome に接続する**のが基本になる。

### なぜ portproxy では接続できないのか

Chrome 152 以降、デバッグポートは常に `127.0.0.1` にバインドされ、`--remote-debugging-address=0.0.0.0` は廃止された。
さらに DevTools の HTTP エンドポイントは **DNS リバインディング対策**として、リクエストの `Host` ヘッダーが `localhost` / `127.0.0.1` 以外だと黙って切断する。

devcontainer は Docker Desktop（WSL2 バックエンド）上の 2 段 NAT の内側にいるため、Windows の Chrome に届くには IP（`172.17.160.1` など）を指定するしかなく、その時点で `Host` ヘッダーが IP になり Chrome に拒否される。

`netsh portproxy` は TCP をそのまま転送するだけで `Host` ヘッダーを書き換えないため、この問題を解決できない。
また、WSL2 の localhost 転送（mirrored networking）も、この環境では Tailscale の影響で無効になっており使えない。

そこで、**Windows 直上にリバースプロキシを立てて `Host` ヘッダーを `localhost` に書き換える**方式を使う。

```
[devcontainer] ──(0.0.0.0:9223)──> [Windows のプロキシ] ──(127.0.0.1:9222, Host: localhost)──> [Chrome]
```

### 4-1. Windows 側で Chrome をデバッグポート付きで起動する

「2-1」と同じ。Windows 側でポート 9222 を開いた状態で Chrome を起動する。

```powershell
& "C:\Program Files\Google\Chrome\Application\chrome.exe" `
  --remote-debugging-port=9222 `
  --user-data-dir="$env:TEMP\chrome-debug-profile"
```

### 4-2. Windows 側にリバースプロキシを立てる

Node.js の `http-proxy` で、Chrome の `localhost:9222` へ転送する。
Chrome 152 の仕様変更（IPv6 バインド・DNS リバインディング対策）に対応するため、以下の 3 点を行う。

1. 転送先を `localhost:9222` にする（Chrome 152 は `[::1]:9222` にバインドするため `127.0.0.1` では届かない）
2. `Host` ヘッダーを `localhost:9222` に書き換える（DNS リバインディング対策を回避）
3. レスポンス内の `webSocketDebuggerUrl` をプロキシのアドレスに書き換える（`ws://localhost:9222` のままだと devcontainer 内の `localhost` に接続してしまう）

```powershell
# プロキシ用フォルダを作成して依存を導入
mkdir $env:TEMP\chrome-proxy; cd $env:TEMP\chrome-proxy
npm init -y
npm install http-proxy
```

`chrome-proxy.js` を作成する:

```js
const httpProxy = require("http-proxy");
const proxy = httpProxy.createProxyServer({
  target: "http://localhost:9222",
  ws: true,
  changeOrigin: true,
  selfHandleResponse: true,
});

proxy.on("error", (err, req, res) => {
  console.error("proxy error:", err.message);
  if (res && !res.headersSent) res.writeHead(502);
  if (res) res.end("proxy error: " + err.message);
});

// Host ヘッダーを localhost に書き換える（Chrome 152 の DNS リバインディング対策を回避）
proxy.on("proxyReq", (proxyReq, req) => {
  proxyReq.setHeader("Host", "localhost:9222");
});

// レスポンス内の webSocketDebuggerUrl をプロキシのアドレスに書き換える
proxy.on("proxyRes", (proxyRes, req, res) => {
  const chunks = [];
  proxyRes.on("data", (chunk) => chunks.push(chunk));
  proxyRes.on("end", () => {
    let body = Buffer.concat(chunks).toString("utf8");
    const host = req.headers.host; // 例: 172.17.160.1:9223
    body = body.replace(/ws:\/\/localhost:9222/g, `ws://${host}`);

    // Content-Length を書き換え後のボディ長に合わせて再計算する
    const headers = { ...proxyRes.headers };
    headers["content-length"] = Buffer.byteLength(body);

    res.writeHead(proxyRes.statusCode, headers);
    res.write(body);
    res.end();
  });
});

// 待受ポートは 9223 にする（9222 は Chrome が使用中のため衝突する）
proxy.listen(9223, "0.0.0.0");
console.log("Chrome proxy listening on 0.0.0.0:9223");
```

起動する:

```powershell
node chrome-proxy.js
```

### 4-3. プロキシを Windows サービスとして常駐させる（任意）

毎回手動起動するのが面倒な場合は、NSSM（Non-Sucking Service Manager）でサービス化して自動起動にする。

```powershell
# NSSM をインストール後、管理者 PowerShell で実行
nssm install ChromeProxy node "C:\path\to\chrome-proxy.js"
nssm set ChromeProxy AppDirectory "C:\path\to\chrome-proxy"
nssm set ChromeProxy Start SERVICE_AUTO_START
nssm start ChromeProxy
```

- `C:\path\to\chrome-proxy` は 4-2 で作成したフォルダの実パスに置き換える。
- サービス化すると Windows 起動時にプロキシが自動起動し、devcontainer から常に接続できる。

### 4-4. devcontainer から Windows の Chrome に接続する

devcontainer 内からは Windows ホストの IP を指定する。デフォルトゲートウェイが Windows ホストの IP になる。

```bash
# devcontainer 内で実行
WIN_HOST=$(ip route show default | awk '{print $3}')
echo "$WIN_HOST"
```

```ts
const browser = await chromium.connectOverCDP(
  `http://${process.env.WIN_HOST}:9223`,
);
```

- devcontainer は `network_mode: service:postgresql` で postgresql コンテナとネットワークネームスペースを共有しているが、デフォルトゲートウェイ（Windows ホスト）への到達には影響しない。
- 接続先の `localhost` は devcontainer 自身を指すため、Windows ホストへは必ず `WIN_HOST` の IP を使う。
- 接続先ポートはプロキシの待受ポート **9223** を指定する（9222 は Chrome が使用中）。

### 4-5. テストを実行する

```bash
# devcontainer → Windows の Chrome
CDP_ENDPOINT=http://<WIN_HOST>:9223 npx playwright test --config playwright.connect.config.ts
```

- `<WIN_HOST>` は 4-4 で取得した Windows ホストの IP に置き換える。
- devcontainer の `forwardPorts` に 9223 を追加する必要はない（コンテナから外へ出る接続であり、ホストからコンテナへ入る接続ではないため）。

---

## 5. devcontainer 内で実行し、devcontainer 内で起動中の Chrome に接続する

devcontainer 内に Chrome（Linux 版）をインストールし、ヘッドレスで起動した Chrome に接続する。
GUI が無いため必ず `--headless=new` を付ける。手順は「3. WSL で実行し、WSL で起動中の Chrome に接続する」とほぼ同じ。

### 5-1. devcontainer に Chrome をインストールする

```bash
wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | sudo gpg --dearmor -o /usr/share/keyrings/google-chrome.gpg
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-chrome.gpg] http://dl.google.com/linux/chrome/deb/ stable main" | sudo tee /etc/apt/sources.list.d/google-chrome.list
sudo apt-get update
sudo apt-get install -y google-chrome-stable
```

### 5-2. Chrome をデバッグポート付きで起動する

```bash
google-chrome \
  --remote-debugging-port=9222 \
  --user-data-dir=/tmp/chrome-debug-profile \
  --no-sandbox \
  --headless=new &
```

- devcontainer は `vscode` ユーザーで動くが、コンテナ環境では `--no-sandbox` が必要になる場合がある。
- ヘッドレス環境のため `--headless=new` を必ず付ける。

### 5-3. Playwright から接続する

```ts
const browser = await chromium.connectOverCDP("http://localhost:9222");
```

- devcontainer 内の `localhost` は devcontainer 自身を指すため、そのまま接続できる。

### 5-4. 表示を Windows 相当に揃える依存関係をインストールする

「3-4」と同じ。スナップショット比較を安定させるため、フォントや描画ライブラリを導入する。

```bash
npx playwright install --with-deps
sudo apt-get install -y fonts-noto-cjk fonts-ipafont-gothic fonts-ipafont-mincho fonts-vlgothic fonts-takao fonts-liberation fonts-dejavu fontconfig
fc-cache -fv
```

---

## 6. 2FA が必要な既存サイトへのログインを自動化する

ログイン認証が必要なサイトの自動化では、**2FA（Google Authenticator の TOTP やメールに届くコード）の入力が最大の障壁**になる。
既存サイトではシークレットやメールボックスを自由に扱えないことが多いため、以下の方針が現実的。

### 6-1. 方針

| 方式                        | 内容                                           | 向き                             |
| --------------------------- | ---------------------------------------------- | -------------------------------- |
| ① MFA を無効にする          | テスト用アカウントで 2FA を外す                | テスト専用アカウントを作れる場合 |
| ② otplib で TOTP を生成     | 共有シークレットからコードを自動生成           | シークレットを取得できる場合     |
| ③ Mailpit でメール受信      | テスト用 SMTP でコードを受信                   | メール確認を自動化する場合       |
| ④ 手入力 + セッション再利用 | 2FA だけ人間が入力し、以後はセッションを再利用 | **既存サイトで最も現実的**       |

既存サイトでは ②③ が使えないことが多いため、**④（手入力 + セッション再利用）**を基本とする。

### 6-2. 手入力 + セッション再利用（推奨）

`waitForURL`（手入力完了の自動検知）と `storageState`（セッション保存・再利用）を組み合わせる。

```ts
import { test, expect, chromium, type BrowserContext } from "@playwright/test";
import { existsSync } from "node:fs";

const AUTH_FILE = process.env.AUTH_FILE ?? "auth.json";
const LOGIN_URL = process.env.LOGIN_URL ?? "https://example.com/login";
const LOGIN_AFTER = process.env.LOGIN_AFTER ?? "**/dashboard";

async function launchAndConnect(): Promise<BrowserContext> {
  const endpoint = process.env.CDP_ENDPOINT ?? "http://localhost:9222";
  try {
    const browser = await chromium.connectOverCDP(endpoint);
    return browser.contexts()[0];
  } catch {
    const userDataDir = `${process.env.TEMP ?? "/tmp"}\\chrome-debug-profile`;
    return await chromium.launchPersistentContext(userDataDir, {
      channel: "chrome",
      headless: false,
      args: ["--remote-debugging-port=9222"],
    });
  }
}

test("2FA を手入力で突破し、セッションを保存する", async () => {
  test.skip(existsSync(AUTH_FILE), "auth.json が存在するためログイン不要");

  const context = await launchAndConnect();
  const page = context.pages()[0] ?? (await context.newPage());

  // パスワードまでは自動入力
  await page.goto(LOGIN_URL);
  await page.getByLabel("ユーザー名").fill(process.env.USERNAME!);
  await page.getByLabel("パスワード").fill(process.env.PASSWORD!);
  await page.getByRole("button", { name: "ログイン" }).click();

  // 2FA は人間が手入力。ログイン成功後の URL に遷移するまで待つ
  await page.waitForURL(LOGIN_AFTER, { timeout: 120_000 });

  // ログイン済みセッションを保存（以後のテストで再利用）
  await context.storageState({ path: AUTH_FILE });
});

test("保存したセッションでログイン済み状態を確認する", async () => {
  test.skip(
    !existsSync(AUTH_FILE),
    "auth.json が存在しないため先にログインが必要",
  );

  const context = await launchAndConnect();
  await context.storageState({ path: AUTH_FILE });

  const page = context.pages()[0] ?? (await context.newPage());
  await page.goto(LOGIN_AFTER.replace("**", "https://example.com"));

  await expect(page).toHaveTitle(/ダッシュボード/);
});
```

- 初回は `waitForURL` で「人間が 2FA を入力してログイン完了するまで」待ち、完了したら `auth.json` に保存する。
- 2回目以降は `auth.json` を再利用するため、2FA を毎回通す必要がない。
- セッションが切れたら `auth.json` を削除して再生成する。

実行方法:

```bash
# 初回（2FA を手入力してセッション保存）
USERNAME=xxx PASSWORD=yyy npx playwright test --config playwright.login.config.ts

# 2回目以降（auth.json を再利用、2FA 不要）
npx playwright test --config playwright.login.config.ts
```

- セレクタ（`getByLabel("ユーザー名")` 等）は対象サイトに合わせて調整する。
- 専用設定 `playwright.login.config.ts`（単一プロジェクト・並列なし）で実行する。

### 6-3. otplib で TOTP を自動生成する（Keycloak の場合）

TOTP の共有シークレットを取得できる場合は、`otplib` でコードを自動生成できる。

```bash
cd e2e
npm install otplib
```

```ts
import { authenticator } from "otplib";

// Keycloak の既定ポリシー（HmacSHA1 / 6桁 / 30秒）に合わせる
authenticator.options = { digits: 6, step: 30, algorithm: "SHA1" };

const code = authenticator.generate(process.env.TOTP_SECRET!);
await page.getByLabel("認証コード").fill(code);
```

- `TOTP_SECRET` は環境変数で渡す（リポジトリにコミットしない）。
- Keycloak の `otpPolicyAlgorithm` が `HmacSHA1` なので `algorithm: 'SHA1'` を明示する。

### 6-4. Mailpit でメール確認を自動化する

Mailpit は devcontainer の compose で起動済み（SMTP `1025`、API/UI `8025`）。Keycloak の SMTP も `mailpit:1025` に向いている。

```ts
// Mailpit API v1 で最新メッセージを取得
const res = await fetch("http://localhost:8025/api/v1/messages");
const data = await res.json();
const latest = data.messages[0];

// 本文から 6 桁コードを抽出
const code = latest.Snippet.match(/\d{6}/)?.[0];
await page.getByLabel("確認コード").fill(code!);
```

- Mailpit の API は `http://localhost:8025/api/v1/messages`（一覧）、`/api/v1/message/{ID}`（詳細）。
- 本文全文が欲しい場合は詳細 API の `Text` フィールドを使う。
- コードの桁数・形式は Keycloak のメールテンプレートに合わせて正規表現を調整する。

---

## 7. サイト内部の操作をコード化する（AI との協業）

ログイン後の画面操作を Playwright コードに落とす際、**AI と協力して画面を解析しながら進める**のが効率的。
ここでは、デバッグポート接続 + `ariaSnapshot()` を使った協業ワークフローを説明する。

### 7-1. 画面解析用スクリプト（`aria-snapshot.ts`）

デバッグポートで起動中の Chrome に接続し、現在開いているタブのアクセシビリティツリーを取得する。

```ts
import { chromium } from "@playwright/test";

async function main() {
  const endpoint = process.env.CDP_ENDPOINT ?? "http://localhost:9222";
  const url = process.argv[2];

  const browser = await chromium.connectOverCDP(endpoint);
  const context = browser.contexts()[0];
  const page = context.pages()[0] ?? (await context.newPage());

  if (url) {
    await page.goto(url);
  }

  const snapshot = await page.locator("body").ariaSnapshot();
  console.log(snapshot);
}

main();
```

使い方:

```bash
npx tsx aria-snapshot.ts            # 現在のタブを解析
npx tsx aria-snapshot.ts <URL>      # 指定 URL を開いて解析
```

- `ariaSnapshot()` は Playwright 1.49+ の機能。
- 出力は role / name / 階層のみの YAML で、HTML 全体より大幅に小さい。`getByRole` ベースのコード生成に最適。
- 生の CDP `Accessibility.getFullAXTree` は `none` / `generic` / `StaticText` などのノイズが多く、`ariaSnapshot()` の方がトークン効率が良い。

### 7-2. 協業ワークフロー（1ページごと）

画面遷移のたびに DOM とロケータが変わるため、**1ページごとに解析とコード化を繰り返す**のが確実。

```
1. あなた: Chrome で対象サイトを操作して目的の画面に到達
2. あなた: 「この画面をコード化して」と AI に指示
3. AI: aria-snapshot.ts を実行して画面構造を取得
4. AI: 解析して getByRole ベースの操作コードを生成
5. あなた: 生成コードを確認・実行 → 次の画面に遷移
6. 2 に戻って繰り返し
```

- 動的要素（モーダル・非同期読み込み・SPA の部分更新）は、遷移直後の状態を確認しないと誤るため、1ページごとの確認が安全。
- 書き込み系の操作（データ登録・削除）は、AI がブラウザを操作して遷移させると対象サイトの状態が変わるため、特に注意する。

### 7-3. 効率化（連続実行）

遷移が単純で予測可能な場合（読み取り系など）は、AI に連続実行を任せて一気にコード化できる。

```
あなた: 「このフロー全体をコード化して」
AI: 画面A解析 → クリック → 画面B解析 → クリック → … と連続で進めてコード生成
```

| 状況                           | 推奨                            |
| ------------------------------ | ------------------------------- |
| 複雑・動的な画面、書き込み操作 | 1ページごとにやり取り（確実）   |
| 単純・読み取り系の遷移         | AI に連続実行を任せる（効率的） |

- まずは 1ページごとのやり取りで進め、慣れたら連続実行に移行するのが良い。

### 7-4. ロケータの品質

コード化の成否は「どのロケータを使うか」で決まる。優先順位は以下の通り。

1. `getByRole`（アクセシビリティ属性ベース、最も安定）
2. `getByLabel` / `getByText` / `getByPlaceholder`
3. `data-testid` 属性（可能ならサイト側に追加）
4. CSS / XPath（最終手段、壊れやすい）

`ariaSnapshot()` の出力は `getByRole` で使える role 名・アクセシブル名がそのまま見えるため、安定したロケータを選びやすい。

---

## 8. Web アプリのテストで何を検証すべきか

検証はレイヤーで整理すると漏れがなくなる。**Playwright（E2E）で行うべき範囲と、専用ツールに任せる範囲**を明確にする。

### 8-1. 検証レイヤーと担当

| レイヤー | 内容 | 担当 |
| ------- | ---- | ---- |
| 機能の正しさ | 操作結果が期待通りか | **Playwright（E2E）** |
| 表示の正しさ（VRT） | 見た目が崩れていないか | **Playwright（E2E）** |
| アクセシビリティ | ラベル・コントラスト・キーボード操作 | Playwright + axe-core（任意） |
| パフォーマンス | 読み込み時間・応答時間 | **専用ツール（Lighthouse 等）** |
| セキュリティ | XSS・CSRF・認可バイパス | **専用ツール（OWASP ZAP 等）** |

- **Playwright で行うのは「機能の正しさ」と「表示の正しさ（VRT）」**。この2つが費用対効果が最も高い。
- **パフォーマンスとセキュリティは Playwright では行わない**。E2E では不安定・不十分なため、専用ツールに任せる。

### 8-2. 機能の正しさ（最重要・必須）

ユーザー操作の結果が期待通りかを検証する。E2E の中心。

| 検証項目 | 例 |
| ------- | ---- |
| 画面遷移 | クリック後に正しい URL / 画面に遷移する |
| 入力 → 登録 → 反映 | フォーム送信後、一覧・詳細に反映される |
| 更新・削除 | 編集が反映、削除で消える |
| バリデーション | 不正入力でエラー表示、正しい入力で通る |
| 認可 | 権限のない操作が拒否される |

```ts
await page.getByRole('button', { name: '登録' }).click();
await expect(page.getByText('登録しました')).toBeVisible();
```

- **入力値の状態**はここに含まれる。正常系・異常系・境界値（空、最大長、特殊文字）を網羅する。

### 8-3. 表示の正しさ（VRT / スナップショット）

見た目が崩れていないかを検証する。このリポジトリは `toHaveScreenshot` を採用済み。

**コンポーネント単位で撮影する**のが推奨。画面全体ではなく、特定の要素だけを切り出す。

```ts
// 画面全体ではなく、特定のコンポーネントだけを撮影
await expect(page.getByRole('navigation')).toHaveScreenshot('navbar.png');
await expect(page.getByRole('form')).toHaveScreenshot('form.png');
await expect(page.getByRole('table')).toHaveScreenshot('table.png');
```

- `toHaveScreenshot` は `page` だけでなく `locator` にも使える。対象要素の矩形だけが切り出される。
- 周囲の変化（広告・日付・動的コンテンツ）に影響されないため、画面全体より壊れにくい。

| 観点 | 画面全体 | コンポーネント単位 |
| ---- | -------- | ----------------- |
| 壊れやすさ | 高（どこか変わると全滅） | 低（対象だけ） |
| 保守コスト | 高 | 低 |
| 差分の特定 | どこが変わったか分かりにくい | どの部品が変わったか明確 |

- **頻繁に変わる部分（広告・日付・ユーザー名）は撮らない**。安定した部品（ナビ・フォーム・テーブル・ボタン）を対象にする。
- 動的要素は `mask` オプションで隠すか、`maxDiffPixelRatio` で許容を調整する。

```ts
await expect(page.getByRole('table')).toHaveScreenshot('table.png', {
  mask: [page.getByText(/2026/)] // 日付など動的な部分をマスク
});
```

### 8-4. アクセシビリティ（任意）

`getByRole` で操作できること自体がアクセシビリティの担保になるが、明示的な検証も可能。

```ts
import AxeBuilder from '@axe-core/playwright';
const results = await new AxeBuilder({ page }).analyze();
expect(results.violations).toEqual([]);
```

- ラベル欠落、コントラスト不足、キーボード操作不能などを検出する。

### 8-5. パフォーマンス・セキュリティ（Playwright では行わない）

- **パフォーマンス**: ページ読み込み時間・API 応答時間の計測は、E2E では不安定になりがち。**Lighthouse 等の専用ツール**に任せる。
- **セキュリティ**: XSS・CSRF・認可バイパスは、E2E より **OWASP ZAP 等のセキュリティテスト専用ツール**が適切。

---

## 補足

- 接続先 Chrome のバージョンと Playwright のバージョンが大きく離れていると、CDP の互換性で失敗することがある。その場合は Playwright を更新するか、Chrome のバージョンを合わせる。
- スナップショット比較は **ブラウザ・OS ごとに描画が微妙に異なる**ため、CI とローカルで OS を揃えるか、`--update-snapshots` で環境ごとに更新する運用を推奨する。
- 既存の実行スクリプト（`execute-test.sh` / `headed-test.sh` / `codegen.sh` など）は Playwright 管理ブラウザを使う。起動中 Chrome への接続は上記の `connectOverCDP` を spec 内に書く形になる。
