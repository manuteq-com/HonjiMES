using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;
using HonjiMES.Filter;
using HonjiMES.CncModels;
using HonjiMES.ViewModels;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 連機資料
    /// </summary>
    //[JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialLogsController : ControllerBase
    {
        private readonly HonjiCncContext _context;
        public MaterialLogsController(HonjiCncContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        // GET: api/MachineLogs
        /// <summary>
        /// 機台加工記錄
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CncModels.MachineInformation>>> GetMachineLogs([FromQuery] DataSourceLoadOptions FromQuery)
        {
            // var data = await _context.MachineLogs.AsQueryable().Where(x => x.Status == 1)
            // .Include(x => x.Machine)
            // .ThenInclude(x => x.NcFileInformations).Where(x => x.Machine.NcFileInformations.Where(y => string.IsNullOrWhiteSpace(y.Comment)).Any()).Take(10).ToListAsync();
            var data = _context.MachineInformations.AsQueryable();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/MachineLogs
        /// <summary>
        /// 機台加工記錄名細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FromQuery"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CncLogVM>>> GetMachineLogsDetails(int id, [FromQuery] DataSourceLoadOptions FromQuery)
        {
            // var data = await _context.MachineLogs.AsQueryable().Where(x => x.Status == 1)
            // .Include(x => x.Machine)
            // .ThenInclude(x => x.NcFileInformations).Where(x => x.Machine.NcFileInformations.Where(y => string.IsNullOrWhiteSpace(y.Comment)).Any()).Take(10).ToListAsync();
            char[] charsToTrim = { '(', ')' };
            // var data = _context.MachineLogs.Where(x => x.Status == 1 && x.MachineId == id).LeftOuterJoin(_context.NcFileInformations.AsQueryable().Where(x => x.Source == id && !string.IsNullOrWhiteSpace(x.Comment.Trim(charsToTrim))), x => x.FileId, y => y.Id, (MachineLogs, NcFileInformations) => new CncLogVM
            // {
            //     Id = MachineLogs.Id,
            //     StartTime = MachineLogs.StartTime,
            //     EndTime = MachineLogs.EndTime,
            //     CompletedNumber = MachineLogs.CompletedNumber,
            //     Comment = NcFileInformations.Comment,
            //     FileName = NcFileInformations.Name
            // }).Where(x => !string.IsNullOrWhiteSpace(x.FileName)).Take(20).ToList();

            var data = _context.MachineLogs.Where(x => x.Status == 1 && x.MachineId == id && x.FileId.HasValue).GroupBy(x => x.FileId).Select(x => new CncLogVM
            {

                Id = x.Key.Value,
                StartTime = x.Min(y => y.StartTime),
                EndTime = x.Max(y => y.EndTime),
                CompletedNumber = x.Sum(y => y.CompletedNumber),
                // Name = _context.NcFileInformations.Where(y => y.Source == id && y.Id == x.Key.Value).FirstOrDefault().Name,
                // Comment = _context.NcFileInformations.Where(y => y.Source == id && y.Id == x.Key.Value).FirstOrDefault().Comment
            }).ToList();
            //var trim = new char[] { '(', ')' };
            foreach (var item in data)
            {
                var FileInf = _context.NcFileInformations.Where(y => y.Source == id && y.Id == item.Id).FirstOrDefault();
                if (FileInf != null)
                {
                    item.FileInf = FileInf;
                    item.Name = FileInf.Name;
                    item.Comment = FileInf.Comment.Replace("(", "").Replace(")", "");
                }
            }
            // var data = _context.MachineLogs.Where(x => x.Status == 1 && x.MachineId == id).Select(x => new CncLogVM
            // {
            //     Id = x.Id,
            //     StartTime = x.StartTime,
            //     EndTime = x.EndTime,
            //     CompletedNumber = x.CompletedNumber,
            //     Name = _context.NcFileInformations.Where(y => y.Source == x.MachineId && y.Id == x.FileId).First().Name,
            //     Comment = _context.NcFileInformations.Where(y => y.Source == x.MachineId && y.Id == x.FileId).First().Comment
            // }).ToList();
            //var sql = data.ToSql();
            // var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(data));
        }
    }
}
