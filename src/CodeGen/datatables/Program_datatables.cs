using System.Linq;
using System.Text;

using CodeGen;

public partial class Program
{
    public static void DataTables(string classFilePath, string className, string outputDirectory, string dbContextPath
        , string namespaceName, bool authorize = false)
    {
        Console.WriteLine($"DataTables target classFilePath:{classFilePath} className:{className} output:{outputDirectory}");

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
        var templateArgs = new MvcTemplateArgs()
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
        templateArgs.CreateBindAttributes = TemplateControl.PropertyNameArguments(targetClassInfo.Properties, false);
        templateArgs.EntitySetExist = TemplateControl.EntitySetExist(templateArgs.EntitySetName, targetClassInfo.PrimaryProperties());
        templateArgs.ViewTargetModelHeader = TemplateControl.ViewTargetModelHeader(targetClassInfo.Properties);
        templateArgs.ViewTargetModelDetail = TemplateControl.ViewTargetModelDetail(targetClassInfo.Properties);
        templateArgs.ViewTargetModelCreate = TemplateControl.ViewTargetModelCreate(targetClassInfo.Properties);
        templateArgs.ViewTargetModelDetails = TemplateControl.ViewTargetModelDetails(targetClassInfo.Properties);
        templateArgs.ViewTargetModelEditPrimaryKey = TemplateControl.ViewTargetModelEditPrimaryKey(targetClassInfo.PrimaryProperties());
        templateArgs.ViewTargetModelEditProperties = TemplateControl.ViewTargetModelEditProperties(targetClassInfo.ExcludePrimaryProperties());
        templateArgs.ViewLinkIndexPrimaryKey = TemplateControl.ViewLinkPrimaryKey(targetClassInfo.PrimaryProperties(), "item");
        templateArgs.ViewLinkPrimaryKey = TemplateControl.ViewLinkPrimaryKey(targetClassInfo.PrimaryProperties(), "Model");

        TargetTemplateDirectory(typeof(MvcTemplateArgs), templateArgs, "Templates/datatables", outputDirectory, className);

        Console.WriteLine("Success generate:" + outputDirectory);
    }
}
