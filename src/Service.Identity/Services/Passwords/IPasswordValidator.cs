using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services.Passwords;

public interface IPasswordValidator
{
    Task<IdentityResult> ValidateAsync(string password);
}