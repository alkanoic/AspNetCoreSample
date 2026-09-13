"""項目 YAML を JSON Schema で検証する。"""

from __future__ import annotations

from jsonschema import Draft202012Validator

from .base import Context, Issue

NAME = "schema"


def _validate(ctx: Context, schema_name: str, item, issues: list[Issue]) -> None:
    schema = ctx.schemas.get(schema_name)
    if schema is None:
        return
    validator = Draft202012Validator(schema)
    for error in sorted(validator.iter_errors(item.meta), key=lambda e: list(e.path)):
        location = "/".join(str(part) for part in error.path) or "(root)"
        issues.append(Issue(NAME, "error", f"{location}: {error.message}", item.path))


def run(ctx: Context) -> list[Issue]:
    issues: list[Issue] = []

    for path, message in ctx.model.parse_errors:
        issues.append(Issue(NAME, "error", message, path))

    for entity in ctx.model.entities:
        _validate(ctx, "entity", entity, issues)
    for event in ctx.model.explicit_events:
        _validate(ctx, "event", event, issues)

    return issues
