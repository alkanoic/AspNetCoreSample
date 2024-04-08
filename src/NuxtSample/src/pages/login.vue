<template>
  <div class="hero min-h-screen bg-base-200">
    <div v-if="!authStore.isAuthenticated" class="hero-content flex-col">
      <div class="hero-content flex-col">
        <div class="text-center lg:text-left">
          <h1 class="text-5xl font-bold">Login</h1>
        </div>
        <div class="card w-full max-w-sm shrink-0 bg-base-100 shadow-2xl">
          <div class="card-body">
            <div class="form-control mt-6">
              <button class="btn btn-primary" @click="login">Login</button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-else class="hero-content text-center">
      <div class="max-w-md">
        <h1 class="text-5xl font-bold">
          Welcome, {{ authStore.getUsername }}!
        </h1>
        <p class="py-6">You are now logged in.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useAuthStore } from "@/composables/useAuth";

  const authStore = useAuthStore();

  const login = async () => {
    await authStore.login();
  };
</script>
