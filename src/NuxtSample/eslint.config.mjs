import { createConfigForNuxt } from "@nuxt/eslint-config/flat";
import tailwindcss from "eslint-plugin-tailwindcss";

export default createConfigForNuxt({
  features: {
    stylistic: {
      indent: 2,
      semi: true,
      quotes: "double",
    },
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
    "@stylistic/arrow-parens": "off",
    "@stylistic/brace-style": "off",
    "@stylistic/comma-dangle": "off",
    "@typescript-eslint/no-unused-vars": "warn",
    "vue/first-attribute-linebreak": "off",
    "vue/html-closing-bracket-newline": "off",
    "vue/html-indent": "off",
    "vue/max-attributes-per-line": "off",
    "vue/multi-word-component-names": "off",
    "vue/singleline-html-element-content-newline": "off",
  });
