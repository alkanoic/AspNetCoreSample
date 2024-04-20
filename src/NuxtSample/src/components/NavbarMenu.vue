<template>
  <div>
    <div class="navbar bg-gray-800 text-white">
      <div class="navbar-start">
        <div class="dropdown lg:hidden">
          <label for="my-drawer" tabindex="0" class="btn btn-circle btn-ghost">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="size-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h16M4 18h7"
              />
            </svg>
          </label>
        </div>
      </div>
      <div class="navbar-center">
        <NuxtLink class="btn btn-ghost text-xl normal-case" to="/"
          >NuxtSample</NuxtLink
        >
      </div>
      <div class="navbar-end"></div>
    </div>
    <div class="drawer-mobile drawer lg:drawer-open">
      <input
        id="my-drawer"
        v-model="openedDrawer"
        type="checkbox"
        class="drawer-toggle"
      />
      <div class="drawer-content">
        <div class="p-4">
          <slot />
        </div>
      </div>
      <div class="drawer-side">
        <label for="my-drawer" class="drawer-overlay"></label>
        <ul class="menu min-h-full w-60 bg-gray-800 p-4">
          <label class="mb-4 text-xl text-white lg:hidden">NuxtSample</label>
          <li class="menu-item">
            <NuxtLink to="/table" @click="closeDrawer">Table</NuxtLink>
          </li>
          <li class="menu-item">
            <NuxtLink to="/tabulator" @click="closeDrawer">Tabulator</NuxtLink>
          </li>
          <li class="menu-item">
            <NuxtLink to="/tantable" @click="closeDrawer"
              >TanStackTable</NuxtLink
            >
          </li>
          <li class="menu-item">
            <NuxtLink to="/ref-counter" @click="closeDrawer"
              >RefCounter</NuxtLink
            >
          </li>
          <li class="menu-item">
            <NuxtLink to="/state-counter" @click="closeDrawer"
              >StateCounter</NuxtLink
            >
          </li>
          <li class="menu-item">
            <NuxtLink to="/parent-child" @click="closeDrawer"
              >ParentChild</NuxtLink
            >
          </li>
          <li class="menu-item">
            <NuxtLink to="/qrod" @click="closeDrawer">QROD</NuxtLink>
          </li>
          <li class="menu-item">
            <NuxtLink to="/login" @click="closeDrawer">Login</NuxtLink>
          </li>
          <li class="menu-item">
            <NuxtLink to="/logined" @click="closeDrawer">Logined</NuxtLink>
          </li>
          <li class="menu-item">
            <NuxtLink to="/keycloak-login" @click="closeDrawer"
              >KeycloakLogin</NuxtLink
            >
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  const openedDrawer = ref(false);
  const closeDrawer = () => {
    openedDrawer.value = false;
  };
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
</style>
