using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Mongo;
using Common.Redis;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Authorize]
[ApiController]
public class DeleteUserController : ControllerBase
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly ICacheRepository<UserTokenCache> _userToken;

    public DeleteUserController(
        IMongoRepository<UserDocument> userRepository,
        ICacheRepository<UserTokenCache> userToken
    )
    {
        _userRepository = userRepository;
        _userToken = userToken;
    }

    [Authorize]
    [HttpGet]
    [Route("user/delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] string userId)
    {
        if (User.Claims.IsAdmin() && User.Claims.Id() != userId)
        {
            await _userToken.DeleteAsync(new UserTokenCache
            {
                UserId = userId
            });
            await _userRepository.DeleteByIdAsync(userId);
            return Ok();
        }
        return BadRequest();
    }

    [Authorize]
    [HttpGet]
    [Route("user/delete/all")]
    public async Task<IActionResult> DeleteAllUser()
    {
        if (User.Claims.IsAdmin())
        {
            string currentId = User.Claims.Id();
            IList<UserTokenCache> list = await _userToken.ToListAsync();
            foreach (var token in list)
            {
                if (token.UserId != currentId)
                {
                    await _userToken.DeleteAsync(new UserTokenCache
                    {
                        UserId = token.UserId
                    });
                }
            }
            await _userRepository.DeleteManyAsync(x => x.Roles.All(i => i.Id != UserRoles.Admin));
            return Ok();
        }

        return BadRequest();
    }
}