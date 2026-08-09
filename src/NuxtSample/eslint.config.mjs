import { createConfigForNuxt } from "@nuxt/eslint-config/flat";
import tailwindcss from "eslint-plugin-tailwindcss";

export default createConfigForNuxt({
  features: {
    stylistic: false,
    typescript: {
      strict: true,
    },
  },
})
  // @ts-ignore
  .append({
    plugins: {
      tailwindcss,
    },
    rules: {
      "tailwindcss/classnames-order": "warn",
      "tailwindcss/enforces-negative-arbitrary-values": "warn",
      "tailwindcss/enforces-shorthand": "warn",
      "tailwindcss/migration-from-tailwind-2": "warn",
      "tailwindcss/no-arbitrary-value": "off",
      "tailwindcss/no-contradicting-classname": "error",
    },
  })
  .overrideRules({
    "@typescript-eslint/no-unused-vars": "warn",
    "vue/html-self-closing": [
      "warn",
      {
        html: {
          void: "any",
        },
      },
    ],
    "vue/multi-word-component-names": "off",
    "vue/singleline-html-element-content-newline": "off",
  });
