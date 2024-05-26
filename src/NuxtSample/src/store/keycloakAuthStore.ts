import { defineStore } from "pinia";
import Keycloak from "keycloak-js";
import { jwtDecode } from "jwt-decode";

interface KeycloakAuthState {
  isAuthenticated: boolean;
  username: string | null;
  firstName: string | null;
  lastName: string | null;
  email: string | null;
  token: string | null;
  refreshToken: string | null;
  roles: string[] | null;
  exp: string | null;
  iat: string | null;
  keycloak: Keycloak | null;
}

function calcJpTime(exp: number) {
  // expをミリ秒に変換してDateオブジェクトを作成
  const expDate = new Date(exp * 1000);
  // 日本時間（JST）に変換
  const options = { timeZone: "Asia/Tokyo", hour12: false };
  return expDate.toLocaleString("ja-JP", options);
}

export const useKeycloakAuthStore = defineStore("auth", {
  state: (): KeycloakAuthState => ({
    isAuthenticated: false,
    username: null,
    firstName: null,
    lastName: null,
    email: null,
    token: null,
    refreshToken: null,
    roles: null,
    exp: null,
    iat: null,
    keycloak: null,
  }),

  getters: {
    getUsername: (state) => state.username,
    getLastName: (state) => state.lastName,
    getFirstName: (state) => state.firstName,
    getEmail: (state) => state.email,
    getToken: (state) => state.token,
    getRefreshToken: (state) => state.refreshToken,
    getRoles: (state) => state.roles,
    getExpire: (state) => state.exp,
    getIat: (state) => state.iat,
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
          silentCheckSsoFallback: false,
          silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
        });
        if (this.keycloak.authenticated) {
          const profile = await this.keycloak.loadUserProfile();
          this.username = profile.username ?? "";
          this.lastName = profile.lastName ?? "";
          this.firstName = profile.firstName ?? "";
          this.email = profile.email ?? "";
          this.token = this.keycloak.token!;
          this.refreshToken = this.keycloak.refreshToken!;
          this.roles = this.keycloak.realmAccess?.roles || [];
          const decode = jwtDecode(this.token);
          this.exp = calcJpTime(decode.exp!);
          this.iat = calcJpTime(decode.iat!);
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
    async RefreshAccessToken() {
      if (this.keycloak?.isTokenExpired) {
        await this.keycloak?.updateToken();
        this.token = this.keycloak?.token!;
        this.refreshToken = this.keycloak?.refreshToken!;
        const decode = jwtDecode(this.token);
        this.exp = calcJpTime(decode.exp!);
        this.iat = calcJpTime(decode.iat!);
      }
    },
  },
});
