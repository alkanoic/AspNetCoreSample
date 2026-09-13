"""チェックの共通型。"""

from __future__ import annotations

from dataclasses import dataclass, field
from pathlib import Path
from typing import Any

from ..model import Model


@dataclass
class Issue:
    check: str
    severity: str  # "error" | "warning"
    message: str
    path: str | None = None

    def format(self) -> str:
        location = self.path or "<project>"
        return f"[{self.severity.upper()}] {location}: {self.message} ({self.check})"


@dataclass
class Context:
    root: Path
    model: Model
    schemas: dict[str, dict[str, Any]]
    outputs: dict[str, str] = field(default_factory=dict)
