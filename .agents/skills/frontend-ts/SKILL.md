---
name: frontend-ts
description: Use when working with the TypeScript/Vite frontends in this repo: AspNetCoreSample.Mvc vite bundle, NuxtSample (Nuxt 4, pnpm), src/typescript (lit/vue-webcomponent/openapi-typescript/quicktype). Trigger keywords: "vite", "npm run build", "npm run publish", "tsc", "Nuxt", "lit", "vue", "quicktype", "フロントエンド", "CSS/SCSS", "pnpm".
---

# フロントエンド（Vite / Nuxt / TypeScript）

このリポジトリのフロントエンドは複数系統がある。どれを触るかでディレクトリ・コマンドが異なる。

## 1. Mvc の Vite バンドル（`src/AspNetCoreSample.Mvc`）

- `package.json` の scripts: `copy` / `dev`（vite watch）/ `build`（tsc --noEmit + vite build）/ `publish`（production build）。
- Vite エントリは `vite/js/*.ts`、SCSS は `vite/css/*.scss`、設定は `vite.config.mjs`。
- `wwwroot/lib/` は `npm run copy` で `node_modules` から静的コピー（bootstrap / signalr / jquery / html5-qrcode / jquery-validation）。

```bash
cd src/AspNetCoreSample.Mvc
npm ci
npm run build     # 開発用
npm run publish   # 本番（CI main.yml が使用）
npm run dev       # watch モード
```

- **vite は `NODE_ENV` 指定あり**（build/publish はスクリプト内で設定済み）。
- `.gitignore` で `wwwroot/lib` が無視されていることがあるため、「生成物」扱いを把握（`npm run copy` で再現）。

## 2. Nuxt 4（`src/NuxtSample`）

- Nuxt 4 + TypeScript + Nuxt UI + Pinia（`keycloakAuthStore` 等）。
- **パッケージ管理は pnpm**（`pnpm-lock.yaml`、npm の package-lock は無し）。エージェントが依存追加をする場合は pnpm で揃える。
- デプロイは GitHub Actions で SWA（Azure Static Web Apps）へ `.output/public` をアップロード。

```bash
cd src/NuxtSample
pnpm install        # pnpm-lock.yaml 準拠でインストール
pnpm run generate   # static 生成
```

- middleware（`keycloakAuth` / `keycloakRole`）は個別の認証管理サンプル。触るときは既存のストア・middleware に合わせる。

## 3. 個別 TypeScript（`src/typescript`）

| 配下 | 概要 |
| ---- | ---- |
| `lit-component/` | Lit Web Component + vite。`src/*.ts`、`shadow-webapi-component.ts` 等 |
| `vue-webcomponent/` | Vue 3 コンポーネント + histoire（story） |
| `openapi-typescript/` | OpenAPI から TS 型生成 |
| `quicktype/` | swagger.json から `webapi.ts` 生成（`create-ts.sh`） |

- 各パッケージ個別に `npm ci` / `npm run build`。
- API クライアントは WebApi の Swagger から生成（`src/typescript/quicktype/create-ts.sh`、`openapi-typescript/`、`CodeGen.Result.Kiota/typescript`）—— swagger 更新時は再生成。

## CI の フロント工程（main.yml）

- Mvc: `npm ci` → `npm run publish` → `dotnet build -c Release -f net10.0` → publish。
- Nuxt: `pnpm install`（corepack 等で pnpm を有効化）→ `pnpm run generate` → SWA デプロイ。