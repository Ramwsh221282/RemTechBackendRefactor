namespace Identity.Adapter.Storage.Seeding;

public interface IUserSeeding
{
    Task SeedUser(string email, string login, string password);
}
