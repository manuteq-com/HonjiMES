using HonjiMES.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HonjiMES.Filter
{
    public class JWTAuthorizeAttribute : AuthorizeAttribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var rd = context.HttpContext.Request.RouteValues;
            string currentAction = rd["action"].ToString();
            string currentController = rd["controller"].ToString();
            // string currentArea = rd.Values["area"] as string;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(MyFun.APIResponseError("請重新登入系統"));
                return;
            }
            else
            {

            }
        }
    }
}
