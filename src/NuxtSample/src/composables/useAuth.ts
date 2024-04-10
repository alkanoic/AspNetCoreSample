import { defineStore } from "pinia";
import Keycloak from "keycloak-js";

interface AuthState {
  isAuthenticated: boolean;
  accessToken: string | null;
  username: string | null;
}

export const useAuthStore = defineStore("auth", {
  state: (): AuthState => ({
    isAuthenticated: false,
    accessToken: null,
    username: null,
    keycloak: null,
  }),

  getters: {
    getUsername: (state) => state.username,
  },

  actions: {
    async login() {
      const runtimeConfig = useRuntimeConfig();
      this.keycloak = new Keycloak({
        url: runtimeConfig.public.keycloakUrl,
        realm: runtimeConfig.public.keycloakRealm,
        clientId: runtimeConfig.public.keycloakClientId,
      });

      try {
        await this.keycloak.init({
          onLoad: "check-sso",
          silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
        });
        if (this.keycloak.authenticated) {
          const profile = await this.keycloak.loadUserProfile();
          console.log(profile);
          console.log(this.keycloak.token);
        } else {
          await this.keycloak.login();
        }
      } catch (error) {
        console.error("Authentication failed:", error);
      }
    },
  },
});
