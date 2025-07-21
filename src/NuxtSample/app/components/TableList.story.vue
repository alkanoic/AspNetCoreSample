<template>
  <Story>
    <Variant title="Default">
      <TableList :headers="headers" :rows="rows" />
    </Variant>
  </Story>
</template>

<script setup lang="ts">
const headers = ["name", "age", "email"];
const rows = [
  { id: 1, name: "John Doe", age: 25, email: "john@example.com" },
  { id: 2, name: "Jane Doe", age: 30, email: "jane@example.com" },
  { id: 3, name: "Bob Smith", age: 40, email: "bob@example.com" },
];
</script>
