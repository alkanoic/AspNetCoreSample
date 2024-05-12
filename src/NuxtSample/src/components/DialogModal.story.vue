<template>
  <Story>
    <Variant title="Default">
      <button class="btn" @click="openDialog">Open Modal</button>
      <DialogModal
        :is-open="showModal"
        :title="title"
        :message="message"
        @close="closeDialog"
      ></DialogModal>
    </Variant>
  </Story>
</template>

<script setup lang="ts">
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
