import { AnonymousAuthenticationProvider } from '@microsoft/kiota-abstractions';
import { FetchRequestAdapter } from '@microsoft/kiota-http-fetchlibrary';
import { ApiClient } from './api-client/apiClient';
import { IdRequestBuilderGetQueryParameters } from './api-client/api/sampleTableApi/id';

(async () => {
  const authProvider = new AnonymousAuthenticationProvider();
  const adapter = new FetchRequestAdapter(authProvider);
  const client = new ApiClient(adapter);

  const sample = await client.api.sampleTableApi.get();
  console.log(sample);
  const sampleid = await client.api.sampleTableApi.id.get({ queryParameters: { id: 1 } });
  console.log(sampleid);

  const multi = await client.api.multiTableApi.get();
  console.log(multi);
  const multiid = await client.api.multiTableApi.id.charid.get({
    queryParameters: { id: 1, charid: '1' },
  });
  console.log(multiid);
})();
