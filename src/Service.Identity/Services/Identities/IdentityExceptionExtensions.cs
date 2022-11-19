using System.ComponentModel;
using System.Reflection;

namespace Identity.Services.Identities;

public static class IdentityExceptionExtensions
{
    public static void Throw(this IdentityErrorCode code)
    {
        throw Exception(code);
    }

    public static IdentityException Exception(this IdentityErrorCode code, params object[] arguments)
    {
        var description = code.GetType().GetCustomAttribute<DescriptionAttribute>();
        var message = description?.Description ?? code.ToString();
        message = string.Format(message, arguments);
        return new IdentityException(code, message);
    }
}