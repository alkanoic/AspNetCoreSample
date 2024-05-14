const { createApp, ref } = Vue;

const init_target = JSON.parse(document.querySelector("#vue-target").value);

createApp({
  setup() {
    const userName = ref(init_target.UserName);
    const email = ref(init_target.Email);
    const age = ref(init_target.Age);
    const birthday = ref(init_target.Birthday);

    return {
      userName,
      email,
      age,
      birthday,
    };
  },
}).mount("#app");
