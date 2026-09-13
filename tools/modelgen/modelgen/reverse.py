"""C# の DbContext から項目モデルを逆抽出して乖離を検出するための解析。"""

from __future__ import annotations

import re
from typing import Any

_ENTITY_RE = re.compile(r"modelBuilder\.Entity<(\w+)>\(entity =>\s*\{(.*?)\n {8}\}\);", re.DOTALL)
_TABLE_RE = re.compile(r'\.ToTable\("([^"]+)"')
_PROP_RE = re.compile(r'\.Property\(e => e\.(\w+)\)(?:[\s\S]*?)\.HasColumnName\("([^"]+)"\)')
_KEY_RE = re.compile(r"\.HasKey\(e => (?:new \{ ([^}]+) \}|e\.(\w+))\)")


def parse_sample_context(text: str) -> dict[str, dict[str, Any]]:
    """クラス名 -> {table, columns, key_columns} を返す。"""
    entities: dict[str, dict[str, Any]] = {}

    for match in _ENTITY_RE.finditer(text):
        class_name = match.group(1)
        body = match.group(2)

        table_match = _TABLE_RE.search(body)
        prop_to_column = {prop: column for prop, column in _PROP_RE.findall(body)}

        key_props: list[str] = []
        key_match = _KEY_RE.search(body)
        if key_match:
            if key_match.group(1):
                key_props = re.findall(r"e\.(\w+)", key_match.group(1))
            elif key_match.group(2):
                key_props = [key_match.group(2)]

        entities[class_name] = {
            "table": table_match.group(1) if table_match else None,
            "columns": set(prop_to_column.values()),
            "key_columns": {prop_to_column[p] for p in key_props if p in prop_to_column},
        }

    return entities
