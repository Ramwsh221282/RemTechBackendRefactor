namespace Identity.Adapter.Storage.Seeding;

public interface IRolesSeeding
{
    void AddRole(string roleName);
    Task Seed();
}
