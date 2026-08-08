<template>
  <div>
    <h1 class="text-primary">Person Page</h1>
    <PersonControl :person="personProps" @person-click="personClick" />
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

const personClick = (person: PersonProps): void => {
  console.log(person);
};
</script>
