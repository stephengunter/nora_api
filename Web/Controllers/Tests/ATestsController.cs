using System.Runtime.CompilerServices;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   private readonly AppSettings _appSettings;
   private readonly AdminSettings _adminSettings; 

   public ATestsController(IUsersService usersService, IOptions<AppSettings> appSettings, 
      IOptions<AdminSettings> adminSettings)
   {
      _usersService = usersService;
      _appSettings = appSettings.Value;
      _adminSettings = adminSettings.Value;
   }

   [HttpGet("")]
   public async Task<ActionResult> Index()
   {
      // var adminUser = await _usersService.FindByEmailAsync(_adminSettings.Email);
      // if (adminUser == null) 
      // {
      //    ModelState.AddModelError("user", "User Not Found");
      //    return BadRequest();
      // }
      // bool result = await _usersService.HasPasswordAsync(adminUser);
      //User.Claims.UserName();
      if(User.Claims.IsNullOrEmpty()) return Ok("no");
		return Ok(User.Claims.First(c => c.Type == "id")?.Value ?? String.Empty);
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
