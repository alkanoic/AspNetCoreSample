// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  // Nuxt modules
  modules: ["@pinia/nuxt", "@vee-validate/nuxt", "@nuxt/ui"],
  ssr: false,
  css: [
    "~/assets/css/main.css",
    "tabulator-tables/dist/css/tabulator_bootstrap5.min.css",
    "@fortawesome/fontawesome-svg-core/styles.css",
  ],
  runtimeConfig: {
    public: {
      keycloakUrl: process.env.NUXT_PUBLIC_KEYCLOAK_URL || "http://keycloak:8080",
      keycloakRealm: "Test",
      keycloakClientId: "spa-client",
      apiBaseUrl: process.env.API_BASE_URL || "https://localhost:7036",
    },
  },
  build: { transpile: ["vue-qrcode-reader"] },
  compatibilityDate: "2026-08-08",
  typescript: { tsConfig: { extends: "@tsconfig/strictest/tsconfig.json" } },
});
