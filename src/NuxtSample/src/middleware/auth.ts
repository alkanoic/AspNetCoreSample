export default defineNuxtRouteMiddleware((to, from) => {
  const cookie = useCookie("access_token");
  if (!cookie.value) {
    return { path: "/login" };
  }
});
