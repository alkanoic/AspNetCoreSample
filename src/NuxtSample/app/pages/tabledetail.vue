<template>
  <div class="overflow-x-auto">
    <table class="table">
      <thead>
        <tr>
          <th />
          <th>Name</th>
          <th>Job</th>
          <th>Favorite Color</th>
          <th />
        </tr>
      </thead>
      <tbody>
        <template v-for="(person, index) in people" :key="index">
          <tr class="hover">
            <th>{{ index + 1 }}</th>
            <td>{{ person.name }}</td>
            <td>{{ person.job }}</td>
            <td>{{ person.favoriteColor }}</td>
            <td>
              <button class="btn btn-primary btn-sm" @click="toggleDetails(index)">
                Details
              </button>
            </td>
          </tr>
          <tr v-show="showDetails[index]">
            <td colspan="5">
              <pre>{{ JSON.stringify(person, null, 2) }}</pre>
            </td>
          </tr>
        </template>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref } from "vue";

const people = ref([
  {
    name: "Cy Ganderton",
    job: "Quality Control Specialist",
    favoriteColor: "Blue",
  },
  {
    name: "Hart Hagerty",
    job: "Desktop Support Technician",
    favoriteColor: "Purple",
  },
  { name: "Brice Swyre", job: "Tax Accountant", favoriteColor: "Red" },
]);

const showDetails = ref([]);

const toggleDetails = (index) => {
  showDetails.value[index] = !showDetails.value[index];
};
</script>
