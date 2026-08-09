# フロントエンド

本リポジトリには複数のフロントエンド実装が含まれています。

## NuxtSample

Nuxt 4 + Vue 3 + TypeScript による SPA フロントエンド。

### 技術スタック

| 技術 | 用途 |
| ---- | ---- |
| Nuxt 4 | フレームワーク |
| Vue 3 | UI フレームワーク |
| TypeScript | 型安全 |
| Pinia | 状態管理 |
| VeeValidate | フォームバリデーション |
| Nuxt UI | UI コンポーネントライブラリ |
| Tailwind CSS | スタイリング |
| keycloak-js | Keycloak 認証 |
| Tabulator | テーブル表示 |

### 起動

```bash
cd src/NuxtSample
pnpm install
pnpm run dev
# http://localhost:3000
```

### ディレクトリ構成

```
app/
├── app.vue                 # ルートコンポーネント
├── components/             # 共通コンポーネント
├── layouts/default.vue     # デフォルトレイアウト
├── middleware/              # ルートガード
│   ├── auth.ts             # Pinia ベース認証チェック
│   ├── authRole.ts         # ロールベース認可
│   ├── keycloakAuth.ts     # Keycloak JS 認証チェック
│   └── keycloakRole.ts     # Keycloak ロールチェック
├── pages/                  # 22 ページ
├── plugins/fontawesome.ts  # FontAwesome プラグイン
└── store/                  # Pinia ストア
    ├── authStore.ts        # JWT ベース認証
    ├── keycloakAuthStore.ts # Keycloak JS 統合
    ├── counter.ts          # カウンターデモ
    └── fruitStore.ts       # フルーツデモ
```

## TypeScript プロジェクト

`src/typescript/` 配下に複数の TypeScript プロジェクトがあります。

### lit-component

Vite + Lit による Web Components。

```bash
cd src/typescript/lit-component
npm install
npm run dev
```

### vue-webcomponent

Vue 3 Web Components + Histoire（Storybook）。

```bash
cd src/typescript/vue-webcomponent
npm install
npm run story:dev
```

### openapi-typescript

OpenAPI 仕様から TypeScript 型を生成します。

```bash
cd src/typescript/openapi-typescript
bash create-ts.sh
```

### openapi2aspida

Aspida を使用した型安全 API クライアント生成。

```bash
cd src/typescript/openapi2aspida
bash create-ts.sh
```

### quicktype

JSON から TypeScript 型を生成します。

```bash
cd src/typescript/quicktype
bash create-ts.sh
```

## MVC 内のフロントエンド

MVC アプリケーション内でも Vite を使用してフロントエンド資産をビルドします。

```bash
cd src/AspNetCoreSample.Mvc
npm ci
npm run publish  # Vite ビルド → wwwroot/ に出力
```
