<template>
  <TableList :headers="headers" :rows="rows" />
</template>

<script setup>
  const headers = ["Name", "Age", "Email"];
  const rows = [
    { id: 1, name: "John Doe", age: 25, email: "john@example.com" },
    { id: 2, name: "Jane Doe", age: 30, email: "jane@example.com" },
    { id: 3, name: "Bob Smith", age: 40, email: "bob@example.com" },
  ];
</script>
