import { defineStore } from "pinia";

interface AuthState {
  accessToken: string | null;
  roles: string[] | null;
}

export const useAuthStore = defineStore("auth", {
  state: (): AuthState => ({
    accessToken: null,
    roles: null,
  }),

  getters: {
    isAuthenticate: (): boolean => {
      const cookie = useCookie("access_token");
      return cookie.value ? true : false;
    },
    getAccessToken: (state) => state.accessToken,
    getRoles: (state) => state.roles,
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
        const data = await response.json();
        if (response.ok) {
          // アクセストークンをHTTPOnly Cookieに保存
          const cookie = useCookie("access_token", {
            path: "/",
            sameSite: "strict",
            maxAge: 3600, // 1時間
          });
          cookie.value = data.access_token;

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
    },
  },
});
