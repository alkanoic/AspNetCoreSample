<template>
  <div>
    <h1 class="text-secondary">Person Control</h1>
    <p>Name: {{ person.Name }}</p>
    <p>Age: {{ person.Age }}</p>
    <button class="btn btn-sm btn-primary" @click="personClick()">Click</button>
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

  const emit = defineEmits<{
    (event: "personClick", person: PersonProps): void;
  }>();

  const personClick = (): void => {
    emit("personClick", props.person);
  };
</script>
