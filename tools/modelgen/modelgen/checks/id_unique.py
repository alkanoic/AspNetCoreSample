"""項目 id の重複を検出する。"""

from __future__ import annotations

from .base import Context, Issue

NAME = "id-unique"


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    for kind, item_id in ctx.model.duplicate_ids:
        issues.append(Issue(NAME, "error", f"{kind} id が重複しています: {item_id}"))
    return issues
