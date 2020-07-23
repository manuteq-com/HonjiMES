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
        /// 產生新的工單號
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderHead>>> GetWorkOrderNumber()
        {
            var key = "WO";
            var dt = DateTime.Now;
            var WorkOrderNo = dt.ToString("yyMMdd");

            var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                // if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                // }
            }
            var WorkOrderHeadData = new WorkOrderHead{
                CreateTime = dt,
                WorkOrderNo = key + WorkOrderNo + NoCount.ToString("000")
            };
            return Ok(MyFun.APIResponseOK(WorkOrderHeadData));
        }

        /// <summary>
        /// 產生新的工單號(藉由日期)
        /// </summary>
        /// <returns></returns>
        // POST: api/Processes
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetWorkOrderNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null) {
                var key = "WO";
                var WorkOrderNo = CreateNoData.CreateTime.ToString("yyMMdd");
                
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
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
        /// 藉由工單狀態取得工單資訊
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
                var WorkOrderHeads = new List<WorkOrderHead>();
                if (id == 0) {
                    WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                } else {
                    WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.Status == id && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                }
                
                foreach (var item in WorkOrderHeads)
                {
                    var BasicDataNo = "";
                    var BasicDataName = "";
                    if (item.DataType == 0) {
                        var BasicData = _context.MaterialBasics.Find(item.DataId);
                        BasicDataNo = BasicData.MaterialNo;
                        BasicDataName = BasicData.Name;
                    } else if (item.DataType == 1) {
                        var BasicData = _context.ProductBasics.Find(item.DataId);
                        BasicDataNo = BasicData.ProductNo;
                        BasicDataName = BasicData.Name;
                    } else if (item.DataType == 2) {
                        var BasicData = _context.WiproductBasics.Find(item.DataId);
                        BasicDataNo = BasicData.WiproductNo;
                        BasicDataName = BasicData.Name;
                    }

                    var nProcessesData = new ProcessesData
                    {
                        Key = item.Id,
                        WorkOrderNo = item.WorkOrderNo,
                        BasicDataName = BasicDataName,
                        BasicDataNo = BasicDataNo,
                        MachineNo = item.MachineNo,
                        Count = item.Count,
                        Status = item.Status
                    };

                    var i = 0;
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        var gitem = item.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToList();
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
        /// 藉由工單狀態取得工單資訊(舊)
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
                    BasicDataName = ProductBasics.Name,
                    BasicDataNo = ProductBasics.ProductNo,
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
        /// 用WorkOrderID取得製程資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderData>> GetProcessByWorkOrderId(int id)
        {
            var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(id);
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == id && x.DeleteFlag == 0).ToListAsync();
            var WorkOrderData = new WorkOrderData{
                WorkOrderHead = WorkOrderHeads,
                WorkOrderDetail = WorkOrderDetails
            };
            // var WorkOrder = await _context.WorkOrderHeads.Include(x => x.WorkOrderDetails).Where(x => x.Id == id).Where(x => x.WorkOrderDetails.Where(y => y.DeleteFlag == 0)).FirstOrDefaultAsync();
            if (WorkOrderHeads == null || WorkOrderDetails == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(WorkOrderData));
        }

        /// <summary>
        /// 新增工單資訊
        /// </summary>
        /// <param name="WorkOrderData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> PostWorkOrderList(WorkOrderData WorkOrderData)
        {
            if (WorkOrderData.WorkOrderHead.DataId != 0)
            {
                //取得工單號
                var key = "WO";
                var WorkOrderNo = WorkOrderData.WorkOrderHead.CreateTime.ToString("yyMMdd");
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1) {
                    var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                    var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                    // if (NoCount <= NoLast) {
                        NoCount = NoLast + 1;
                    // }
                }
                var workOrderNo = key + WorkOrderNo + NoCount.ToString("000");

                var DataType = 1;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                if (DataType == 0) {

                } else if (DataType == 1) {
                    var BasicData = _context.ProductBasics.Find(WorkOrderData.WorkOrderHead.DataId);
                    BasicDataID = BasicData.Id;
                    BasicDataNo = BasicData.ProductNo;
                    BasicDataName = BasicData.Name;
                } else if (DataType == 2) {

                }
                var nWorkOrderHead = new WorkOrderHead
                {
                    WorkOrderNo = workOrderNo,
                    MachineNo = WorkOrderData.WorkOrderHead.MachineNo,
                    DataType = DataType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    Count = WorkOrderData.WorkOrderHead.Count,
                    CreateUser = 1
                };

                foreach (var item in WorkOrderData.WorkOrderDetail)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nWorkOrderDetail = new WorkOrderDetail
                    {
                        SerialNumber = item.SerialNumber ,
                        ProcessId = item.ProcessId,
                        ProcessNo = ProcessInfo.Code,
                        ProcessName = ProcessInfo.Name,
                        ProcessLeadTime = item.ProcessLeadTime,
                        ProcessTime = item.ProcessTime,
                        ProcessCost = item.ProcessCost,
                        Count = WorkOrderData.WorkOrderHead.Count,
                        // PurchaseId
                        DrawNo = item.DrawNo,
                        Manpower = item.Manpower,
                        ProducingMachine = item.ProducingMachine,
                        Remarks = item.Remarks,
                        DueStartTime = item.DueStartTime,
                        DueEndTime = item.DueEndTime,
                        ActualStartTime = item.ActualStartTime,
                        ActualEndTime = item.ActualEndTime,
                        CreateUser = 1
                    };
                    nWorkOrderHead.WorkOrderDetails.Add(nWorkOrderDetail);
                }
                _context.WorkOrderHeads.Add(nWorkOrderHead);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK("OK"));
            } else {
                return Ok(MyFun.APIResponseError("工單新增失敗!"));
            }
        }
 
        /// <summary>
        /// 更新工單資訊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutWorkOrderList(int id, WorkOrderData WorkOrderData)
        {
            if (id != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var OWorkOrderHeads = _context.WorkOrderHeads.Find(id);
                foreach (var item in OWorkOrderHeads.WorkOrderDetails)
                {
                    item.DeleteFlag = 1;
                }
                foreach (var item in WorkOrderData.WorkOrderDetail)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nWorkOrderDetail = new WorkOrderDetail
                    {
                        SerialNumber = item.SerialNumber ,
                        ProcessId = item.ProcessId,
                        ProcessNo = ProcessInfo.Code,
                        ProcessName = ProcessInfo.Name,
                        ProcessLeadTime = item.ProcessLeadTime,
                        ProcessTime = item.ProcessTime,
                        ProcessCost = item.ProcessCost,
                        Count = WorkOrderData.WorkOrderHead.Count,
                        ProducingMachine = item.ProducingMachine,
                        Remarks = item.Remarks,
                        CreateUser = 1
                    };
                    OWorkOrderHeads.WorkOrderDetails.Add(nWorkOrderDetail);
                }

                var Msg = MyFun.MappingData(ref OWorkOrderHeads, WorkOrderData.WorkOrderHead);
                OWorkOrderHeads.UpdateTime = DateTime.Now;
                OWorkOrderHeads.UpdateUser = 1;
                try
                {
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.LazyLoadingEnabled = false;
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
                return Ok(MyFun.APIResponseOK(WorkOrderData));
            } else {
                return Ok(MyFun.APIResponseError("工單更新失敗!"));
            }
        }

        /// <summary>
        /// 刪除工單資訊
        /// </summary>
        /// <returns></returns>
        // DELETE: api/Processes/
        [HttpDelete("{id}")]
        public async Task<ActionResult<WorkOrderData>> DeleteWorkOrderList(int id)
        {
            if (id != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = _context.WorkOrderHeads.Find(id);
                foreach (var item in WorkOrderHeads.WorkOrderDetails)
                {
                    item.DeleteFlag = 1;
                }
                WorkOrderHeads.DeleteFlag = 1;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            } else {
                return Ok(MyFun.APIResponseError("工單刪除失敗!"));
            }
        }

        private bool ProcesseExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
