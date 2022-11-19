using System;
using System.Collections.Generic;
using Identity.Repositories;

namespace Identity.Services.Users;

public class UserScopeProvider : IUserScopeProvider
{
    private readonly Dictionary<string, string[]> _map = new Dictionary<string, string[]>();

    public UserScopeProvider(UserRoleScopes[] roleScope)
    {
        foreach (var item in roleScope)
        {
            if (item.Scope != null)
            {
                _map[item.Role] = item.Scope!;
            }
        }
    }

    public string[] GetScopeByRole(UserRole role)
    {
        if (_map.TryGetValue(role.Id, out var result))
        {
            return result;
        }
        return Array.Empty<string>();
    }
}