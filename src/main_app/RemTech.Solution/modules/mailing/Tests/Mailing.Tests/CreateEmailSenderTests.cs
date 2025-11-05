namespace Mailing.Tests;

public sealed class CreateEmailSenderTests : IClassFixture<MailingTestServices>
{
    private readonly MailingTestServices _services;

    public CreateEmailSenderTests(MailingTestServices services)
    {
        _services = services;
    }

    [Fact]
    private async Task Create_sender_OOP_success()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        Entities entities = new(tcs);
        entities.Add(Guid.NewGuid(), "");
        bool result = await tcs.Task;
    }

    public interface IEntitiesStorage
    {
        void Insert(Guid id, string name);
    }

    public interface IEntitiesObserver
    {
        void NotifyCreated(Guid id, string name);
    }

    public sealed class Entities : IEntitiesStorage, IEntitiesObserver
    {
        private event Action<Guid, string> handler;

        public Entities(TaskCompletionSource<bool> tcs)
        {
            handler += async (id, name) => await HandleEntityCreation(id, name, tcs);
        }

        public void Add(Guid id, string entity) => new Entity(id, entity, this);

        public void Insert(Guid id, string name) => Console.WriteLine("Entitiy inserted");

        public void NotifyCreated(Guid id, string name) => handler.Invoke(id, name);

        private async Task HandleEntityCreation(Guid id, string name, TaskCompletionSource<bool> tcs)
        {
            await Task.Yield();
            try
            {
                new Entity(id, name).Save(this);
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                tcs.SetResult(false);
            }
        }
    }

    public sealed class Entity
    {
        private readonly Guid _id;
        private readonly string _name;

        public Entity(Guid id, string name, IEntitiesObserver? observer = null)
        {
            _id = id;
            _name = name;
            observer?.NotifyCreated(id, name);
        }

        public void Save(IEntitiesStorage storage)
        {
            if (string.IsNullOrWhiteSpace(_name))
                throw new Exception("invalid name");
            storage.Insert(_id, _name);
        }
    }
}