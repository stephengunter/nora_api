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

   // [HttpPut("pw/{id}")]
   // public async Task<IActionResult> SetPassword(string id, SetPasswordRequest model)
   // {
   //    var user = await _usersService.FindByIdAsync(id);
   //    if(user == null) return NotFound();

   //    bool hasPassword = await _usersService.HasPasswordAsync(user);

   //    if(String.IsNullOrEmpty(model.Password)) ModelState.AddModelError("password", "password can not be empty!");
   //    if(hasPassword && String.IsNullOrEmpty(model.Token)) ModelState.AddModelError("token", "token can not be empty!");

   //    bool isValid = await _usersService.CheckPasswordAsync(user, model.Password);
   //    if(!isValid) ModelState.AddModelError("password", "password is not valid!");

   //    if(!ModelState.IsValid) return BadRequest(ModelState);

   //    return NoContent();
   // }

}