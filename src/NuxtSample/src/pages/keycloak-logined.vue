<template>
  <div>
    <label>keycloak-logined</label>
    <p>UserName: {{ keycloakAuthStore.getUsername }}</p>
    <p>FirstName: {{ keycloakAuthStore.getFirstName }}</p>
    <p>LastName: {{ keycloakAuthStore.getLastName }}</p>
    <p>Email: {{ keycloakAuthStore.getEmail }}</p>
    <p>Token: {{ keycloakAuthStore.getToken }}</p>
  </div>
</template>

<script setup lang="ts">
import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";
const keycloakAuthStore = useKeycloakAuthStore();

definePageMeta({
  middleware: ["keycloak-auth"],
});

</script>
