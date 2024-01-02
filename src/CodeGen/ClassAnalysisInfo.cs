namespace CodeGen;

public class ClassAnalysisInfo()
{
    public required string Name { get; set; }

    public List<PropertyAnalysisInfo> Properties { get; set; } = new();

    public List<MethodAnalysisInfo> Methods { get; set; } = new();
}

public class PropertyAnalysisInfo()
{
    public required string Name { get; set; }

    public required string TypeName { get; set; }

    public List<AttributeAnalysisInfo> Attributes { get; set; } = new();
}

public class AttributeAnalysisInfo()
{
    public required string Name { get; set; }

    public List<ArgumentAnalysisInfo> Arguments { get; set; } = new();
}

public class ArgumentAnalysisInfo()
{
    public required string Name { get; set; }

    public required string Value { get; set; }
}

public class MethodAnalysisInfo()
{
    public required string Name { get; set; }
}
