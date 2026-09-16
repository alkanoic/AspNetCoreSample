"""modelgen の CLI。check / generate を提供する。"""

from __future__ import annotations

import argparse
import json
from pathlib import Path

from .checks import CHECKS
from .checks.base import Context
from .generators import TRACKED_DIRS, build_outputs
from .loader import load_model
from .report import render_text, to_json

SCHEMA_NAMES = ("entity", "event", "service", "screen", "app_policy")


def build_context(root: Path) -> Context:
    model = load_model(root)
    schemas: dict[str, dict] = {}
    for name in SCHEMA_NAMES:
        path = root / "schemas" / f"{name}.schema.json"
        if path.exists():
            schemas[name] = json.loads(path.read_text(encoding="utf-8"))
    return Context(root=root, model=model, schemas=schemas, outputs=build_outputs(model, root))


def _generate(ctx: Context) -> None:
    expected = set(ctx.outputs.keys())
    for dirname in TRACKED_DIRS:
        tracked_dir = ctx.root / dirname
        tracked_dir.mkdir(parents=True, exist_ok=True)
        for path in sorted(tracked_dir.rglob("*")):
            if path.is_file() and path.relative_to(ctx.root).as_posix() not in expected:
                path.unlink()

    for rel, content in sorted(ctx.outputs.items()):
        path = ctx.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(content, encoding="utf-8")


def run_check(args: argparse.Namespace) -> int:
    root = Path(args.root)
    ctx = build_context(root)
    issues = []
    for check in CHECKS:
        issues.extend(check(ctx))
    print(render_text(ctx, issues))
    if args.json_out:
        Path(args.json_out).write_text(
            json.dumps(to_json(ctx, issues), ensure_ascii=False, indent=2) + "\n",
            encoding="utf-8",
        )
    errors = sum(1 for issue in issues if issue.severity == "error")
    warnings = sum(1 for issue in issues if issue.severity == "warning")
    if errors or (args.strict and warnings):
        return 1
    return 0


def run_generate(args: argparse.Namespace) -> int:
    ctx = build_context(Path(args.root))
    _generate(ctx)
    print(f"generated {len(ctx.outputs)} file(s)")
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        prog="modelgen",
        description="設計項目モデルからグラフ・イベント定義を生成し、双方向の整合性を検証する。",
    )
    parser.add_argument("--root", default=".", help="リポジトリルート (default: .)")
    sub = parser.add_subparsers(dest="command", required=True)

    check = sub.add_parser("check", help="整合性チェック")
    check.add_argument("--strict", action="store_true", help="warning も失敗として扱う")
    check.add_argument("--json", dest="json_out", help="結果を JSON へ出力")
    check.set_defaults(func=run_check)

    generate = sub.add_parser("generate", help="generated/ を生成")
    generate.set_defaults(func=run_generate)

    args = parser.parse_args(argv)
    return args.func(args)
