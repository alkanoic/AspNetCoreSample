"""サービスとイベントの producer/consumer を双方向で検証する。"""

from __future__ import annotations

from .base import Context, Issue

NAME = "service-link"


def _as_set(value) -> set[str]:
    if isinstance(value, list):
        return {v for v in value if isinstance(v, str)}
    return set()


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    events = ctx.model.event_map()
    services = ctx.model.service_map()
    producers = ctx.model.event_producers()
    consumers = ctx.model.event_consumers()

    for service in ctx.model.services:
        for event_id in service.produces:
            if event_id not in events:
                issues.append(
                    Issue(NAME, "error", f"produces の参照先イベントが存在しません: {event_id}", service.path)
                )
        for event_id in service.consumes:
            if event_id not in events:
                issues.append(
                    Issue(NAME, "error", f"consumes の参照先イベントが存在しません: {event_id}", service.path)
                )
        for event_id in sorted(set(service.produces) & set(service.consumes)):
            issues.append(
                Issue(NAME, "warning", f"同一イベントを produce と consume しています: {event_id}", service.path)
            )

    for event in ctx.model.events():
        if not producers.get(event.id):
            issues.append(
                Issue(NAME, "error", f"イベント '{event.id}' に producer がありません", event.path)
            )

    for event in ctx.model.explicit_events:
        declared_producers = _as_set(event.meta.get("producers"))
        declared_consumers = _as_set(event.meta.get("consumers"))
        derived_producers = producers.get(event.id, set())
        derived_consumers = consumers.get(event.id, set())

        for service_id in sorted((declared_producers | declared_consumers) - set(services)):
            issues.append(
                Issue(NAME, "error", f"参照先サービスが存在しません: {service_id}", event.path)
            )
        for service_id in sorted(declared_producers - derived_producers):
            issues.append(
                Issue(NAME, "error", f"producers '{service_id}' が service.produces にありません", event.path)
            )
        for service_id in sorted(derived_producers - declared_producers):
            issues.append(
                Issue(NAME, "error", f"service.produces にある '{service_id}' が producers にありません", event.path)
            )
        for service_id in sorted(declared_consumers - derived_consumers):
            issues.append(
                Issue(NAME, "error", f"consumers '{service_id}' が service.consumes にありません", event.path)
            )
        for service_id in sorted(derived_consumers - declared_consumers):
            issues.append(
                Issue(NAME, "error", f"service.consumes にある '{service_id}' が consumers にありません", event.path)
            )

    return issues
