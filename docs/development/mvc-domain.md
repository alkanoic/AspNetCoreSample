---
title: ドメイン定義書
---

# ドメイン定義書

<!-- er.a5er から 2026-09-16 に一回限りで生成。以降は手動メンテナンス。 -->

同一の列名は1行にまとめています。

| 名前 | 英語名 | 説明 | 型 | システム許容文字 | 備考 |
| ---- | ------ | ---- | -- | -------------- | ---- |
| マルチStringId | `charid` |  | varchar(10) | 全角・半角文字（10文字以内） | 主キー、必須 |
| 子テーブルbit型 | `child_bit` |  | boolean | 真偽値 |  |
| 子テーブルDate型 | `child_date` |  | date | 日付（YYYY-MM-DD） |  |
| 子テーブルdeimal型 | `child_decimal` |  | decimal(10,2) | 半角数字（小数可） |  |
| 子テーブルint型 | `child_int` |  | int | 半角数字（整数） |  |
| 子テーブル要素名 | `child_name` |  | varchar(30) | 全角・半角文字（30文字以内） | 必須 |
| 作成日時 | `create_at` |  | timestamp | 日時（YYYY-MM-DD HH:MM:SS） | 必須 |
| 作成ユーザー | `create_user` |  | varchar(20) | 全角・半角文字（20文字以内） | 必須 |
| Enum列 | `enum_column` |  | int | 半角数字（整数） | 必須 |
| マルチID / 子テーブルId / 親テーブルid | `id` |  | int / serial | 半角数字（整数） | 主キー、必須、自動採番（child_table, enum_sample, name） |
| 名前列 | `name` |  | text | 全角・半角文字 | 必須 |
| 親テーブルid | `parent_id` |  | int | 半角数字（整数） | 必須 |
| ポリシー名 | `policy_name` |  | varchar(255) | 全角・半角文字（255文字以内） | 主キー、必須 |
| ロール名 | `role_name` |  | varchar(255) | 全角・半角文字（255文字以内） | 主キー、必須 |
| Bit型 / 親テーブルbit型 | `target_bit` |  | boolean | 真偽値 |  |
| 日付型 / 親テーブルdate型 | `target_date` |  | date | 日付（YYYY-MM-DD） |  |
| decimal型 / 親テーブルdecimal型 | `target_decimal` |  | decimal(10,2) | 半角数字（小数可） |  |
| int型 / 親テーブルint型 | `target_int` |  | int | 半角数字（整数） |  |
| 名前 / 親テーブル要素名 | `target_name` |  | varchar(30) | 全角・半角文字（30文字以内） | 必須 |
| 更新日時 | `update_at` |  | timestamp | 日時（YYYY-MM-DD HH:MM:SS） | 必須 |
| 更新ユーザー | `update_user` |  | varchar(20) | 全角・半角文字（20文字以内） | 必須 |
