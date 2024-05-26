<template>
  <div>
    <label>keycloak-logined</label>
    <p>UserName: {{ keycloakAuthStore.getUsername }}</p>
    <p>FirstName: {{ keycloakAuthStore.getFirstName }}</p>
    <p>LastName: {{ keycloakAuthStore.getLastName }}</p>
    <p>Email: {{ keycloakAuthStore.getEmail }}</p>
    <p>Token: {{ keycloakAuthStore.getToken }}</p>
    <p>RefreshToken: {{ keycloakAuthStore.getRefreshToken }}</p>
    <p>Roles: {{ keycloakAuthStore.getRoles }}</p>
    <button class="btn btn-primary" @click="logout">logout</button>
  </div>
</template>

<script setup lang="ts">
import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";
const keycloakAuthStore = useKeycloakAuthStore();

definePageMeta({
  middleware: "keycloak-auth",
});

async function logout() {
  await keycloakAuthStore.Logout(location.origin);
}
</script>
