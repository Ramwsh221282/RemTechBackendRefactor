namespace Users.Module.Features.UpdateUserProfile;

internal sealed class OnlyRootUserCanPromoteRootException : Exception
{
    public OnlyRootUserCanPromoteRootException()
        : base("Только пользователь с Root правами может сделать Root пользователя.") { }
}
