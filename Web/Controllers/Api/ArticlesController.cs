using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Filters;
using Web.Models;
using ApplicationCore.Consts;
using ApplicationCore.Models;

namespace Web.Controllers.Api;

public class ArticlesController : BaseApiController
{
  
   private readonly IMapper _mapper;

  
   public ArticlesController(IMapper mapper)
   {
     
      _mapper = mapper;
   }

   [HttpGet("id")]
   public async Task<ActionResult> Get(int id)
   {
      
      return Ok("api/articles" + id.ToString());
   }

}