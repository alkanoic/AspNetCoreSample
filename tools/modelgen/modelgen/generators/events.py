"""項目定義からイベント payload (JSON Schema) を生成する。"""

from __future__ import annotations

import json
from typing import Any

from ..model import Entity, Event

_TYPE_MAP: dict[str, dict[str, Any]] = {
    "integer": {"type": "integer"},
    "decimal": {"type": "number"},
    "boolean": {"type": "boolean"},
    "date": {"type": "string", "format": "date"},
    "datetime": {"type": "string", "format": "date-time"},
    "string": {"type": "string"},
}


def field_schema(field: dict[str, Any]) -> dict[str, Any]:
    field_type = field.get("type", "string")
    schema = dict(_TYPE_MAP.get(field_type, {"type": "string"}))
    if field_type == "string" and field.get("max_length"):
        schema["maxLength"] = field["max_length"]
    if field.get("description"):
        schema["description"] = field["description"]
    return schema


def data_schema(entity: Entity, event: Event) -> dict[str, Any]:
    """イベント payload の data 部分を項目の fields から生成する。"""
    action = event.action
    if action == "deleted":
        names = list(entity.key)
    elif action is None:
        # 意図イベント: data_fields を明示的に使う
        names = list(event.data_fields)
    else:
        names = [f["name"] for f in entity.fields if "name" in f]

    properties: dict[str, Any] = {}
    for field in entity.fields:
        name = field.get("name")
        if name in names:
            properties[name] = field_schema(field)

    if action == "updated":
        required = [name for name in entity.key if name in properties]
    elif action == "created":
        required = [
            f["name"]
            for f in entity.fields
            if f.get("name") in properties and (f.get("required") or f.get("key"))
        ]
    else:
        required = list(properties.keys())

    schema: dict[str, Any] = {"type": "object", "properties": properties}
    if required:
        schema["required"] = required
    return schema


def event_schema(entity: Entity, event: Event) -> dict[str, Any]:
    """CloudEvents 風の封筒 + data を持つイベントスキーマ。"""
    return {
        "$schema": "https://json-schema.org/draft/2020-12/schema",
        "$id": f"https://example.com/generated/events/{event.id}.json",
        "title": event.id,
        "description": event.meta.get("description", ""),
        "type": "object",
        "required": ["specversion", "type", "source", "id", "time", "data"],
        "properties": {
            "specversion": {"const": "1.0"},
            "type": {"const": event.id},
            "source": {"const": f"entity:{entity.id}"},
            "id": {"type": "string"},
            "time": {"type": "string", "format": "date-time"},
            "data": data_schema(entity, event),
        },
    }


def render_json(schema: dict[str, Any]) -> str:
    return json.dumps(schema, ensure_ascii=False, indent=2) + "\n"
