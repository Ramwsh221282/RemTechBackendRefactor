using Npgsql;
using Pgvector;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.Utilities;

namespace RemTech.Spares.Module.Features.SinkSpare.Persistance;

internal sealed class SpareEmbeddingStringSource
    : ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>
{
    private readonly List<string> _texts;
    private readonly IEmbeddingGenerator _generator;

    public SpareEmbeddingStringSource(IEmbeddingGenerator generator)
    {
        _texts = [];
        _generator = generator;
    }

    public void Add(string text)
    {
        _texts.Add(text);
    }

    private ReadOnlyMemory<float> Embeddings()
    {
        string raw = MakeTextRaw();
        return _generator.Generate(raw);
    }

    private string MakeTextRaw()
    {
        if (_texts.Count == 0)
            throw new SpareEmbeddingStringEmptyException();
        string text = string.Join(' ', _texts);
        if (string.IsNullOrWhiteSpace(text))
            throw new SpareEmbeddingStringEmptyException();
        string formatted = new StringForVectorStoring(text).Read();
        if (string.IsNullOrWhiteSpace(formatted))
            throw new SpareEmbeddingStringEmptyException();
        return formatted;
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        NpgsqlCommand command = persistance.Read();
        command.Parameters.AddWithValue("@embedding", new Vector(Embeddings()));
        return new SpareSqlPersistanceCommand(command);
    }
}
