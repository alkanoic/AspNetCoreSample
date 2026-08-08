<template>
  <div class="flex h-screen">
    <!-- Mobile menu button -->
    <div v-if="!openedDrawer" class="fixed top-4 left-4 z-50 lg:hidden">
      <UButton
        icon="i-heroicons-bars-3"
        variant="solid"
        color="gray"
        size="sm"
        @click="openedDrawer = !openedDrawer"
      />
    </div>

    <!-- Header -->
    <div class="fixed top-0 right-0 left-0 z-40 bg-gray-800 p-4 text-white lg:hidden">
      <div class="text-center">
        <NuxtLink to="/" class="text-xl font-bold">NuxtSample</NuxtLink>
      </div>
    </div>

    <!-- Overlay for mobile -->
    <div
      v-if="openedDrawer"
      class="fixed inset-0 z-30 bg-black/50 lg:hidden"
      @click="openedDrawer = false"
    />

    <!-- Sidebar -->
    <div :class="[
      'fixed inset-y-0 left-0 z-40 w-60 bg-gray-800 transition-transform duration-300 ease-in-out lg:static',
      openedDrawer ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
    ]">
      <div class="flex h-full flex-col p-4">
        <!-- Mobile close button -->
        <div class="mb-4 flex items-center justify-between lg:hidden">
          <NuxtLink to="/" class="text-xl font-bold text-white">NuxtSample</NuxtLink>
          <UButton
            icon="i-heroicons-x-mark"
            variant="ghost"
            color="white"
            size="sm"
            @click="closeDrawer"
          />
        </div>

        <div class="mb-6 hidden lg:block">
          <NuxtLink to="/" class="text-xl font-bold text-white">NuxtSample</NuxtLink>
        </div>

        <nav class="flex-1 space-y-2">
          <div v-for="section in menuItems" :key="section.label" class="space-y-1">
            <div class="px-3 py-2 font-medium text-white">{{ section.label }}</div>
            <div class="space-y-1 pl-2">
              <NuxtLink
                v-for="link in section.links"
                :key="link.to"
                :to="link.to"
                :class="[
                  'block rounded-md px-3 py-2 text-sm transition-colors',
                  route.path === link.to
                    ? 'bg-gray-700 text-white'
                    : 'text-gray-300 hover:bg-gray-700 hover:text-white',
                ]"
                @click="closeDrawer"
              >
                {{ link.label }}
              </NuxtLink>
            </div>
          </div>
        </nav>

        <div class="mt-4">
          <USelect
            v-model="selectedFruit"
            :options="fruitOptions"
            placeholder="Who shot first?"
            @change="handleChange"
          />
        </div>
      </div>
    </div>

    <!-- Main content -->
    <div class="flex-1 lg:ml-0">
      <div :class="[
        'h-full overflow-auto',
        'pt-16 lg:pt-0',
        'p-4',
      ]">
        <slot />
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

const fruitOptions = computed(() =>
  fruits.map(fruit => ({ label: fruit, value: fruit }))
);

function handleChange(value: string) {
  console.log("Selected fruit:", value);
  fruitStore.setSelectedFruit(value);
}

const menuItems = [
  {
    label: "Tables",
    defaultOpen: true,
    links: [
      { label: "Table", to: "/table" },
      { label: "TableDetail", to: "/tabledetail" },
      { label: "Tabulator", to: "/tabulator" },
      { label: "TabulatorTable", to: "/tabulator-table" },
      { label: "TanStackTable", to: "/tantable" }
    ]
  },
  {
    label: "Counters",
    defaultOpen: true,
    links: [
      { label: "RefCounter", to: "/ref-counter" },
      { label: "StateCounter", to: "/state-counter" }
    ]
  },
  {
    label: "Auth",
    defaultOpen: true,
    links: [
      { label: "Login", to: "/login" },
      { label: "Logined", to: "/logined" },
      { label: "LoginAdminPage", to: "/login-admin" },
      { label: "KeycloakLogined", to: "/keycloak-logined" },
      { label: "KeycloakAdminPage", to: "/keycloak-admin" }
    ]
  },
  {
    label: "Others",
    defaultOpen: false,
    links: [
      { label: "ParentChild", to: "/parent-child" },
      { label: "Person", to: "/person" },
      { label: "QROD", to: "/qrod" },
      { label: "Modal", to: "/modal" },
      { label: "Accordion", to: "/accordion" },
      { label: "Validate", to: "/validate" },
      { label: "Textarea", to: "/textarea" }
    ]
  }
];
</script>
