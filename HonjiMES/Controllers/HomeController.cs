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
        public ActionResult<string> SignIn(LoginViewModel login)
        {
            if (ValidateUser(login))
            {
                return _jwt.GenerateToken(login.Username);
            }
            else
            {
                return BadRequest();
            }

        }
        private bool ValidateUser(LoginViewModel login)
        {
            return true; // TODO
        }
    }
}
