<template>
  <div>
    <h1>Person Control</h1>
    <p>Name: {{ person.Name }}</p>
    <p>Age: {{ person.Age }}</p>
  </div>
</template>

<script setup lang="ts">
  export interface PersonProps {
    Name: string;
    Age: int;
  }

  const props = defineProps({
    person: {
      type: Object as PersonProps,
      required: true,
    },
  });
</script>
