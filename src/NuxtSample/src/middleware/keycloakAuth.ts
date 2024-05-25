import { useKeycloakAuthStore } from "~/store/keycloakAuthStore";

/**
 * なぜかasync、awaitできないためPromiseで記述
 * middlewareのもとの呼び出しに問題ありそう
 */
export default defineNuxtRouteMiddleware((to) => {
  const keycloakStore = useKeycloakAuthStore();
  if (!keycloakStore.keycloak?.authenticated) {
    return keycloakStore
      .login(location.origin + to.path)
      .then(() => {
        if (!keycloakStore.getToken) {
          return;
        }
      })
      .catch((error) => {
        console.log(error);
      })
      .finally(() => {
        return { path: "/" };
      });
  }
  return;
});
