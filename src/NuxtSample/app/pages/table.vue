<template>
  <TableList :headers="headers" :rows="rows" />
</template>

<script setup lang="ts">
import { useFruitStore } from "@/store/fruitStore";

const headers = ["name", "age", "email"];
const rows = [
  { id: 1, name: "John Doe", age: 25, email: "john@example.com" },
  { id: 2, name: "Jane Doe", age: 30, email: "jane@example.com" },
  { id: 3, name: "Bob Smith", age: 40, email: "bob@example.com" },
];

const fruitStore = useFruitStore();
const { selectedFruit } = storeToRefs(fruitStore);
watch(() => selectedFruit.value, handleValueChange);
console.log(`page:${selectedFruit.value}`);

function handleValueChange(newValue: string, oldValue: string) {
  console.log(`new:${newValue} old:${oldValue}`);
}
</script>
