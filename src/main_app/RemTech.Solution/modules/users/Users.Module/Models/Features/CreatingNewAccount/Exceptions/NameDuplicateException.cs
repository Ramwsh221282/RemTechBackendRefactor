namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class NameDuplicateException : Exception
{
    public NameDuplicateException()
        : base("Пользователь с таким псевдонимом уже существует.") { }
}
