using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.Cors;
using Web.Models;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
   protected string RemoteIpAddress => HttpContext.Connection.RemoteIpAddress is null ? "" : HttpContext.Connection.RemoteIpAddress.ToString();
    
}

//[EnableCors("Api")]
[Route("api/[controller]")]
public abstract class BaseApiController : BaseController
{
   
}

//[EnableCors("Admin")]
[Route("admin/[controller]")]
//[Authorize(Policy = "Admin")]
public class BaseAdminController : BaseController
{
   protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
   {
      if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

   }
}

[EnableCors("Global")]
[Route("tests/[controller]")]
public abstract class BaseTestController : BaseController
{

}



