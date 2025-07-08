using System.Text.Json;
using RemTech.Json.Library.Serialization.Primitives;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Json;

public sealed class Human
{
    private readonly Guid _id;
    private readonly bool _registered;
    private readonly int _age;
    private readonly string _name;
    private readonly Human[] _friends;
    private readonly DateTime _dateRegistered;

    public Human(Human origin, Human friend)
    {
        _age = origin._age;
        _name = origin._name;
        _id = origin._id;
        _registered = origin._registered;
        _dateRegistered = origin._dateRegistered;
        _friends = [.. origin._friends, friend];
    }

    public Human(int age, string name)
    {
        _age = age;
        _name = name;
        _id = Guid.NewGuid();
        _registered = true;
        _dateRegistered = DateTime.UtcNow;
        _friends = [];
    }

    public Guid Id() => _id;

    public bool Registered() => _registered;

    public int Age() => _age;

    public string Name() => _name;

    public Human[] Friends() => _friends;

    public DateTime DateRegistered() => _dateRegistered;

    public Human WithFriend(Human friend)
    {
        return new Human(this, friend);
    }
}

public class JsonTests
{
    [Fact]
    private void Create_Mike_Plain_Json_Without_Friends()
    {
        Human mike = new(18, "Mike");
        Human bob = new(20, "Bob");
        Human alice = new(19, "Alice");
        mike = mike.WithFriend(bob).WithFriend(alice);
        ObjectsArraySerJson<Human> arraySer = new("friends", mike.Friends());
        string jsonString = new PlainSerJson()
            .With(new GuidSerJson("id", mike.Id()))
            .With(new DateTimeSerJson("dateRegistered", mike.DateRegistered()))
            .With(new BooleanSerJson("registered", mike.Registered()))
            .With(new IntegerSerJson("age", mike.Age()))
            .With(new StringSerJson("name", mike.Name()))
            .With(
                arraySer.ForEach(f =>
                    new PlainSerJson()
                        .With(new GuidSerJson("id", f.Id()))
                        .With(new DateTimeSerJson("dateRegistered", f.DateRegistered()))
                        .With(new BooleanSerJson("registered", f.Registered()))
                        .With(new IntegerSerJson("age", f.Age()))
                        .With(new StringSerJson("name", f.Name()))
                )
            )
            .Read();
        JsonDocument document = JsonDocument.Parse(jsonString);
    }
}
