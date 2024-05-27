<template>
  <div>
    <label>logined</label>
    <p>Name: {{ authStore.getName }}</p>
    <p>PreferredUsername: {{ authStore.getPreferredUsername }}</p>
    <p>GivenName: {{ authStore.getGivenName }}</p>
    <p>FamilyName: {{ authStore.getFamilyName }}</p>
    <p>Email: {{ authStore.getEmail }}</p>
    <p>accessToken: {{ authStore.getAccessToken }}</p>
    <p>refreshToken: {{ authStore.getRefreshToken }}</p>
    <p>Roles: {{ authStore.getRoles }}</p>
    <p>details: {{ authStore.getDetails }}</p>
    <p>Expire: {{ authStore.getExpire }}</p>
    <p>Iat: {{ authStore.getIat }}</p>
    <button class="btn btn-primary" @click="logout">logout</button>
    <hr class="my-3" />
    <button class="btn btn-secondary" @click="fetchWebapi">webapi</button>
    <p>{{ webapi }}</p>
    <button class="btn btn-primary" @click="refreshAccessToken">Refresh</button>
    <p>{{ refresh }}</p>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "~/store/authStore"
const authStore = useAuthStore();
const webapi = ref("");
const refresh = ref();

definePageMeta({
  middleware: ["auth"],
});

function logout() {
  authStore.logout();
  navigateTo("/login");
}

async function fetchWebapi() {
  webapi.value = "";
  try {
    const runtimeConfig = useRuntimeConfig();
    const params = {
      sample: "sample"
    };
    const response = await fetch(
      `${runtimeConfig.public.apiBaseUrl}/api/auth/sample?` + new URLSearchParams(params),
      {
        method: "GET",
        headers: {
          "accept": "application/json",
          "Authorization": `Bearer ${authStore.getAccessToken}`
        }
      }
    );
    if (response.ok) {
      const data = await response.json();
      webapi.value = data;
      return true;
    } else {
      console.log("webapi failed");
      return false;
    }
  } catch (error) {
    console.error(error);
    return false;
  }
}

async function refreshAccessToken() {
  if (!await authStore.refreshAccessToken()) {
    refresh.value = "まだ有効期限内です"
    return;
  }
  refresh.value = new Date();
  accessToken.value = authStore.getAccessToken;
  refreshToken.value = authStore.getRefreshToken;
}
</script>
