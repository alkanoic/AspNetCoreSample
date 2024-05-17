<template>
  <div>
    <form @submit="onSubmit">
      <h1>バリデーションサンプル</h1>
      <label class="input input-bordered flex items-center gap-2">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 16 16"
          fill="currentColor"
          class="size-4 opacity-70"
        >
          <path
            d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6ZM12.735 14c.618 0 1.093-.561.872-1.139a6.002 6.002 0 0 0-11.215 0c-.22.578.254 1.139.872 1.139h9.47Z"
          />
        </svg>
        <input v-model="name" type="text" class="grow" placeholder="Username" />
        <span v-if="errors.name">{{ errors.name }}</span>
      </label>
      <label class="input input-bordered flex items-center gap-2">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 16 16"
          fill="currentColor"
          class="size-4 opacity-70"
        >
          <path
            d="M2.5 3A1.5 1.5 0 0 0 1 4.5v.793c.026.009.051.02.076.032L7.674 8.51c.206.1.446.1.652 0l6.598-3.185A.755.755 0 0 1 15 5.293V4.5A1.5 1.5 0 0 0 13.5 3h-11Z"
          />
          <path
            d="M15 6.954 8.978 9.86a2.25 2.25 0 0 1-1.956 0L1 6.954V11.5A1.5 1.5 0 0 0 2.5 13h11a1.5 1.5 0 0 0 1.5-1.5V6.954Z"
          />
        </svg>
        <input v-model="email" type="text" class="grow" placeholder="Email" />
        <span v-if="errors.email">{{ errors.email }}</span>
      </label>
      <button class="btn btn-primary">Submit</button>
    </form>
  </div>
</template>

<script setup lang="ts">
  import { useField, useForm } from "vee-validate";
  import * as yup from "yup";

  const { handleSubmit, errors } = useForm({
    validationSchema: yup.object().shape({
      name: yup
        .string()
        .required("この項目は必須です")
        .min(3, "3文字以上で入力してください"),
      email: yup
        .string()
        .required("この項目は必須です")
        .email("メールアドレスの形式で入力してください"),
    }),
  });

  const { value: name } = useField("name");
  const { value: email } = useField("email");

  const onSubmit = handleSubmit((values) => {
    console.log(values);
  });
</script>
