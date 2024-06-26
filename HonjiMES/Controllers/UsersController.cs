﻿using DevExtreme.AspNet.Mvc;
using HonjiMES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 帳戶
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HonjiContext _context;

        public UsersController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        ///  帳戶列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Users.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.Username);
            var Users = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Users));
        }

        /// <summary>
        ///  帳戶列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersNoSuper()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Users.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Id != 1).OrderBy(x => x.Username);
            var Users = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Users));
        }

        /// <summary>
        /// 取帳戶列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderHead>>> GetUsersNoSuperByFilter(
            [FromQuery] DataSourceLoadOptions FromQuery,
            [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.Users.Where(x => x.DeleteFlag == 0 && x.Id != 1 && x.Permission > 10).OrderBy(x => x.Username);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);

            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 用ID最帳戶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(user));
        }

        /// <summary>
        /// 查詢帳戶 By 員工號碼
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<User>> GetUserByUserNo(string DataNo)
        {
            var User = await _context.Users.Where(x => x.Username == DataNo && x.DeleteFlag == 0).ToListAsync();
            if (User.Count() == 0) {
                return Ok(MyFun.APIResponseError("查無員工資訊! [ " + DataNo + " ]"));
            }
            return Ok(MyFun.APIResponseOK(User.FirstOrDefault()));
        }

        /// <summary>
        /// 新增帳戶
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            user.Id = id;
            var Osupplier = _context.Users.Find(id);
            var Csupplier = Osupplier;
            // if (!string.IsNullOrWhiteSpace(user.Code))
            // {
            //     Csupplier.Code = user.Code;
            // }
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                Csupplier.Username = user.Username;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.Users.AsQueryable().Where(x => x.Id != id && x.Username == Csupplier.Username && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("帳戶的名稱 [" + Csupplier.Username + "] 重複!", Csupplier));
            }

            var Msg = MyFun.MappingData(ref Osupplier, user);
            Osupplier.UpdateTime = DateTime.Now;
            Osupplier.UpdateUser = MyFun.GetUserID(HttpContext);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(user));
        }
        /// <summary>
        /// 新增帳戶及權限
        /// </summary>
        /// <param name="creatuser"></param>
        /// <returns></returns>
        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(PostUserViewModel creatuser)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.Users.AsQueryable().Where(x => x.Username == creatuser.user.Username && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("帳戶名稱已存在!"));
            }
            creatuser.user.Password = MyFun.Encryption(creatuser.user.Password);
            creatuser.user.CreateUser = MyFun.GetUserID(HttpContext);
            var UserRoleList = MyFun.ReturnRole(creatuser.MenuList);
            foreach (var UserRole in UserRoleList)
            {
                creatuser.user.UserRoles.Add(UserRole);
            }
            _context.Users.Add(creatuser.user);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(creatuser));
        }
        /// <summary>
        /// 刪除帳戶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.DeleteFlag = 1;
            // _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(user));
        }
        /// <summary>
        ///  帳戶列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuListViewModel>>> GetUsersMenu()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var Menus = await _context.Menus.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Pid != null).ToListAsync();
            var MenuList = new List<MenuListViewModel>();
            foreach (var item in Menus)
            {
                MenuList.Add(new MenuListViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Query = false,
                    Creat = false,
                    Edit = false,
                    Delete = false
                });
            }
            return Ok(MyFun.APIResponseOK(MenuList));
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        /// <summary>
        ///  使用者權限
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<MenuListViewModel>>> GetUsersMenuRoles(int id)
        {
            var Menus = await _context.Menus.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Pid != null).ToListAsync();
            var UserRoles = await _context.UserRoles.AsQueryable().Where(x => x.DeleteFlag == 0 && x.UsersId == id).ToListAsync();
            var MenuList = new List<MenuListViewModel>();
            foreach (var item in Menus)
            {
                var Query = false;
                var Creat = false;
                var Edit = false;
                var Delete = false;
                var UserRolesitem = UserRoles.Where(x => x.MenuId == item.Id).FirstOrDefault();
                if (UserRolesitem != null)
                {
                    try
                    {
                        var Rolearray = UserRolesitem.Roles.ToCharArray();
                        Query = Rolearray[0] == '1';
                        Creat = Rolearray[1] == '1';
                        Edit = Rolearray[2] == '1';
                        Delete = Rolearray[3] == '1';
                    }
                    catch
                    {

                    }
                }
                else
                {
                    UserRolesitem = new UserRole
                    {
                        Roles = "0000",
                        MenuId = item.Id,
                        UsersId = id,
                    };
                    _context.UserRoles.Add(UserRolesitem);
                    _context.SaveChanges();
                }
                MenuList.Add(new MenuListViewModel
                {
                    Id = UserRolesitem.Id,
                    Name = item.Name,
                    Query = Query,
                    Creat = Creat,
                    Edit = Edit,
                    Delete = Delete
                });
            }
            return Ok(MyFun.APIResponseOK(MenuList));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersMenuRoles(int id, MenuListViewModel MenuList)
        {
            var UserRoles = _context.UserRoles.Find(id);
            if (UserRoles == null)
            {

            }
            var Query = "0";
            var Creat = "0";
            var Edit = "0";
            var Delete = "0";
            try
            {
                var Rolearray = UserRoles.Roles.ToCharArray();// 權限順序 查詢 新增 修改 刪除，如有需要就自己往下加
                Query = Rolearray[0].ToString();
                Creat = Rolearray[1].ToString();
                Edit = Rolearray[2].ToString();
                Delete = Rolearray[3].ToString();
            }
            catch
            {

            }
            var Roles = "";
            Roles += MenuList.Query.HasValue ? MenuList.Query.Value ? "1" : "0" : Query;
            Roles += MenuList.Creat.HasValue ? MenuList.Creat.Value ? "1" : "0" : Creat;
            Roles += MenuList.Edit.HasValue ? MenuList.Edit.Value ? "1" : "0" : Edit;
            Roles += MenuList.Delete.HasValue ? MenuList.Delete.Value ? "1" : "0" : Delete;
            UserRoles.Roles = Roles;
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(MenuList));
        }

        public async Task<IActionResult> GetUserIDCheck(string Username)
        {
            var Users = await _context.Users.Where(x => x.DeleteFlag == 0 && x.Username == Username).AnyAsync();
            if (Users)
            {
                return Ok(MyFun.APIResponseError("帳號重複"));
            }
            else
            {
                return Ok(MyFun.APIResponseOK(null));
            }
        }
    }
}
