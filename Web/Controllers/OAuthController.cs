using Microsoft.AspNetCore.Mvc;
using Web.Models;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Google.Apis.Auth;
using ApplicationCore.Consts;

namespace Web.Controllers;

public class OAuthController : BaseController
{
	private readonly IUsersService _usersService;
	private readonly IJwtTokenService _jwtTokenService;
	private readonly IOAuthService _oauthService;

	public OAuthController(IUsersService usersService, IJwtTokenService jwtTokenService, IOAuthService oauthService)
	{
		_usersService = usersService;
		_jwtTokenService = jwtTokenService;
		_oauthService = oauthService;
	}


	[HttpPost("google")]
	public async Task<ActionResult> Google([FromBody] OAuthLoginRequest model)
	{
		var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, new GoogleJsonWebSignature.ValidationSettings());
		
		var user = await _usersService.FindByEmailAsync(payload.Email);

		if (user == null)
		{
			bool emailConfirmed = true;
			user = await _usersService.CreateAsync(payload.Email, emailConfirmed);
		}

		var oAuth = await _oauthService.FindByProviderAsync(user, OAuthProvider.Google);
		if(oAuth == null)
		{
			oAuth = await _oauthService.CreateAsync(new OAuth
			{
				OAuthId = payload.Subject,
				Provider = OAuthProvider.Google,
				GivenName = payload.GivenName,
				FamilyName = payload.FamilyName,
				Name = payload.Name,
				UserId = user.Id,
				PictureUrl = payload.Picture
			});
		}
		else
		{
			await _oauthService.UpdateAsync(oAuth);
		}

		var roles = await _usersService.GetRolesAsync(user);

		var accessToken = await _jwtTokenService.CreateAccessTokenAsync(RemoteIpAddress, user, roles, oAuth);
		string refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(RemoteIpAddress, user);

		var response = new AuthResponse(accessToken.Token, accessToken.ExpiresIn, refreshToken);
		return Ok(response);
	}


}
