using RemTech.Core.Shared.Enumerable;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed class VehicleLocationPath
{
    public const char Separator = '.';
    private const char ReplacementChar = '_';

    public string Path { get; }

    private VehicleLocationPath(string path) => Path = path;

    public static Status<VehicleLocationPath> Create(IEnumerable<string> parts)
    {
        string[] array = parts.Select(Sanitize).ToArray();

        if (!array.AllUnique(p => p))
        {
            return Error.Validation($"Путь локации: {string.Join(Separator, array)} содержит повторяющиеся значения.");
        }

        string path = string.Join(Separator, array);
        return new VehicleLocationPath(path);
    }

    private static string Sanitize(string input)
    {
        input = input.Trim();
        input = input.Replace('.', ReplacementChar);
        return input;
    }
}