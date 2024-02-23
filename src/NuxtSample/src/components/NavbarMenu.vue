<template>
  <div>
    <div class="navbar bg-base-100">
      <div class="navbar-start">
        <div class="dropdown lg:hidden">
          <label for="my-drawer" tabindex="0" class="btn-ghost btn-circle btn">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
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
        <NuxtLink class="btn-ghost btn text-xl normal-case" to="/"
          >NuxtSample</NuxtLink
        >
      </div>
      <div class="navbar-end"></div>
    </div>
    <div class="drawer-mobile drawer" style="height: calc(100dvh - 64px)">
      <input
        id="my-drawer"
        v-model="openedDrawer"
        type="checkbox"
        class="drawer-toggle"
      />
      <div class="drawer-content">
        <!-- Page content here -->
        <div class="p-4">
          <slot />
        </div>
      </div>
      <div class="drawer-side">
        <label for="my-drawer" class="drawer-overlay"></label>
        <ul class="menu w-80 bg-base-100 p-4 text-base-content">
          <!-- Sidebar content here -->
          <li><NuxtLink to="/table" @click="closeDrawer">Table</NuxtLink></li>
          <li>
            <NuxtLink to="/tabulator" @click="closeDrawer">Tabulator</NuxtLink>
          </li>
          <li>
            <NuxtLink to="/tantable" @click="closeDrawer"
              >TanStackTable</NuxtLink
            >
          </li>
          <li>
            <NuxtLink to="/ref-counter" @click="closeDrawer"
              >RefCounter</NuxtLink
            >
          </li>
          <li>
            <NuxtLink to="/state-counter" @click="closeDrawer"
              >StateCounter</NuxtLink
            >
          </li>
          <li>
            <NuxtLink to="/parent-child" @click="closeDrawer"
              >ParentChild</NuxtLink
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
