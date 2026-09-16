"""MkDocs hooks: serve/build の開始前に modelgen の生成物を同期する。

generated/ と docs/development/mvc/ は gitignore 対象のため、
初回 serve 時や model/ 変更後に存在しない・古い可能性がある。
ここで generate しておくことで、デバッグ実行でも include 解決できる。

検証（schema/link/drift の厳密チェック）は行わない。
厳密チェックは `tools/modelgen/run.sh check --strict` と CI が担う。
"""

from __future__ import annotations

import sys
from pathlib import Path

TOOL_DIR = Path(__file__).resolve().parent
ROOT = TOOL_DIR.parent
# modelgen パッケージ実体は tools/modelgen/ 配下にある
MODELGEN_DIR = TOOL_DIR / "modelgen"


def on_startup(command, dirty: bool = False) -> None:
    if str(MODELGEN_DIR) not in sys.path:
        sys.path.insert(0, str(MODELGEN_DIR))
    try:
        from modelgen.cli import build_context, _generate
    except ImportError as exc:
        print(
            "[modelgen] generate をスキップします "
            f"(依存未導入: {exc} / pip install -r tools/modelgen/requirements.txt)"
        )
        return
    ctx = build_context(ROOT)
    _generate(ctx)
    print(f"[modelgen] generated {len(ctx.outputs)} file(s)", flush=True)
