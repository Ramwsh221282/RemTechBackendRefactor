using System.Reflection;

namespace RemTech.Shared.Configuration;

internal sealed class ConfigurationPrinter
{
    public void PrintConfigurations(IEnumerable<IConfigurationPart> configurationParts)
    {
        foreach (IConfigurationPart part in configurationParts)
        {
            Type type = part.GetType();
            FieldInfo[] fields = type.GetFields();
            PrintTypeName(type);
            PrintTypeFields(fields, part);
        }
    }

    private void PrintTypeFields(FieldInfo[] fields, IConfigurationPart part)
    {
        foreach (FieldInfo field in fields)
        {
            string name = field.Name;
            string value = (string)field.GetValue(part)!;
            Console.WriteLine($"{name} = {value}");
        }
    }

    private void PrintTypeName(Type type)
    {
        string name = type.Name;
        Console.WriteLine($"Configuration: {name}");
    }
}
