"""生成器の集約。build_outputs がリポジトリルートからの相対パス -> 内容を返す。"""

from __future__ import annotations

from ..model import Model
from ..reverse_er import parse_er
from . import asyncapi, docs, events, mvc_docs

# drift/generate の管理対象ディレクトリ
TRACKED_DIRS = ("generated", "docs/development/mvc")

ER_SOURCE = "er.a5er"


def load_er(root) -> dict | None:
    """er.a5er を読み込み、なければ None を返す。"""
    path = root / ER_SOURCE
    if not path.exists():
        return None
    return parse_er(path.read_text(encoding="utf-8-sig"))


def build_outputs(model: Model, root=None) -> dict[str, str]:
    """リポジトリルートからの相対パス -> ファイル内容。"""
    outputs: dict[str, str] = {}
    entity_map = model.entity_map()
    er = load_er(root) if root is not None else None

    for event in model.events():
        entity = entity_map.get(event.source)
        if entity is None:
            continue
        outputs[f"generated/events/{event.id}.json"] = events.render_json(
            events.event_schema(entity, event)
        )

    outputs["generated/asyncapi.yaml"] = asyncapi.render(model)
    outputs["generated/model-graph.md"] = docs.render(model, er)
    outputs["generated/app-policy.md"] = mvc_docs.build_app_policy_doc(model)
    for rel, content in mvc_docs.build_outputs(model).items():
        outputs[f"generated/{rel}"] = content
    for screen_rel, content in mvc_docs.build_screen_pages(model).items():
        outputs[f"generated/{screen_rel}"] = content
    outputs.update(mvc_docs.build_doc_pages(model))
    return outputs
