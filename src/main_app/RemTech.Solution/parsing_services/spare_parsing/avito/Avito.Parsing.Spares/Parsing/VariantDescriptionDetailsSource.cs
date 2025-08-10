namespace Avito.Parsing.Spares.Parsing;

public sealed class VariantDescriptionDetailsSource : IAvitoDescriptionDetailsSource
{
    private readonly Queue<IAvitoDescriptionDetailsSource> _sources = [];

    public VariantDescriptionDetailsSource With(IAvitoDescriptionDetailsSource source)
    {
        _sources.Enqueue(source);
        return this;
    }

    public async Task Add(AvitoSpare spare)
    {
        while (_sources.Count > 0)
        {
            IAvitoDescriptionDetailsSource source = _sources.Dequeue();
            await source.Add(spare);
        }
    }
}
