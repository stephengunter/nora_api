using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Auth;
using ApplicationCore.Services;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using Web.Models;
using ApplicationCore.Exceptions;
using ApplicationCore.Authorization;

namespace Web.Controllers;

public class AuthController : BaseController
{
	private readonly IUsersService _usersService;
	private readonly IJwtTokenService _jwtTokenService;
	private readonly IOAuthService _oAuthService;


	public AuthController(IOptions<AdminSettings> adminSettings, IUsersService usersService, 
		IOAuthService oAuthService, IJwtTokenService jwtTokenService)
	{
		
		_usersService = usersService;
		_jwtTokenService = jwtTokenService;
		_oAuthService = oAuthService;
	}

	// [HttpPost("")]
	// public async Task<ActionResult> Login([FromBody] OAuthLoginRequest model)
	// {
	// 	var user = _usersService.FindUserByPhone(model.Token);

	// 	if (user == null)
	// 	{
	// 		ModelState.AddModelError("auth", "登入失敗.");
	// 		return BadRequest(ModelState);
	// 	}

	// 	var roles = await _usersService.GetRolesAsync(user);

	// 	var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, roles);

	// 	return Ok(responseView);;
	// }

	//POST api/auth/refreshtoken
	[HttpPost("refreshtoken")]
	public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
	{
		var cp = _jwtTokenService.ResolveClaimsFromToken(model.AccessToken);
		if(cp is null)
		{
			throw new TokenResolveFailedException();
		}
		if(cp.Claims.IsNullOrEmpty())
		{
			throw new TokenResolveFailedException("Claims IsNullOrEmpty!");
		}

		string userId = cp.Claims.UserId();
		var oauthProvider = cp.Claims.Provider();
		
		var user = await _usersService.FindByIdAsync(userId);
		if(user is null)
		{
		   throw new RefreshTokenFailedException($"User NotFound By Id: {userId}");
		}
		await ValidateRequestAsync(model, user);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var roles = await _usersService.GetRolesAsync(user);
		var oauth = await _oAuthService.FindByProviderAsync(user, oauthProvider);
		if(oauth is null)  throw new RefreshTokenFailedException($"OAuth NotFound By Provider: {oauthProvider.ToString()}");
		
		var accessToken = await _jwtTokenService.CreateAccessTokenAsync(RemoteIpAddress, user, roles, oauth);
		string refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(RemoteIpAddress, user);

		var response = new AuthResponse(accessToken.Token, accessToken.ExpiresIn, refreshToken);
		return Ok(response);

	}

	async Task ValidateRequestAsync(RefreshTokenRequest model, User user)
	{
		bool isValid = await _jwtTokenService.IsValidRefreshTokenAsync(model.RefreshToken, user);
		if(!isValid) ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");
	}



}

