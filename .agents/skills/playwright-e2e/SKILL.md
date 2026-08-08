---
name: playwright-e2e
description: Use when working with the Node.js Playwright E2E suite in e2e/ (npx playwright test, spec files, snapshots, Allure, Prisma, BASE_URL). Trigger keywords: "e2e", "Playwright", "spec", "スクリーンショット更新", "Allure", "codegen", "headed", "develop/production config".
---

# E2E（`e2e/` Node 版 Playwright）

`e2e/` は Node（TypeScript）の Playwright テスト。開発環境（develop）と本番（production）の 2 構成に分かれている。

## セットアップ

```bash
cd e2e
npm ci
bash install-deps.sh        # npx playwright install --with-deps chromium chrome msedge webkit firefox
```

- パッケージ: `@playwright/test`、`allure-commandline`、`allure-playwright`、Prisma（`@prisma/client`）。
- `tsconfig.json` 参照。

## 設定ファイル

| ファイル | 用途 |
| -------- | ---- |
| `playwright.config.ts` | デフォルト（develop/production 共通の設定。5 ブラウザ + Edge/Chrome。locale `ja-JP`、`timezoneId Asia/Tokyo`、`ignoreHTTPSErrors`） |
| `playwright.develop.config.ts` | develop 用 |
| `playwright.production.config.ts` | production 用 |

- `BASE_URL` 環境変数で対象を切替（既定は `https://localhost:7079/`）。

## 実行スクリプト（e2e/ 内）

| スクリプト | 内容 |
| ---------- | ---- |
| `execute-test.sh` | `before-e2e-test.sh` → production + develop 全テスト → `after-e2e-test.sh` |
| `before-e2e-test.sh` | コンテナ再起動（`docker compose down -v; up -d`）+ Mvc 起動（`dotnet run --project ../src/AspNetCoreSample.Mvc`）+ mysql healthy 待ち |
| `after-e2e-test.sh` | 起動した Mvc のプロセス Kill |
| `ui-test.sh` | `--ui` モード実行 |
| `headed-test.sh` | `--headed name-db.spec.ts` |
| `codegen.sh` | `npx playwright codegen` |
| `codegen-mobile.sh` | iPhone 12 デバイス指定の codegen |
| `update-screenshot.sh` | `--update-snapshots` でスナップショットを更新 |
| `install-deps.sh` | ブラウザ install |

```bash
cd e2e
bash execute-test.sh
bash update-screenshot.sh
```

## テスト配置規約

- テストは `tests/develop/` と `tests/production/` に分かれる（例: `tests/develop/name-db.spec.ts`、`tests/production/link.spec.ts`）。
- スクリーンショットスナップショットは `*.spec.ts-snapshots/*.png` にコミット保存。**ブラウザごと（chromium/firefox/webkit/Edge/Chrome/Mobile）に画像がある**ため、UI 変更時は develop/production 双方の `--update-snapshots` が必要。

## Allure / レポート

- 実行後 `allure-results/` に保存、`allure-create.sh` で `allure-report/` を生成。
- HTML レポートは `playwright-report*/` にも出力。

## Prisma

- `prisma/schema.prisma` を保持。DB 更新後は `prisma-pull-db.sh`（`prisma db pull && prisma generate`）を実行して型を同期。

## 注意

- `dotnet run` を使う E2E は「開発用自己署名証明書」のため `ignoreHTTPSErrors` を設定済み。
- **テスト対象の DB コンテナは devcontainer の compose を再起動（-v）する**ため、既存データはリセットされる。