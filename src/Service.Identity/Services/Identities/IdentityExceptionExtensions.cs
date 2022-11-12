using System.ComponentModel;
using System.Reflection;

namespace Identity.Services.Identities;

public static class IdentityExceptionExtensions
{
    public static void Throw(this IdentityErrorCode code)
    {
        throw Exception(code);
    }

    public static IdentityException Exception(this IdentityErrorCode code)
    {
        var description = code.GetType().GetCustomAttribute<DescriptionAttribute>();
        return new IdentityException(code, description?.Description ?? code.ToString());
    }
}