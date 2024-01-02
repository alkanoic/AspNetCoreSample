using Cocona;
using CodeGen;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CA1305

CoconaApp.Run((string classFilePath, string className, string outputPath) =>
{
    Console.WriteLine($"target classFilePath:{classFilePath} className:{className} output:{outputPath}");

    var args = new SampleGenerateArgs()
    {
        ClassFilePath = classFilePath,
        ClassName = className
    };

    var analysis = new ClassAnalysis(args);
    var classInfo = analysis.ReadCode();

    var templateText = File.ReadAllText("Templates/MainTemplate.cs", new UTF8Encoding(false));

    var templateArgs = new MainTemplateArgs()
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

    var dict = TemplateControl.ClassPropertiesConvertToDictionary(typeof(MainTemplateArgs), templateArgs);
    foreach (var key in dict.Keys)
    {
        templateText = templateText.Replace(key, dict[key]);
    }

    File.WriteAllText(outputPath, templateText, new UTF8Encoding(false));
    Console.WriteLine("Success generate:" + outputPath);
});

public partial class Program
{
    private static readonly string[] types = new[] { "bool", "byte", "int", "double" };
}
