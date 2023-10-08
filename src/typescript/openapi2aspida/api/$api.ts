import type { AspidaClient, BasicHeaders } from 'aspida';
import type { Methods as Methods_q4zxaw } from './Simple';
import type { Methods as Methods_30uj97 } from './WeatherForecast';

const api = <T>({ baseURL, fetch }: AspidaClient<T>) => {
  const prefix = (baseURL === undefined ? '' : baseURL).replace(/\/$/, '');
  const PATH0 = '/Simple';
  const PATH1 = '/WeatherForecast';
  const GET = 'GET';
  const POST = 'POST';

  return {
    Simple: {
      /**
       * @returns Success
       */
      get: (option: { body: Methods_q4zxaw['get']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_q4zxaw['get']['resBody'], BasicHeaders, Methods_q4zxaw['get']['status']>(prefix, PATH0, GET, option).json(),
      /**
       * @returns Success
       */
      $get: (option: { body: Methods_q4zxaw['get']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_q4zxaw['get']['resBody'], BasicHeaders, Methods_q4zxaw['get']['status']>(prefix, PATH0, GET, option).json().then(r => r.body),
      /**
       * @returns Success
       */
      post: (option: { body: Methods_q4zxaw['post']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_q4zxaw['post']['resBody'], BasicHeaders, Methods_q4zxaw['post']['status']>(prefix, PATH0, POST, option).json(),
      /**
       * @returns Success
       */
      $post: (option: { body: Methods_q4zxaw['post']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_q4zxaw['post']['resBody'], BasicHeaders, Methods_q4zxaw['post']['status']>(prefix, PATH0, POST, option).json().then(r => r.body),
      $path: () => `${prefix}${PATH0}`,
    },
    WeatherForecast: {
      /**
       * @returns Success
       */
      get: (option?: { config?: T | undefined } | undefined) =>
        fetch<Methods_30uj97['get']['resBody'], BasicHeaders, Methods_30uj97['get']['status']>(prefix, PATH1, GET, option).json(),
      /**
       * @returns Success
       */
      $get: (option?: { config?: T | undefined } | undefined) =>
        fetch<Methods_30uj97['get']['resBody'], BasicHeaders, Methods_30uj97['get']['status']>(prefix, PATH1, GET, option).json().then(r => r.body),
      /**
       * @returns Success
       */
      post: (option: { body: Methods_30uj97['post']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_30uj97['post']['resBody'], BasicHeaders, Methods_30uj97['post']['status']>(prefix, PATH1, POST, option).json(),
      /**
       * @returns Success
       */
      $post: (option: { body: Methods_30uj97['post']['reqBody'], config?: T | undefined }) =>
        fetch<Methods_30uj97['post']['resBody'], BasicHeaders, Methods_30uj97['post']['status']>(prefix, PATH1, POST, option).json().then(r => r.body),
      $path: () => `${prefix}${PATH1}`,
    },
  };
};

export type ApiInstance = ReturnType<typeof api>;
export default api;
