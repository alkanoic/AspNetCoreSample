export default defineNuxtRouteMiddleware(() => {
  const cookie = useCookie("access_token");
  if (!cookie.value) {
    return { path: "/login" };
  }
});
