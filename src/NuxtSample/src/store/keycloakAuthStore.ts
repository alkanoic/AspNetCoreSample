import { defineStore } from "pinia";
import Keycloak from "keycloak-js";

interface KeycloakAuthState {
  isAuthenticated: boolean;
  username: string | null;
  firstName: string | null;
  lastName: string | null;
  email: string | null;
  token: string | null;
  roles: string[] | null;
  keycloak: Keycloak | null;
}

export const useKeycloakAuthStore = defineStore("auth", {
  state: (): KeycloakAuthState => ({
    isAuthenticated: false,
    username: null,
    firstName: null,
    lastName: null,
    email: null,
    token: null,
    roles: null,
    keycloak: null,
  }),

  getters: {
    getUsername: (state) => state.username,
    getLastName: (state) => state.lastName,
    getFirstName: (state) => state.firstName,
    getEmail: (state) => state.email,
    getToken: (state) => state.token,
    getRoles: (state) => state.roles,
  },

  actions: {
    async login(redirectUri: string) {
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
          this.username = profile.username ?? "";
          this.lastName = profile.lastName ?? "";
          this.firstName = profile.firstName ?? "";
          this.email = profile.email ?? "";
          this.token = this.keycloak.token!;
          this.roles = this.keycloak.realmAccess?.roles || [];
        } else {
          await this.keycloak.login({
            redirectUri: redirectUri,
          });
        }
      } catch (error) {
        console.error("Authentication failed:", error);
      }
    },
    async Logout(redirectUri: string) {
      await this.keycloak?.logout({
        redirectUri: redirectUri,
      });
    },
  },
});
