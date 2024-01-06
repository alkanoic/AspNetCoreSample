using System.Linq;
using System.Text;
using CodeGen;

#pragma warning disable CA1305

public partial class Program
{
    public static void Api(string classFilePath, string className, string outputPath, string dbContextPath
        , string namespaceName, string controllerName, bool authorize = false)
    {
        Console.WriteLine($"api target classFilePath:{classFilePath} className:{className} output:{outputPath}");

        var targetArgs = new ClassAnalysisArgs()
        {
            ClassFilePath = classFilePath,
            ClassName = className
        };
        var targetClassInfo = new ClassAnalysis(targetArgs).ReadCode();

        var dbContextArgs = new ClassAnalysisArgs()
        {
            ClassFilePath = dbContextPath
        };
        var dbContextClassInfo = new ClassAnalysis(dbContextArgs).ReadCode();

        var templateText = TemplateControl.ReadTemplateText("ApiTemplate.cs");

        var templateArgs = new ApiTemplateArgs()
        {
            UsingNamespaces = $"using {dbContextClassInfo.NamespaceName};",
            NamespaceName = namespaceName,
            Authorize = authorize ? "[Authorize]" : "",
            ControllerName = controllerName,
            ContextTypeName = dbContextClassInfo.Name,
            ModelTypeName = targetClassInfo.Name,
            EntitySetName = dbContextClassInfo.Properties.Single(x => x.GenericInnerTypeName == targetClassInfo.Name).Name,
            PrimaryKeyShortTypeName = targetClassInfo.Properties.Single(x => x.Attributes.Any(y => y.Name == "Key")).TypeName,
            PrimaryKeyName = targetClassInfo.Properties.Single(x => x.Attributes.Any(y => y.Name == "Key")).Name
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
