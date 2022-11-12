using System.Net;
using System.Threading.Tasks;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[Authorize]
public class SignOutController : ControllerBase
{
    private readonly IUserJwtTokenRepository _userJwt;
    public SignOutController(
        IUserJwtTokenRepository userJwt
    )
    {
        _userJwt = userJwt;
    }

    [Authorize]
    [HttpPost]
    [Route(SignOutRequest.Route)]
    [ProducesResponseType(typeof(SignOutResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignOut(SignOutRequest request)
    {
        string userId = HttpContext.User.Claims.Id();
        await _userJwt.Purge(userId);
        return Ok();
    }
}