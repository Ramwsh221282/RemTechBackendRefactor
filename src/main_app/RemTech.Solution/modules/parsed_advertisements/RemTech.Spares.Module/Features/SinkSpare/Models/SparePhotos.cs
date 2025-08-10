using RemTech.Spares.Module.Features.SinkSpare.Json;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SparePhotos : ISpareJsonObjectModifier
{
    private readonly HashSet<string> _sources;

    public SparePhotos(IEnumerable<string> sources)
    {
        _sources = new HashSet<string>(sources);
    }

    public SparePhotos()
    {
        _sources = [];
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        jsonObject.Add("photos", _sources);
    }
}
