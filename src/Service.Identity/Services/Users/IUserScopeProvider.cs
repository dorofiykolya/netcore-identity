using Identity.Repositories;

namespace Identity.Services.Users;

public interface IUserScopeProvider
{
    string[] GetScopeByRole(UserRole role);
}