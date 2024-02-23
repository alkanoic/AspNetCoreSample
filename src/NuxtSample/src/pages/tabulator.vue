<template>
  <div ref="table"></div>
</template>

<script setup lang="ts">
  import { TabulatorFull as Tabulator } from "tabulator-tables"; // import Tabulator library

  const TABLE_COLUMN = [
    { title: "Name", field: "name" },
    { title: "Progress", field: "progress", sorter: "number" },
    { title: "Gender", field: "gender" },
    { title: "Rating", field: "rating" },
    { title: "Date Of Birth", field: "dob", align: "center" },
  ];

  const TABLE_DATA = [
    {
      id: 1,
      name: "Stephania Auer",
      progress: 64,
      gender: "male",
      rating: 4,
      dob: "03/08/1989",
    },
    {
      id: 2,
      name: "Mrs. Sebastian Thiel",
      progress: 31,
      gender: "male",
      rating: 1,
      dob: "08/10/1989",
    },
    {
      id: 3,
      name: "Treva Schuster",
      progress: 88,
      gender: "male",
      rating: 3,
      dob: "19/09/1993",
    },
    {
      id: 4,
      name: "Travis Jaskolski",
      progress: 46,
      gender: "male",
      rating: 3,
      dob: "19/09/1990",
    },
    {
      id: 5,
      name: "Melvina Tremblay",
      progress: 84,
      gender: "female",
      rating: 5,
      dob: "19/09/1987",
    },
  ];

  const table = ref(null); // reference to your table element
  const tabulator = ref(null); // variable to hold your table
  const tableData = reactive(TABLE_DATA); // data for table to display

  onMounted(() => {
    // instantiate Tabulator when element is mounted
    tabulator.value = new Tabulator(table.value, {
      data: tableData, // link data to table
      reactiveData: true, // enable data reactivity
      columns: TABLE_COLUMN, // define table columns
    });
  });
</script>

<style>
  /** 細かいデザインは公式で公開されているクラスに付加 */
  .tabulator-header .tabulator-col {
    background-color: #1a9386 !important;
    color: #ffffff;
  }
</style>
