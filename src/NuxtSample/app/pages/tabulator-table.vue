<template>
  <TabulatorModal :data-name="dataName" :columns="columns" :row-data="state.list" />
</template>

<script setup lang="ts">
import type { ColumnDefinition } from "tabulator-tables";

const dataName = "sample";
type TableDataRow = {
  id: number;
  value: number;
  name: string;
  date: string;
  other: string;
};

interface State {
  list: TableDataRow[];
}

const state = reactive<State>({
  list: [
    { id: 1, value: 1, name: "abc", date: new Date().toISOString(), other: "aaa" },
    { id: 2, value: 2, name: "abc2", date: new Date().toISOString(), other: "" },
    { id: 5, value: 5, name: "abc5", date: new Date().toISOString(), other: "asc" },
    { id: 4, value: 4, name: "abc4", date: new Date().toISOString(), other: "ascsa" },
    { id: 3, value: 3, name: "abc3", date: new Date().toISOString(), other: "other" },
  ],
});

const columns: ColumnDefinition[] = [
  { field: "id", title: "id", headerFilter: true },
  { field: "value", title: "value", headerFilter: true },
  { field: "name", title: "name-column", headerFilter: true },
  { field: "date", title: "date-column", headerFilter: true },
];
</script>
