<template>
  <div>
    <h1>grpc Page</h1>
    <button class="btn btn-primary" @click="greeter()">greeter</button>
    <p>{{ display }}</p>
  </div>
</template>
<script setup lang="ts">
  import { HelloRequest } from "~/middleware/greet_pb.js";
  import { GreeterClient } from "~/middleware/greet_grpc_web_pb.js";

  const display = ref("");
  function greeter() {
    const client = new GreeterClient("https://localhost:7189");
    const request = new HelloRequest();
    // リクエストに必要なデータを設定
    request.name = "your value";

    const result = client.Greeter(request, {}).then((response) => {
      // レスポンスを処理
      return { response: response.toObject() };
    });
    display.value = result.response;
  }

  // const helloRequest = new HelloRequest();
  // client.Greeter(helloRequest, {}, (err, response) => {
  //   if (err) console.log(err);
  //   console.log(response.toObject());
  // });

  // import { ref } from "vue";
  // import { createGrpcWebHandler } from "grpc-web";
  // import { greetProto } from "~/middleware/greet_pb";

  // const name = ref("");
  // const message = ref("");

  // const handler = createGrpcWebHandler("http://localhost:3000");
  // const client = new greetProto.GreetService(handler, null, null);

  // async function greeter() {
  //   const request = { name: name.value };
  //   const response = await new Promise<greetProto.HelloResponse>(
  //     (resolve, reject) => {
  //       client.sayHello(request, (err, res) => {
  //         if (err) {
  //           reject(err);
  //         } else {
  //           resolve(res);
  //         }
  //       });
  //     }
  //   );
  //   message.value = response.message;
  // }
</script>
