<template>
  <div>
    <label>logined</label>
    <button class="btn btn-primary" @click="logout">logout</button>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "~/store/authStore"
const authStore = useAuthStore();

definePageMeta({
  middleware: ["auth"],
});

function logout() {
  authStore.logout();
  navigateTo("/login");
}
</script>
