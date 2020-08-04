using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Helper;
using HonjiMES.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 產生一維碼、二維碼
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    public class CodesController : Controller
    {
        private readonly HonjiContext _context;

        public CodesController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetQrCode()
        {
            var idlist = new List<int>();
            var Querylist = Request.Query["id"];
            if (!Querylist.Any())
            {
                return new BadRequestResult();
            }
            else
            {
                foreach (var item in Querylist[0].Split(','))
                {
                    var id = 0;
                    if (int.TryParse(item, out id))
                    {
                        idlist.Add(id);
                    }
                }
            }
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);//產一個A4的PDF
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, ms);  //實例化
                PdfPTable table = new PdfPTable(4);//產4個欄位
                var count = 0;
                foreach (var item in idlist)
                {
                    //由左到右，由上到下照順序填滿表格
                    var WorkOrderHeads = _context.WorkOrderHeads.Find(item);
                    var txt = WorkOrderHeads.WorkOrderNo;
                    var bQrCode = BarcodeHelper.CreateQrCode(txt, 50, 50);
                    var QrCodeimg = iTextSharp.text.Image.GetInstance(bQrCode);
                    PdfPTable itemtable = new PdfPTable(1);
                    PdfPCell borderCell1 = new PdfPCell(QrCodeimg);
                    borderCell1.FixedHeight = 40;
                    borderCell1.Border = PdfPCell.NO_BORDER;//無框線
                    borderCell1.VerticalAlignment = PdfPCell.ALIGN_CENTER;//置中
                    borderCell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;//置中
                    itemtable.AddCell(borderCell1);
                    PdfPCell borderCell2 = new PdfPCell(new Paragraph(txt));
                    borderCell2.Border = PdfPCell.NO_BORDER;//無框線
                    borderCell2.VerticalAlignment = PdfPCell.ALIGN_CENTER;//置中
                    borderCell2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;//置中
                    borderCell1.FixedHeight = 10;
                    itemtable.AddCell(borderCell2);
                    table.AddCell(itemtable);
                    count++;
                }
                var len = 4 - (count % 4);
                len = len == 4 ? 0 : len;
                for (int i = 0; i < len; i++)
                {
                    table.AddCell("");//補上空的，沒補會有錯誤
                }
                document.Open();                         //打開文件  
                document.Add(table);
                // document.Add(addImage(document));               //添加圖片
                document.Close();                        //關閉文件
                                                         // return new FileStreamResult(ms, "image/png");
                return File(ms.ToArray(), "application/pdf", "QrCode.pdf");
            }
        }
        /// <summary>
        /// PDF添加圖片
        /// </summary>
        /// <returns></returns>
        private iTextSharp.text.Image addImage(Document document)
        {
            iTextSharp.text.Image hgLogo = iTextSharp.text.Image.GetInstance("yijing.jpg");
            hgLogo.ScalePercent(4f);  //圖片比例
            hgLogo.SetAbsolutePosition(40f, document.PageSize.Height - 100f); //iamge 位置 
            return hgLogo;
        }
    }
}