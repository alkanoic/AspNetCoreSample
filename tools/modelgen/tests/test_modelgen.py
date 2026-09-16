"""modelgen の自己テスト (unittest + jsonschema)。"""

from __future__ import annotations

import json
import shutil
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

import yaml

from modelgen.checks import drift, event_link, relation_inverse, schema_check, service_link, source_drift
from modelgen.cli import _generate, build_context

REPO = Path(__file__).resolve().parents[3]
SCHEMAS = REPO / "schemas"


def entity(
    item_id: str, relations=None, auto_events=None, fields=None, emits=None, table=None
) -> dict:
    data = {
        "kind": "entity",
        "id": item_id,
        "title": item_id.upper(),
        "key": ["id"],
        "fields": fields
        or [{"name": "id", "type": "integer", "key": True, "required": True}],
    }
    if relations:
        data["relations"] = relations
    if auto_events:
        data["auto_events"] = auto_events
    if emits:
        data["emits"] = emits
    if table:
        data["table"] = table
    return data


def service(item_id: str, produces=None, consumes=None) -> dict:
    data = {"kind": "service", "id": item_id, "title": item_id}
    if produces:
        data["produces"] = produces
    if consumes:
        data["consumes"] = consumes
    return data


def event(item_id: str, source: str, data_fields=None, producers=None, consumers=None) -> dict:
    data = {
        "kind": "event",
        "id": item_id,
        "source": source,
        "trigger": "test",
        "data_fields": data_fields or [],
    }
    if producers:
        data["producers"] = producers
    if consumers:
        data["consumers"] = consumers
    return data


def sample_context(entities: list[tuple[str, str, list[tuple[str, str]]]]) -> str:
    blocks = []
    for class_name, table, props in entities:
        lines = [
            f"        modelBuilder.Entity<{class_name}>(entity =>",
            "        {",
            f'            entity.HasKey(e => e.{props[0][0]}).HasName("{table}_pkc");',
            "",
            f'            entity.ToTable("{table}");',
            "",
        ]
        for prop, column in props:
            lines.append(f"            entity.Property(e => e.{prop})")
            lines.append('                .HasColumnType("int")')
            lines.append(f'                .HasColumnName("{column}");')
        lines.append("        });")
        blocks.append("\n".join(lines))
    return "\n\n".join(blocks) + "\n"


class Project:
    def __init__(self, root: Path) -> None:
        self.root = root

    def write(self, rel: str, data: dict) -> None:
        path = self.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(
            yaml.safe_dump(data, allow_unicode=True, sort_keys=False), encoding="utf-8"
        )

    def write_text(self, rel: str, text: str) -> None:
        path = self.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding="utf-8")

    def context(self):
        return build_context(self.root)

    def generate(self) -> None:
        _generate(self.context())


class ModelgenTest(unittest.TestCase):
    def setUp(self) -> None:
        self._tmp = tempfile.TemporaryDirectory()
        self.root = Path(self._tmp.name)
        (self.root / "schemas").mkdir()
        for name in ("entity", "event"):
            shutil.copy(SCHEMAS / f"{name}.schema.json", self.root / "schemas" / f"{name}.schema.json")
        (self.root / "model" / "entities").mkdir(parents=True)
        (self.root / "model" / "events").mkdir(parents=True)
        self.project = Project(self.root)

    def tearDown(self) -> None:
        self._tmp.cleanup()

    def test_valid_relations_have_no_errors(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", relations=[{"id": "bs", "type": "has-many", "target": "b", "inverse": "a"}]),
        )
        self.project.write(
            "model/entities/b.yml",
            entity("b", relations=[{"id": "a", "type": "belongs-to", "target": "a", "inverse": "bs"}]),
        )
        issues = relation_inverse.run(self.project.context())
        self.assertEqual(issues, [], [i.format() for i in issues])

    def test_missing_inverse_is_error(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", relations=[{"id": "bs", "type": "has-many", "target": "b", "inverse": "a"}]),
        )
        self.project.write("model/entities/b.yml", entity("b"))
        issues = relation_inverse.run(self.project.context())
        self.assertTrue(any(i.check == "relation-inverse" and i.severity == "error" for i in issues))

    def test_wrong_inverse_type_is_error(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", relations=[{"id": "bs", "type": "has-many", "target": "b", "inverse": "a"}]),
        )
        self.project.write(
            "model/entities/b.yml",
            entity("b", relations=[{"id": "a", "type": "has-many", "target": "a", "inverse": "bs"}]),
        )
        issues = relation_inverse.run(self.project.context())
        self.assertTrue(any(i.check == "relation-inverse" and i.severity == "error" for i in issues))

    def test_event_source_missing_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a"))
        self.project.write("model/events/x.yml", event("a.custom", "missing"))
        issues = event_link.run(self.project.context())
        self.assertTrue(any(i.check == "event-link" and i.severity == "error" for i in issues))

    def test_event_data_field_missing_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a"))
        self.project.write("model/events/x.yml", event("a.custom", "a", ["nope"]))
        issues = event_link.run(self.project.context())
        self.assertTrue(any(i.check == "event-link" and i.severity == "error" for i in issues))

    def test_missing_emits_backlink_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a"))
        self.project.write("model/events/x.yml", event("a.custom", "a"))
        issues = event_link.run(self.project.context())
        self.assertTrue(any(i.check == "event-link" and i.severity == "error" for i in issues))

    def test_emits_backlink_ok(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", emits=["a.custom"]))
        self.project.write("model/events/x.yml", event("a.custom", "a"))
        issues = event_link.run(self.project.context())
        self.assertEqual(issues, [], [i.format() for i in issues])

    def test_schema_violation_is_error(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", fields=[{"name": "id", "type": "badtype", "key": True}]),
        )
        issues = schema_check.run(self.project.context())
        self.assertTrue(any(i.check == "schema" and i.severity == "error" for i in issues))

    def test_auto_generated_events_present(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", auto_events=["created", "deleted"]))
        ctx = self.project.context()
        ids = {e.id for e in ctx.model.events()}
        self.assertEqual(ids, {"a.created", "a.deleted"})
        schema = json.loads(ctx.outputs["generated/events/a.created.json"])
        self.assertIn("id", schema["properties"]["data"]["properties"])

    def test_drift_detected_then_resolved(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", auto_events=["created"]))
        self.project.generate()
        self.assertEqual(drift.run(self.project.context()), [])

        # モデルを変更して再生成しない -> ドリフト
        self.project.write(
            "model/entities/a.yml",
            entity(
                "a",
                auto_events=["created"],
                fields=[
                    {"name": "id", "type": "integer", "key": True, "required": True},
                    {"name": "extra", "type": "string"},
                ],
            ),
        )
        issues = drift.run(self.project.context())
        self.assertTrue(any(i.check == "drift" and i.severity == "error" for i in issues))

        # 再生成すれば解消
        self.project.generate()
        self.assertEqual(drift.run(self.project.context()), [])

    def test_service_reference_missing_event_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", auto_events=["created"]))
        self.project.write("model/services/s.yml", service("s", produces=["a.missing"]))
        issues = service_link.run(self.project.context())
        self.assertTrue(any(i.check == "service-link" and i.severity == "error" for i in issues))

    def test_event_without_producer_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", auto_events=["created"]))
        issues = service_link.run(self.project.context())
        self.assertTrue(any(i.check == "service-link" and i.severity == "error" for i in issues))

    def test_producer_backlink_mismatch_is_error(self) -> None:
        self.project.write("model/entities/a.yml", entity("a", emits=["a.custom"]))
        self.project.write("model/services/s.yml", service("s"))
        self.project.write(
            "model/events/x.yml",
            event("a.custom", "a", producers=["s"]),
        )
        issues = service_link.run(self.project.context())
        self.assertTrue(any(i.check == "service-link" and i.severity == "error" for i in issues))

    def test_source_drift_ok(self) -> None:
        self.project.write_text(
            "src/AspNetCoreSample.DataModel/Models/SampleContext.cs",
            sample_context([("A", "a", [("Id", "id"), ("Name", "name")])]),
        )
        self.project.write(
            "model/entities/a.yml",
            entity(
                "a",
                table="a",
                fields=[
                    {"name": "id", "column": "id", "type": "integer", "key": True, "required": True},
                    {"name": "name", "column": "name", "type": "string", "required": True},
                ],
            ),
        )
        issues = source_drift.run(self.project.context())
        self.assertEqual(issues, [], [i.format() for i in issues])

    def test_source_drift_detects_extra_field(self) -> None:
        self.project.write_text(
            "src/AspNetCoreSample.DataModel/Models/SampleContext.cs",
            sample_context([("A", "a", [("Id", "id")])]),
        )
        self.project.write(
            "model/entities/a.yml",
            entity(
                "a",
                table="a",
                fields=[
                    {"name": "id", "column": "id", "type": "integer", "key": True, "required": True},
                    {"name": "extra", "column": "extra", "type": "string"},
                ],
            ),
        )
        issues = source_drift.run(self.project.context())
        self.assertTrue(any(i.check == "source-drift" and i.severity == "error" for i in issues))

    def test_source_drift_detects_missing_entity(self) -> None:
        self.project.write_text(
            "src/AspNetCoreSample.DataModel/Models/SampleContext.cs",
            sample_context([("A", "a", [("Id", "id")])]),
        )
        self.project.write("model/entities/a.yml", entity("a", table="a"))
        self.project.write("model/entities/b.yml", entity("b", table="b"))
        issues = source_drift.run(self.project.context())
        self.assertTrue(any(i.check == "source-drift" and i.severity == "error" for i in issues))

    def test_mvc_view_drift_detects_unmodeled_view(self) -> None:
        self.project.write_text(
            "src/AspNetCoreSample.Mvc/Views/Test/Index.cshtml",
            "<h1>Test</h1>\n",
        )
        self.project.write_text(
            "src/AspNetCoreSample.Mvc/Controllers/TestController.cs",
            "public class TestController {}\n",
        )
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "test.index",
                        "title": "Test",
                        "route": "/Test/Index",
                        "controller": "Test",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/Test/Index.cshtml",
                    }
                ],
            },
        )
        self.project.write_text(
            "src/AspNetCoreSample.Mvc/Views/Test/Extra.cshtml",
            "<h1>Extra</h1>\n",
        )

        issues = source_drift.run(self.project.context())
        self.assertTrue(any("Extra.cshtml" in i.message for i in issues))

    def test_screen_missing_transition_target_is_error(self) -> None:
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "transitions": [{"to": "missing.index", "trigger": "go"}],
                    }
                ],
            },
        )
        from modelgen.checks import screen_link

        issues = screen_link.run(self.project.context())
        self.assertTrue(any(i.check == "screen-link" and i.severity == "error" for i in issues))

    def test_screen_pages_generated_per_screen(self) -> None:
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "view": "Index",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "fields": [{"id": "name", "label": "名前", "type": "text"}],
                        "transitions": [{"to": "b.index", "trigger": "go", "carry": ["name"]}],
                        "events": [{"id": "a.submit", "trigger": "送信"}],
                    },
                    {
                        "kind": "screen",
                        "id": "b.index",
                        "title": "B",
                        "route": "/B/Index",
                        "controller": "B",
                        "view": "Index",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/B/Index.cshtml",
                        "fields": [{"id": "name", "label": "名前", "type": "text"}],
                    },
                ],
            },
        )
        outputs = self.project.context().outputs
        self.assertIn("generated/mvc/screens/a.index.md", outputs)
        self.assertIn("generated/mvc/screens/b.index.md", outputs)
        page = outputs["generated/mvc/screens/a.index.md"]
        self.assertIn("# A (`a.index`)", page)
        self.assertIn("`name`", page)
        self.assertIn("`b.index`", page)
        self.assertIn("docs/development/mvc/a.index.md", outputs)
        stub = outputs["docs/development/mvc/a.index.md"]
        self.assertIn('--8<-- "generated/mvc/screens/a.index.md"', stub)
        pages = outputs["docs/development/mvc/.pages"]
        self.assertIn("a.index.md", pages)
        self.assertIn("b.index.md", pages)

    def test_combined_docs_have_no_per_screen_sections(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity(
                "a",
                table="a",
                fields=[{"name": "id", "type": "integer"}],
                auto_events=["created"],
            ),
        )
        self.project.write(
            "model/services/s.yml",
            service("s", produces=["a.created"]),
        )
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "view": "Index",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "fields": [{"id": "name", "label": "名前", "type": "text"}],
                        "events": [
                            {
                                "id": "a.submit",
                                "trigger": "送信",
                                "external": "ExtApi",
                                "policy_refs": ["p.ok"],
                                "db_operations": [
                                    {"store": "a", "operation": "read", "fields": ["id"]}
                                ],
                            }
                        ],
                    }
                ],
            },
        )
        outputs = self.project.context().outputs
        self.assertNotIn("generated/mvc/screen-flow.md", outputs)
        self.assertNotIn("generated/mvc/screen-items.md", outputs)
        self.assertNotIn("generated/mvc/screen-events.md", outputs)
        common = outputs["generated/mvc/common-design.md"]
        self.assertIn("# 共通の画面設計書", common)
        self.assertIn("## 画面遷移図", common)
        self.assertIn("## 項目の凡例と規約", common)
        self.assertIn("## イベント命名規約", common)
        self.assertIn("## DB操作種別", common)
        self.assertNotIn("## 遷移と引き継ぎ項目", common)
        self.assertNotIn("## 外部連携先別イベント", common)
        self.assertNotIn("## DB更新先別イベント", common)
        self.assertNotIn("## 方式参照別イベント", common)
        self.assertNotIn("## A (`a.index`)", common)
        page = outputs["generated/mvc/screens/a.index.md"]
        self.assertIn("### 外部連携先別", page)
        self.assertIn("#### ExtApi", page)
        self.assertIn("### DB更新先別", page)
        self.assertIn("#### a", page)
        self.assertIn("### 方式参照別", page)
        self.assertIn("#### `p.ok`", page)
        self.assertNotIn("generated/mvc/domain.md", outputs)
        graph = outputs["generated/model-graph.md"]
        self.assertIn("## エンティティ一覧", graph)
        self.assertIn("| `a` | `a` |", graph)
        self.assertNotIn("## サービス一覧", graph)
        self.assertNotIn("erDiagram", graph)
        self.assertNotIn("flowchart", graph)
        self.assertNotIn("主キー", graph)
        self.assertNotIn("data_fields", graph)
        page = outputs["generated/mvc/screens/a.index.md"]
        self.assertIn("../mvc-common-design.md", page)
        self.assertIn("../mvc-domain.md", page)
        self.assertNotIn("mvc-screen-items.md", page)
        self.assertIn("## 関連ドメインイベント", page)
        self.assertIn("### A (`a`)", page)
        self.assertIn("- `a.created`（発行: `s`）", page)

    def test_policy_ref_missing_is_error(self) -> None:
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "events": [
                            {"id": "a.submit", "trigger": "送信", "policy_refs": ["missing.policy"]}
                        ],
                    }
                ],
            },
        )
        from modelgen.checks import screen_link

        issues = screen_link.run(self.project.context())
        self.assertTrue(
            any(
                i.check == "screen-link"
                and i.severity == "error"
                and "missing.policy" in i.message
                for i in issues
            )
        )

    def test_policy_ref_resolves(self) -> None:
        self.project.write(
            "model/app_policy/p.yml",
            {"kind": "app_policy", "id": "p.ok", "title": "P", "area": "auth"},
        )
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "events": [{"id": "a.submit", "trigger": "送信", "policy_refs": ["p.ok"]}],
                    }
                ],
            },
        )
        from modelgen.checks import screen_link

        issues = screen_link.run(self.project.context())
        self.assertEqual([i for i in issues if i.check == "screen-link"], [])

    def test_branches_rendered_in_screen_page(self) -> None:
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                        "events": [
                            {
                                "id": "a.submit",
                                "trigger": "送信",
                                "policy_refs": ["p.ok"],
                                "branches": [{"when": "invalid", "result": "redisplay"}],
                            }
                        ],
                    }
                ],
            },
        )
        outputs = self.project.context().outputs
        page = outputs["generated/mvc/screens/a.index.md"]
        self.assertIn("`p.ok`", page)
        self.assertIn("`invalid` → redisplay", page)

    def test_app_policy_doc_generated(self) -> None:
        self.project.write(
            "model/app_policy/p.yml",
            {
                "kind": "app_policy",
                "id": "p.ok",
                "title": "P",
                "area": "validation",
                "rules": [{"id": "r1", "text": "規則内容"}],
            },
        )
        outputs = self.project.context().outputs
        self.assertIn("generated/app-policy.md", outputs)
        doc = outputs["generated/app-policy.md"]
        self.assertIn("## 検証方式", doc)
        self.assertIn("`p.ok`", doc)
        self.assertIn("規則内容", doc)

    def test_docs_stub_drift_detected(self) -> None:
        self.project.write(
            "model/screens.yml",
            {
                "kind": "screen_catalog",
                "screens": [
                    {
                        "kind": "screen",
                        "id": "a.index",
                        "title": "A",
                        "route": "/A/Index",
                        "controller": "A",
                        "source_view": "src/AspNetCoreSample.Mvc/Views/A/Index.cshtml",
                    }
                ],
            },
        )
        self.project.generate()
        self.assertEqual(drift.run(self.project.context()), [])

        stub = self.root / "docs/development/mvc/a.index.md"
        stub.unlink()

        issues = drift.run(self.project.context())
        self.assertTrue(
            any(
                i.check == "drift"
                and i.severity == "error"
                and "docs/development/mvc/a.index.md" in i.message
                for i in issues
            )
        )


    def test_er_diagram_generated_from_a5er(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", table="a", fields=[{"name": "id", "type": "integer"}]),
        )
        self.project.write_text(
            "er.a5er",
            "# A5:ER FORMAT:19\r\n"
            "[Entity]\r\n"
            "PName=a\r\n"
            "LName=A\r\n"
            'Field="id","id","int","NOT NULL",0,"","",$FFFFFFFF,""\r\n'
            "[Relation]\r\n"
            "Entity1=a\r\n"
            "Entity2=b\r\n"
            "Fields1=id\r\n"
            "Fields2=a_id\r\n"
            "RelationType1=2\r\n"
            "RelationType2=3\r\n",
        )
        outputs = self.project.context().outputs
        self.assertNotIn("generated/er-diagram.mmd", outputs)
        graph = outputs["generated/model-graph.md"]
        self.assertIn("erDiagram", graph)
        self.assertIn("    a {", graph)
        self.assertIn("PK", graph)
        self.assertIn('a ||--o{ b : "a_id"', graph)
        self.assertIn("| `a` | `a` |", graph)

    def test_er_table_mismatch_is_error(self) -> None:
        self.project.write(
            "model/entities/a.yml",
            entity("a", table="a", fields=[{"name": "id", "type": "integer"}]),
        )
        self.project.write(
            "model/entities/b.yml",
            entity("b", table="b", fields=[{"name": "id", "type": "integer"}]),
        )
        self.project.write_text(
            "er.a5er",
            "[Entity]\r\nPName=a\r\nLName=A\r\n",
        )
        issues = source_drift.run(self.project.context())
        messages = [i.message for i in issues if i.check == "source-drift"]
        self.assertTrue(any("'b'" in message for message in messages))


if __name__ == "__main__":
    unittest.main()
