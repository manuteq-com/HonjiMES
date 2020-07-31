using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Filter;
using HonjiMES.Helper;
using HonjiMES.Models;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public ActionResult<string> SingIn(LoginViewModel login)
        {
            var User = ValidateUser(login);
            if (User != null)
            {
                var expireMinutes = 1440;//幾分鐘後timeout
                var UserRolesToken = new UserRolesToken();
                UserRolesToken.Token = _jwt.GenerateToken(User, expireMinutes);
                UserRolesToken.Username = User.Username;
                UserRolesToken.Timeout = DateTime.Now.AddMinutes(expireMinutes);
                UserRolesToken.Menu = GetMenu(User.Id);
                return Ok(MyFun.APIResponseOK(UserRolesToken));
            }
            else
            {
                return Ok(MyFun.APIResponseError("帳號密碼錯誤"));
            }

        }

        private MenuViewModel[] GetMenu(int Id)
        {
            var MenuViewModellist = new List<MenuViewModel>();
            var Allmenu = _context.Menus.Where(x => x.DeleteFlag == 0).ToList();
            var UserRoles = new List<int>();

            if (Id == 1)//SUPER USER 可以使用全部的頁面，不必設權限
            {
                UserRoles = Allmenu.Select(x => x.Id).ToList();
            }
            else
            {
                UserRoles = _context.UserRoles.AsQueryable().Where(x => x.DeleteFlag == 0 && x.UsersId == Id).Select(x => x.MenuId).ToList();//&& x.Roles.Contains("1") 要開權限時把這裡加回去
            }
            foreach (var item in Allmenu.Where(x => !x.Pid.HasValue))
            {
                var Menuitem = GetMenuitem(item.Id, Allmenu, UserRoles);
                if (Menuitem.Any())
                {
                    MenuViewModellist.Add(new MenuViewModel
                    {
                        label = item.Name,
                        icon = item.Icon,
                        items = Menuitem
                    });
                }
            }
            return MenuViewModellist.ToArray();
        }
        /// <summary>
        /// 查詢可用的子頁面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allmenu"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        private MenuViewModel[] GetMenuitem(int id, List<Menu> allmenu, List<int> userRoles)
        {
            var MenuViewModellist = new List<MenuViewModel>();
            foreach (var item in allmenu.Where(x => x.Pid == id && userRoles.Contains(x.Id)))
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

        private User ValidateUser(LoginViewModel login)
        {
            return _context.Users.Find(1);
            var Password = MyFun.Encryption(login.Password);
            var Users = _context.Users.Where(x => x.DeleteFlag == 0 && x.Username == login.Username && x.Password == Password).FirstOrDefault();
            if (Users == null)
            {
                return null;
            }
            else
            {
                return Users;
            }
        }

        [HttpGet]
        public ActionResult<string> TestToken()
        {
            int identity = MyFun.GetUserID(HttpContext);
            return Ok(MyFun.APIResponseOK(null, "登入中"));

        }
    }
}
