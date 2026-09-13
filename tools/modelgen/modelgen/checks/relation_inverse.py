"""エンティティ間の関係が双方向で整合しているか検証する。"""

from __future__ import annotations

from .base import Context, Issue

NAME = "relation-inverse"

# 関係タイプ -> 相手側に許される関係タイプ
INVERSE_TYPES: dict[str, set[str]] = {
    "has-many": {"belongs-to"},
    "has-one": {"belongs-to"},
    "belongs-to": {"has-one", "has-many"},
    "many-to-many": {"many-to-many"},
    "references": {"references"},
}


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    entities = ctx.model.entity_map()

    for entity in ctx.model.entities:
        for relation in entity.relations:
            target_id = relation.get("target")
            target = entities.get(target_id)
            if target is None:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"関係 '{relation.get('id')}' の参照先エンティティが存在しません: {target_id}",
                        entity.path,
                    )
                )
                continue

            inverse_id = relation.get("inverse")
            inverse = next(
                (r for r in target.relations if r.get("id") == inverse_id), None
            )
            if inverse is None:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"関係 '{relation.get('id')}' の逆関係 '{inverse_id}' が "
                        f"{target_id} に定義されていません",
                        entity.path,
                    )
                )
                continue

            if inverse.get("target") != entity.id:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"逆関係 '{target_id}.{inverse_id}' の target が {entity.id} を指していません",
                        target.path,
                    )
                )
            if inverse.get("inverse") != relation.get("id"):
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"逆関係 '{target_id}.{inverse_id}' の inverse が "
                        f"'{relation.get('id')}' を指していません",
                        target.path,
                    )
                )
            allowed = INVERSE_TYPES.get(relation.get("type", ""), set())
            if inverse.get("type") not in allowed:
                issues.append(
                    Issue(
                        NAME,
                        "error",
                        f"関係 '{entity.id}.{relation.get('id')}' ({relation.get('type')}) の逆は "
                        f"{sorted(allowed)} である必要がありますが '{inverse.get('type')}' です",
                        entity.path,
                    )
                )

    return issues
