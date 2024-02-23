<template>
  <div class="border-danger mb-2 rounded border p-2">
    <div ref="tableElm"></div>

    <div class="mt-2">
      <button type="button" class="btn-success btn" @click="csvDownload()">
        CsvDownload
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { TabulatorFull as Tabulator } from "tabulator-tables";

  const tabulator = ref<Tabulator | null>(null);
  const tableElm = ref<HTMLElement | null>(null);

  type TableDataRow = {
    id: number;
    value: number;
  };

  interface State {
    list: TableDataRow[];
  }

  const state = reactive<State>({
    list: [
      { id: 1, value: 1 },
      { id: 2, value: 2 },
    ],
  });

  const columns: any[] = [
    { field: "id", title: "id" },
    { field: "value", title: "value" },
  ];

  const csvDownload = () => {
    if (tabulator.value === null) return;
    tabulator.value.download("csv", "sample.csv");
  };

  onMounted(() => {
    if (tableElm.value === null) return;

    tabulator.value = new Tabulator(tableElm.value, {
      columns,
      data: state.list,
      reactiveData: true,
    });
    tabulator.value.on("rowClick", (e: UIEvent, row: any) => {
      console.log(
        `tabulator.value.on('rowClick' value: ${row.getData().value})`
      );
      e.stopPropagation();
    });
  });
</script>
