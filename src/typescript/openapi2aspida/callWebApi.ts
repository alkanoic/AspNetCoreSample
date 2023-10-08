import { SimpleInput, SimpleOutput } from './api/@types';
import client from './client';

async function fetchSimple(input: SimpleInput) {
  const response = await client.Simple.get({ body: input });
  return response.body;
}

// 利用例
fetchSimple({ input: '12345' }).then((output: SimpleOutput) => {
  console.log(output.output);
});
