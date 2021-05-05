using DevExtreme.AspNet.Mvc;
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
    /// 製程基本資料列表
    /// </summary>
    [JWTAuthorize]
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
            var data = _context.Processes.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code);
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
            Oprocess.UpdateUser = MyFun.GetUserID(HttpContext);
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
            var key = "HJ";
            var dt = DateTime.Now;
            var WorkOrderNo = dt.ToString("yyMMdd");

            var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                // if (NoCount <= NoLast) {
                NoCount = NoLast + 1;
                // }
            }
            var WorkOrderHeadData = new WorkOrderHead
            {
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
            if (CreateNoData != null)
            {
                var key = "HJ";
                var WorkOrderNo = CreateNoData.CreateTime.ToString("yyMMdd");

                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
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
        public async Task<ActionResult<ProcessesStatus>> GetWorkOrderByMode(int mode)
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
                if (mode == 0) // 取得全部狀態的工單
                {
                    WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                }
                else // 取得已派工狀態的工單
                {
                    WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.Status == 1 && x.Status == 2 && x.DeleteFlag == 0).OrderByDescending(x => x.DispatchTime).ToListAsync();
                }

                foreach (var item in WorkOrderHeads)
                {
                    var BasicDataNo = "";
                    var BasicDataName = "";
                    // if (item.DataType == 1)
                    // {
                    var BasicData = _context.MaterialBasics.Find(item.DataId);
                    BasicDataNo = BasicData.MaterialNo;
                    BasicDataName = BasicData.Name;
                    // }
                    // else if (item.DataType == 2)
                    // {
                    //     var BasicData = _context.ProductBasics.Find(item.DataId);
                    //     BasicDataNo = BasicData.ProductNo;
                    //     BasicDataName = BasicData.Name;
                    // }
                    // else if (item.DataType == 3)
                    // {
                    //     var BasicData = _context.WiproductBasics.Find(item.DataId);
                    //     BasicDataNo = BasicData.WiproductNo;
                    //     BasicDataName = BasicData.Name;
                    // }

                    var nProcessesData = new ProcessesData
                    {
                        Key = item.Id,
                        WorkOrderNo = item.WorkOrderNo,
                        BasicDataName = BasicDataName,
                        BasicDataNo = BasicDataNo,
                        MachineNo = item.MachineNo,
                        OrderCount = item.OrderDetailAndWorkOrderHeads.Where(y => y.DataType == item.DataType && y.DataId == item.DataId && y.DeleteFlag == 0).Sum(y => y.OrdeCount),
                        Count = item.Count,
                        Status = item.Status,
                        DueEndTime = item.DueEndTime?.ToString("yyyy/MM/dd") ?? ""
                    };

                    var i = 0;
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        var gitem = item.WorkOrderDetails.Where(x => x.DeleteFlag == 0).OrderBy(x => x.SerialNumber).ToList();
                        if (gitem.Count > i)
                        {
                            if (typeitem.Name == "Temp" + i.ToString())
                            {
                                var nTempString = new TempString
                                {
                                    value0 = '[' + gitem[i].ProcessNo + ']' + gitem[i].ProcessName,
                                    value1 = gitem[i].ProcessTime.ToString(),
                                    value2 = gitem[i].ProducingMachine,
                                    value3 = gitem[i].Status,
                                    value4 = gitem[i].Type,
                                    value5 = gitem[i].Process.Type
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
        /// 用WorkOrderID取得製程資料 Head + Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderData>> GetProcessByWorkOrderId(int id)
        {
            var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(id);
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == id && x.DeleteFlag == 0)
            .Include(x => x.Process).Include(x => x.WorkOrderHead).ThenInclude(y => y.OrderDetail).OrderBy(x => x.SerialNumber).ToListAsync();

            var WorkOrderDetailDataList = new List<WorkOrderDetailData>();
            foreach (var item in WorkOrderDetails)
            {
                WorkOrderDetailDataList.Add(new WorkOrderDetailData
                {
                    Id = item.Id,
                    WorkOrderHeadId = item.WorkOrderHeadId,
                    SerialNumber = item.SerialNumber,
                    ProcessId = item.ProcessId,
                    ProcessNo = item.ProcessNo,
                    ProcessName = item.ProcessName,
                    ProcessLeadTime = item.ProcessLeadTime,
                    ProcessTime = item.ProcessTime,
                    ProcessCost = item.ProcessCost,
                    Count = item.Count,
                    PurchaseId = item.PurchaseId,
                    SupplierId = item.SupplierId,
                    DrawNo = item.DrawNo,
                    CodeNo = item.CodeNo,
                    Manpower = item.Manpower,
                    ProducingMachine = item.ProducingMachine,
                    Status = item.Status,
                    Type = item.Type,
                    Remarks = item.Remarks,
                    ReCount = item.ReCount,
                    RePrice = item.RePrice,
                    NgCount = item.NgCount,
                    TotalTime = item.TotalTime,
                    DueStartTime = item.DueStartTime,
                    DueEndTime = item.DueEndTime,
                    ActualStartTime = item.ActualStartTime,
                    ActualEndTime = item.ActualEndTime,
                    DeleteFlag = item.DeleteFlag,
                    CreateTime = item.CreateTime,
                    CreateUser = item.CreateUser,
                    UpdateTime = item.UpdateTime,
                    UpdateUser = item.UpdateUser,
                    MCount = item.MCount,
                    ProcessType = item.Process.Type,
                    WorkOrderHead = item.WorkOrderHead,
                    ExpectedlTotalTime = (item.ProcessLeadTime + item.ProcessTime) * WorkOrderHeads.Count,
                });
            }
            var WorkOrderData = new WorkOrderData2
            {
                WorkOrderHead = WorkOrderHeads,
                WorkOrderDetail = WorkOrderDetailDataList
            };
            // var WorkOrder = await _context.WorkOrderHeads.Include(x => x.WorkOrderDetails).Where(x => x.Id == id).Where(x => x.WorkOrderDetails.Where(y => y.DeleteFlag == 0)).FirstOrDefaultAsync();
            if (WorkOrderHeads == null || WorkOrderDetails == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(WorkOrderData));
        }

        /// <summary>
        /// 用WorkOrderID取得製程資料 Head
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderData>> GetProcessByWorkOrderHead(int id)
        {
            var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(id);
            if (WorkOrderHeads == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(WorkOrderHeads));
        }
        /// <summary>
        /// 用WorkOrderID取得製程資料 Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderData>> GetProcessByWorkOrderDetail(int id)
        {
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == id && x.DeleteFlag == 0).OrderBy(x => x.SerialNumber).ToListAsync();
            if (WorkOrderDetails == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(WorkOrderDetails));
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
                var key = "HJ";
                var workOrderNo = "";//新建工單的工單號
                var WorkOrderNo = WorkOrderData.WorkOrderHead.CreateTime.ToString("yyMMdd");
                //計算工單在同個日期有幾筆資料
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                //計算表處轉工單的工單有幾筆資料
                var sNoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 14 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var sNoCount = sNoData.Count() + 1;

                if (NoCount != 1)
                {
                    var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                    var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                    // if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                    // }
                }
                if (WorkOrderData.mod == "new")
                {
                    workOrderNo = key + WorkOrderNo + NoCount.ToString("000");
                }
                else if (WorkOrderData.mod == "surfacetreat")
                {
                    if (sNoCount != 1)
                    {
                        var sLastWorkOrderNo = sNoData.FirstOrDefault().WorkOrderNo;
                        var sNoLast = Int32.Parse(sLastWorkOrderNo.Substring(sLastWorkOrderNo.Length - 2, 2));
                        sNoCount = sNoLast + 1;
                    }
                    workOrderNo = key + WorkOrderNo + '-' + sNoCount.ToString("00");
                }

                // var DataType = 0;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                // if (DataType == 1)
                // {
                var BasicData = _context.MaterialBasics.Find(WorkOrderData.WorkOrderHead.DataId);
                BasicDataID = BasicData.Id;
                BasicDataNo = BasicData.MaterialNo;
                BasicDataName = BasicData.Name;
                // DataType = BasicData.MaterialType == 1 ? 1 : 2;
                // }
                // else if (DataType == 2)
                // {
                //     var BasicData = _context.ProductBasics.Find(WorkOrderData.WorkOrderHead.DataId);
                //     BasicDataID = BasicData.Id;
                //     BasicDataNo = BasicData.ProductNo;
                //     BasicDataName = BasicData.Name;
                // }
                // else if (DataType == 3)
                // {

                // }
                var nWorkOrderHead = new WorkOrderHead
                {
                    WorkOrderNo = workOrderNo,
                    MachineNo = WorkOrderData.WorkOrderHead.MachineNo,
                    DataType = BasicData.MaterialType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    DueStartTime = WorkOrderData.WorkOrderHead.DueStartTime,
                    DueEndTime = WorkOrderData.WorkOrderHead.DueEndTime,
                    Count = WorkOrderData.WorkOrderHead.Count,
                    CreateUser = MyFun.GetUserID(HttpContext)
                };

                foreach (var item in WorkOrderData.WorkOrderDetail)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nWorkOrderDetail = new WorkOrderDetail
                    {
                        SerialNumber = item.SerialNumber,
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
                        ProducingMachine = item.ProducingMachine == "" ? null : item.ProducingMachine,
                        Type = item.Type,
                        Remarks = item.Remarks,
                        DueStartTime = item.DueStartTime,
                        DueEndTime = item.DueEndTime,
                        ActualStartTime = item.ActualStartTime,
                        ActualEndTime = item.ActualEndTime,
                        CreateUser = MyFun.GetUserID(HttpContext)
                    };
                    nWorkOrderHead.WorkOrderDetails.Add(nWorkOrderDetail);
                }
                _context.WorkOrderHeads.Add(nWorkOrderHead);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
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
                var updataCheck = new List<int>();
                var OWorkOrderHeads = _context.WorkOrderHeads.Find(id);
                foreach (var item in WorkOrderData.WorkOrderDetail) // 依照新的工序清單逐一更新(新建)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    if (item.Id != 0)   // 如果ID不為0，則表示為既有工序，只進行更新
                    {
                        updataCheck.Add(item.Id);
                        var OWorkOrderDetail = OWorkOrderHeads.WorkOrderDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                        OWorkOrderDetail.SerialNumber = item.SerialNumber;
                        OWorkOrderDetail.ProcessId = item.ProcessId;
                        OWorkOrderDetail.ProcessNo = ProcessInfo.Code;
                        OWorkOrderDetail.ProcessName = ProcessInfo.Name;
                        OWorkOrderDetail.ProcessLeadTime = item.ProcessLeadTime;
                        OWorkOrderDetail.ProcessTime = item.ProcessTime;
                        OWorkOrderDetail.ProcessCost = item.ProcessCost;
                        OWorkOrderDetail.Count = WorkOrderData.WorkOrderHead.Count;
                        // OWorkOrderDetail.PurchaseId
                        OWorkOrderDetail.DrawNo = item.DrawNo;
                        OWorkOrderDetail.Manpower = item.Manpower;
                        OWorkOrderDetail.ProducingMachine = item.ProducingMachine == "" ? null : item.ProducingMachine;
                        OWorkOrderDetail.Type = item.Type;
                        OWorkOrderDetail.Remarks = item.Remarks;
                        OWorkOrderDetail.DueStartTime = item.DueStartTime;
                        OWorkOrderDetail.DueEndTime = item.DueEndTime;
                        OWorkOrderDetail.ActualStartTime = item.ActualStartTime;
                        OWorkOrderDetail.ActualEndTime = item.ActualEndTime;
                        OWorkOrderDetail.UpdateUser = MyFun.GetUserID(HttpContext);
                    }
                    else // 如ID為0，則表示該工序為新增
                    {
                        var nWorkOrderDetail = new WorkOrderDetail
                        {
                            SerialNumber = item.SerialNumber,
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
                            ProducingMachine = item.ProducingMachine == "" ? null : item.ProducingMachine,
                            Status = OWorkOrderHeads.Status,
                            Type = item.Type,
                            Remarks = item.Remarks,
                            DueStartTime = item.DueStartTime,
                            DueEndTime = item.DueEndTime,
                            ActualStartTime = item.ActualStartTime,
                            ActualEndTime = item.ActualEndTime,
                            CreateUser = MyFun.GetUserID(HttpContext),
                            UpdateUser = MyFun.GetUserID(HttpContext)
                        };
                        OWorkOrderHeads.WorkOrderDetails.Add(nWorkOrderDetail);
                    }
                }
                foreach (var item in OWorkOrderHeads.WorkOrderDetails) // 檢查剩下未更新的工序，變更為[刪除]
                {
                    if (!updataCheck.Exists(x => x == item.Id) && item.Id != 0 && item.DeleteFlag == 0)
                    {
                        item.DeleteFlag = 1;
                    }
                }

                var Msg = MyFun.MappingData(ref OWorkOrderHeads, WorkOrderData.WorkOrderHead);
                var MaterialBasic = await _context.MaterialBasics.FindAsync(OWorkOrderHeads.DataId);
                OWorkOrderHeads.DataNo = MaterialBasic.MaterialNo;
                OWorkOrderHeads.DataName = MaterialBasic.Name;
                OWorkOrderHeads.UpdateTime = DateTime.Now;
                OWorkOrderHeads.UpdateUser = MyFun.GetUserID(HttpContext);
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
            }
            else
            {
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
                foreach (var item in WorkOrderHeads.OrderDetailAndWorkOrderHeads)
                {
                    item.DeleteFlag = 1;
                }

                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單刪除失敗!"));
            }
        }

        private bool ProcesseExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }

        /// <summary>
        /// 取工單列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderHead>>> GetWorkOrderList(
            [FromQuery] DataSourceLoadOptions FromQuery,
            [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var MaterialBasics = await _context.MaterialBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
            // var ProductBasics = await _context.ProductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
            // var WiproductBasics = await _context.WiproductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
            // var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.OrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            foreach (var item in data)
            {
                // if (item.DataType == 1)
                // {
                var BasicData = MaterialBasics.Find(x => x.Id == item.DataId);
                item.DataNo = BasicData.MaterialNo;
                item.DataName = BasicData.Name;
                // }
                // else if (item.DataType == 2)
                // {
                //     var BasicData = ProductBasics.Find(x => x.Id == item.DataId);
                //     item.DataNo = BasicData.ProductNo;
                //     item.DataName = BasicData.Name;
                // }
                // else if (item.DataType == 3)
                // {
                //     var BasicData = WiproductBasics.Find(x => x.Id == item.DataId);
                //     item.DataNo = BasicData.WiproductNo;
                //     item.DataName = BasicData.Name;
                // }
            }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);

            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

    }
}
