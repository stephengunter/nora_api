using ApplicationCore.Models;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Models;
using Azure.Core;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   private readonly AppSettings _appSettings;
   private readonly IJwtTokenService _jwtTokenService;
	private readonly IOAuthService _oAuthService;

   public ATestsController(IUsersService usersService, IOptions<AppSettings> appSettings)
   {
      _usersService = usersService;
      _appSettings = appSettings.Value;
   }

   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var user = await _usersService.FindByEmailAsync("stephen@gmail.com");
      if (user == null) return NotFound();

      await _usersService.AddPasswordAsync(user, "79@Stephen");
      return Ok();
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
