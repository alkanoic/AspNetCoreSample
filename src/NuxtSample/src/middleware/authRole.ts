import { useAuthStore } from "~/store/authStore";

export default defineNuxtRouteMiddleware(() => {
  const authStore = useAuthStore();
  if (authStore.getRoles.includes("admin")) {
    return;
  }
  return { path: "/login-access-denied" };
});
