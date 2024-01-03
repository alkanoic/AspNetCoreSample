namespace CodeGen;

public class ApiTemplateArgs
{
    public required string NamespaceName { get; set; }

    public string Authorize { get; set; } = "";

    public required string ControllerName { get; set; }

    public required string ContextTypeName { get; set; }

    public required string ModelTypeName { get; set; }

    public required string EntitySetName { get; set; }

    public required string PrimaryKeyShortTypeName { get; set; }

    public required string PrimaryKeyName { get; set; }
}
