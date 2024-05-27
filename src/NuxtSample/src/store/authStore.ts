import { jwtDecode, type JwtPayload } from "jwt-decode";
import { defineStore } from "pinia";

interface KeycloakJwtPayload extends JwtPayload {
  realm_access: {
    roles: string[];
  };
  name: string;
  preferred_username: string;
  given_name: string;
  family_name: string;
  email: string;
}

function getAccessTokenDecode(accessToken: string): KeycloakJwtPayload {
  return jwtDecode<KeycloakJwtPayload>(accessToken);
}

export const useAuthStore = defineStore("auth", {
  state: () => ({
    accessToken: "",
    refreshToken: "",
  }),
  getters: {
    isAuthenticate: (state): boolean => (state.accessToken ? true : false),
    getAccessToken: (state): string => state.accessToken ?? "",
    getRefreshToken: (state): string => state.refreshToken ?? "",
    getRoles: (state): string[] =>
      getAccessTokenDecode(state.accessToken ?? "").realm_access.roles,
    getName: (state) => getAccessTokenDecode(state.accessToken ?? "").name,
    getPreferredUsername: (state) =>
      getAccessTokenDecode(state.accessToken ?? "").preferred_username,
    getGivenName: (state) =>
      getAccessTokenDecode(state.accessToken ?? "").given_name,
    getFamilyName: (state) =>
      getAccessTokenDecode(state.accessToken ?? "").family_name,
    getEmail: (state) => getAccessTokenDecode(state.accessToken ?? "").email,
    getDetails: (state) => getAccessTokenDecode(state.accessToken ?? ""),
  },

  actions: {
    async login(username: string, password: string): Promise<boolean> {
      try {
        const runtimeConfig = useRuntimeConfig();
        // ここでWebAPIに対してユーザー名とパスワードを送信し、アクセストークンを取得する
        const response = await fetch(
          `${runtimeConfig.public.apiBaseUrl}/api/auth`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              userName: username,
              password: password,
            }),
          }
        );
        if (response.ok) {
          const data = await response.json();
          // アクセストークンをHTTPOnly Cookieに保存
          const cookie = useCookie("access_token", {
            path: "/",
            sameSite: "strict",
            maxAge: 3600, // 1時間
          });
          cookie.value = data.access_token;
          this.accessToken = data.access_token;
          this.refreshToken = data.refresh_token;

          return true;
        } else {
          console.log("login failed");
          return false;
        }
      } catch (error) {
        console.error(error);
        return false;
      }
    },
    logout() {
      const cookie = useCookie("access_token");
      cookie.value = null;
      this.accessToken = "";
      this.refreshToken = "";
    },
    async refreshAccessToken() {
      try {
        const runtimeConfig = useRuntimeConfig();
        // ここでWebAPIに対してユーザー名とパスワードを送信し、アクセストークンを取得する
        const response = await fetch(
          `${runtimeConfig.public.apiBaseUrl}/api/auth/updatetoken`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              refreshToken: this.refreshToken,
            }),
          }
        );
        if (response.ok) {
          const data = await response.json();
          // アクセストークンをHTTPOnly Cookieに保存
          const cookie = useCookie("access_token", {
            path: "/",
            sameSite: "strict",
            maxAge: 3600, // 1時間
          });
          cookie.value = data.access_token;
          this.accessToken = data.access_token;
          this.refreshToken = data.refresh_token;

          return true;
        } else {
          console.log("token update failed");
          return false;
        }
      } catch (error) {
        console.error(error);
        return false;
      }
    },
  },
});
