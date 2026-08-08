import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";

export default defineNuxtRouteMiddleware(() => {
  const keycloakStore = useKeycloakAuthStore();
  if (keycloakStore.getRoles?.includes("admin")) {
    return;
  }
  return { path: "/keycloak-access-denied" };
});
