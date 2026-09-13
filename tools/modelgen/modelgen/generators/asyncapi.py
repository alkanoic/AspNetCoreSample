"""イベント項目から AsyncAPI ドキュメントを生成する。"""

from __future__ import annotations

from typing import Any

import yaml

from ..model import Model


def build_asyncapi(model: Model) -> dict[str, Any]:
    channels: dict[str, Any] = {}
    messages: dict[str, Any] = {}

    for event in sorted(model.events(), key=lambda e: e.id):
        channels[event.id] = {
            "address": event.id,
            "messages": {event.id: {"$ref": f"#/components/messages/{event.id}"}},
        }
        messages[event.id] = {
            "name": event.id,
            "title": event.meta.get("title", event.id),
            "summary": event.trigger,
            "payload": {"$ref": f"./events/{event.id}.json"},
        }

    return {
        "asyncapi": "3.0.0",
        "info": {
            "title": model_title(model),
            "version": "1.0.0",
            "description": "項目モデルから自動生成。手編集禁止 (tools/modelgen generate)。",
        },
        "channels": channels,
        "operations": {
            event.id: {
                "action": "send",
                "channel": {"$ref": f"#/channels/{event.id}"},
            }
            for event in sorted(model.events(), key=lambda e: e.id)
        },
        "components": {"messages": messages},
    }


def model_title(model: Model) -> str:
    return "AspNetCoreSample 設計項目イベント"


def render(model: Model) -> str:
    return yaml.safe_dump(
        build_asyncapi(model),
        allow_unicode=True,
        sort_keys=False,
        default_flow_style=False,
    )
