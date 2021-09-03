using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FriendsAppNoORM.Utilities
{

    public class ProfileRequiredAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!context.HttpContext.User.HasProfile())
                {
                    var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                    var controller = descriptor.ControllerName;
                    var action = descriptor.ActionName;

                    if(action == "LogOff"){
                        return;
                    }                    
                    else if (action == "Create" && controller == "Profile"){
                        return;
                    }

                    context.Result = new RedirectToActionResult("Create", "Profile", null);
                }
            }
        }
    }
}