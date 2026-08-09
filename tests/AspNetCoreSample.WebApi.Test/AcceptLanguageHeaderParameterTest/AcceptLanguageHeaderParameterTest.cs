using NSwag;
using NSwag.Generation.Processors.Contexts;

namespace AspNetCoreSample.WebApi.Test;

public sealed class AcceptLanguageHeaderParameterTest
{
    [Fact]
    [Trait("Category", nameof(AcceptLanguageHeaderParameterTest))]
    public void ProcessAddsAcceptLanguageHeaderParameter()
    {
        var processor = new AcceptLanguageHeaderParameter();
        var document = new OpenApiDocument();
        var operationDescription = new OpenApiOperationDescription
        {
            Operation = new OpenApiOperation(),
            Path = "/api/test",
            Method = "GET"
        };
        var methodInfo = typeof(AcceptLanguageHeaderParameterTest).GetMethod(nameof(ProcessAddsAcceptLanguageHeaderParameter))!;
        var settings = new NSwag.Generation.OpenApiDocumentGeneratorSettings();
        var schemaSettings = new NJsonSchema.Generation.SystemTextJsonSchemaGeneratorSettings();
        var schemaResolver = new NJsonSchema.Generation.JsonSchemaResolver(document, schemaSettings);
        var context = new OperationProcessorContext(
            document,
            operationDescription,
            typeof(object),
            methodInfo,
            new NSwag.Generation.OpenApiDocumentGenerator(settings, schemaResolver),
            schemaResolver,
            settings,
            []);

        var result = processor.Process(context);

        Assert.True(result);
        Assert.Contains(operationDescription.Operation.Parameters, p => p.Name == "Accept-Language");
        var parameter = operationDescription.Operation.Parameters.First(p => p.Name == "Accept-Language");
        Assert.Equal(OpenApiParameterKind.Header, parameter.Kind);
        Assert.True(parameter.IsRequired);
        Assert.Equal("ja-JP", parameter.Default);
        Assert.Contains("ja-JP", parameter.Schema.Enumeration);
        Assert.Contains("en-US", parameter.Schema.Enumeration);
    }
}
