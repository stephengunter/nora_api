using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Models;
using Microsoft.AspNetCore.Cors;

namespace Web.Controllers.Api;

[EnableCors("Global")]
public class PasswordsController : BaseApiController
{
   private readonly IUsersService _usersService;

   public PasswordsController(IUsersService usersService)
   {
      _usersService = usersService;
   }

   [HttpPost]
   public async Task<ActionResult> Store(SetPasswordRequest request)
   {
      string id = "a906b84f-5825-4528-990b-bd7e6a3a6413";
      var user = await _usersService.FindByIdAsync(id);
      if(user == null) return NotFound();

      if(String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("password", "Password Can Not Be Empty.");
      if(!ModelState.IsValid) return BadRequest(ModelState);

      await _usersService.AddPasswordAsync(user, request.Password);
      return Ok();
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> Update(string id, SetPasswordRequest request)
   {
      var user = await _usersService.FindByIdAsync(id);
      if(user == null) return NotFound();

      if(String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("password", "Password Can Not Be Empty.");      
      if(String.IsNullOrEmpty(request.Token)) ModelState.AddModelError("token", "token can not be empty!");
      if(!ModelState.IsValid) return BadRequest(ModelState);

      await _usersService.ChangePasswordAsync(user, request.Token, request.Password);
      return NoContent();
   }

}