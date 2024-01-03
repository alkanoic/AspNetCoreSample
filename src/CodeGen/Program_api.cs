using System.Text;
using CodeGen;

#pragma warning disable CA1305

public partial class Program
{
    public static void Api(string classFilePath, string className, string outputPath)
    {
        Console.WriteLine($"api target classFilePath:{classFilePath} className:{className} output:{outputPath}");

        var args = new ClassAnalysisArgs()
        {
            ClassFilePath = classFilePath,
            ClassName = className
        };

        var analysis = new ClassAnalysis(args);
        var classInfo = analysis.ReadCode();

        var templateText = File.ReadAllText("Templates/ApiTemplate.cs", new UTF8Encoding(false));

        var templateArgs = new ApiTemplateArgs()
        {
            NamespaceName = "TargetNamespace",
            Authorize = "[Authorize]",
            ControllerName = "TargetController",
            ContextTypeName = "TargetContext",
            ModelTypeName = "TargetModelTypeName",
            EntitySetName = "TargetEntitySetName",
            PrimaryKeyShortTypeName = "int",
            PrimaryKeyName = "id"
        };

        var tc = new TemplateControl()
        {
            TargetClass = typeof(ApiTemplateArgs),
            Instance = templateArgs,
            TemplateText = templateText,
            OutputPath = outputPath
        };
        tc.WriteOverrideText();
        Console.WriteLine("Success generate:" + outputPath);
    }
}
