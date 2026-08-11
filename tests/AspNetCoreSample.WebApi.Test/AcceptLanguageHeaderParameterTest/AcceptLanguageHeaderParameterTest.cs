using NSwag;
using NSwag.Generation.Processors.Contexts;

namespace AspNetCoreSample.WebApi.Test;

public sealed class AcceptLanguageHeaderParameterTest
{
    [Test]
    [Category(nameof(AcceptLanguageHeaderParameterTest))]
    public async Task ProcessAddsAcceptLanguageHeaderParameter()
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

        await Assert.That(result).IsTrue();
        await Assert.That(operationDescription.Operation.Parameters.Any(p => p.Name == "Accept-Language")).IsTrue();
        var parameter = operationDescription.Operation.Parameters.First(p => p.Name == "Accept-Language");
        await Assert.That(parameter.Kind).IsEqualTo(OpenApiParameterKind.Header);
        await Assert.That(parameter.IsRequired).IsTrue();
        await Assert.That(parameter.Default).IsEqualTo("ja-JP");
        await Assert.That(parameter.Schema.Enumeration).Contains("ja-JP");
        await Assert.That(parameter.Schema.Enumeration).Contains("en-US");
    }
}
