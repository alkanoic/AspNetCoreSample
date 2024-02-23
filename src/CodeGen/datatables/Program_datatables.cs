using System.Linq;
using System.Text;

using CodeGen;

public partial class Program
{
    public static void DataTables(string classFilePath, string className, string outputDirectory, string dbContextPath
        , string namespaceName, bool authorize = false)
    {
        Console.WriteLine($"DataTables target classFilePath:{classFilePath} className:{className} output:{outputDirectory}");

        var templateArgs = GetCommonTemplateArgs<DataTablesTemplateArgs>(classFilePath, className, dbContextPath);

        templateArgs.NamespaceName = namespaceName;
        templateArgs.Authorize = TemplateControl.AuthorizeAttribute(authorize);

        TargetTemplateDirectory(typeof(DataTablesTemplateArgs), templateArgs, "Templates/datatables", outputDirectory, className);

        Console.WriteLine("Success generate:" + outputDirectory);
    }
}
