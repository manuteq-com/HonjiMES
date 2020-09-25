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

namespace HonjiMES.Controllers
{
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MbomModelsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MbomModelsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 模型列表
        /// </summary>
        /// <returns></returns>
        // GET: api/MbomModelHead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MbomModelHead>>> GetMbomModelHeads()
        {
            var data = _context.MbomModelHeads.Where(x => x.DeleteFlag == 0).OrderBy(x => x.ModelCode);
            var MbomModels = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(MbomModels));
        }

        /// <summary>
        /// 用MbomModelHeadID取BOM表的製程資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/MbomModels/
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetProcessByMbomModelHeadId(int id)
        {
            if (id != 0)
            {
                var MbomModelDetail = await _context.MbomModelDetails.AsQueryable().Where(x => x.MbomModelHeadId == id && x.DeleteFlag == 0).OrderBy(x => x.SerialNumber).ToListAsync();

                if (MbomModelDetail == null)
                {
                    return NotFound();
                }
                return Ok(MyFun.APIResponseOK(MbomModelDetail));

            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 新增模型資訊
        /// </summary>
        /// <param name="MbomModelData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MbomModelData>> PostMbomModels(MbomModelData MbomModelData)
        {
            if (!string.IsNullOrWhiteSpace(MbomModelData.MbomModelHead.ModelCode) && !string.IsNullOrWhiteSpace(MbomModelData.MbomModelHead.ModelName))
            {
                var MbomModelHeads= await _context.MbomModelHeads.Where(x=>x.ModelCode == MbomModelData.MbomModelHead.ModelCode && x.ModelName == MbomModelData.MbomModelHead.ModelName && x.DeleteFlag == 0).ToListAsync();
                if(MbomModelHeads.Count()==0){
                    var nMbomModelHead = new MbomModelHead
                    {
                        ModelCode = MbomModelData.MbomModelHead.ModelCode,
                        ModelName = MbomModelData.MbomModelHead.ModelName,
                        ModelRemarks = MbomModelData.MbomModelHead.ModelRemarks,
                        CreateTime = DateTime.Now,
                        CreateUser = MyFun.GetUserID(HttpContext)
                    };

                    foreach (var item in MbomModelData.MbomModelDetail)
                    {
                        var ProcessInfo = _context.Processes.Find(item.ProcessId);
                        var nMbomModelDetail = new MbomModelDetail
                        {
                            SerialNumber = item.SerialNumber,
                            ProcessId = item.ProcessId,
                            ProcessNo = ProcessInfo.Code,
                            ProcessName = ProcessInfo.Name,
                            ProcessLeadTime = item.ProcessLeadTime,
                            ProcessTime = item.ProcessTime,
                            ProcessCost = item.ProcessCost,
                            DrawNo = item.DrawNo,
                            Manpower = item.Manpower,
                            ProducingMachine = item.ProducingMachine,
                            Remarks = item.Remarks,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        };
                        nMbomModelHead.MbomModelDetails.Add(nMbomModelDetail);
                    }
                    _context.MbomModelHeads.Add(nMbomModelHead);
                    await _context.SaveChangesAsync();
                    return Ok(MyFun.APIResponseOK("OK"));
                }else{
                    
                    return Ok(MyFun.APIResponseError("代號名稱已經存在!"));
                }

                
            }
            else
            {
                return Ok(MyFun.APIResponseError("模型新增失敗!"));
            }
        }

        /// <summary>
        /// 更新模型資訊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="MbomModelData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutMbomModels(int id, MbomModelData MbomModelData)
        {
            if (id != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var updataCheck = new List<int>();
                var OMbomModelHeads = _context.MbomModelHeads.Find(id);
                foreach (var item in MbomModelData.MbomModelDetail) // 依照新的工序清單逐一更新(新建)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    if (item.Id != 0)   // 如果ID不為0，則表示為既有工序，只進行更新
                    {
                        updataCheck.Add(item.Id);
                        var OMbomModelDetail = OMbomModelHeads.MbomModelDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                        OMbomModelDetail.SerialNumber = item.SerialNumber;
                        OMbomModelDetail.SerialNumber = item.SerialNumber;
                        OMbomModelDetail.ProcessId = item.ProcessId;
                        OMbomModelDetail.ProcessNo = ProcessInfo.Code;
                        OMbomModelDetail.ProcessName = ProcessInfo.Name;
                        OMbomModelDetail.ProcessLeadTime = item.ProcessLeadTime;
                        OMbomModelDetail.ProcessTime = item.ProcessTime;
                        OMbomModelDetail.ProcessCost = item.ProcessCost;
                        OMbomModelDetail.DrawNo = item.DrawNo;
                        OMbomModelDetail.Manpower = item.Manpower;
                        OMbomModelDetail.ProducingMachine = item.ProducingMachine;
                        OMbomModelDetail.Remarks = item.Remarks;
                        OMbomModelDetail.UpdateUser = MyFun.GetUserID(HttpContext);
                    }
                    else // 如ID為0，則表示該工序為新增
                    {
                        var nMbomModelDetail = new MbomModelDetail
                        {
                            SerialNumber = item.SerialNumber,
                            ProcessId = item.ProcessId,
                            ProcessNo = ProcessInfo.Code,
                            ProcessName = ProcessInfo.Name,
                            ProcessLeadTime = item.ProcessLeadTime,
                            ProcessTime = item.ProcessTime,
                            ProcessCost = item.ProcessCost,
                            DrawNo = item.DrawNo,
                            Manpower = item.Manpower,
                            ProducingMachine = item.ProducingMachine,
                            Remarks = item.Remarks,
                            CreateUser = MyFun.GetUserID(HttpContext),
                            UpdateUser = MyFun.GetUserID(HttpContext)
                        };
                        OMbomModelHeads.MbomModelDetails.Add(nMbomModelDetail);
                    }
                }
                foreach (var item in OMbomModelHeads.MbomModelDetails) // 檢查剩下未更新的工序，變更為[刪除]
                {
                    if (!updataCheck.Exists(x => x == item.Id) && item.Id != 0 && item.DeleteFlag == 0)
                    {
                        item.DeleteFlag = 1;
                    }
                }

                var Msg = MyFun.MappingData(ref OMbomModelHeads, MbomModelData.MbomModelHead);
                OMbomModelHeads.UpdateTime = DateTime.Now;
                OMbomModelHeads.UpdateUser = MyFun.GetUserID(HttpContext);
                try
                {
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return Ok(MyFun.APIResponseOK(MbomModelData));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單更新失敗!"));
            }
        }

        /// <summary>
        /// 刪除模型資訊
        /// </summary>
        /// <returns></returns>
        // DELETE: api/MbomModels/
        [HttpDelete("{id}")]
        public async Task<ActionResult<MbomModelData>> DeleteMbomModels(int id)
        {
            if (id != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var MbomModelHeads = _context.MbomModelHeads.Find(id);
                foreach (var item in MbomModelHeads.MbomModelDetails)
                {
                    item.DeleteFlag = 1;
                }
                MbomModelHeads.DeleteFlag = 1;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("模型刪除失敗!"));
            }
        }

    }
}
