<template>
  <div>
    <button class="btn" @click="openDialog">Open Modal</button>
    <DialogModal :is-open="showModal" :title="title" :message="message" @close="closeDialog" />
  </div>
</template>

<script setup>
import { ref } from "vue";
import DialogModal from "@/components/DialogModal.vue";

const showModal = ref(false);
const title = ref("");
const message = ref("");

const openDialog = () => {
  showModal.value = true;
  title.value = "aa";
  message.value = "aaa";
};

const closeDialog = () => {
  showModal.value = false;
};
</script>
