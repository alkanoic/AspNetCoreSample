import { defineConfig } from "vite";
import fg from "fast-glob";
import path from "path";

export default defineConfig({
  build: {
    outDir: "wwwroot/dist",
    sourcemap: process.env.NODE_ENV !== "production",
    target: "es2020",
    rollupOptions: {
      input: Object.fromEntries(
        fg
          .sync("{js,css}/*.{ts,js,scss}", {
            ignore: ["**/_*.{ts,js,scss}"],
            cwd: "./vite",
          })
          .map((file) => {
            const { dir, name } = path.parse(file);
            return [`${dir}/${name}`, path.resolve("vite", file)];
          })
      ),
      // 出力CSSファイル名はassetFileNamesで指定する。inputが.scssなら、[ext]には"css"が入る
      output: {
        format: "es",
        entryFileNames: "[name].js",
        chunkFileNames: "assets/[name]-[hash].js",
        assetFileNames: "[name].[ext]",
      },
    },
  },

  // postcssも簡単に指定できる
  //   css: {
  //     postcss: {
  //       plugins: [require("tailwindcss"), require("autoprefixer")],
  //     },
  //   },
});
