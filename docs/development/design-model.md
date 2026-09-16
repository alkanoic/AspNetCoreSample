# 設計項目モデル

設計書の**項目（エンティティ/イベント）を Single Source of Truth** として YAML で定義し、そこからイベント定義と一覧を自動生成し、項目間の整合性を双方向で検証する仕組みです。mkdocs 本体には依存しません。ER図は A5SQL Mk-2 で管理します。

## 目的

- 項目間の関係をグラフとして扱う
- 項目定義からイベント定義を自動生成して省力化する
- 生成物を再現可能にする
- 項目間の参照を双方向で検証する

## 構成

| 層 | 役割 | 実体 |
| -- | ---- | ---- |
| データ層 | 項目の定義 (SSOT) | `model/entities/*.yml` / `model/events/*.yml` / `model/services/*.yml` |
| 画面層 | MVC の画面・項目・遷移・イベント | `model/screens.yml` |
| 方式層 | 全体にかかわる内部設計（AP処理方式） | `model/app_policy/*.yml` |
| スキーマ層 | 形式の定義 | `schemas/entity.schema.json` / `schemas/event.schema.json` / `schemas/service.schema.json` / `schemas/app_policy.schema.json` |
| 生成層 | グラフ・イベント定義を生成 | `tools/modelgen` → `generated/` |
| 検証層 | 双方向の整合性チェック | `tools/modelgen check` |

## 項目の定義

エンティティは `model/entities/` に 1 項目 1 ファイルで定義します。`fields` が項目定義、`relations` が項目間の関係、`auto_events` が自動生成する状態変化イベントです。

```yaml
kind: entity
id: parent_table
title: 親テーブル
table: parent_table
key: [id]
auto_events: [created, updated, deleted]
fields:
  - { name: id, column: id, type: integer, key: true, required: true }
relations:
  - { id: children, type: has-many, target: child_table, inverse: parent, foreign_key: parent_id }
```

意図イベントは `model/events/` に定義し、`source` に発生元エンティティの id を書きます。

```yaml
kind: event
id: role_policy.granted
source: role_policy
trigger: command.grant_role_policy
data_fields: [role_name, policy_name]
producers: [security_api]
consumers: [audit_sink]
```

サービスは `model/services/` に定義し、`produces` / `consumes` でイベントを参照します。イベント側の `producers` / `consumers` を書いた場合はサービス側と双方向に一致する必要があります。

```yaml
kind: service
id: audit_sink
title: 監査シンク
consumes: [policy.created, policy.deleted, role_policy.granted]
```

## 生成されるもの

`model/` から以下を `generated/` に生成します。手で編集・コミットしてはいけません（gitignore 対象、CI とデプロイ時に再生成します）。

| 生成物 | 内容 |
| ------ | ---- |
| `generated/events/*.json` | イベント payload の JSON Schema (CloudEvents 風の封筒) |
| `generated/asyncapi.yaml` | イベント契約の AsyncAPI ドキュメント |
| `generated/model-graph.md` | エンティティ一覧とER図の表示用生成（正本は `er.a5er`） |

MVC 画面については `generated/mvc/` に共通の画面設計書と画面別設計書を生成します。ドメイン定義書は自動生成せず、`docs/development/mvc-domain.md` に一覧を手書きします。

`generated/model-graph.md` は `docs/development/model-graph.md` から include され、サイトにも表示されます。

## 逆抽出 (C# との乖離検出)

`source-drift` は `src/AspNetCoreSample.DataModel/Models/SampleContext.cs` を解析し、エンティティ・カラム・主キーが `model/` と一致するか検証します。C# 側だけ変更して `model/` を更新し忘れると検出されます。

## 双方向チェック

| チェック | 内容 |
| -------- | ---- |
| `schema` | 項目 YAML が JSON Schema に適合するか |
| `id-unique` | エンティティ/サービス/イベントの id が衝突していないか |
| `relation-inverse` | 関係の逆関係が相手側にあり、相互に整合しているか |
| `event-link` | `source` の存在、`data_fields` の解決、`emits` と `source` の相互参照 |
| `service-link` | `produces`/`consumes` の参照、全イベントに producer があるか、イベント側 `producers`/`consumers` とサービス側の双方向一致 |
| `screen-link` | 遷移先の存在、引き継ぎ項目の送受信一致、DB更新の解決、方式参照の解決 |
| `source-drift` | C# の `SampleContext.cs` と `model/` の逆抽出ドリフト (カラム・主キーの不一致) |
| `drift` | `generated/` が `model/` と同期しているか |

## 実行

```bash
# 依存の導入 (初回のみ)
pip install -r tools/modelgen/requirements.txt

# 生成 (generated/ と docs/development/mvc/ を更新)
tools/modelgen/run.sh generate

# 検証 (error があれば終了コード 1)
tools/modelgen/run.sh check --strict
```

チェッカー自体のテストは標準の unittest で実行できます。

```bash
PYTHONPATH=tools/modelgen python -m unittest discover -s tools/modelgen/tests
```

## CI と pre-commit

- `main.yml` の `lint` ジョブが全 push で `--strict` 実行する。
- `.githooks/pre-commit` は `model/` / `schemas/` / `tools/modelgen/` / `generated/` の変更時に実行する。
- `model/` を変更したら `generate` して `generated/` を更新する。忘れると `drift` が検出する。
- `mkdocs serve` / `mkdocs build` の開始時に `tools/mkdocs_hooks.py` が自動で `generate` する。デバッグ実行前の手動生成は不要。

## 拡張

- 項目タイプを増やす場合は `schemas/` にスキーマを追加し、`tools/modelgen/modelgen/` にローダとチェックを足す。
- イベントの生成規則は `tools/modelgen/modelgen/generators/events.py` が担う。
