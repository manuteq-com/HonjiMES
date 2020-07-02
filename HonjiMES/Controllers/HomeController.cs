using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Helper;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;

namespace HonjiMES.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HonjiContext _context;
        private readonly JwtHelpers _jwt;
        public HomeController(HonjiContext context, JwtHelpers jwt)
        {
            _jwt = jwt;
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> SingIn(LoginViewModel login)
        {
            if (ValidateUser(login))
            {
                var UserRolesToken = new UserRolesToken();
                UserRolesToken.Token = _jwt.GenerateToken(login.Username);
                UserRolesToken.Username = login.Username;
                UserRolesToken.Timeout = DateTime.Now.AddMinutes(30);
                UserRolesToken.Menu = GetMenu();
                return Ok(MyFun.APIResponseOK(UserRolesToken));
            }
            else
            {
                return Ok(MyFun.APIResponseError("帳號密碼錯誤"));
            }

        }

        private MenuViewModel[] GetMenu()
        {
            var MenuViewModellist = new List<MenuViewModel>();
            var Allmenu = _context.Menus.Where(x => x.DeleteFlag == 0).ToList();
            foreach (var item in Allmenu.Where(x => !x.Pid.HasValue))
            {
                MenuViewModellist.Add(new MenuViewModel
                {
                    label = item.Name,
                    icon = item.Icon,
                    items = GetMenuitem(item.Id, Allmenu)
                });
            }
            return MenuViewModellist.ToArray();
        }

        private MenuViewModel[] GetMenuitem(int id, List<Menu> allmenu)
        {
            var MenuViewModellist = new List<MenuViewModel>();
            foreach (var item in allmenu.Where(x => x.Pid == id))
            {
                MenuViewModellist.Add(new MenuViewModel
                {
                    label = item.Name,
                    icon = item.Icon,
                    routerLink = item.RouterLink.Split(',')
                });
            }
            return MenuViewModellist.ToArray();
        }

        private bool ValidateUser(LoginViewModel login)
        {
            return true; // TODO
        }
    }
}
