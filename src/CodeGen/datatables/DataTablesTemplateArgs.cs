namespace CodeGen;

internal sealed class DataTablesTemplateArgs : CommonTemplateArgs
{
    public DataTablesTemplateArgs() { Console.WriteLine(""); }

    public string NamespaceName { get; set; } = "";

    public string Authorize { get; set; } = "";
}
