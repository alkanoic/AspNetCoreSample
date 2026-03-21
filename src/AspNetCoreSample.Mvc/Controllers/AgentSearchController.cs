using AspNetCoreSample.Mvc.Models;
using AspNetCoreSample.Mvc.Options;

using Azure;
using Azure.Search.Documents.Indexes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Data;

namespace AspNetCoreSample.Mvc.Controllers;

public class AgentSearchController(
    IOptions<AzureOpenAIOptions> aoaiOptions,
    IOptions<AzureAISearchOptions> searchOptions) : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        var aoai = aoaiOptions.Value;
        var search = searchOptions.Value;

        var searchIndexClient = new SearchIndexClient(
            new Uri(search.Endpoint),
            new AzureKeyCredential(search.ApiKey));

        var vectorStore = new AzureAISearchVectorStore(searchIndexClient);

        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(aoai.DeploymentName, aoai.Endpoint, aoai.ApiKey)
            .AddAzureOpenAITextEmbeddingGeneration(aoai.EmbeddingDeploymentName, aoai.Endpoint, aoai.ApiKey)
            .Build();

        var embeddingService = kernel.GetRequiredService<Microsoft.SemanticKernel.Embeddings.ITextEmbeddingGenerationService>();

        var pastQACollection = vectorStore.GetCollection<string, QARecord>(search.PastQAIndexName);
        var manualQACollection = vectorStore.GetCollection<string, QARecord>(search.ManualQAIndexName);

        var pastQASearch = new VectorStoreTextSearch<QARecord>(pastQACollection, embeddingService);
        var manualQASearch = new VectorStoreTextSearch<QARecord>(manualQACollection, embeddingService);

        kernel.Plugins.Add(pastQASearch.CreateWithGetTextSearchResults("PastQASearch", "過去のQAデータを検索します"));
        kernel.Plugins.Add(manualQASearch.CreateWithGetTextSearchResults("ManualQASearch", "マニュアルのQAデータを検索します"));

        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory();
        history.AddSystemMessage(
            "あなたはシステムサポートエージェントです。" +
            "ユーザーの質問に対して、PastQASearchとManualQASearchの両方を使って関連情報を検索し、" +
            "得られた情報をもとに正確で分かりやすい日本語で回答してください。" +
            "情報が見つからない場合はその旨を伝えてください。");
        history.AddUserMessage(request.Query);

        var executionSettings = new AzureOpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var result = await chat.GetChatMessageContentAsync(history, executionSettings, kernel);
        return Json(new { answer = result.Content });
    }
}

public record SearchRequest(string Query);
