"""項目一覧を Markdown で生成する。ER図の正本は er.a5er（A5SQL Mk-2）。"""

from __future__ import annotations

from typing import Any

from ..model import Model

HEADER = "<!-- 自動生成: tools/modelgen generate。手編集禁止 -->"

# A5SQL の RelationType の組み合わせから Mermaid 記法への対応。
# 本リポジトリの er.a5er に現れる (2, 3) は 1:N を表す。
_RELATIONSHIP = {("2", "3"): "||--o{"}


def build_er_diagram(er: dict[str, Any]) -> list[str]:
    """er.a5er から Mermaid 定義を生成する。"""
    lines = ["```mermaid", "erDiagram"]
    for pname in sorted(er["tables"]):
        table = er["tables"][pname]
        lines.append(f"    {pname} {{")
        for field in table["fields"]:
            base = field["type"].split("(")[0]
            pk = " PK" if field["key"] else ""
            comment = field["lname"].replace('"', "")
            lines.append(f'        {base} {field["pname"]}{pk} "{comment}"')
        lines.append("    }")
    for relation in er["relations"]:
        cardinality = _RELATIONSHIP.get(
            (relation.get("relationtype1"), relation.get("relationtype2")), "}o--o{"
        )
        lines.append(
            f'    {relation["entity1"]} {cardinality} {relation["entity2"]} '
            f': "{relation.get("fields2", "")}"'
        )
    lines.append("```")
    lines.append("")
    return lines


def build_entity_list(model: Model) -> list[str]:
    lines = [
        "| table | id | title |",
        "| ----- | -- | ----- |",
    ]
    for entity in sorted(model.entities, key=lambda e: (e.meta.get("table", ""), e.id)):
        lines.append(
            f"| `{entity.meta.get('table', '')}` | `{entity.id}` | {entity.meta.get('title', '')} |"
        )
    lines.append("")
    return lines


def render(model: Model, er: dict[str, Any] | None = None) -> str:
    sections: list[str] = [
        HEADER,
        "# 項目モデル",
        "",
        "`model/` を Single Source of Truth として自動生成した一覧です。",
        "ER図の正本は `er.a5er`（A5SQL Mk-2）であり、以下は表示用に生成したものです。",
        "",
    ]
    if er and er.get("tables"):
        sections += ["## ER図", "", *build_er_diagram(er)]
    sections += [
        "## エンティティ一覧",
        "",
        *build_entity_list(model),
    ]
    return "\n".join(sections)
