"""チェックの登録。追加する場合はここに run 関数を足す。"""

from __future__ import annotations

from . import (
    drift,
    event_link,
    id_unique,
    relation_inverse,
    schema_check,
    service_link,
    screen_link,
    source_drift,
)

CHECKS = [
    schema_check.run,
    id_unique.run,
    relation_inverse.run,
    event_link.run,
    service_link.run,
    screen_link.run,
    source_drift.run,
    drift.run,
]
