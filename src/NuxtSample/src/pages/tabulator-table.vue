<template>
  <div class="border-danger mb-2 rounded border p-2">
    <div ref="tableElement"></div>

    <div class="mt-2">
      <button type="button" class="btn btn-success" @click="csvDownload('csv')">
        CsvDownload
      </button>
      <button type="button" class="btn btn-success" @click="csvDownload('xlsx')">
        XlsxDownload
      </button>
      <button type="button" class="btn btn-success" @click="csvDownload('json')">
        JsonDownload
      </button>
    </div>
    <DialogModal :is-open="showModal" :title="title" :message="message" @close="closeDialog"></DialogModal>
  </div>
</template>

<script setup lang="ts">
import { TabulatorFull as Tabulator, type DownloadType } from "tabulator-tables";
import DialogModal from "@/components/DialogModal.vue";

const tabulator = ref<Tabulator | null>(null);
const tableElement = ref<HTMLElement | null>(null);
const showModal = ref(false);
const title = ref("");
const message = ref("");

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

const columns: any[] = [
  { field: "id", title: "id", headerFilter: true, topCalc: "count" },
  { field: "value", title: "value", headerFilter: true },
  { field: "name", title: "name-column", headerFilter: true },
  { field: "date", title: "date-column", headerFilter: true },
];

const csvDownload = (type: DownloadType) => {
  if (tabulator.value === null) return;
  tabulator.value.download(type, `sample.${type}`);
};

onMounted(() => {
  if (tableElement.value === null) return;

  tabulator.value = new Tabulator(tableElement.value, {
    columns,
    data: state.list,
    reactiveData: true,
    movableColumns: true,
    selectableRange: true,
    selectableRangeRows: true,
    selectableRangeColumns: true,
    clipboard: true,
    clipboardCopyRowRange: "range",
  });
  tabulator.value.on("rowClick", function (e, row) {
    const detail = row.getData();
    showModal.value = true;
    title.value = row.getIndex();
    message.value = JSON.stringify(detail);
  });
});

const closeDialog = () => {
  showModal.value = false;
};
</script>
