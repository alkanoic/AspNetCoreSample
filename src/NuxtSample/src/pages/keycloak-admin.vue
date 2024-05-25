<template>
    <p>Keycloak Admin Page</p>
</template>

<script setup lang="ts">
definePageMeta({
    middleware: ["keycloak-auth", "keycloak-role"],
});
</script>
