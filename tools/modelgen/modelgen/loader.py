"""model/ 配下の項目 YAML を読み込む。"""

from __future__ import annotations

from pathlib import Path
from typing import Any

import yaml

from .model import Entity, Event, Model, Service


def _load_yaml(path: Path) -> tuple[dict[str, Any] | None, str | None]:
    try:
        with path.open(encoding="utf-8") as handle:
            data = yaml.safe_load(handle)
    except yaml.YAMLError as exc:
        return None, f"YAML として解析できません: {exc}"
    if data is None:
        return {}, None
    if not isinstance(data, dict):
        return None, "マッピング (key: value) である必要があります"
    return data, None


def load_model(root: Path) -> Model:
    model = Model()
    model_dir = root / "model"

    for path in sorted((model_dir / "entities").glob("*.yml")):
        rel = path.relative_to(root).as_posix()
        data, error = _load_yaml(path)
        if error is not None:
            model.parse_errors.append((rel, error))
            continue
        assert data is not None
        entity_id = data.get("id") or path.stem
        if not isinstance(entity_id, str):
            model.parse_errors.append((rel, "id は文字列である必要があります"))
            continue
        model.entities.append(Entity(id=entity_id, meta=data, path=rel))

    for path in sorted((model_dir / "events").glob("*.yml")):
        rel = path.relative_to(root).as_posix()
        data, error = _load_yaml(path)
        if error is not None:
            model.parse_errors.append((rel, error))
            continue
        assert data is not None
        event_id = data.get("id") or path.stem
        if not isinstance(event_id, str):
            model.parse_errors.append((rel, "id は文字列である必要があります"))
            continue
        model.explicit_events.append(Event(id=event_id, meta=data, path=rel))

    for path in sorted((model_dir / "services").glob("*.yml")):
        rel = path.relative_to(root).as_posix()
        data, error = _load_yaml(path)
        if error is not None:
            model.parse_errors.append((rel, error))
            continue
        assert data is not None
        service_id = data.get("id") or path.stem
        if not isinstance(service_id, str):
            model.parse_errors.append((rel, "id は文字列である必要があります"))
            continue
        model.services.append(Service(id=service_id, meta=data, path=rel))

    model.detect_duplicates()
    return model
