using System.Linq;
using System.Text;

using CodeGen;

#pragma warning disable CA1305

public partial class Program
{

    private static void TemplateOutputFile(Type type, object templateArgs, string templateFilePath, string outputPath)
    {
        var templateText = TemplateControl.ReadTemplateText(templateFilePath);
        var tc = new TemplateControl()
        {
            TargetClass = type,
            Instance = templateArgs,
            TemplateText = templateText,
            OutputPath = outputPath
        };
        tc.WriteOverrideText();
    }

    private static void TargetTemplateDirectory(Type type, object templateArgs, string templateDirectory, string outputDirectory, string className)
    {
        foreach (var f in Directory.GetFiles(templateDirectory, "*.*", SearchOption.AllDirectories))
        {
            TemplateOutputFile(type, templateArgs, f, Path.Combine(outputDirectory, f.Replace($"{templateDirectory}/", "").Replace("__ClassName__", className)));
        }
    }

    private static T GetCommonTemplateArgs<T>(string classFilePath, string className, string dbContextPath) where T : CommonTemplateArgs, new()
    {
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

        var controllerName = className + "Controller";
        var commonTemplateArgs = new T()
        {
            UsingNamespaces = TemplateControl.UsingNamespace(targetClassInfo.NamespaceName),
            ControllerName = controllerName,
            ContextTypeName = dbContextClassInfo.Name,
            ModelTypeName = targetClassInfo.Name,
            EntitySetName = dbContextClassInfo.Properties.Single(x => x.GenericInnerTypeName == targetClassInfo.Name).Name,
            CompareTargetToArguments = TemplateControl.CompareTargetToArguments(targetClassInfo.PrimaryProperties()),
            PrimaryKeyNameAttributes = TemplateControl.PrimaryKeyNameAttributes(targetClassInfo.PrimaryProperties()),
            PrimaryKeyNameArguments = TemplateControl.PropertyNameArguments(targetClassInfo.PrimaryProperties(), true),
            PrimaryKeyNameTargetArguments = TemplateControl.PrimaryKeyNameTargetArguments(targetClassInfo.PrimaryProperties()),
            PrimaryKeyNameNewObject = TemplateControl.PrimaryKeyNameNewObject(targetClassInfo.PrimaryProperties()),
            PrimaryKeyArguments = TemplateControl.PrimaryKeyArguments(targetClassInfo.PrimaryProperties())
        };
        commonTemplateArgs.ContextFindPrimaryKey = TemplateControl.ContextFindPrimaryKey(commonTemplateArgs.EntitySetName, targetClassInfo.PrimaryProperties(), true);
        commonTemplateArgs.CreateBindAttributes = TemplateControl.PropertyNameArguments(targetClassInfo.Properties, false);
        commonTemplateArgs.EntitySetExist = TemplateControl.EntitySetExist(commonTemplateArgs.EntitySetName, targetClassInfo.PrimaryProperties());
        commonTemplateArgs.ViewTargetModelHeader = TemplateControl.ViewTargetModelHeader(targetClassInfo.Properties);
        commonTemplateArgs.ViewTargetModelDetail = TemplateControl.ViewTargetModelDetail(targetClassInfo.Properties);
        commonTemplateArgs.ViewTargetModelCreate = TemplateControl.ViewTargetModelCreate(targetClassInfo.Properties);
        commonTemplateArgs.ViewTargetModelDetails = TemplateControl.ViewTargetModelDetails(targetClassInfo.Properties);
        commonTemplateArgs.ViewTargetModelEditPrimaryKey = TemplateControl.ViewTargetModelEditPrimaryKey(targetClassInfo.PrimaryProperties());
        commonTemplateArgs.ViewTargetModelEditProperties = TemplateControl.ViewTargetModelEditProperties(targetClassInfo.ExcludePrimaryProperties());
        commonTemplateArgs.ViewLinkIndexPrimaryKey = TemplateControl.ViewLinkPrimaryKey(targetClassInfo.PrimaryProperties(), "item");
        commonTemplateArgs.ViewLinkPrimaryKey = TemplateControl.ViewLinkPrimaryKey(targetClassInfo.PrimaryProperties(), "Model");

        return commonTemplateArgs;
    }
}
