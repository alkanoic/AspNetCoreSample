<template>
  <div>
    <h1>Person Page</h1>
    <PersonControl :person="personProps" />
    <PersonArrayControl :people="peopleProps" />
  </div>
</template>

<script setup lang="ts">
  const personProps: PersonProps = { Name: "name", Age: 1 };

  const peopleProps: PersonProps[] = [
    {
      Name: "Name1",
      Age: "10",
    },
    {
      Name: "Name2",
      Age: "12",
    },
    {
      Name: "Name3",
      Age: "15",
    },
  ];
</script>
