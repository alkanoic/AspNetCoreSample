<template>
  <div>
    <div>
      <button class="btn btn-primary" @click="increment()">increment</button>
      <button class="btn btn-secondary" @click="reset()">reset</button>
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
