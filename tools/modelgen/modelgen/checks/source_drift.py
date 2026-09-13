"""C# の DbContext と項目モデルの乖離 (逆抽出ドリフト) を検出する。"""

from __future__ import annotations

from ..reverse import parse_sample_context
from .base import Context, Issue

NAME = "source-drift"
DEFAULT_SOURCE = "src/AspNetCoreSample.DataModel/Models/SampleContext.cs"


def _compare(ctx: Context, parsed: dict) -> list[Issue]:
    issues: list[Issue] = []
    model_by_table = {
        entity.meta.get("table"): entity
        for entity in ctx.model.entities
        if entity.meta.get("table")
    }

    for class_name, info in sorted(parsed.items()):
        table = info["table"]
        entity = model_by_table.get(table) if table else None
        if entity is None:
            issues.append(
                Issue(
                    NAME,
                    "error",
                    f"C# エンティティ {class_name} (table {table}) が model にありません",
                    DEFAULT_SOURCE,
                )
            )
            continue

        model_columns = {
            f.get("column", f["name"]) for f in entity.fields if "name" in f
        }
        cs_columns = info["columns"]
        for column in sorted(cs_columns - model_columns):
            issues.append(
                Issue(NAME, "error", f"{table}: C# カラム '{column}' が model にありません", entity.path)
            )
        for column in sorted(model_columns - cs_columns):
            issues.append(
                Issue(NAME, "error", f"{table}: model のカラム '{column}' が C# にありません", entity.path)
            )

        model_key_columns = {
            next(
                (f.get("column", f["name"]) for f in entity.fields if f.get("name") == key),
                key,
            )
            for key in entity.key
        }
        if info["key_columns"] and model_key_columns != info["key_columns"]:
            issues.append(
                Issue(
                    NAME,
                    "error",
                    f"{table}: 主キーが不一致 (C#={sorted(info['key_columns'])}, "
                    f"model={sorted(model_key_columns)})",
                    entity.path,
                )
            )

    cs_tables = {info["table"] for info in parsed.values()}
    for entity in ctx.model.entities:
        table = entity.meta.get("table")
        if table and table not in cs_tables:
            issues.append(
                Issue(
                    NAME,
                    "error",
                    f"model エンティティ {entity.id} (table {table}) が C# SampleContext にありません",
                    entity.path,
                )
            )

    return issues


def run(ctx: Context) -> list[Issue]:
    path = ctx.root / DEFAULT_SOURCE
    if not path.exists():
        return []

    parsed = parse_sample_context(path.read_text(encoding="utf-8"))
    if not parsed:
        return [Issue(NAME, "warning", f"{DEFAULT_SOURCE} からエンティティを抽出できませんでした")]
    return _compare(ctx, parsed)
