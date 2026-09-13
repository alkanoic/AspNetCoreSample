"""チェック結果のレポート出力。"""

from __future__ import annotations

import dataclasses
from typing import Any

from .checks.base import Context, Issue


def render_text(ctx: Context, issues: list[Issue]) -> str:
    errors = [issue for issue in issues if issue.severity == "error"]
    warnings = [issue for issue in issues if issue.severity == "warning"]

    lines = [
        "== modelgen check ==",
        f"entities: {len(ctx.model.entities)} / events: {len(ctx.model.events())} "
        f"(explicit: {len(ctx.model.explicit_events)})",
        f"outputs: {len(ctx.outputs)}",
        "",
    ]
    for issue in errors + warnings:
        lines.append(issue.format())
    if not issues:
        lines.append("問題は検出されませんでした。")
    lines.append("")
    lines.append(f"-- error: {len(errors)}, warning: {len(warnings)}")
    return "\n".join(lines)


def to_json(ctx: Context, issues: list[Issue]) -> dict[str, Any]:
    return {
        "entities": [
            {"id": e.id, "path": e.path, "relations": e.relations} for e in ctx.model.entities
        ],
        "events": [
            {"id": e.id, "source": e.source, "generated": e.generated}
            for e in ctx.model.events()
        ],
        "services": [
            {"id": s.id, "produces": s.produces, "consumes": s.consumes}
            for s in ctx.model.services
        ],
        "outputs": sorted(ctx.outputs.keys()),
        "issues": [dataclasses.asdict(issue) for issue in issues],
    }
