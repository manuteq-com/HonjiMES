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
        /// 工單號
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrderNumber()
        {
            var key = "WO";
            var dt = DateTime.Now;
            var WorkOrderNo = dt.ToString("yyMMdd");

            var NoData = await _context.WorkOrders.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                // if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                // }
            }
            var WorkOrderHeadData = new WorkOrder{
                CreateTime = dt,
                WorkOrderNo = key + WorkOrderNo + NoCount.ToString("000")
            };
            return Ok(MyFun.APIResponseOK(WorkOrderHeadData));
        }

        /// <summary>
        /// 工單號
        /// </summary>
        /// <returns></returns>
        // POST: api/Processes
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetWorkOrderNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null) {
                var key = "WO";
                var WorkOrderNo = CreateNoData.CreateTime.ToString("yyMMdd");
                
                var NoData = await _context.WorkOrders.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1) {
                    var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                    var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                    // if (NoCount <= NoLast) {
                        NoCount = NoLast + 1;
                    // }
                }
                CreateNoData.CreateNumber = key + WorkOrderNo + NoCount.ToString("000");
                return Ok(MyFun.APIResponseOK(CreateNoData));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessesStatus>> GetWorkOrderByStatus(int id)
        {
            var ptype = typeof(ProcessesData);
            var ProcessesStatus = new ProcessesStatus();
            var ColumnOptionlist = new List<ColumnOption> {
                new ColumnOption{ key="Temp0", title ="製程1",show=false,Columnlock=""},
                new ColumnOption{ key="Temp1", title ="製程2",show=false,Columnlock=""},
                new ColumnOption{ key="Temp2", title ="製程3",show=false,Columnlock=""},
                new ColumnOption{ key="Temp3", title ="製程4",show=false,Columnlock=""},
                new ColumnOption{ key="Temp4", title ="製程5",show=false,Columnlock=""},
                new ColumnOption{ key="Temp5", title ="製程6",show=false,Columnlock=""},
                new ColumnOption{ key="Temp6", title ="製程7",show=false,Columnlock=""},
                new ColumnOption{ key="Temp7", title ="製程8",show=false,Columnlock=""},
                new ColumnOption{ key="Temp8", title ="製程9",show=false,Columnlock=""},
                new ColumnOption{ key="Temp9", title ="製程10",show=false,Columnlock=""},
                new ColumnOption{ key="Temp10", title ="製程11",show=false,Columnlock=""},
                new ColumnOption{ key="Temp11", title ="製程12",show=false,Columnlock=""},
                new ColumnOption{ key="Temp12", title ="製程13",show=false,Columnlock=""},
                new ColumnOption{ key="Temp13", title ="製程14",show=false,Columnlock=""},
                new ColumnOption{ key="Temp14", title ="製程15",show=false,Columnlock=""},
                new ColumnOption{ key="Temp15", title ="製程16",show=false,Columnlock=""},
                new ColumnOption{ key="Temp16", title ="製程17",show=false,Columnlock=""},
                new ColumnOption{ key="Temp17", title ="製程18",show=false,Columnlock=""},
                new ColumnOption{ key="Temp18", title ="製程19",show=false,Columnlock=""},
                new ColumnOption{ key="Temp19", title ="製程20",show=false,Columnlock=""},
            };

            _context.ChangeTracker.LazyLoadingEnabled = true;
            var ProcessesDataList = new List<ProcessesData>();
            try
            {
                var WorkOrdersHeads = _context.WorkOrders.Where(x => x.Status == id).ToList().GroupBy(x => x.WorkOrderNo);
                foreach (var item in WorkOrdersHeads)
                {
                    var nProcessesData = new ProcessesData
                    {
                        Key = item.FirstOrDefault().Id,
                        WorkOrderNo = item.Key,
                        Name = item.FirstOrDefault().ProductBasic.Name,
                        ProductNo = item.FirstOrDefault().ProductBasic.ProductNo,
                        MachineNo = item.FirstOrDefault().MachineNo,
                        Count = item.FirstOrDefault().Count
                    };

                    var i = 0;
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        var gitem = item.ToList();
                        if (gitem.Count > i)
                        {
                            if (typeitem.Name == "Temp" + i.ToString())
                            {
                                var nTempString = new TempString
                                {
                                    value0 = gitem[i].ProcessNo,
                                    value1 = gitem[i].ProcessName,
                                    value2 = gitem[i].ProducingMachine
                                };
                                typeitem.SetValue(nProcessesData, nTempString);
                                foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + i.ToString()))
                                {
                                    Columnitem.show = true;
                                }
                                i++;
                            }
                        }
                    }
                    ProcessesDataList.Add(nProcessesData);
                }
                ProcessesStatus.ColumnOptionlist = ColumnOptionlist;//顯示項目
                ProcessesStatus.ProcessesDataList = ProcessesDataList;
            }
            catch (System.Exception ex)
            {
                throw;
            }
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
        
        /// <summary>
        /// 用WorkOrderNo取製程資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetProcessByWorkOrderNo(int id)
        {
            var WorkOrder = await _context.WorkOrders.Where(x => x.Id == id).FirstOrDefaultAsync();
            var WorkOrders = await _context.WorkOrders.Where(x => x.WorkOrderNo == WorkOrder.WorkOrderNo).ToListAsync();
            if (WorkOrders == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(WorkOrders));
        }

        /// <summary>
        /// 新增MBOM表
        /// </summary>
        /// <param name="WordOrderData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WordOrderData>> PostWorkOrderList(WordOrderData WordOrderData)
        {
            if (WordOrderData.ProductBasicId != 0)
            {
                var checkIndex = 0;
                var workOrderNo = "";
                foreach (var item in WordOrderData.MBillOfMaterialList)
                {
                    //取得工單號
                    if (checkIndex == 0) {
                        var key = "WO";
                        var WorkOrderNo = WordOrderData.CreateTime.ToString("yyMMdd");
                        var NoData = await _context.WorkOrders.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                        var NoCount = NoData.Count() + 1;
                        if (NoCount != 1) {
                            var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                            var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                            // if (NoCount <= NoLast) {
                                NoCount = NoLast + 1;
                            // }
                        }
                        workOrderNo = key + WorkOrderNo + NoCount.ToString("000");
                    }

                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nWorkOrder = new WorkOrder
                    {
                        WorkOrderNo = workOrderNo,
                        SerialNumber = item.SerialNumber ,
                        MachineNo = WordOrderData.MachineNo,
                        ProductBasicId = WordOrderData.ProductBasicId,
                        ProcessId = item.ProcessId,
                        ProcessNo = ProcessInfo.Code,
                        ProcessName = ProcessInfo.Name,
                        ProcessLeadTime = item.ProcessLeadTime,
                        ProcessTime = item.ProcessTime,
                        ProcessCost = item.ProcessCost,
                        Count = WordOrderData.Count,
                        ProducingMachine = item.ProducingMachine,
                        Remarks = item.Remarks,
                        CreateUser = 1
                    };
                    _context.WorkOrders.Add(nWorkOrder);
                }
                await _context.SaveChangesAsync();
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        private bool ProcesseExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
