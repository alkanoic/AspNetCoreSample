import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";

export default defineNuxtRouteMiddleware((to) => {
  const keycloakStore = useKeycloakAuthStore();
  if (!keycloakStore.keycloak?.authenticated) {
    keycloakStore.login(location.origin + to.path);
    if (!keycloakStore.getToken) {
      return;
    }
    return { path: "/" };
  }
  return;
});
