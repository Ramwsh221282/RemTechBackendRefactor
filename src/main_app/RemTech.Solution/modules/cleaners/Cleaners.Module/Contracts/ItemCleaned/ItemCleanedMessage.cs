using Npgsql;

namespace Cleaners.Module.Contracts.ItemCleaned;

public sealed class ItemCleanedMessage(string id)
{
    public void FillCommand(NpgsqlCommand command) =>
        command.Parameters.Add(new NpgsqlParameter<string>("@id", id));
}
