"""C# の DbContext と項目モデルの乖離 (逆抽出ドリフト) を検出する。"""

from __future__ import annotations

from ..reverse import parse_sample_context
from ..reverse_er import parse_er
from .base import Context, Issue

NAME = "source-drift"
DEFAULT_SOURCE = "src/AspNetCoreSample.DataModel/Models/SampleContext.cs"
MVC_VIEWS_ROOT = "src/AspNetCoreSample.Mvc/Views"


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
    issues: list[Issue] = []

    path = ctx.root / DEFAULT_SOURCE
    if path.exists():
        parsed = parse_sample_context(path.read_text(encoding="utf-8"))
        if not parsed:
            issues.append(Issue(NAME, "warning", f"{DEFAULT_SOURCE} からエンティティを抽出できませんでした"))
        else:
            issues.extend(_compare(ctx, parsed))

    issues.extend(_compare_mvc_views(ctx))
    issues.extend(_compare_er(ctx))
    return issues


def _compare_er(ctx: Context) -> list[Issue]:
    """model のテーブル集合と er.a5er の一致を検証する。"""
    issues: list[Issue] = []
    path = ctx.root / "er.a5er"
    if not path.exists():
        return issues
    er_tables = set(parse_er(path.read_text(encoding="utf-8-sig"))["tables"])
    model_tables = {
        entity.meta.get("table")
        for entity in ctx.model.entities
        if entity.meta.get("table")
    }
    for table in sorted(model_tables - er_tables):
        issues.append(Issue(NAME, "error", f"model のテーブル '{table}' が er.a5er にありません"))
    for table in sorted(er_tables - model_tables):
        issues.append(Issue(NAME, "error", f"er.a5er のテーブル '{table}' が model にありません", "er.a5er"))
    return issues


def _compare_mvc_views(ctx: Context) -> list[Issue]:
    """Razor View の追加・削除を画面設計へ反映し忘れていないか検証する。"""
    issues: list[Issue] = []
    views_root = ctx.root / MVC_VIEWS_ROOT
    if not views_root.exists():
        return issues

    actual_views = {
        path.relative_to(ctx.root).as_posix()
        for path in views_root.rglob("*.cshtml")
        if "Shared" not in path.parts and not path.name.startswith("_")
    }
    modeled_views = {
        screen.meta.get("source_view")
        for screen in ctx.model.screens
        if screen.meta.get("source_view")
    }

    for source_view in sorted(modeled_views - actual_views):
        issues.append(Issue(NAME, "error", f"画面定義の View が存在しません: {source_view}"))
    for source_view in sorted(actual_views - modeled_views):
        issues.append(Issue(NAME, "error", f"MVC View が画面定義にありません: {source_view}"))

    controllers_root = ctx.root / "src/AspNetCoreSample.Mvc/Controllers"
    for screen in ctx.model.screens:
        controller = screen.meta.get("controller")
        if controller and not (controllers_root / f"{controller}Controller.cs").exists():
            issues.append(
                Issue(
                    NAME,
                    "error",
                    f"画面定義の Controller が存在しません: {controller}",
                    screen.path,
                )
            )

    return issues
