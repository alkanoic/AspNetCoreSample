/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./app/**/*.html", "./app/**/*.vue", "./app/**/*.jsx"],
  theme: { extend: {} },
  plugins: [require("daisyui")],
  daisyui: {
    logs: false,
    themes: [
      {
        mytheme: {
          primary: "#047AFF",
          secondary: "#463AA2",
          accent: "#C148AC",
          neutral: "#021431",
          "base-100": "#FFFFFF",
          info: "#93E7FB",
          success: "#81CFD1",
          warning: "#EFD7BB",
          error: "#E58B8B",
        },
      },
    ],
  },
};
