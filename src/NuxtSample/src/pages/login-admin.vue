<template>
    <p>Login Admin Page</p>
</template>

<script setup lang="ts">
definePageMeta({
    middleware: ["auth", "auth-role"],
});
</script>
