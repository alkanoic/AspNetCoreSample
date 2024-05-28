import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { resolve } from "path";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    lib: {
      entry: resolve(__dirname, "src/main.js"),
      name: "MyVueWebComponent",
      fileName: (format) => `my-vue-webcomponent.${format}.js`,
      formats: ["es", "umd"],
    },
    rollupOptions: {
      // 外部依存関係として定義（必要に応じて）
      external: ["vue"],
      output: {
        globals: {
          vue: "Vue",
        },
      },
    },
  },
});
