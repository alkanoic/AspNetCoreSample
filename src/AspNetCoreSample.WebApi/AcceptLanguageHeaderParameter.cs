using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace AspNetCoreSample.WebApi;

public class AcceptLanguageHeaderParameter : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var parameter = new OpenApiParameter
        {
            Name = "Accept-Language",
            Kind = OpenApiParameterKind.Header,
            Description = "Language preference for the response.",
            IsRequired = true,
            IsNullableRaw = true,
            Default = "ja-JP",
            Schema = new NJsonSchema.JsonSchema()
            {
                Type = NJsonSchema.JsonObjectType.String,
                Item = new NJsonSchema.JsonSchema() { Type = NJsonSchema.JsonObjectType.String },
            },
        };
        parameter.Schema.Enumeration.Add("ja-JP");
        parameter.Schema.Enumeration.Add("en-US");
        context.OperationDescription.Operation.Parameters.Add(parameter);
        return true;
    }
}
