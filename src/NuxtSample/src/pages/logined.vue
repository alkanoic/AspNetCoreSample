<template>
  <div>
    <label>logined</label>
    <p>accessToken: {{ accessToken }}</p>
    <p>Roles: {{ roles }}</p>
    <p>Name: {{ name }}</p>
    <p>PreferredUsername: {{ preferredUsername }}</p>
    <p>GivenName: {{ givenName }}</p>
    <p>FamilyName: {{ familyName }}</p>
    <p>Email: {{ email }}</p>
    <p>details: {{ details }}</p>
    <button class="btn btn-primary" @click="logout">logout</button>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "~/store/authStore"
const authStore = useAuthStore();
const accessToken = ref("");
const roles = ref("");
const name = ref("");
const preferredUsername = ref("");
const givenName = ref("");
const familyName = ref("");
const email = ref("");
const details = ref("");

definePageMeta({
  middleware: ["auth"],
});

onMounted(() => {
  accessToken.value = authStore.getAccessToken;
  roles.value = authStore.getRoles;
  name.value = authStore.getName;
  preferredUsername.value = authStore.getPreferredUsername;
  givenName.value = authStore.getGivenName;
  familyName.value = authStore.getFamilyName;
  email.value = authStore.getEmail;
  details.value = authStore.getDetails;
});

function logout() {
  authStore.logout();
  navigateTo("/login");
}
</script>
