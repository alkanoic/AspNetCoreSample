// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  nitro: { preset: "node" },
  css: [
    "@/assets/css/main.css",
    "tabulator-tables/dist/css/tabulator_bootstrap5.min.css",
    "@fortawesome/fontawesome-svg-core/styles.css",
  ],
  postcss: {
    plugins: {
      tailwindcss: {
        exposeConfig: true,
        configPath: "tailwind.config", // 拡張子は不要
      },
    },
  },
  modules: ["@nuxtjs/tailwindcss", "@pinia/nuxt"],
  typescript: { tsConfig: { extends: "@tsconfig/strictest/tsconfig.json" } },
  ssr: false,
  build: { transpile: ["vue-qrcode-reader"] },
  runtimeConfig: {
    public: {
      keycloakUrl: "http://keycloak:8080",
      keycloakRealm: "Test",
      keycloakClientId: "spa-client",
      apiBaseUrl: process.env.API_BASE_URL || "https://localhost:7036",
    },
  },
});
