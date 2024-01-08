namespace CodeGen;

internal sealed class ApiTemplateArgs
{
    public string UsingNamespaces { get; set; } = "";

    public required string NamespaceName { get; set; }

    public string Authorize { get; set; } = "";

    public required string ControllerName { get; set; }

    public required string ContextTypeName { get; set; }

    public required string ModelTypeName { get; set; }

    public required string EntitySetName { get; set; }

    public string CompareTargetToArguments { get; set; } = "";

    public string PrimaryKeyNameAttributes { get; set; } = "";

    public string PrimaryKeyNameArguments { get; set; } = "";

    public string PrimaryKeyNameTargetArguments { get; set; } = "";

    public string PrimaryKeyNameNewObject { get; set; } = "";

    public string PrimaryKeyArguments { get; set; } = "";

    public string ContextFindPrimaryKey { get; set; } = "";

    public string EntitySetExist { get; set; } = "";
}
