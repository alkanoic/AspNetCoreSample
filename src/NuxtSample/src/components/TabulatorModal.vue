<template>
  <div class="border-danger mb-2 rounded border p-2">
    <div class="my-2 grid grid-cols-3 gap-x-5">
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
    <p>件数：{{ count }}</p>
    <div ref="tableElement"></div>
    <DialogModal :is-open="showModal" :title="title" :message="message" @close="closeDialog"></DialogModal>
  </div>
</template>

<script setup lang="ts">
import { TabulatorFull as Tabulator, type ColumnDefinition, type DownloadType } from "tabulator-tables";
import DialogModal from "@/components/DialogModal.vue";

const tabulator = ref<Tabulator | null>(null);
const tableElement = ref<HTMLElement | null>(null);
const showModal = ref(false);
const title = ref("");
const message = ref("");
const count = ref(0);

const props = defineProps<{
  dataName: string;
  rowData: object[];
  columns: ColumnDefinition[];
}>();

const csvDownload = (type: DownloadType) => {
  if (tabulator.value === null) return;
  tabulator.value.download(type, `${props.dataName}.${type}`);
};

onMounted(() => {
  if (tableElement.value === null) return;

  count.value = props.rowData.length;
  tabulator.value = new Tabulator(tableElement.value, {
    columns: props.columns,
    data: props.rowData,
    movableColumns: true,
    selectableRange: true,
    selectableRangeRows: true,
    selectableRangeColumns: true,
    clipboard: true,
    clipboardCopyRowRange: "range",
  });
  tabulator.value.on("rowClick", function (_e, row) {
    const detail = row.getData();
    showModal.value = true;
    title.value = `${props.dataName}: ${row.getIndex()}`;
    message.value = JSON.stringify(detail, null, "\t");
  });
});

const closeDialog = () => {
  showModal.value = false;
};
</script>
