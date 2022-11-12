using System.Threading.Tasks;

namespace Identity.Services.Passwords;

public interface IPasswordGenerator
{
    public Task<string> GenerateAsync();
}