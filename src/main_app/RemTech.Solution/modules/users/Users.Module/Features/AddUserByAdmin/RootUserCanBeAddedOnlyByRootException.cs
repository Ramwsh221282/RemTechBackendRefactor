namespace Users.Module.Features.AddUserByAdmin;

internal sealed class RootUserCanBeAddedOnlyByRootException : Exception
{
    public RootUserCanBeAddedOnlyByRootException()
        : base("Пользователь с правами ROOT может быть добавлен только ROOT пользователем.") { }
}
