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
    /// 製程基本資料列表
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProcessesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProcessesController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;
        }
        /// <summary>
        /// 查詢製程基本資料列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcesses()
        {
            var data = _context.Processes.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Processes = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Processes));
        }

        /// <summary>
        /// 使用ID查詢製程基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Process>> GetProcess(int id)
        {
            var process = await _context.Processes.FindAsync(id);

            if (process == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 修改製程基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        // PUT: api/Processes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcess(int id, Process process)
        {
            process.Id = id;
            var Oprocess = _context.Processes.Find(id);
            var Cprocess = Oprocess;
            if (!string.IsNullOrWhiteSpace(process.Code))
            {
                Cprocess.Code = process.Code;
            }
            if (!string.IsNullOrWhiteSpace(process.Name))
            {
                Cprocess.Name = process.Name;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.Processes.AsQueryable().Where(x => x.Id != id && (x.Name == Cprocess.Name || x.Code == Cprocess.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("製程的的 [代號] 或 [名稱] 重複!", Cprocess));
            }

            var Msg = MyFun.MappingData(ref Oprocess, process);
            Oprocess.UpdateTime = DateTime.Now;
            Oprocess.UpdateUser = 1;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcesseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 新增製程基本資料列表
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        // POST: api/Processes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Process>> PostProcess(Process process)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.Processes.AsQueryable().Where(x => (x.Name == process.Name || x.Code == process.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("製程的 [代號] 或 [名稱] 已存在!", process));
            }
            _context.Processes.Add(process);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 刪除製程基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Processes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Process>> DeleteProcess(int id)
        {
            var process = await _context.Processes.FindAsync(id);
            if (process == null)
            {
                return NotFound();
            }
            process.DeleteFlag = 1;
            // _context.Processes.Remove(process);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(process));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessesStatus>> GetWorkOrderByStatus(int id)
        {
            var Processesname = _context.Processes.Select(x => x.Name).ToList();
            var ptype = typeof(ProcessesData);
            var ProcessesStatus = new ProcessesStatus();
            var ColumnOptionlist = new List<ColumnOption> {
                new ColumnOption{ key="Temp0", title ="製程1",show=true,Columnlock=""},
                new ColumnOption{ key="Temp1", title ="製程2",show=true,Columnlock=""},
                new ColumnOption{ key="Temp2", title ="製程3",show=true,Columnlock=""},
                new ColumnOption{ key="Temp3", title ="製程4",show=true,Columnlock=""},
                new ColumnOption{ key="Temp4", title ="製程5",show=true,Columnlock=""},
                new ColumnOption{ key="Temp5", title ="製程6",show=true,Columnlock=""},
                new ColumnOption{ key="Temp6", title ="製程7",show=true,Columnlock=""},
                new ColumnOption{ key="Temp7", title ="製程8",show=true,Columnlock=""},
                new ColumnOption{ key="Temp8", title ="製程9",show=true,Columnlock=""},
                new ColumnOption{ key="Temp9", title ="製程10",show=true,Columnlock=""},
                new ColumnOption{ key="Temp10", title ="製程11",show=true,Columnlock=""},
                new ColumnOption{ key="Temp11", title ="製程12",show=true,Columnlock=""},
                new ColumnOption{ key="Temp12", title ="製程13",show=true,Columnlock=""},
                new ColumnOption{ key="Temp13", title ="製程14",show=true,Columnlock=""},
                new ColumnOption{ key="Temp14", title ="製程15",show=true,Columnlock=""},
                new ColumnOption{ key="Temp15", title ="製程16",show=true,Columnlock=""},
                new ColumnOption{ key="Temp16", title ="製程17",show=true,Columnlock=""},
                new ColumnOption{ key="Temp17", title ="製程18",show=true,Columnlock=""},
                new ColumnOption{ key="Temp18", title ="製程19",show=true,Columnlock=""},
                new ColumnOption{ key="Temp19", title ="製程20",show=true,Columnlock=""},
            };
            
            var j = 0;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var ProcessesDataList = new List<ProcessesData>();
            try
            {
                var WorkOrdersHeads = _context.WorkOrders.Where(x => x.Status == id).ToList().GroupBy(x => x.WorkOrderNo);
                foreach (var item in WorkOrdersHeads)
                {
                    // var WorkOrdersDetails = await _context.WorkOrders.Where(x => x.Status == id && x.WorkOrderNo == item).ToListAsync();
                    var nProcessesData = new ProcessesData
                    {
                        Key = item.FirstOrDefault().Id,
                        Name = item.FirstOrDefault().ProductBasic.Name,
                        ProductNo = item.FirstOrDefault().ProductBasic.ProductNo,
                        Count = item.FirstOrDefault().Count
                    };
                    var i = 0;
                    foreach (var item2 in item)
                    {
                        foreach (var Columnitem in ColumnOptionlist)
                        {
                            foreach (var item3 in ptype.GetProperties())
                            {
                                if (Columnitem.key == item3.Name)
                                {
                                    
                                    try
                                    {
                                        var nTempString = new TempString
                                        {
                                            value0 = Processesname[j],
                                            value1 = item3.Name,
                                            value2 = Columnitem.Columnlock
                                        };
                                        item3.SetValue(nProcessesData, nTempString);
                                        j++;
                                        if (j > Processesname.Count - 1)
                                        {
                                            j = 0;
                                        }
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                        }
                        ProcessesDataList.Add(nProcessesData);
                    }
                }    
            }
            catch (System.Exception ex)
            {
                throw;
            }
            
            for (int i = 1; i < 6; i++)
            {
                var ProductBasics = await _context.ProductBasics.Where(x => x.Id == i).FirstOrDefaultAsync();
                var nProcessesData = new ProcessesData
                {
                    Key = ProductBasics.Id,
                    Name = ProductBasics.Name,
                    ProductNo = ProductBasics.ProductNo,
                    Count = 1
                };
                foreach (var Columnitem in ColumnOptionlist)
                {
                    foreach (var item in ptype.GetProperties())
                    {
                        if (Columnitem.key == item.Name)
                        {
                            try
                            {
                                var nTempString = new TempString
                                {
                                    value0 = Processesname[j],
                                    value1 = item.Name,
                                    value2 = Columnitem.Columnlock
                                };
                                item.SetValue(nProcessesData, nTempString);
                                j++;
                                if (j > Processesname.Count - 1)
                                {
                                    j = 0;
                                }
                            }
                            catch
                            {

                            }

                        }
                    }
                }
                ProcessesDataList.Add(nProcessesData);
            }
            ProcessesStatus.ColumnOptionlist = ColumnOptionlist;//顯示項目
            ProcessesStatus.ProcessesDataList = ProcessesDataList;
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(ProcessesStatus));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessesStatus>> GetProcessesStatus(int id)
        {
            var Processesname = _context.Processes.Select(x => x.Name).ToList();
            var ptype = typeof(ProcessesData);
            var ProcessesStatus = new ProcessesStatus();
            var ColumnOptionlist = new List<ColumnOption> {
                new ColumnOption{ key="Temp0", title ="製程1",show=true,Columnlock=""},
                new ColumnOption{ key="Temp1", title ="製程2",show=true,Columnlock=""},
                new ColumnOption{ key="Temp2", title ="製程3",show=true,Columnlock=""},
                new ColumnOption{ key="Temp3", title ="製程4",show=true,Columnlock=""},
                new ColumnOption{ key="Temp4", title ="製程5",show=true,Columnlock=""},
                new ColumnOption{ key="Temp5", title ="製程6",show=true,Columnlock=""},
                new ColumnOption{ key="Temp6", title ="製程7",show=true,Columnlock=""},
                new ColumnOption{ key="Temp7", title ="製程8",show=true,Columnlock=""},
                new ColumnOption{ key="Temp8", title ="製程9",show=true,Columnlock=""},
                new ColumnOption{ key="Temp9", title ="製程10",show=true,Columnlock=""},
                new ColumnOption{ key="Temp10", title ="製程11",show=true,Columnlock=""},
                new ColumnOption{ key="Temp11", title ="製程12",show=true,Columnlock=""},
                new ColumnOption{ key="Temp12", title ="製程13",show=true,Columnlock=""},
                new ColumnOption{ key="Temp13", title ="製程14",show=true,Columnlock=""},
                new ColumnOption{ key="Temp14", title ="製程15",show=true,Columnlock=""},
                new ColumnOption{ key="Temp15", title ="製程16",show=true,Columnlock=""},
                new ColumnOption{ key="Temp16", title ="製程17",show=true,Columnlock=""},
                new ColumnOption{ key="Temp17", title ="製程18",show=true,Columnlock=""},
                new ColumnOption{ key="Temp18", title ="製程19",show=true,Columnlock=""},
                new ColumnOption{ key="Temp19", title ="製程20",show=true,Columnlock=""},
            };
            var j = 0;
            var ProcessesDataList = new List<ProcessesData>();
            for (int i = 1; i < 6; i++)
            {
                var ProductBasics = await _context.ProductBasics.Where(x => x.Id == i).FirstOrDefaultAsync();
                var nProcessesData = new ProcessesData
                {
                    Key = ProductBasics.Id,
                    Name = ProductBasics.Name,
                    ProductNo = ProductBasics.ProductNo,
                    Count = 1
                };
                foreach (var Columnitem in ColumnOptionlist)
                {
                    foreach (var item in ptype.GetProperties())
                    {
                        if (Columnitem.key == item.Name)
                        {
                            try
                            {
                                var nTempString = new TempString
                                {
                                    value0 = Processesname[j],
                                    value1 = item.Name,
                                    value2 = Columnitem.Columnlock
                                };
                                item.SetValue(nProcessesData, nTempString);
                                j++;
                                if (j > Processesname.Count - 1)
                                {
                                    j = 0;
                                }
                            }
                            catch
                            {

                            }

                        }
                    }
                }
                ProcessesDataList.Add(nProcessesData);
            }
            ProcessesStatus.ColumnOptionlist = ColumnOptionlist;//顯示項目
            ProcessesStatus.ProcessesDataList = ProcessesDataList;
            return Ok(MyFun.APIResponseOK(ProcessesStatus));
        }
        private bool ProcesseExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
