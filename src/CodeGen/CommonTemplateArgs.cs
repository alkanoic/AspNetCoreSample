namespace CodeGen;

internal class CommonTemplateArgs
{
    public string UsingNamespaces { get; set; } = "";

    public string ControllerName { get; set; } = "";

    public string ContextTypeName { get; set; } = "";

    public string ModelTypeName { get; set; } = "";

    public string EntitySetName { get; set; } = "";

    public string CompareTargetToArguments { get; set; } = "";

    public string PrimaryKeyNameAttributes { get; set; } = "";

    public string PrimaryKeyNameArguments { get; set; } = "";

    public string PrimaryKeyNameTargetArguments { get; set; } = "";

    public string PrimaryKeyNameNewObject { get; set; } = "";

    public string PrimaryKeyArguments { get; set; } = "";

    public string ContextFindPrimaryKey { get; set; } = "";

    public string EntitySetExist { get; set; } = "";

    public string CreateBindAttributes { get; set; } = "";

    public string ViewTargetModelHeader { get; set; } = "";

    public string ViewTargetModelDetail { get; set; } = "";

    public string ViewTargetModelCreate { get; set; } = "";

    public string ViewTargetModelDetails { get; set; } = "";

    public string ViewTargetModelEditPrimaryKey { get; set; } = "";

    public string ViewTargetModelEditProperties { get; set; } = "";

    public string ViewLinkIndexPrimaryKey { get; set; } = "";

    public string ViewLinkPrimaryKey { get; set; } = "";
}
