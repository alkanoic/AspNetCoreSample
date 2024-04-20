<template>
  <div>
    <h1>Child</h1>
    <button class="btn-primary btn" @click="handleClick">Click me!</button>
    <h1>parent: {{ parentData }}</h1>
  </div>
</template>

<script setup lang="ts">
  const emit = defineEmits<{ (e: "clickEmit", value?: string): void }>();

  const handleClick = () => {
    emit("clickEmit", "Child to Parent");
  };

  const props = defineProps({
    parentProp: {
      type: String,
      default: "",
    },
  });
  const parentData = ref(props.parentProp);
</script>
