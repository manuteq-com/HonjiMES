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
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachinesController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/Machines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineInformation>>> GetMachines()
        {
            var data = _context.MachineInformations.AsQueryable().Where(x => x.EnableState == 1);
            var Machines = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Machines));
        }

        // GET: api/Machines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineInformation>> GetMaterial(int id)
        {
            // var machine = await _context.Machines.FindAsync(id);
            var machine = await _context.MachineInformations.AsQueryable().Where(x => x.Id == id).FirstAsync();

            if (machine == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(machine));
        }

    }
}
