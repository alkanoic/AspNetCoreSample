namespace SourceGeneratorSample;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class LogMethodAttribute : Attribute
{
    public LogMethodAttribute()
    {
    }
}
