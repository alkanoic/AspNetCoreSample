using SourceGeneratorSample;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

MyClass myClass = new MyClass();
myClass.MyMethod("test", 123);

public class MyClass
{
    [LogMethod]
    public void MyMethod(string param1, int param2)
    {
        // Original method body
        Console.WriteLine("Inside MyMethod");
    }
}
