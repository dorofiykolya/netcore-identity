using System.Threading.Tasks;
using Common.Mongo;

namespace Identity.Repositories;

public static class UserRepositoryExtensions
{
    public static async Task<UserDocument> CreateUser(this IMongoRepository<UserDocument> repository)
    {
        var user = new UserDocument();
        await repository.InsertOneAsync(user);
        return user;
    }
}