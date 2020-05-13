using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 帳戶
    /// </summary>
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
            var data = _context.Users.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Users = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Users));
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
            Osupplier.UpdateUser = 1;

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
        /// 新增帳戶
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.Users.AsQueryable().Where(x => x.Username == user.Username && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("帳戶名稱已存在!", user));
            }
            user.CreateUser = 1;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(user));
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
