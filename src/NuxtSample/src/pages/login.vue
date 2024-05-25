<template>
  <div class="flex min-h-screen items-center justify-center bg-base-200">
    <div class="card w-96 bg-base-100 shadow-xl">
      <div class="card-body">
        <h2 class="card-title">Login</h2>
        <p class="text text-error">{{ error }}</p>
        <form @submit.prevent="login">
          <div class="form-control">
            <label class="label">
              <span class="label-text">Username</span>
            </label>
            <input v-model="username" type="text" placeholder="username" class="input input-bordered" required />
          </div>
          <div class="form-control">
            <label class="label">
              <span class="label-text">Password</span>
            </label>
            <input v-model="password" type="password" placeholder="password" class="input input-bordered" required />
          </div>
          <div class="form-control mt-6">
            <button class="btn btn-primary">Login</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "~/store/authStore"
const authStore = useAuthStore();
const username = ref("");
const password = ref("");
const error = ref("");

onMounted(() => {
  if (authStore.isAuthenticate) {
    navigateTo("/logined");
  }
});

async function login() {
  error.value = "";
  const result = await authStore.login(username.value, password.value);
  if (result) {
    navigateTo("/logined");
  } else {
    error.value = "login failed: username or password is invalid";
    username.value = "";
    password.value = "";
  }
}
</script>
