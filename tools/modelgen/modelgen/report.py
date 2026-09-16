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
        f"services: {len(ctx.model.services)} / screens: {len(ctx.model.screens)}",
        f"policies: {len(ctx.model.policies)}",
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
        "screens": [
            {"id": s.id, "route": s.meta.get("route"), "view": s.meta.get("source_view")}
            for s in ctx.model.screens
        ],
        "policies": [
            {"id": p.id, "area": p.area, "rules": len(p.rules)} for p in ctx.model.policies
        ],
        "outputs": sorted(ctx.outputs.keys()),
        "issues": [dataclasses.asdict(issue) for issue in issues],
    }
