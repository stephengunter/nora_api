﻿using System.Threading.Tasks;
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

	[HttpPost]
	public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
	{
		ValidateRequest(request);
		if(!ModelState.IsValid) return BadRequest(ModelState);

		var user = await _usersService.FindByEmailAsync(request.Username);
		if(user is null)
		{
			ModelState.AddModelError("", "身分驗證失敗. 請重新登入.");
			return BadRequest(ModelState);
		}

		bool isValid = await _usersService.CheckPasswordAsync(user, request.Password);
		if(!isValid)
		{
			ModelState.AddModelError("", "身分驗證失敗. 請重新登入.");
			return BadRequest(ModelState);
		}

		var roles = await _usersService.GetRolesAsync(user);

		var accessToken = await _jwtTokenService.CreateAccessTokenAsync(RemoteIpAddress, user, roles);
		string refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(RemoteIpAddress, user);

		return new AuthResponse(accessToken.Token, accessToken.ExpiresIn, refreshToken);
	}

	
	[HttpPut("{id}")]
	public async Task<ActionResult<AuthResponse>> RefreshToken(string id, [FromBody] RefreshTokenRequest request)
	{
		var user = await _usersService.FindByIdAsync(id);
		if(user is null) return NotFound();

		var cp = _jwtTokenService.ResolveClaimsFromToken(request.AccessToken);
		if(cp is null) throw new TokenResolveFailedException();
		if(cp.Claims.IsNullOrEmpty()) throw new TokenResolveFailedException("Claims IsNullOrEmpty!");
		if(cp.Claims.UserId() != id) throw new RefreshTokenFailedException($"User Id Not Equals To Put Id: {id}");
		
		await ValidateRequestAsync(request, user);
		if (!ModelState.IsValid) return BadRequest(ModelState);		
		
		OAuth? oauth = null;
		if(cp.Claims.Provider() != OAuthProvider.Unknown)
		{
			oauth = await _oAuthService.FindByProviderAsync(user, cp.Claims.Provider());
			if(oauth is null)  throw new RefreshTokenFailedException($"OAuth NotFound By Provider: {cp.Claims.Provider().ToString()}");
		}		
		
		var roles = await _usersService.GetRolesAsync(user);
		var accessToken = await _jwtTokenService.CreateAccessTokenAsync(RemoteIpAddress, user, roles, oauth);
		string refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(RemoteIpAddress, user);

		return new AuthResponse(accessToken.Token, accessToken.ExpiresIn, refreshToken);

	}
	async Task ValidateRequestAsync(RefreshTokenRequest request, User user)
	{
		bool isValid = await _jwtTokenService.IsValidRefreshTokenAsync(request.RefreshToken, user);
		if(!isValid) ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");
	}
	void ValidateRequest(LoginRequest request)
	{
		if(String.IsNullOrEmpty(request.Username)) ModelState.AddModelError("name", ValidationMeesages.Empty("name"));
		if(String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("password", ValidationMeesages.Empty("password"));
	}



}

