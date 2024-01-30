using System.Text;

using CodeGen;

#pragma warning disable CA1305

public partial class Program
{
    public static void Sample(string classFilePath, string className, string outputPath)
    {
        Console.WriteLine($"sample target classFilePath:{classFilePath} className:{className} output:{outputPath}");

        var args = new ClassAnalysisArgs()
        {
            ClassFilePath = classFilePath,
            ClassName = className
        };

        var analysis = new ClassAnalysis(args);
        var classInfo = analysis.ReadCode();

        var templateText = TemplateControl.ReadTemplateText("Templates/sample/SampleTemplate.cs");

        var templateArgs = new SampleTemplateArgs()
        {
            NamespaceName = "TargetNamespace",
            ClassName = "TargetClass",
            MethodName = "MethodName"
        };

        var s = new StringBuilder();
        foreach (var t in types)
        {
            s.Append($$"""
                    public static bool TryParse(string s, out {{t}} x) => {{t}}.TryParse(s, out x);

                """);
        }
        templateArgs.MethodLevel = s.ToString();

        var tc = new TemplateControl()
        {
            TargetClass = typeof(SampleTemplateArgs),
            Instance = templateArgs,
            TemplateText = templateText,
            OutputPath = outputPath
        };
        tc.WriteOverrideText();
        Console.WriteLine("Success generate:" + outputPath);
    }
    private static readonly string[] types = new[] { "bool", "byte", "int", "double" };
}
