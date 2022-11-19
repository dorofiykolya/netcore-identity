using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
public class SignInByGuestController : ControllerBase
{
    private readonly IUserJwtTokenRepository _userJwt;
    private readonly IMongoRepository<UserDocument> _userRepository;

    public SignInByGuestController(
        IMongoRepository<UserDocument> userRepository,
        IUserJwtTokenRepository userJwt
    )
    {
        _userRepository = userRepository;
        _userJwt = userJwt;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(SignInByGuestRequest.Route)]
    [ProducesResponseType(typeof(SignInByGuestResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignInByGuest(SignInByGuestRequest request)
    {
        var user = await _userRepository.FindByGuestAsync(request.Id);
        if (user == null)
        {
            user = await _userRepository.CreateUser(document =>
            {
                document.Name = request.Id;
                document.Identities.Add(new UserGuestIdentity
                {
                    Subject = request.Id
                });
                document.Roles.Add(new UserIdentityRole
                {
                    Id = UserRoles.Guest
                });
            });
        }

        var token = await _userJwt.UpdateTokenWithIdentity(user, Identities.Guest);

        return Ok(new SignInByGuestResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken
        });
    }
}