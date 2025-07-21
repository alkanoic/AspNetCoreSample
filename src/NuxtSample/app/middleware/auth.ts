import { useAuthStore } from "~/store/authStore";

export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore();
  if (!authStore.isAuthenticate) {
    return navigateTo(`/login?redirectUri=${to.path}`);
  }
  return;
});
