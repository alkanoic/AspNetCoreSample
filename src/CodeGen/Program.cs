using Cocona;
using CodeGen;
using System.Text;
using System.Text.RegularExpressions;

var app = CoconaApp.Create();

#pragma warning disable CA1305

app.AddCommand("sample", (string classFilePath, string className, string outputPath) =>
{
    Console.WriteLine($"target classFilePath:{classFilePath} className:{className} output:{outputPath}");

    var args = new SampleGenerateArgs()
    {
        ClassFilePath = classFilePath,
        ClassName = className
    };

    var analysis = new ClassAnalysis(args);
    var classInfo = analysis.ReadCode();

    var templateText = File.ReadAllText("Templates/SampleTemplate.cs", new UTF8Encoding(false));

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
});

app.Run();
