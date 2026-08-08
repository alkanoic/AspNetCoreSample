import { defineStore } from "pinia";

export const useFruitStore = defineStore("fruit", {
  state: () => ({
    fruits: [""],
    selectedFruit: "",
  }),
  getters: {},
  actions: {
    setFruits(fruits: string[]) {
      this.fruits = fruits;
    },
    setDefaults() {
      this.fruits = ["Apple", "Banana", "Orange", "Grape"];
    },
    setSelectedFruit(fruit: string) {
      this.selectedFruit = fruit;
    },
  },
});
