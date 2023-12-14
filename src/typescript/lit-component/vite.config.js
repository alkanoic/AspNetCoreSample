// vite.config.js

import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    rollupOptions: {
      output: {
        entryFileNames: 'entry.js', // エントリーファイルの名前の指定
        chunkFileNames: 'chunk.js', // チャンクファイルの名前の指定
        assetFileNames: 'asset.[ext]', // アセットファイルの名前の指定
      },
    },
  },
});
