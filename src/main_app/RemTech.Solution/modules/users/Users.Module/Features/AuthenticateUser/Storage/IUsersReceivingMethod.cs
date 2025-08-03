namespace Users.Module.Features.AuthenticateUser.Storage;

internal interface IUsersReceivingMethod<out TModified, in TIn>
{
    public TModified ModifyQuery(TIn query);
}
