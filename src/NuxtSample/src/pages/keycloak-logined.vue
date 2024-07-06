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
    <p>Expire: {{ keycloakAuthStore.getExpire }}</p>
    <p>Iat: {{ keycloakAuthStore.getIat }}</p>
    <button class="btn btn-primary" @click="logout">logout</button>
    <hr class="my-3">
    <button class="btn btn-secondary" @click="fetchWebapi">webapi</button>
    <p>{{ webapi }}</p>
    <button class="btn btn-primary" @click="refreshAccessToken">Refresh</button>
    <p>{{ refresh }}</p>
  </div>
</template>

<script setup lang="ts">
import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";

const keycloakAuthStore = useKeycloakAuthStore();
const webapi = ref("");
const refresh = ref();

definePageMeta({
  middleware: "keycloak-auth",
});

async function logout() {
  await keycloakAuthStore.Logout(location.origin);
}

async function fetchWebapi() {
  webapi.value = "";
  try {
    const runtimeConfig = useRuntimeConfig();
    const params = {
      sample: "sample",
    };
    const response = await fetch(
      `${runtimeConfig.public.apiBaseUrl}/api/auth/sample?` + new URLSearchParams(params),
      {
        method: "GET",
        headers: {
          accept: "application/json",
          Authorization: `Bearer ${keycloakAuthStore.getToken}`,
        },
      },
    );
    if (response.ok) {
      const data = await response.json();
      webapi.value = data;
      return true;
    }
    else {
      console.log("webapi failed");
      return false;
    }
  }
  catch (error) {
    console.error(error);
    return false;
  }
}

async function refreshAccessToken() {
  await keycloakAuthStore.RefreshAccessToken();
  refresh.value = new Date();
}
</script>
