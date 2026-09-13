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
        schema = json.loads(ctx.outputs["events/a.created.json"])
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


if __name__ == "__main__":
    unittest.main()
