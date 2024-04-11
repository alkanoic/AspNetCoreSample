<template>
  <div class="min-h-screen flex items-center justify-center bg-base-200">
    <div class="card w-96 bg-base-100 shadow-xl">
      <div class="card-body">
        <h2 class="card-title">Login</h2>
        <form @submit.prevent="login">
          <div class="form-control">
            <label class="label">
              <span class="label-text">Username</span>
            </label>
            <input
              v-model="username"
              type="text"
              placeholder="username"
              class="input input-bordered"
              required
            />
          </div>
          <div class="form-control">
            <label class="label">
              <span class="label-text">Password</span>
            </label>
            <input
              v-model="password"
              type="password"
              placeholder="password"
              class="input input-bordered"
              required
            />
          </div>
          <div class="form-control mt-6">
            <button class="btn btn-primary">Login</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
  const username = ref("");
  const password = ref("");

  const login = async () => {
    try {
      const runtimeConfig = useRuntimeConfig();
      // ここでWebAPIに対してユーザー名とパスワードを送信し、アクセストークンを取得する
      const response = await fetch(`${runtimeConfig.public.apiBaseUrl}/auth`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          userName: username.value,
          password: password.value,
        }),
      });
      const data = await response.json();
      if (response.ok) {
        // アクセストークンを保存する
        // アクセストークンをHTTPOnly Cookieに保存
        const cookie = useCookie("access_token", {
          path: "/",
          sameSite: "strict",
          maxAge: 3600, // 1時間
        });
        cookie.value = data.access_token;
        // ログイン後の画面に遷移する
        navigateTo("/");
      } else {
        // エラーメッセージを表示する
        alert(data.error);
      }
    } catch (error) {
      console.error(error);
    }
  };
</script>
