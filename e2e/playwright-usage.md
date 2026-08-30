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
import { test, expect, chromium } from '@playwright/test';

test('起動中の Chrome に接続する', async () => {
  const browser = await chromium.connectOverCDP('http://localhost:9222');
  const context = browser.contexts()[0];
  const page = context.pages()[0] ?? await context.newPage();

  await page.goto('https://localhost:7079/');
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
const browser = await chromium.connectOverCDP('http://localhost:9222');
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
const browser = await chromium.connectOverCDP('http://localhost:9222');
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
const httpProxy = require('http-proxy');
const proxy = httpProxy.createProxyServer({
  target: 'http://localhost:9222',
  ws: true,
  changeOrigin: true,
  selfHandleResponse: true,
});

proxy.on('error', (err, req, res) => {
  console.error('proxy error:', err.message);
  if (res && !res.headersSent) res.writeHead(502);
  if (res) res.end('proxy error: ' + err.message);
});

// Host ヘッダーを localhost に書き換える（Chrome 152 の DNS リバインディング対策を回避）
proxy.on('proxyReq', (proxyReq, req) => {
  proxyReq.setHeader('Host', 'localhost:9222');
});

// レスポンス内の webSocketDebuggerUrl をプロキシのアドレスに書き換える
proxy.on('proxyRes', (proxyRes, req, res) => {
  const chunks = [];
  proxyRes.on('data', (chunk) => chunks.push(chunk));
  proxyRes.on('end', () => {
    let body = Buffer.concat(chunks).toString('utf8');
    const host = req.headers.host; // 例: 172.17.160.1:9223
    body = body.replace(/ws:\/\/localhost:9222/g, `ws://${host}`);

    // Content-Length を書き換え後のボディ長に合わせて再計算する
    const headers = { ...proxyRes.headers };
    headers['content-length'] = Buffer.byteLength(body);

    res.writeHead(proxyRes.statusCode, headers);
    res.write(body);
    res.end();
  });
});

// 待受ポートは 9223 にする（9222 は Chrome が使用中のため衝突する）
proxy.listen(9223, '0.0.0.0');
console.log('Chrome proxy listening on 0.0.0.0:9223');
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
const browser = await chromium.connectOverCDP(`http://${process.env.WIN_HOST}:9223`);
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
const browser = await chromium.connectOverCDP('http://localhost:9222');
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

## 補足

- 接続先 Chrome のバージョンと Playwright のバージョンが大きく離れていると、CDP の互換性で失敗することがある。その場合は Playwright を更新するか、Chrome のバージョンを合わせる。
- スナップショット比較は **ブラウザ・OS ごとに描画が微妙に異なる**ため、CI とローカルで OS を揃えるか、`--update-snapshots` で環境ごとに更新する運用を推奨する。
- 既存の実行スクリプト（`execute-test.sh` / `headed-test.sh` / `codegen.sh` など）は Playwright 管理ブラウザを使う。起動中 Chrome への接続は上記の `connectOverCDP` を spec 内に書く形になる。
