<template>
  <div>
    <div>
      <button class="btn-primary btn" @click="increment()">increment</button>
      <button class="btn-secondary btn" @click="reset()">reset</button>
    </div>
    <div>count: {{ counter }}</div>
    <div>double: {{ double }}</div>
  </div>
</template>

<script setup lang="ts">
  const counter = ref(0);
  const double = ref(0);

  function increment() {
    counter.value += 1;
    double.value += 2;
  }

  function reset() {
    counter.value = 0;
    double.value = 0;
  }
</script>
