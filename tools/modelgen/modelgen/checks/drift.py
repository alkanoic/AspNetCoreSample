"""生成物が項目モデルと同期しているか (ドリフト) を検証する。"""

from __future__ import annotations

from pathlib import Path

from .base import Context, Issue

NAME = "drift"


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    generated_dir = ctx.root / "generated"

    for rel, expected in sorted(ctx.outputs.items()):
        path = generated_dir / rel
        if not path.exists():
            issues.append(
                Issue(NAME, "error", f"生成物がありません: generated/{rel}。generate を実行してください")
            )
            continue
        actual = path.read_text(encoding="utf-8")
        if actual != expected:
            issues.append(
                Issue(NAME, "error", f"生成物が古い/差分があります: generated/{rel}。generate を実行してください")
            )

    known = set(ctx.outputs.keys())
    if generated_dir.exists():
        for path in sorted(generated_dir.rglob("*")):
            if path.is_file() and path.relative_to(generated_dir).as_posix() not in known:
                issues.append(
                    Issue(
                        NAME,
                        "warning",
                        f"モデルに対応しない生成物が残っています: generated/"
                        f"{path.relative_to(generated_dir).as_posix()}",
                    )
                )

    return issues
