"""設計項目モデルの型と派生ロジック。"""

from __future__ import annotations

from dataclasses import dataclass, field
from typing import Any

STATE_ACTIONS = ("created", "updated", "deleted")


@dataclass
class Entity:
    """エンティティ項目 (model/entities/*.yml)。"""

    id: str
    meta: dict[str, Any]
    path: str = ""

    @property
    def fields(self) -> list[dict[str, Any]]:
        return self.meta.get("fields", [])

    @property
    def relations(self) -> list[dict[str, Any]]:
        return self.meta.get("relations", [])

    @property
    def key(self) -> list[str]:
        return self.meta.get("key", [])

    @property
    def auto_events(self) -> list[str]:
        return self.meta.get("auto_events", [])

    @property
    def emits(self) -> list[str]:
        return self.meta.get("emits", [])

    def field_names(self) -> set[str]:
        return {f["name"] for f in self.fields if "name" in f}

    def get_field(self, name: str) -> dict[str, Any] | None:
        for f in self.fields:
            if f.get("name") == name:
                return f
        return None

    def data_fields_for(self, action: str) -> list[str]:
        if action == "deleted":
            return list(self.key)
        return [f["name"] for f in self.fields if "name" in f]

    def derived_events(self) -> list["Event"]:
        events: list[Event] = []
        for action in self.auto_events:
            if action not in STATE_ACTIONS:
                continue
            events.append(
                Event(
                    id=f"{self.id}.{action}",
                    meta={
                        "kind": "event",
                        "id": f"{self.id}.{action}",
                        "title": f"{self.meta.get('title', self.id)} {action}",
                        "source": self.id,
                        "trigger": f"entity.{action}",
                        "data_fields": self.data_fields_for(action),
                    },
                    action=action,
                    generated=True,
                    path=f"generated/events/{self.id}.{action}.json",
                )
            )
        return events


@dataclass
class Event:
    """イベント項目 (意図イベント or 項目定義から派生した状態変化イベント)。"""

    id: str
    meta: dict[str, Any]
    action: str | None = None
    generated: bool = False
    path: str = ""

    @property
    def source(self) -> str:
        return self.meta.get("source", "")

    @property
    def trigger(self) -> str:
        return self.meta.get("trigger", "")

    @property
    def data_fields(self) -> list[str]:
        return self.meta.get("data_fields", [])


@dataclass
class Service:
    """サービス項目 (model/services/*.yml)。イベントの producer/consumer を宣言する。"""

    id: str
    meta: dict[str, Any]
    path: str = ""

    @property
    def produces(self) -> list[str]:
        return self.meta.get("produces", [])

    @property
    def consumes(self) -> list[str]:
        return self.meta.get("consumes", [])


@dataclass
class Model:
    """項目モデル全体。"""

    entities: list[Entity] = field(default_factory=list)
    explicit_events: list[Event] = field(default_factory=list)
    services: list[Service] = field(default_factory=list)
    parse_errors: list[tuple[str, str]] = field(default_factory=list)
    duplicate_ids: list[tuple[str, str]] = field(default_factory=list)

    def entity_map(self) -> dict[str, Entity]:
        result: dict[str, Entity] = {}
        for entity in self.entities:
            result.setdefault(entity.id, entity)
        return result

    def service_map(self) -> dict[str, Service]:
        result: dict[str, Service] = {}
        for service in self.services:
            result.setdefault(service.id, service)
        return result

    def event_producers(self) -> dict[str, set[str]]:
        result: dict[str, set[str]] = {}
        for service in self.services:
            for event_id in service.produces:
                result.setdefault(event_id, set()).add(service.id)
        return result

    def event_consumers(self) -> dict[str, set[str]]:
        result: dict[str, set[str]] = {}
        for service in self.services:
            for event_id in service.consumes:
                result.setdefault(event_id, set()).add(service.id)
        return result

    def events(self) -> list[Event]:
        """派生イベントを含む全イベント。"""
        result = list(self.explicit_events)
        seen = {e.id for e in result}
        for entity in self.entities:
            for event in entity.derived_events():
                if event.id in seen:
                    continue
                result.append(event)
        return result

    def event_map(self) -> dict[str, Event]:
        result: dict[str, Event] = {}
        for event in self.events():
            result.setdefault(event.id, event)
        return result

    def detect_duplicates(self) -> None:
        """id の衝突 (エンティティ/サービス/イベント) を検出する。"""
        seen_entities: dict[str, str] = {}
        for entity in self.entities:
            if entity.id in seen_entities:
                self.duplicate_ids.append(("entity", entity.id))
            seen_entities[entity.id] = entity.path

        seen_services: dict[str, str] = {}
        for service in self.services:
            if service.id in seen_services:
                self.duplicate_ids.append(("service", service.id))
            seen_services[service.id] = service.path

        seen_events: dict[str, str] = {}
        for event in self.explicit_events:
            if event.id in seen_events:
                self.duplicate_ids.append(("event", event.id))
            seen_events[event.id] = event.path
        for entity in self.entities:
            for event in entity.derived_events():
                if event.id in seen_events:
                    self.duplicate_ids.append(("event", event.id))
