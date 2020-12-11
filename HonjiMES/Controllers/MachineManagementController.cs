using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 顧客列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachineManagementController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachineManagementController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢顧客列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var data = _context.Customers.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Customers = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Customers));
        }

        public async Task<ActionResult<IEnumerable<machine>>> GetMachineData()
        {
            var machinedata = new List<machine>();
            for (int i = 0; i < 16; i++)
            {
                var nmachinedata = new machine{
                    Id = i,
                    Date = DateTime.Now.AddDays(i),
                    Name = "A" + i
                };
                for (int j = 0; i < 3; j++)
                {
                    nmachinedata.machineOrderList.Add(new machineOrder{
                    Id = i,
                    Name = "LHAIHDI" + i + j 
                });
                }
            }
            return Ok(MyFun.APIResponseOK(machinedata));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
