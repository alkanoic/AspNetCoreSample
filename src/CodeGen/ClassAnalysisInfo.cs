namespace CodeGen;

internal sealed class ClassAnalysisInfo()
{
    public required string Name { get; set; }

    public required string NamespaceName { get; set; }

    public List<PropertyAnalysisInfo> Properties { get; set; } = new();

    public List<MethodAnalysisInfo> Methods { get; set; } = new();

    public List<PropertyAnalysisInfo> PrimaryProperties()
    {
        return Properties.Where(x => x.Attributes.Any(y => y.Name == "Key")).ToList();
    }
}

internal sealed class PropertyAnalysisInfo()
{
    public required string Name { get; set; }

    public required string TypeName { get; set; }

    public string? GenericTypeName { get; set; }

    public string? GenericInnerTypeName { get; set; }

    public List<AttributeAnalysisInfo> Attributes { get; set; } = new();
}

internal sealed class AttributeAnalysisInfo()
{
    public required string Name { get; set; }

    public List<ArgumentAnalysisInfo> Arguments { get; set; } = new();
}

internal sealed class ArgumentAnalysisInfo()
{
    public required string Name { get; set; }

    public required string Value { get; set; }
}

internal sealed class MethodAnalysisInfo()
{
    public required string Name { get; set; }
}
