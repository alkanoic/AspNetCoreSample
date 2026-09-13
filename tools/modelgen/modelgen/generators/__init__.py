"""生成器の集約。build_outputs が generated/ 配下の内容を返す。"""

from __future__ import annotations

from ..model import Model
from . import asyncapi, docs, events


def build_outputs(model: Model) -> dict[str, str]:
    """generated/ からの相対パス -> ファイル内容。"""
    outputs: dict[str, str] = {}
    entity_map = model.entity_map()

    for event in model.events():
        entity = entity_map.get(event.source)
        if entity is None:
            continue
        outputs[f"events/{event.id}.json"] = events.render_json(
            events.event_schema(entity, event)
        )

    outputs["asyncapi.yaml"] = asyncapi.render(model)
    outputs["model-graph.md"] = docs.render(model)
    return outputs
