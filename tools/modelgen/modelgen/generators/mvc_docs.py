"""MVC 画面設計書とドメイン定義書を生成する。"""

from __future__ import annotations

from ..model import Model

HEADER = "<!-- 自動生成: tools/modelgen generate。手編集禁止 -->"


def _node(screen_id: str) -> str:
    return screen_id.replace(".", "_")


def _incoming(model: Model) -> dict[str, list[tuple[str, dict]]]:
    result: dict[str, list[tuple[str, dict]]] = {screen.id: [] for screen in model.screens}
    for screen in model.screens:
        for transition in screen.transitions:
            target = transition.get("to")
            if target in result:
                result[target].append((screen.id, transition))
    for item in model.navigation:
        target = item.get("to")
        if target in result:
            result[target].append(("layout.nav", {"trigger": f"共通ナビ {item.get('label', '')}", "carry": []}))
    return result


def _flow_lines(model: Model) -> list[str]:
    lines = [
        "## 画面遷移図",
        "",
        "```mermaid",
        "flowchart LR",
    ]
    if model.navigation:
        lines.append('    layout_nav["共通ナビ"]')
    for screen in sorted(model.screens, key=lambda item: item.id):
        lines.append(f'    {_node(screen.id)}["{screen.meta.get("title", screen.id)}"]')
    for item in model.navigation:
        target = item.get("to", "")
        lines.append(f'    layout_nav -->|{item.get("label", "ナビ")}| {_node(str(target))}')
    for screen in sorted(model.screens, key=lambda item: item.id):
        for transition in screen.transitions:
            target = transition.get("to")
            carry = ", ".join(transition.get("carry", []))
            label = transition.get("trigger", "遷移")
            if carry:
                label += f" / 引継ぎ: {carry}"
            lines.append(f'    {_node(screen.id)} -->|{label}| {_node(str(target))}')
    lines.extend(["```", ""])
    return lines


def _distinct_field_values(model: Model, key: str) -> list[str]:
    values: set[str] = set()
    for screen in model.screens:
        for field in screen.fields:
            value = field.get(key, "")
            if value:
                values.add(str(value))
    return sorted(values)


def _items_common_lines(model: Model) -> list[str]:
    return [
        "## 項目の凡例と規約",
        "",
        "- 必須 Yes: 入力が必須の項目",
        "- 引継ぎ Yes: 遷移時に値を引き継ぐ項目",
        "",
        "### 型一覧",
        "",
        *[f"- `{value}`" for value in _distinct_field_values(model, "type")],
        "",
        "### データソース規約",
        "",
        "- `Entity.field` 形式はドメイン定義書の項目を参照する",
        "- 空欄は画面内でのみ扱う項目を示す",
        "",
        "#### 使用中のデータソース",
        "",
        *[f"- `{value}`" for value in _distinct_field_values(model, "source")],
        "",
    ]


def _updates(event: dict) -> str:
    updates = []
    for operation in event.get("db_operations", []):
        fields = ", ".join(operation.get("fields", []))
        updates.append(f"{operation.get('store')}:{operation.get('operation')}({fields})")
    return "; ".join(updates)


def _policy_refs(event: dict) -> str:
    return ", ".join(f"`{ref}`" for ref in event.get("policy_refs", []))


def _branch_lines(event: dict) -> list[str]:
    lines: list[str] = []
    for branch in event.get("branches", []):
        text = f"- `{branch.get('when', '')}` → {branch.get('result', '')}"
        extras: list[str] = []
        if branch.get("transition_to"):
            extras.append(f"遷移: `{branch['transition_to']}`")
        if branch.get("carry"):
            extras.append("引継ぎ: " + ", ".join(f"`{value}`" for value in branch["carry"]))
        if branch.get("db"):
            extras.append(
                "; ".join(
                    f"{op.get('store')}:{op.get('operation')}({', '.join(op.get('fields', []))})"
                    for op in branch["db"]
                )
            )
        if extras:
            text += "（" + "、".join(extras) + "）"
        lines.append(text)
    return lines


def _events_common_lines(model: Model) -> list[str]:
    lines = [
        "## イベント命名規約",
        "",
        "- イベント ID は `{画面の接頭辞}.{動作}` 形式とする（例: `name.update`）",
        "",
        "## DB操作種別",
        "",
        "- `read` 参照、`insert` 追加、`update` 更新、`delete` 削除、`cache` キャッシュ更新、`session` セッション更新",
        "",
        "外部連携先別・DB更新先別・方式参照別のイベント一覧は、各画面の設計書に記載します。",
        "",
    ]
    return lines


def _common_design(model: Model) -> str:
    lines = [
        HEADER,
        "# 共通の画面設計書",
        "",
        "> 本書は全画面に共通する設計事項のみを記載します。",
        "> 画面ごとの詳細は「画面詳細」の各画面ページを参照してください。",
        "",
        "## 目次",
        "",
        "- 画面遷移図",
        "- 項目の凡例と規約",
        "- イベント命名規約とDB操作種別",
        "",
    ]
    lines.extend(_flow_lines(model))
    lines.extend(_items_common_lines(model))
    lines.extend(_events_common_lines(model))
    return "\n".join(lines) + "\n"


def build_outputs(model: Model) -> dict[str, str]:
    return {
        "mvc/common-design.md": _common_design(model),
    }


def _related_domain_lines(screen, model: Model) -> list[str]:
    """画面の DB 操作対象エンティティから関連ドメインイベントを導出する。"""
    entities = model.entity_map()
    producers = model.event_producers()
    consumers = model.event_consumers()
    stores: list[str] = []
    for event in screen.events:
        for operation in event.get("db_operations", []):
            store = str(operation.get("store", ""))
            if store in entities and store not in stores:
                stores.append(store)
    with_events = [
        store
        for store in sorted(stores)
        if any(item.source == store for item in model.events())
    ]
    if not with_events:
        return []
    lines = ["", "## 関連ドメインイベント", ""]
    for store in with_events:
        entity = entities[store]
        lines.append(f"### {entity.meta.get('title', store)} (`{store}`)")
        lines.append("")
        for event in sorted(
            (item for item in model.events() if item.source == store),
            key=lambda item: item.id,
        ):
            parts = [f"発行: `{service}`" for service in sorted(producers.get(event.id, set()))]
            parts += [f"購読: `{service}`" for service in sorted(consumers.get(event.id, set()))]
            suffix = f"（{'、'.join(parts)}）" if parts else ""
            lines.append(f"- `{event.id}`{suffix}")
        lines.append("")
    return lines


def build_screen_page(screen, incoming: list[tuple[str, dict]], model: Model) -> str:
    lines = [
        HEADER,
        f"# {screen.meta.get('title', screen.id)} (`{screen.id}`)",
        "",
        "> 共通: [共通画面設計書](../mvc-common-design.md)・[ドメイン](../mvc-domain.md)",
        "",
        f"- Route: `{screen.meta.get('route', '')}`",
        f"- Controller/View: `{screen.meta.get('controller', '')}` / `{screen.meta.get('view', '')}`",
        f"- Source: `{screen.meta.get('source_view', '')}`",
        f"- Authentication: `{screen.meta.get('auth', 'anonymous')}`",
        "",
        "## 項目",
        "",
        "| 項目 ID | 表示名 | 型 | データソース | 必須 | 引継ぎ |",
        "| ------- | ------ | -- | ------------ | ---- | ------ |",
    ]
    for field in screen.fields:
        lines.append(
            f"| `{field.get('id', '')}` | {field.get('label', '')} | `{field.get('type', '')}` "
            f"| `{field.get('source', '')}` | {'Yes' if field.get('required') else 'No'} "
            f"| {'Yes' if field.get('carry') else 'No'} |"
        )
    lines.extend(["", "## この画面へ遷移する", "", "| 遷移元 | 契機 | 引き継ぎ項目 |", "| ------ | ---- | ------------ |"])
    for source_id, transition in incoming:
        carry = ", ".join(f"`{value}`" for value in transition.get("carry", []))
        lines.append(f"| `{source_id}` | {transition.get('trigger', '')} | {carry} |")
    lines.extend(["", "## この画面から遷移する", "", "| 遷移先 | 契機 | 引き継ぎ項目 |", "| ------ | ---- | ------------ |"])
    for transition in screen.transitions:
        carry = ", ".join(f"`{value}`" for value in transition.get("carry", []))
        lines.append(
            f"| `{transition.get('to', '')}` | {transition.get('trigger', '')} | {carry} |"
        )
    lines.extend(["", "## イベント", "", "| イベント | 契機 | DB/状態更新 | 外部連携 | 遷移先 | 方式参照 |", "| -------- | ---- | ----------- | -------- | ------ | -------- |"])
    for event in screen.events:
        lines.append(
            f"| `{event.get('id', '')}` | {event.get('trigger', '')} "
            f"| {_updates(event)} | {event.get('external', '')} | `{event.get('transition_to', '')}` | {_policy_refs(event)} |"
        )
    branched = [event for event in screen.events if event.get("branches")]
    if branched:
        lines.extend(["", "### 分岐"])
        for event in branched:
            lines.append("")
            lines.append(f"#### `{event.get('id', '')}`")
            lines.append("")
            lines.extend(_branch_lines(event))
    by_external: dict[str, list[dict]] = {}
    for event in screen.events:
        if event.get("external"):
            by_external.setdefault(str(event["external"]), []).append(event)
    if by_external:
        lines.extend(["", "### 外部連携先別", ""])
        for external in sorted(by_external):
            lines.append(f"#### {external}")
            lines.append("")
            for event in by_external[external]:
                lines.append(f"- `{event.get('id', '')}`（{event.get('trigger', '')}）")
            lines.append("")
    by_store: dict[str, list[tuple[dict, dict]]] = {}
    for event in screen.events:
        for operation in event.get("db_operations", []):
            by_store.setdefault(str(operation.get("store", "")), []).append((event, operation))
    if by_store:
        lines.extend(["", "### DB更新先別", ""])
        for store in sorted(by_store):
            lines.append(f"#### {store}")
            lines.append("")
            for event, operation in by_store[store]:
                fields = ", ".join(operation.get("fields", []))
                lines.append(
                    f"- `{event.get('id', '')}`（{operation.get('operation')}({fields})）"
                )
            lines.append("")
    by_policy: dict[str, list[dict]] = {}
    for event in screen.events:
        for ref in event.get("policy_refs", []):
            by_policy.setdefault(str(ref), []).append(event)
    if by_policy:
        lines.extend(["", "### 方式参照別", ""])
        for policy in sorted(by_policy):
            lines.append(f"#### `{policy}`")
            lines.append("")
            for event in by_policy[policy]:
                lines.append(f"- `{event.get('id', '')}`（{event.get('trigger', '')}）")
            lines.append("")
    lines.extend(_related_domain_lines(screen, model))
    lines.append("")
    return "\n".join(lines)


AREA_TITLES = {
    "auth": "認証認可",
    "state": "状態管理",
    "validation": "検証方式",
    "errors": "エラー方針",
}


def build_app_policy_doc(model: Model) -> str:
    lines = [
        HEADER,
        "# AP処理方式",
        "",
        "全体にかかわる内部設計。画面のイベントから `policy_refs` で参照される。",
        "",
    ]
    by_area: dict[str, list] = {}
    for policy in model.policies:
        by_area.setdefault(policy.area, []).append(policy)
    for area in ("auth", "state", "validation", "errors"):
        lines.append(f"## {AREA_TITLES[area]}")
        lines.append("")
        for policy in sorted(by_area.get(area, []), key=lambda item: item.id):
            lines.append(f"### `{policy.id}` {policy.meta.get('title', '')}")
            lines.append("")
            if policy.meta.get("description"):
                lines.append(policy.meta["description"])
                lines.append("")
            lines.append("| 規則 ID | 内容 |")
            lines.append("| ------- | ---- |")
            for rule in policy.rules:
                lines.append(f"| `{rule.get('id', '')}` | {rule.get('text', '')} |")
            lines.append("")
    return "\n".join(lines)


def build_screen_pages(model: Model) -> dict[str, str]:
    """画面ごとの設計書。キーは generated/ からの相対パス。"""
    incoming = _incoming(model)
    return {
        f"mvc/screens/{screen.id}.md": build_screen_page(screen, incoming.get(screen.id, []), model)
        for screen in model.screens
    }


def build_doc_pages(model: Model) -> dict[str, str]:
    """画面ごとの docs スタブと nav。キーはリポジトリルートからの相対パス。"""
    outputs: dict[str, str] = {}
    names: list[str] = []
    for screen in sorted(model.screens, key=lambda item: item.id):
        filename = f"{screen.id}.md"
        names.append(filename)
        outputs[f"docs/development/mvc/{filename}"] = (
            "---\n"
            f"title: {screen.meta.get('title', screen.id)}\n"
            "---\n"
            "\n"
            "<!-- 自動生成: tools/modelgen generate。手編集禁止。 -->\n"
            "\n"
            f'--8<-- "generated/mvc/screens/{filename}"\n'
        )
    outputs["docs/development/mvc/.pages"] = "nav:\n" + "".join(f"  - {name}\n" for name in names)
    return outputs
