<template>
  <div>
    <div class="navbar bg-gray-800 text-white">
      <div class="navbar-start">
        <div class="dropdown lg:hidden">
          <label for="my-drawer" tabindex="0" class="btn btn-circle btn-ghost">
            <svg xmlns="http://www.w3.org/2000/svg" class="size-5" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h7" />
            </svg>
          </label>
        </div>
      </div>
      <div class="navbar-center">
        <NuxtLink class="btn btn-ghost text-xl normal-case" to="/">NuxtSample</NuxtLink>
      </div>
      <div class="navbar-end" />
    </div>
    <div class="drawer-mobile drawer lg:drawer-open">
      <input id="my-drawer" v-model="openedDrawer" type="checkbox" class="drawer-toggle">
      <div class="drawer-content">
        <div class="p-4">
          <slot />
        </div>
      </div>
      <div class="drawer-side">
        <label for="my-drawer" class="drawer-overlay" />
        <ul class="min-h-full w-60 bg-gray-800 p-2">
          <li>
            <label class="mb-4 text-xl text-white lg:hidden">NuxtSample</label>
          </li>
          <li>
            <ul class="menu rounded-box">
              <li>
                <details open>
                  <summary class="menu-item">Tables</summary>
                  <ul>
                    <li class="menu-item">
                      <NuxtLink to="/table" :class="{ active: route.path == '/table' }" @click="closeDrawer">Table
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/tabledetail" :class="{ active: route.path == '/tabledetail' }"
                        @click="closeDrawer">
                        TableDetail
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/tabulator" :class="{ active: route.path == '/tabulator' }" @click="closeDrawer">
                        Tabulator</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/tabulator-table" :class="{ active: route.path == '/tabulator-table' }"
                        @click="closeDrawer">
                        TabulatorTable</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/tantable" :class="{ active: route.path == '/tantable' }" @click="closeDrawer">
                        TanStackTable</NuxtLink>
                    </li>
                  </ul>
                </details>
              </li>
              <li>
                <details open>
                  <summary class="menu-item">Counters</summary>
                  <ul>
                    <li class="menu-item">
                      <NuxtLink to="/ref-counter" :class="{ active: route.path == '/ref-counter' }"
                        @click="closeDrawer">
                        RefCounter</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/state-counter" :class="{ active: route.path == '/state-counter' }"
                        @click="closeDrawer">
                        StateCounter</NuxtLink>
                    </li>
                  </ul>
                </details>
              </li>
              <li>
                <details open>
                  <summary class="menu-item">Auth</summary>
                  <ul>
                    <li class="menu-item">
                      <NuxtLink to="/login" :class="{ active: route.path == '/login' }" @click="closeDrawer">Login
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/logined" :class="{ active: route.path == '/logined' }" @click="closeDrawer">Logined
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/login-admin" :class="{ active: route.path == '/login-admin' }"
                        @click="closeDrawer">
                        LoginAdminPage</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/keycloak-logined" :class="{ active: route.path == '/keycloak-logined' }"
                        @click="closeDrawer">
                        KeycloakLogined</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/keycloak-admin" :class="{ active: route.path == '/keycloak-admin' }"
                        @click="closeDrawer">
                        KeycloakAdminPage</NuxtLink>
                    </li>
                  </ul>
                </details>
              </li>
              <li>
                <details>
                  <summary class="menu-item">Others</summary>
                  <ul>
                    <li class="menu-item">
                      <NuxtLink to="/parent-child" :class="{ active: route.path == '/parent-child' }"
                        @click="closeDrawer">
                        ParentChild</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/person" :class="{ active: route.path == '/person' }" @click="closeDrawer">Person
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/qrod" :class="{ active: route.path == '/qrod' }" @click="closeDrawer">QROD
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/modal" :class="{ active: route.path == '/modal' }" @click="closeDrawer">Modal
                      </NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/accordion" :class="{ active: route.path == '/accordion' }" @click="closeDrawer">
                        Accordion</NuxtLink>
                    </li>
                    <li class="menu-item">
                      <NuxtLink to="/validate" :class="{ active: route.path == '/validate' }" @click="closeDrawer">
                        Validate
                      </NuxtLink>
                    </li>
                  </ul>
                </details>
              </li>
            </ul>
          </li>
          <li class="px-2">
            <select id="fruit-select" v-model="selectedFruit" class="select select-bordered w-full max-w-xs"
              @change="handleChange">
              <option disabled selected value="0">Who shot first?</option>
              <option v-for="fruit in fruits" :key="fruit" :value="fruit">
                {{ fruit }}
              </option>
            </select>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useFruitStore } from "@/store/fruitStore";

const openedDrawer = ref(false);
const closeDrawer = () => {
  openedDrawer.value = false;
};

const route = useRoute();

const fruitStore = useFruitStore();
fruitStore.setDefaults();
const fruits = fruitStore.fruits;
const selectedFruit = ref(0);
function handleChange(this: unknown, event: unknown) {
  console.log("Selected fruit:", event.target.value);
  fruitStore.setSelectedFruit(event.target.value);
}
</script>

<style scoped lang="postcss">
@tailwind utilities;

.menu-item {
  @apply transition-colors rounded-lg text-white hover:bg-gray-500;
}

.menu-item a:active {
  @apply bg-gray-600 text-white;
}

.menu-item a:focus {
  @apply text-white;
}

.menu :where(li ul):before {
  @apply bg-white
}
</style>
