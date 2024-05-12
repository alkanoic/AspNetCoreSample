// src/histoire.setup.ts

import { createPinia } from "pinia";
import { defineSetupVue3 } from "@histoire/plugin-vue";
import "assets/css/main.css";

export const setupVue3 = defineSetupVue3(({ app }) => {
  const pinia = createPinia();
  app.use(pinia);
});
