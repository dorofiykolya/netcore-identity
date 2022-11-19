using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Protocol.Dto.Users;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Services.Identities;
using Identity.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[Authorize]
public class UserInfoController : ControllerBase
{
    private readonly IMongoRepository<UserDocument?> _userRepository;

    public UserInfoController(
        IMongoRepository<UserDocument?> userRepository
    )
    {
        _userRepository = userRepository;
    }

    [Authorize]
    [HttpPost]
    [Route(UserInfoRequest.Route)]
    [ProducesResponseType(typeof(UserInfoResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UserInfo(UserInfoRequest request)
    {
        string userId = HttpContext.User.Claims.Sub();
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw IdentityErrorCode.UserNotFound.Exception();
        }
        return Ok(new UserInfoResponse
        {
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Name = user.Name ?? "",
                Roles = user.Roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                }).ToArray(),
                Identities = user.Identities.Select(x => new IdentityDto
                {
                    Identity = x.Identity,
                    Subject = x.Subject,
                }).ToArray()
            }
        });
    }
}