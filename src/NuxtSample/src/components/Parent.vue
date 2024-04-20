<template>
  <div>
    <h1>Parent</h1>
    <Child :parent-prop="parentData" @click-emit="click" />
    <h1>child data: {{ childData }}</h1>
  </div>
</template>

<script setup lang="ts">
  import Child from "./Child.vue";

  const parentData = ref("input parent data");
  const childData = ref("");
  const click = (value?: string) => {
    if (value === undefined || value === null) return;
    childData.value = value;
  };
</script>
