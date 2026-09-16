"""画面遷移・引き継ぎ項目・画面イベントの整合性を検証する。"""

from __future__ import annotations

from .base import Context, Issue

NAME = "screen-link"
NON_ENTITY_STORES = {"Session", "IDistributedCache", "PushSubscriptionStore"}


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []
    screens = {screen.id: screen for screen in ctx.model.screens}
    entities = ctx.model.entity_map()

    for item in ctx.model.navigation:
        target_id = item.get("to")
        if target_id not in screens:
            issues.append(Issue(NAME, "error", f"共通ナビの遷移先画面が存在しません: {target_id}"))

    policies = ctx.model.policy_map()

    for screen in ctx.model.screens:
        field_names = screen.field_names()
        for transition in screen.transitions:
            target_id = transition.get("to")
            target = screens.get(target_id)
            if target is None:
                issues.append(Issue(NAME, "error", f"遷移先画面が存在しません: {target_id}", screen.path))
                continue
            for field in transition.get("carry", []):
                if field not in field_names:
                    issues.append(
                        Issue(
                            NAME,
                            "error",
                            f"遷移引き継ぎ項目 '{field}' が送信元画面にありません: {screen.id} -> {target_id}",
                            screen.path,
                        )
                    )
                if field not in target.field_names():
                    issues.append(
                        Issue(
                            NAME,
                            "error",
                            f"遷移引き継ぎ項目 '{field}' が遷移先画面にありません: {screen.id} -> {target_id}",
                            screen.path,
                        )
                    )

        for event in screen.events:
            target_id = event.get("transition_to")
            if target_id and target_id not in screens:
                issues.append(Issue(NAME, "error", f"イベントの遷移先画面が存在しません: {target_id}", screen.path))
            for policy_id in event.get("policy_refs", []):
                if policy_id not in policies:
                    issues.append(
                        Issue(NAME, "error", f"方式参照先が存在しません: {policy_id}", screen.path)
                    )
            for operation in event.get("db_operations", []):
                store = operation.get("store")
                entity = entities.get(store)
                if entity is None:
                    if store not in NON_ENTITY_STORES:
                        issues.append(Issue(NAME, "error", f"DB操作の対象ドメインが存在しません: {store}", screen.path))
                    continue
                for field in operation.get("fields", []):
                    if field not in entity.field_names():
                        issues.append(
                            Issue(
                                NAME,
                                "error",
                                f"DB操作項目 '{field}' がドメイン '{store}' にありません",
                                screen.path,
                            )
                        )

    return issues
