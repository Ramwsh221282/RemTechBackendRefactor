using Identity.Core.Permissions.Events;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Identity.Core.Permissions;

public sealed class Permission
{
    private readonly Guid _id;
    private readonly string _name;
    private readonly EventsStore? _events;

    public Permission AttachEventStore(EventsStore store)
    {
        return new Permission(_id, _name, store);
    }
    
    public Permission Register()
    {
        ValidateState();
        _events?.Raise(new PermissionRegistered(
            Id: _id,
            Name: _name
            ));
        return this;
    }
    
    public Permission Rename(string otherName)
    {
        Permission permission = new(_id, otherName);
        permission.ValidateState();
        _events?.Raise(new PermissionRenamed(
            Id: permission._id,
            OldName: _name,
            NewName: permission._name
            ));
        return permission;
    }

    private void ValidateState()
    {
        const int maxNameLength = 128;
        if (_id == Guid.Empty) throw ErrorException.ValueNotSet("Идентификатор разрешения");
        if (string.IsNullOrWhiteSpace(_name)) throw ErrorException.ValueNotSet("Название разрешения");
        if (_name.Length > maxNameLength) throw ErrorException.ValueExcess("Название разрешения", maxNameLength);
    }

    public Permission(Guid id, string name, EventsStore? events = null)
    {
        _id = id;
        _name = name;
        _events = events;
    }
}