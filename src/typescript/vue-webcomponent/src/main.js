// import { createApp } from 'vue'
// import './style.css'
// import App from './App.vue'

// createApp(App).mount('#app')

import { defineCustomElement } from "vue";
import Hello from "./components/Hello.vue";

const HelloElement = defineCustomElement(Hello);

customElements.define("my-hello", HelloElement);
