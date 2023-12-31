namespace CodeGen;

public static class TemplateControl
{
    private static string ReplaceChara = "@";

    public static Dictionary<string, string> ClassPropertiesConvertToDictionary(Type targetClass, object instance)
    {
        var dictionary = new Dictionary<string, string>();

        foreach (var p in targetClass.GetProperties())
        {
            var v = p.GetValue(instance);
            dictionary.Add($"{ReplaceChara}{p.Name}{ReplaceChara}", v is null ? "" : v.ToString()!);
        }
        return dictionary;
    }
}
