<template>
  <div>
    <div
      v-for="(item, index) in items"
      :key="index"
      class="collapse collapse-arrow bg-base-200"
      :class="{ 'mt-2': index >= 1 }"
    >
      <input type="checkbox" />
      <div class="collapse-title text-xl font-medium">
        Click to open this one and close others
      </div>
      <div class="collapse-content">
        <p>{{ item.content }}</p>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
  const items = ref([
    { content: "hello1" },
    { content: "hello2" },
    { content: "hello3" },
  ]);
</script>
