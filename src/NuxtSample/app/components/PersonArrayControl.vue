<template>
  <div>
    <h2>PersonArray</h2>
    <div v-for="(person, index) in props.people" :key="index">
      <PersonControl :person="person" />
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  people: {
    type: Array as PersonProps,
    required: true,
  },
});
</script>
