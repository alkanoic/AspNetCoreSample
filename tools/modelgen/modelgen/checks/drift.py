"""生成物が項目モデルと同期しているか (ドリフト) を検証する。"""

from __future__ import annotations

from pathlib import Path

from ..generators import TRACKED_DIRS
from .base import Context, Issue

NAME = "drift"


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []

    for rel, expected in sorted(ctx.outputs.items()):
        path = ctx.root / rel
        if not path.exists():
            issues.append(
                Issue(NAME, "error", f"生成物がありません: {rel}。generate を実行してください")
            )
            continue
        actual = path.read_text(encoding="utf-8")
        if actual != expected:
            issues.append(
                Issue(NAME, "error", f"生成物が古い/差分があります: {rel}。generate を実行してください")
            )

    known = set(ctx.outputs.keys())
    for dirname in TRACKED_DIRS:
        tracked_dir = ctx.root / dirname
        if tracked_dir.exists():
            for path in sorted(tracked_dir.rglob("*")):
                if path.is_file() and path.relative_to(ctx.root).as_posix() not in known:
                    issues.append(
                        Issue(
                            NAME,
                            "warning",
                            f"モデルに対応しない生成物が残っています: "
                            f"{path.relative_to(ctx.root).as_posix()}",
                        )
                    )

    return issues
