namespace CodeGen;

internal sealed class ClassAnalysisArgs()
{
    public required string ClassFilePath { get; set; }

    public string? ClassName { get; set; }
}
