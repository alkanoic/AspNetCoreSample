using System.Text;
using Cocona;
using T4Execute;

CoconaApp.Run(([Option("ap")] string assemblyPath, [Option("op")] string outputPath) =>
{
    Console.WriteLine($"Hello {assemblyPath}");

    var args = new SampleGenerateArgs()
    {
        AssemblyPath = assemblyPath
    };

    var text = new SampleGenerator(args).TransformText();
    File.WriteAllText(outputPath, text, new UTF8Encoding(false));
    Console.WriteLine("Success generate:" + outputPath);
});
