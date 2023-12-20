using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Consts;

namespace ApplicationCore.Authorization;

public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
{
   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
   {
      Permissions permissione = requirement.Permission;
      if(permissione == Permissions.Admin)
      {
         if(context.User.Claims.IsBoss() || context.User.Claims.IsDev())
         {
            context.Succeed(requirement);
            return Task.CompletedTask;
         }         
      }

      context.Fail();
      return Task.CompletedTask;
   }
}

