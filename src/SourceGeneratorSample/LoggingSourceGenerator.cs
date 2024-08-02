using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGeneratorSample;

[Generator]
public class LoggingSourceGenerator : ISourceGenerator
{
    public List<string> Log { get; } = new();

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this example
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
            return;

        // try
        // {
        //     // context.AddSource("Logs", SourceText.From($@"/*{Environment.NewLine + string.Join(Environment.NewLine, receiver.Log) + Environment.NewLine}*/", Encoding.UTF8));
        //     if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
        //     {
        //         Log.Add($"Found a class named {testClass.Name}");
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Log.Add("Error parsing syntax: " + ex.ToString());
        // }


        // Retrieve the syntax trees in the user's compilation
        // var syntaxTrees = context.Compilation.SyntaxTrees;

        // foreach (var syntaxTree in syntaxTrees)
        // {
        //     // Parse the syntax tree
        //     var root = syntaxTree.GetRoot();

        //     // Find all method declarations
        //     var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        //     foreach (var methodDeclaration in methodDeclarations)
        //     {
        //         // Get the method name
        //         var methodName = methodDeclaration.Identifier.Text;

        //         // Get the parameters
        //         var parameters = methodDeclaration.ParameterList.Parameters;

        //         // Generate logging statements
        //         var loggingStatements = GenerateLoggingStatements(methodName, parameters);

        //         // Generate the new method body
        //         var newMethodBody = methodDeclaration.Body.WithStatements(SyntaxFactory.List(loggingStatements));

        //         // Replace the old method body with the new one
        //         var newRoot = root.ReplaceNode(methodDeclaration.Body, newMethodBody);

        //         // Add the new source file to the compilation
        //         context.AddSource($"{methodName}.g.cs", SourceText.From(newRoot.ToFullString(), Encoding.UTF8));
        //     }
        // }
    }

    private IEnumerable<StatementSyntax> GenerateLoggingStatements(string methodName, SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        var statements = new List<StatementSyntax>();

        // Add logging statements before the method execution
        statements.Add(SyntaxFactory.ParseStatement($"Console.WriteLine(\"Entering {methodName}\");"));

        foreach (var parameter in parameters)
        {
            var parameterName = parameter.Identifier.Text;
            statements.Add(SyntaxFactory.ParseStatement($"Console.WriteLine(\"Parameter {parameterName}: \" + {parameterName});"));
        }

        // Add the original method body statements
        statements.AddRange(parameters.Select(parameter => SyntaxFactory.ParseStatement($"// Original method body")));

        // Add logging statements after the method execution
        statements.Add(SyntaxFactory.ParseStatement($"Console.WriteLine(\"Exiting {methodName}\");"));

        return statements;
    }
}
