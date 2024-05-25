import { useAuthStore } from "~/store/authStore";

export default defineNuxtRouteMiddleware(() => {
  const authStore = useAuthStore();
  if (!authStore.isAuthenticate) {
    return { path: "/login" };
  }
  return;
});
