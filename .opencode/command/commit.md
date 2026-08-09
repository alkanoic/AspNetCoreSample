---
description: AGENTS.md のコミット規約（Conventional Commits / 日本語）に従って git commit を実行する
---

現在の変更を Conventional Commits 形式で日本語のコミットメッセージとしてコミットしてください。

手順:
0. まず `/review` と同じ手順で現在の変更をレビューする。`git status` / `git diff`（`git diff --staged` があれば）を確認し、バグ・意図しない変更・不要ファイルなどを洗い出して報告する。指摘事項がある場合は修正してから次へ進む。
1. `git status` と `git diff`（および `git diff --staged` があれば）を確認し、何が変更されたかを把握する。
2. 意図しないファイル（SBOM・playwright-report・CodeGen 生成物などのアーティファクト、シークレット）が含まれていないか確認する。
3. AGENTS.md の「コミット規約（Conventional Commits / 日本語）」に従い、type / scope / 要約を選定する。要約は動詞で「〜する」まで書き、英語にしない。
4. 必要なファイルだけをステージしてからコミットする。ユーザーが引数（例: メッセージ、ファイル、`--amend`）を渡した場合はその指示を優先する。
5. コミットがフック等で失敗した場合は修正して再コミットする（amend は失敗コミットには使わない）。
6. 完了したらコミットハッシュと要約を簡潔に報告する。

コミットは明示的に指示された場合のみ実行すること（AGENTS.md 方針）。git push はしない。