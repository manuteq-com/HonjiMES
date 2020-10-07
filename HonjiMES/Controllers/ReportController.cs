using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;
using DevExtreme.AspNet.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using HonjiMES.Helper;
using System.Drawing;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.UI;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using NPOI.POIFS.NIO;

namespace HonjiMES.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly HonjiContext _context;
        private readonly IWebHostEnvironment _IWebHostEnvironment;

        public ReportController(HonjiContext context, IWebHostEnvironment environment)
        {
            _IWebHostEnvironment = environment;
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 匯出採購單
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetPurchaseOrderPDF(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var PurchaseOrderReport = new List<PurchaseOrderReportVM>();
            var PurchaseHead = _context.PurchaseHeads.Find(id);
            var Users = _context.Users.Where(x => x.DeleteFlag == 0).ToList();
            var txt = PurchaseHead.PurchaseNo;
            var i = 0;
            foreach (var item in PurchaseHead.PurchaseDetails)
            {
                if (item.DeleteFlag == 0)
                {
                    i = i + 1;
                    PurchaseOrderReport.Add(new PurchaseOrderReportVM
                    {
                        No = i,
                        DataNo = item.DataNo,
                        DataName = item.DataName,
                        Specification = item.Specification,
                        PurchaseQuantity = item.Quantity,
                        PurchasedQuantity = item.PurchasedCount,
                        // Status = 
                        Warehouse = item.Warehouse.Code + item.Warehouse.Name,
                        PurchasePrice = item.OriginPrice,
                        PurchaseTotal = item.Price,
                        DeliveryDate = item.DeliveryTime
                    });
                }
            }
            var title = "";
            if (PurchaseHead.Type == 20)
            {
                title = "《託外處理》";
            }
            else
            {
                title = "";
            }
            var json = JsonConvert.SerializeObject(PurchaseOrderReport);
            var webRootPath = _IWebHostEnvironment.ContentRootPath;
            var ReportPath = Path.Combine(webRootPath, "Reports", "PurchaseOrderReport.repx");
            var report = XtraReport.FromFile(ReportPath);
            report.RequestParameters = false;
            report.Parameters["Title"].Value = title;
            report.Parameters["CreateDate"].Value = DateTime.Now;
            report.Parameters["ContactName"].Value = PurchaseHead.Supplier.ContactName;
            report.Parameters["Address"].Value = PurchaseHead.Supplier.Address;
            report.Parameters["PurchaseDate"].Value = PurchaseHead.PurchaseDate;
            report.Parameters["PurchaseNo"].Value = PurchaseHead.PurchaseNo;
            report.Parameters["PurchaseType"].Value = PurchaseHead.Type == 10 ? "採購" : PurchaseHead.Type == 20 ? "外包" : "表處";
            report.Parameters["PurchaseUser"].Value = Users.Where(x => x.Id == PurchaseHead.CreateUser).FirstOrDefault().Realname;
            report.Parameters["SupplierCode"].Value = PurchaseHead.Supplier.Code;
            report.Parameters["SupplierName"].Value = PurchaseHead.Supplier.Name;
            report.Parameters["SupplierFax"].Value = PurchaseHead.Supplier.Fax;
            report.Parameters["SupplierTel"].Value = PurchaseHead.Supplier.Phone;
            report.Parameters["Total"].Value = PurchaseHead.PriceAll;
            var jsonDataSource = new JsonDataSource();
            jsonDataSource.JsonSource = new CustomJsonSource(json);
            report.DataSource = jsonDataSource;
            report.CreateDocument(true);
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                return File(ms.ToArray(), "application/pdf", txt + '_' + DateTime.Now.ToString("yyyyMMdd") + "_採購單.pdf");
            }
        }

        /// <summary>
        /// 匯出採購單
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetSaleOrderPDF(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var SaleOrderReport = new List<SaleOrderReportVM>();
            var SaleHead = _context.SaleHeads.Find(id);
            var txt = SaleHead.SaleNo;
            var dataCount = SaleHead.SaleDetailNews.Count();
            foreach (var item in SaleHead.SaleDetailNews)
            {
                if (item.DeleteFlag == 0)
                {
                    SaleOrderReport.Add(new SaleOrderReportVM
                    {
                        SaleNo = item.Sale.SaleNo,
                        MachineNo = item.OrderDetail.MachineNo,
                        ProductNo = item.ProductNo,
                        ProductName = item.OrderDetail.ProductBasic.Name,
                        Quantity = item.Quantity,
                        Price = item.OriginPrice,
                        Total = item.Price
                    });
                }
            }
            if (dataCount % 10 != 0)
            {
                for (int i = 0; i < 10 - dataCount % 10; i++)
                {
                    SaleOrderReport.Add(new SaleOrderReportVM
                    {
                        SaleNo = null,
                        MachineNo = null,
                        ProductNo = null,
                        ProductName = null,
                        Quantity = null,
                        Price = null,
                        Total = null
                    });
                }
            }

            var json = JsonConvert.SerializeObject(SaleOrderReport);
            var webRootPath = _IWebHostEnvironment.ContentRootPath;
            var ReportPath = Path.Combine(webRootPath, "Reports", "SaleOrderReport.repx");
            var report = XtraReport.FromFile(ReportPath);
            report.RequestParameters = false;
            report.Parameters["CreateDate"].Value = DateTime.Now;
            report.Parameters["Address"].Value = "";
            report.Parameters["CustomerName"].Value = "";
            var jsonDataSource = new JsonDataSource();
            jsonDataSource.JsonSource = new CustomJsonSource(json);
            report.DataSource = jsonDataSource;
            report.CreateDocument(true);
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                return File(ms.ToArray(), "application/pdf", txt + '_' + DateTime.Now.ToString("yyyyMMdd") + "_銷貨單.pdf");
            }
        }

        /// <summary>
        /// 匯出工單
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetWorkOrderPDF(int id)
        {
            var qcodesize = 100;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var WorkOrderReport = new List<WorkOrderReportVM>();
            var WorkOrderHead = _context.WorkOrderHeads.Find(id);
            var Users = _context.Users.Where(x => x.DeleteFlag == 0).ToList();
            var txt = WorkOrderHead.WorkOrderNo;
            var dbQrCode = BarcodeHelper.CreateQrCode(txt, qcodesize, qcodesize);
            foreach (var item in WorkOrderHead.WorkOrderDetails.OrderBy(x => x.SerialNumber))
            {
                if (item.DeleteFlag == 0)
                {
                    WorkOrderReport.Add(new WorkOrderReportVM
                    {
                        No = item.SerialNumber,
                        ProcessNo = item.ProcessNo,
                        ProcessName = item.ProcessName,
                        ProcessTime = item.ProcessTime,
                        MachineName = item.ProducingMachine,
                        DueStartTime = item.DueStartTime?.ToString("yyyy-MM-dd") ?? "",
                        DueEndTime = item.DueEndTime?.ToString("yyyy-MM-dd") ?? "",
                    });
                }
            }
            var json = JsonConvert.SerializeObject(WorkOrderReport);
            var webRootPath = _IWebHostEnvironment.ContentRootPath;
            var ReportPath = Path.Combine(webRootPath, "Reports", "WorkOrder.repx");
            var report = XtraReport.FromFile(ReportPath);
            report.RequestParameters = false;
            report.Parameters["WorkOrderNo"].Value = WorkOrderHead.WorkOrderNo;
            report.Parameters["DataNo"].Value = "　" + WorkOrderHead.DataNo;
            report.Parameters["DataName"].Value = "　" + WorkOrderHead.DataName;
            report.Parameters["Quantity"].Value = WorkOrderHead.Count;
            report.Parameters["MachineNo"].Value = WorkOrderHead.MachineNo;
            report.Parameters["DueEndTime"].Value = WorkOrderHead.DueEndTime?.ToString("MM/dd") ?? "";
            report.Parameters["CreateUser"].Value = Users.Where(x => x.Id == WorkOrderHead.CreateUser).FirstOrDefault().Realname;
            report.Parameters["CreateTime"].Value = "　" + DateTime.Now;
            report.Parameters["QRCode"].Value = MyFun.ImgToBase64String(dbQrCode);
            var jsonDataSource = new JsonDataSource();
            jsonDataSource.JsonSource = new CustomJsonSource(json);
            report.DataSource = jsonDataSource;
            report.CreateDocument(true);
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                return File(ms.ToArray(), "application/pdf", txt + '_' + DateTime.Now.ToString("yyyyMMdd") + "_工單.pdf");
            }
        }
        
        /// <summary>
        /// 產報工單PDF
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetPackingSlipPDF(int id)
        {
            var qcodesize = 70;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var ProcessReportVMList = new List<ProcessReportVM>();
            var WorkOrder = _context.WorkOrderHeads.Find(id);
            var txt = "";
            foreach (var item in WorkOrder.WorkOrderDetails)
            {
                txt = item.WorkOrderHead.WorkOrderNo;
                var dbQrCode = BarcodeHelper.CreateQrCode(txt + "-" + item.SerialNumber, qcodesize, qcodesize);
                ProcessReportVMList.Add(new ProcessReportVM
                {
                    SerialNumber = item.SerialNumber,
                    ProcessName = item.ProcessName,
                    ProducingMachine = item.ProducingMachine,
                    Status = item.Status.ToString(),
                    Type = item.Type.ToString(),
                    ReCount = item.ReCount,
                    ActualStartTime = item.ActualStartTime,
                    ActualEndTime = item.ActualEndTime,
                    Img = MyFun.ImgToBase64String(dbQrCode),
                });
            }
            ProcessReportVMList.AddRange(ProcessReportVMList);
            ProcessReportVMList.AddRange(ProcessReportVMList);
            var json = JsonConvert.SerializeObject(ProcessReportVMList);
            var bQrCode = BarcodeHelper.CreateQrCode(txt, qcodesize, qcodesize);
            var webRootPath = _IWebHostEnvironment.ContentRootPath;
            var ReportPath = Path.Combine(webRootPath, "Reports", "process.repx");
            var report = XtraReport.FromFile(ReportPath);
            report.RequestParameters = false;
            report.Parameters["WorkOrderNo"].Value = WorkOrder.WorkOrderNo;
            report.Parameters["DataName"].Value = WorkOrder.DataName;
            report.Parameters["DataNo"].Value = WorkOrder.DataNo;
            var PictureBox = (XRPictureBox)report.FindControl("Qrcode", true);
            PictureBox.Image = Image.FromStream(new MemoryStream(bQrCode));
            var jsonDataSource = new JsonDataSource();
            jsonDataSource.JsonSource = new CustomJsonSource(json);
            report.DataSource = jsonDataSource;
            report.CreateDocument(true);
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                return File(ms.ToArray(), "application/pdf", txt + "報工單.pdf");
            }

        }

    }
}
