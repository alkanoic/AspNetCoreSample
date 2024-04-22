<template>
  <dialog class="modal" :open="isOpen">
    <div class="modal-box">
      <h3 class="text-lg font-bold">{{ title }}</h3>
      <p class="py-4">{{ message }}</p>
    </div>
    <form method="dialog" class="modal-backdrop">
      <button @click="closeModal"></button>
    </form>
  </dialog>
</template>

<script setup>
  defineProps({
    isOpen: {
      type: Boolean,
      required: true,
    },
    title: {
      type: String,
      required: false,
      default: "",
    },
    message: {
      type: String,
      required: false,
      default: "",
    },
  });

  const emit = defineEmits(["close"]);
  const closeModal = () => {
    emit("close");
  };
</script>
