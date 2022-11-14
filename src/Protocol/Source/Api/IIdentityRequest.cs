namespace Identity.Protocol.Api
{
    public interface IIdentityRequest<out TResponse> : IIdentityRequest
    {
    }

    public interface IIdentityRequest
    {
    }
}