using System.Runtime.CompilerServices;
using ApplicationCore.Authorization;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Models;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   private readonly AppSettings _appSettings;
   private readonly IJwtTokenService _jwtTokenService;
	private readonly IOAuthService _oAuthService;

   public ATestsController(IUsersService usersService, IOptions<AppSettings> appSettings, 
      IOAuthService oAuthService, IJwtTokenService jwtTokenService)
   {
      _usersService = usersService;
      _appSettings = appSettings.Value;
      _jwtTokenService = jwtTokenService;
		_oAuthService = oAuthService;
   }

   [HttpPost]
   public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
   {
      var cp = _jwtTokenService.ResolveClaimsFromToken(request.AccessToken);
		OAuthProvider provider = cp.Claims.Provider();
      return Ok(provider);
   }

   [HttpGet("version")]
   public ActionResult Version()
   {
      return Ok(_appSettings.ApiVersion);
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
