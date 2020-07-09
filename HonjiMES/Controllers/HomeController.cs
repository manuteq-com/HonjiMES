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
            var UserID = ValidateUser(login);
            if (UserID.HasValue)
            {
                var UserRolesToken = new UserRolesToken();
                UserRolesToken.Token = _jwt.GenerateToken(login.Username);
                UserRolesToken.Username = login.Username;
                UserRolesToken.Timeout = DateTime.Now.AddMinutes(30);
                UserRolesToken.Menu = GetMenu(UserID.Value);
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
            var UserRoles = _context.UserRoles.AsQueryable().Where(x => x.DeleteFlag == 0 && x.UsersId == Id).Select(x => x.MenuId).ToList();//&& x.Roles.Contains("1") 要開權限時把這裡加回去
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

        private int? ValidateUser(LoginViewModel login)
        {
            return 1; // TODO
            var Password = MyFun.Encryption(login.Password);
            var Users = _context.Users.Where(x => x.DeleteFlag == 0 && x.Username == login.Username && x.Password == Password).FirstOrDefault();
            if (Users == null)
            {
                return null;
            }
            else
            {
                return Users.Id;
            }
        }
    }
}
