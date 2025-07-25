namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class ExecutedPgCommands
{
    private readonly Queue<ExecutedPgCommand> _commands = [];

    public ExecutedPgCommands With(ExecutedPgCommand command)
    {
        _commands.Enqueue(command);
        return this;
    }

    public void Executed()
    {
        while (_commands.Count > 0)
        {
            _commands.Dequeue().Executed();
        }
    }
}