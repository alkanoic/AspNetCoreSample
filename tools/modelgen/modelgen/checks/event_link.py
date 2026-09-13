"""エンティティとイベントの相互参照を検証する。"""

from __future__ import annotations

from .base import Context, Issue

NAME = "event-link"


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    entities = ctx.model.entity_map()
    events = ctx.model.event_map()

    for event in ctx.model.events():
        entity = entities.get(event.source)
        if entity is None:
            issues.append(
                Issue(NAME, "error", f"source のエンティティが存在しません: {event.source}", event.path)
            )
            continue

        known = entity.field_names()
        for field_name in event.data_fields:
            if field_name not in known:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"data_fields '{field_name}' が {entity.id} の fields に存在しません",
                        event.path,
                    )
                )

        if not event.generated and event.id not in entity.emits:
            issues.append(
                Issue(
                    NAME,
                    "error",
                    f"意図イベント '{event.id}' が {entity.id}.emits に宣言されていません",
                    event.path,
                )
            )

    for entity in ctx.model.entities:
        for event_id in entity.emits:
            event = events.get(event_id)
            if event is None:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"emits の参照先イベントが存在しません: {event_id}",
                        entity.path,
                    )
                )
            elif event.source != entity.id:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"emits '{event_id}' の source が {entity.id} を指していません",
                        entity.path,
                    )
                )

    return issues
