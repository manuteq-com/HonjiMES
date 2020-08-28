using HonjiMES.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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
using Microsoft.Extensions.Configuration;
using DevExpress.CodeParser;
using System.IdentityModel.Tokens.Jwt;

namespace HonjiMES.Filter
{
    public class JWTAuthorizeAttribute : AuthorizeAttribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var msg = "";

            var user = context.HttpContext.User;
            //var rd = context.HttpContext.Request.RouteValues;
            //string currentAction = rd["action"].ToString();
            //string currentController = rd["controller"].ToString();
            // string currentArea = rd.Values["area"] as string;
            if (!user.Identity.IsAuthenticated)
            {
                msg = "ReLogin";
            }
            else
            {
                string Authorization = context.HttpContext.Request.Headers["Authorization"];
                string routerLink = context.HttpContext.Request.Headers["routerlink"];
                string apitype = context.HttpContext.Request.Headers["apitype"];
                if (string.IsNullOrWhiteSpace(Authorization) || string.IsNullOrWhiteSpace(routerLink) || string.IsNullOrWhiteSpace(apitype))
                {
                    msg = "ReLogin";
                }
                else
                {
                    var handler = new JwtSecurityTokenHandler();

                    Authorization = Authorization.Replace("Bearer ", "");
                    var jsonToken = handler.ReadToken(Authorization);
                    var tokenS = handler.ReadToken(Authorization) as JwtSecurityToken;
                    var sUserID = tokenS.Claims.Where(x => x.Type == "UserID")?.FirstOrDefault().Value;
                    var ConnectionStringMyDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MyDB"];
                    var optionsBuilder = new DbContextOptionsBuilder<HonjiContext>();

                    optionsBuilder.UseMySql(ConnectionStringMyDB, x => x.ServerVersion("8.0.19-mysql"));
                    using (var _context = new HonjiContext(optionsBuilder.Options))
                    {
                        var UserID = 0;
                        if (int.TryParse(sUserID, out UserID))
                        {
                            var UserRoles = _context.UserRoles.Where(x => x.DeleteFlag == 0 && x.UsersId == UserID && x.Menu.RouterLink == routerLink).FirstOrDefault();
                            if (UserRoles == null)
                            {
                                msg = "沒有API使用權限";
                            }
                            else
                            {
                                try
                                {
                                    var Roles = UserRoles.Roles.ToCharArray();
                                    switch (apitype)
                                    {
                                        case "GET":
                                            if (Roles[0] != '1')
                                            {
                                                msg = "沒有查詢權限";
                                            }
                                            break;
                                        case "POST":
                                            if (Roles[1] != '1')
                                            {
                                                msg = "沒有新增權限";
                                            }
                                            break;
                                        case "PUT":
                                            if (Roles[2] != '1')
                                            {
                                                msg = "沒有修改權限";
                                            }
                                            break;
                                        case "DELETE":
                                            if (Roles[3] != '1')
                                            {
                                                msg = "沒有刪除權限";
                                            }
                                            break;
                                        default:
                                            msg = "沒有API使用權限";
                                            break;
                                    }
                                }
                                catch
                                {
                                    msg = "沒有API使用權限";
                                }

                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(MyFun.APIResponseError(msg));
                return;
            }
        }
    }
}
