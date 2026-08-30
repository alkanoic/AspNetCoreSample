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

### 2-2. Windows 側でポート転送とファイアウォールを設定する

Chrome 152 以降は `--remote-debugging-address=0.0.0.0` が廃止され、デバッグポートは**常に `127.0.0.1` にバインド**される。そのため WSL2 から直接は届かない。以下の設定を Windows 側で行う。

```powershell
# 管理者 PowerShell で実行
# ポート転送（127.0.0.1:9222 → 0.0.0.0:9222）
netsh interface portproxy add v4tov4 listenport=9222 listenaddress=0.0.0.0 connectport=9222 connectaddress=127.0.0.1

# ファイアウォールの受信許可（WSL からの接続を許可）
New-NetFirewallRule -DisplayName "ChromeDebug9222" `
  -Direction Inbound -LocalPort 9222 -Protocol TCP -Action Allow
```

- 設定確認: `netsh interface portproxy show all`

#### 設定を削除する場合

```powershell
# 管理者 PowerShell で実行
# ポート転送の削除
netsh interface portproxy delete v4tov4 listenport=9222 listenaddress=0.0.0.0

# ファイアウォール規則の削除
Remove-NetFirewallRule -DisplayName "ChromeDebug9222"
```

### 2-3. WSL から Windows の Chrome に接続する

WSL からは Windows の `localhost` に直接届かないため、**Windows ホストの IP アドレス**を指定する。

```bash
# WSL から見た Windows ホストの IP を取得
WIN_HOST=$(ip route show default | awk '{print $3}')
echo "$WIN_HOST"
```

```ts
const browser = await chromium.connectOverCDP(`http://${process.env.WIN_HOST}:9222`);
```

- WSL2 では `ip route show default` のゲートウェイが Windows ホストの IP になる。
- 上記の portproxy とファイアウォール設定が済んでいれば、`http://<WIN_HOST>:9222` で接続できる。

### 2-4. テストを実行する

```bash
# WSL → Windows の Chrome
CDP_ENDPOINT=http://<WIN_HOST>:9222 npx playwright test --config playwright.connect.config.ts
```

- `<WIN_HOST>` は 2-3 で取得した Windows ホストの IP に置き換える。
- 接続テストは単一の Chrome インスタンスに接続するため、専用設定 `playwright.connect.config.ts`（単一プロジェクト・並列なし）で実行する。

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

## 補足

- 接続先 Chrome のバージョンと Playwright のバージョンが大きく離れていると、CDP の互換性で失敗することがある。その場合は Playwright を更新するか、Chrome のバージョンを合わせる。
- スナップショット比較は **ブラウザ・OS ごとに描画が微妙に異なる**ため、CI とローカルで OS を揃えるか、`--update-snapshots` で環境ごとに更新する運用を推奨する。
- 既存の実行スクリプト（`execute-test.sh` / `headed-test.sh` / `codegen.sh` など）は Playwright 管理ブラウザを使う。起動中 Chrome への接続は上記の `connectOverCDP` を spec 内に書く形になる。
