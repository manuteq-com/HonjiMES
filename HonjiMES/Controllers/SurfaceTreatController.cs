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
    public class SurfaceTreatController : ControllerBase
    {
        private readonly HonjiContext _context;

        public SurfaceTreatController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 表處轉工單前，確認清單
        /// </summary>
        /// <param name="SurfaceData"></param>
        /// <param name="workOrderNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> SurfaceToWorkOrderCheck(SurfaceData SurfaceData)
        {
            if (SurfaceData.Quantity != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeadList = new List<WorkOrderHead>();
                var surfaceCount = 0;
                foreach (var item in SurfaceData.nWorkOrderNo)
                {
                    surfaceCount += SurfaceData.Quantity;
                }
                WorkOrderHeadList = await this.NewWorkOrderByPurchaseCheck(SurfaceData, SurfaceData.nWorkOrderNo.FirstOrDefault());
                WorkOrderHeadList.FirstOrDefault().Count = surfaceCount;
                
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(WorkOrderHeadList));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單派工失敗!"));
            }
        }


        //檢查有無工單，沒有的話給工單號。
        public async Task<List<WorkOrderHead>> NewWorkOrderByPurchaseCheck(PurchaseDetail PurchaseDetail, string workOrderNo)
        {
            var WorkOrderHeadList = new List<WorkOrderHead>();
            if (PurchaseDetail.DataId != 0 )
            {
                var nWorkOrderNo = "";
                //計算長度為14的工單有幾筆資料
                var lNoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(workOrderNo) && x.WorkOrderNo.Length == 14 && x.DeleteFlag == 0)
                .OrderByDescending(x => x.Id).ToListAsync();
                var lNoCount = lNoData.Count() + 1;
                //新工單單號
                //判斷是否有工單號-XX
                if(lNoData.Count() != 0){
                    var lLastWorkOrderNo = lNoData.FirstOrDefault().WorkOrderNo;
                    var lNoLast = Int32.Parse(lLastWorkOrderNo.Substring(lLastWorkOrderNo.Length - 2, 2));
                    lNoCount = lNoLast + 1;
                }
                nWorkOrderNo = workOrderNo + '-' + lNoCount.ToString("00");
                
                //201表處產品數量變動
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).ToListAsync();
                var BasicData = _context.MaterialBasics.Find(PurchaseDetail.DataId);
                BasicDataID = BasicData.Id;
                BasicDataNo = BasicData.MaterialNo;
                BasicDataName = BasicData.Name;

                //工單表頭內容
                var nWorkOrderHead = new WorkOrderHeadInfoBySurfacetreat
                {
                    WorkOrderNo = nWorkOrderNo,
                    PurchaseDetailId = PurchaseDetail.DataId,
                    // MachineNo = PurchaseDetail.,
                    DataType = BasicData.MaterialType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    Count = PurchaseDetail.Quantity,
                    Status = 1, // 注意!原本用於表示該工單目前狀態，這裡借來表示[該工單是否已經建立]
                    CreateUser = MyFun.GetUserID(HttpContext),
                };
                WorkOrderHeadList.Add(nWorkOrderHead);

            }
            return WorkOrderHeadList;
        }
    }
}
