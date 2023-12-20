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

public class ProfilesController : BaseApiController
{
   private readonly IUsersService _usersService;
   private readonly AdminSettings _adminSettings; 
   private readonly IMapper _mapper;

   public ProfilesController(IUsersService usersService, IOptions<AdminSettings> adminSettings,
      IMapper mapper)
   {
      _usersService = usersService;
      _adminSettings = adminSettings.Value;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult<UserViewModel>> Get()
   {
      string id = _adminSettings.Id;
      string email = _adminSettings.Email;
      string key = _adminSettings.Key;
      string phone = _adminSettings.Phone;

      if(String.IsNullOrEmpty(id)) ModelState.AddModelError("id", "AdminSettings.Id Not Found");
      if(String.IsNullOrEmpty(email)) ModelState.AddModelError("email", "AdminSettings.Email Not Found");
      if(String.IsNullOrEmpty(key)) ModelState.AddModelError("key", "AdminSettings.Key Not Found");
      if(String.IsNullOrEmpty(phone)) ModelState.AddModelError("phone", "AdminSettings.Phone Not Found");

      if(!ModelState.IsValid) return BadRequest(ModelState);

      var user = await _usersService.FindByEmailAsync(email);
      if(user == null) throw new AdminSettingsException("User Not Found By Admin.Email.");

      if(id != user.Id) throw new AdminSettingsException("User Id Not Equal To Admin.Id.");

      var roles = await _usersService.GetRolesAsync(user);
      var model = user.MapViewModel(roles, _mapper);
      model.HasPassword = await _usersService.HasPasswordAsync(user);
      return model;
   }

   [HttpPut("id")]
   public async Task<IActionResult> Put(string id, UserViewModel model)
   {
      var user = await _usersService.FindByIdAsync(id);      
      if(user == null) return NotFound();

      if(String.IsNullOrEmpty(model.Name)) ModelState.AddModelError("name", "name can not be empty!");
      if(String.IsNullOrEmpty(model.PhoneNumber)) ModelState.AddModelError("phone", "phone can not be empty!");
      if(!ModelState.IsValid) return BadRequest(ModelState);

      user.Name = model.Name!;
      user.PhoneNumber = model.PhoneNumber;

      await _usersService.UpdateAsync(user);
      
      return NoContent();
   }

   [HttpPut("pw/{id}")]
   public async Task<IActionResult> SetPassword(string id, SetPasswordRequest model)
   {
      var user = await _usersService.FindByIdAsync(id);
      if(user == null) return NotFound();

      bool hasPassword = await _usersService.HasPasswordAsync(user);

      if(String.IsNullOrEmpty(model.Password)) ModelState.AddModelError("password", "password can not be empty!");
      if(hasPassword && String.IsNullOrEmpty(model.Token)) ModelState.AddModelError("token", "token can not be empty!");

      bool isValid = await _usersService.CheckPasswordAsync(user, model.Password);
      if(!isValid) ModelState.AddModelError("password", "password is not valid!");

      if(!ModelState.IsValid) return BadRequest(ModelState);

      return NoContent();
   }

}