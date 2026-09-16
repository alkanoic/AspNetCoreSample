"""A5SQL Mk-2 の er.a5er を解析する。ER図の正本は er.a5er。"""

from __future__ import annotations

import csv
import io
from typing import Any


def _parse_field(value: str) -> dict[str, Any] | None:
    try:
        parts = next(csv.reader(io.StringIO(value)))
    except Exception:
        return None
    if len(parts) < 4:
        return None
    key_order = parts[4].strip() if len(parts) > 4 else ""
    return {
        "lname": parts[0],
        "pname": parts[1],
        "type": parts[2],
        "notnull": parts[3] == "NOT NULL",
        "key": key_order.isdigit(),
        "comment": parts[8].strip() if len(parts) > 8 else "",
    }


def parse_er(text: str) -> dict[str, Any]:
    """テーブルとリレーションを抽出する。"""
    tables: dict[str, dict[str, Any]] = {}
    relations: list[dict[str, str]] = []
    section: str | None = None
    current: dict[str, Any] | None = None

    def flush() -> None:
        nonlocal current
        if current is None:
            return
        if section == "Entity" and current.get("pname"):
            tables[current["pname"]] = current
        elif section == "Relation" and current.get("entity1") and current.get("entity2"):
            relations.append(current)  # type: ignore[arg-type]
        current = None

    for raw_line in text.splitlines():
        line = raw_line.strip()
        if not line or line.startswith("#"):
            continue
        if line.startswith("[") and line.endswith("]"):
            flush()
            section = line[1:-1]
            current = {"fields": []} if section == "Entity" else {}
            continue
        if current is None or "=" not in line:
            continue
        key, _, value = line.partition("=")
        key = key.strip()
        value = value.strip()
        if section == "Entity":
            if key == "PName":
                current["pname"] = value
            elif key == "LName":
                current["lname"] = value
            elif key == "Field":
                field = _parse_field(value)
                if field:
                    current["fields"].append(field)
        elif section == "Relation":
            if key in ("Entity1", "Entity2", "Fields1", "Fields2", "RelationType1", "RelationType2"):
                current[key.lower()] = value
    flush()
    return {"tables": tables, "relations": relations}
