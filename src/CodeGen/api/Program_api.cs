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

        var templateArgs = new ApiTemplateArgs()
        {
            UsingNamespaces = TemplateControl.UsingNamespace(targetClassInfo.NamespaceName),
            NamespaceName = namespaceName,
            Authorize = TemplateControl.AuthorizeAttribute(authorize),
            ControllerName = controllerName,
            ContextTypeName = dbContextClassInfo.Name,
            ModelTypeName = targetClassInfo.Name,
            EntitySetName = dbContextClassInfo.Properties.Single(x => x.GenericInnerTypeName == targetClassInfo.Name).Name
        };
        templateArgs.CompareTargetToArguments = TemplateControl.CompareTargetToArguments(targetClassInfo.PrimaryProperties());
        templateArgs.PrimaryKeyNameAttributes = TemplateControl.PrimaryKeyNameAttributes(targetClassInfo.PrimaryProperties());
        templateArgs.PrimaryKeyNameArguments = TemplateControl.PropertyNameArguments(targetClassInfo.PrimaryProperties(), true);
        templateArgs.PrimaryKeyNameTargetArguments = TemplateControl.PrimaryKeyNameTargetArguments(targetClassInfo.PrimaryProperties());
        templateArgs.PrimaryKeyNameNewObject = TemplateControl.PrimaryKeyNameNewObject(targetClassInfo.PrimaryProperties());
        templateArgs.PrimaryKeyArguments = TemplateControl.PrimaryKeyArguments(targetClassInfo.PrimaryProperties());
        templateArgs.ContextFindPrimaryKey = TemplateControl.ContextFindPrimaryKey(templateArgs.EntitySetName, targetClassInfo.PrimaryProperties(), true);
        templateArgs.EntitySetExist = TemplateControl.EntitySetExist(templateArgs.EntitySetName, targetClassInfo.PrimaryProperties());

        TemplateOutputFile(typeof(ApiTemplateArgs), templateArgs, "api/ApiTemplate.cs", outputPath);
        Console.WriteLine("Success generate:" + outputPath);
    }
}
