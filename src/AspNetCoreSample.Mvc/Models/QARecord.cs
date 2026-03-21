using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace AspNetCoreSample.Mvc.Models;

public class QARecord
{
    [VectorStoreRecordKey]
    public string Id { get; set; } = string.Empty;

    [VectorStoreRecordData(IsFilterable = true)]
    [TextSearchResultName]
    public string Question { get; set; } = string.Empty;

    [VectorStoreRecordData]
    [TextSearchResultValue]
    public string Answer { get; set; } = string.Empty;

    [VectorStoreRecordVector(Dimensions: 1536, DistanceFunction: DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
