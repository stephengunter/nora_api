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
   private readonly IMapper _mapper;

   public ProfilesController(IUsersService usersService, IOptions<AdminSettings> adminSettings,
      IMapper mapper)
   {
      _usersService = usersService;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult<UserViewModel>> Get()
   {
      string id = "008acdba-5c0a-4d3a-90a2-3b31ad6d1d65";

      var user = await _usersService.FindByIdAsync(id);
      if(user == null) return NotFound();

      var roles = await _usersService.GetRolesAsync(user);
      var model = user.MapViewModel(roles, _mapper);
      model.HasPassword = await _usersService.HasPasswordAsync(user);
      return model;
   }

   [HttpPut("id")]
   public async Task<IActionResult> Update(string id, UserViewModel model)
   {
      var user = await _usersService.FindByIdAsync(id);      
      if(user == null) return NotFound();

      if(String.IsNullOrEmpty(model.Name)) ModelState.AddModelError("name", "name can not be empty!");
      if(!ModelState.IsValid) return BadRequest(ModelState);

      user.Name = model.Name!;
      user.PhoneNumber = model.PhoneNumber;

      await _usersService.UpdateAsync(user);
      
      return NoContent();
   }

}