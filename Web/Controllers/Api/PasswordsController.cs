using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Models;

namespace Web.Controllers.Api;

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
      string id = "008acdba-5c0a-4d3a-90a2-3b31ad6d1d65";
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