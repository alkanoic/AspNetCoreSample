"""項目グラフ (Mermaid) と一覧表を Markdown で生成する。"""

from __future__ import annotations

import re
from typing import Any

from ..model import Model

_CARDINALITY: dict[str, str] = {
    "has-many": "||--o{",
    "has-one": "||--||",
    "many-to-many": "}o--o{",
}

HEADER = "<!-- 自動生成: tools/modelgen generate。手編集禁止 -->"


def _safe(value: str) -> str:
    return re.sub(r"[^0-9a-zA-Z_]", "_", value)


def build_mermaid(model: Model) -> str:
    lines = ["```mermaid", "erDiagram"]
    for entity in sorted(model.entities, key=lambda e: e.id):
        for relation in entity.relations:
            cardinality = _CARDINALITY.get(relation.get("type", ""))
            if cardinality is None:
                continue  # belongs-to / references は逆側で描画
            lines.append(f"    {entity.id} {cardinality} {relation['target']} : {relation.get('id', '')}")
    lines.append("```")
    return "\n".join(lines)


def build_service_flow(model: Model) -> str:
    producers = model.event_producers()
    consumers = model.event_consumers()
    lines = ["```mermaid", "flowchart LR"]
    for service in sorted(model.services, key=lambda s: s.id):
        lines.append(f'    svc_{service.id}["{service.meta.get("title", service.id)}"]')
    for event in sorted(model.events(), key=lambda e: e.id):
        lines.append(f'    ev_{_safe(event.id)}(("{event.id}"))')
    for event_id, service_ids in sorted(producers.items()):
        for service_id in sorted(service_ids):
            lines.append(f"    svc_{service_id} -->|publishes| ev_{_safe(event_id)}")
    for event_id, service_ids in sorted(consumers.items()):
        for service_id in sorted(service_ids):
            lines.append(f"    ev_{_safe(event_id)} -->|consumes| svc_{service_id}")
    lines.append("```")
    return "\n".join(lines)


def build_entity_table(model: Model) -> str:
    lines = [
        "| id | title | table | owner | 主キー | フィールド | 関係 |",
        "| -- | ----- | ----- | ----- | ------ | ---------- | ---- |",
    ]
    for entity in sorted(model.entities, key=lambda e: e.id):
        relations = ", ".join(f"{r.get('type')}→{r.get('target')}" for r in entity.relations)
        fields = ", ".join(f["name"] for f in entity.fields if "name" in f)
        lines.append(
            f"| `{entity.id}` | {entity.meta.get('title', '')} | `{entity.meta.get('table', '')}` "
            f"| {entity.meta.get('owner', '')} | {', '.join(entity.key)} | {fields} | {relations} |"
        )
    return "\n".join(lines)


def build_service_table(model: Model) -> str:
    lines = [
        "| id | title | owner | produces | consumes |",
        "| -- | ----- | ----- | -------- | -------- |",
    ]
    for service in sorted(model.services, key=lambda s: s.id):
        lines.append(
            f"| `{service.id}` | {service.meta.get('title', '')} | {service.meta.get('owner', '')} "
            f"| {', '.join(service.produces)} | {', '.join(service.consumes)} |"
        )
    return "\n".join(lines)


def build_event_table(model: Model) -> str:
    producers = model.event_producers()
    consumers = model.event_consumers()
    lines = [
        "| id | source | trigger | 種別 | producers | consumers | data_fields |",
        "| -- | ------ | ------- | ---- | --------- | --------- | ----------- |",
    ]
    for event in sorted(model.events(), key=lambda e: e.id):
        kind = "自動生成" if event.generated else "意図イベント"
        lines.append(
            f"| `{event.id}` | `{event.source}` | `{event.trigger}` | {kind} "
            f"| {', '.join(sorted(producers.get(event.id, set())))} "
            f"| {', '.join(sorted(consumers.get(event.id, set())))} "
            f"| {', '.join(event.data_fields)} |"
        )
    return "\n".join(lines)


def render(model: Model) -> str:
    sections: list[str] = [
        HEADER,
        "# 項目モデル",
        "",
        "`model/` を Single Source of Truth として自動生成した関連グラフと一覧です。",
        "",
        "## エンティティ関連グラフ",
        "",
        build_mermaid(model),
        "",
        "## サービス・イベントフロー",
        "",
        build_service_flow(model),
        "",
        "## エンティティ",
        "",
        build_entity_table(model),
        "",
        "## サービス",
        "",
        build_service_table(model),
        "",
        "## イベント",
        "",
        build_event_table(model),
        "",
    ]
    return "\n".join(sections)
