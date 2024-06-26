﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LinqKit;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// BOM API
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    public class BillOfMaterialsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public BillOfMaterialsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 取BOM表的資料列表
        /// </summary>
        /// <returns></returns>
        // GET: api/BillOfMaterials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillOfMaterial>>> GetBillOfMaterials()
        {
            var BillOfMaterials = await _context.BillOfMaterials.AsQueryable().ToListAsync();
            return Ok(MyFun.APIResponseOK(BillOfMaterials));
        }

        /// <summary>
        /// 用ID取BOM表的資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterial(int id)
        {
            var billOfMaterial = await _context.BillOfMaterials.FindAsync(id);

            if (billOfMaterial == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(billOfMaterial));
        }

        /// <summary>
        /// 用PriductID取BOM表的資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterialByProductBasic(int id)
        {
            var billOfMaterial = await _context.BillOfMaterials.AsQueryable().Where(x => x.ProductBasicId == id).ToListAsync();

            if (billOfMaterial == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(billOfMaterial));
        }

        /// <summary>
        /// 更新BOM表資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billOfMaterial"></param>
        /// <returns></returns>
        // PUT: api/BillOfMaterials/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillOfMaterial(int id, BillOfMaterial billOfMaterial)
        {
            if (id != billOfMaterial.Id)
            {
                return BadRequest();
            }

            _context.Entry(billOfMaterial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillOfMaterialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 新增BOM表資料
        /// </summary>
        /// <param name="billOfMaterial"></param>
        /// <returns></returns>
        // POST: api/BillOfMaterials
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BillOfMaterial>> PostBillOfMaterial(BillOfMaterial billOfMaterial)
        {
            _context.BillOfMaterials.Add(billOfMaterial);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBillOfMaterial", new { id = billOfMaterial.Id }, billOfMaterial);
        }
        /// <summary>
        /// 刪除BOM表資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/BillOfMaterials/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BillOfMaterial>> DeleteBillOfMaterial(int id)
        {
            var billOfMaterial = await _context.BillOfMaterials.FindAsync(id);
            if (billOfMaterial == null)
            {
                return NotFound();
            }

            _context.BillOfMaterials.Remove(billOfMaterial);
            await _context.SaveChangesAsync();

            return billOfMaterial;
        }
        /// <summary>
        /// 用EXCEL水匯入BOM表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<OrderHead> Bomexcel()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var MaterialBasiclist = new List<MaterialBasic>();
            string Fileitem = @"D:\project\鴻銡\給翔呈 成品品號匯入(BOM).xlsx";
            //string Fileitem = @"C:\Users\user\Downloads\基本資料匯入(原料).xlsx";
            var excelcount = 0;
            using (FileStream item = System.IO.File.OpenRead(Fileitem))
            {
                IWorkbook workBook;
                IFormulaEvaluator formulaEvaluator;
                try
                {
                    workBook = new XSSFWorkbook(item);//xlsx格式
                    formulaEvaluator = new XSSFFormulaEvaluator(workBook); // Important!! 取公式值的時候會用到
                    var TempBOM = new BOM();
                    ISheet sheet = workBook.GetSheetAt(0);
                    //foreach (ISheet sheet in workBook)
                    {
                        #region BOM 匯入
                        var starti = 1;
                        for (var i = starti; i <= sheet.LastRowNum; i++)//筆數
                        {
                            var CellvalA = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(0));  //"A": "主件品號",
                            var CellvalB = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));  //"B": "主件品名",
                            var CellvalC = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(2));  //"C": "庫存量",
                            var CellvalD = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(3));  //"D": "庫存警示",
                            var CellvalE = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(4));  //"E": "規格",
                            var CellvalF = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(5));  //"F": "廠內規格",
                            var CellvalG = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(6));  //"G": "單價",
                            var CellvalH = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(7));  //"H": "元件品號",
                            var CellvalI = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(8));  //"I": "存放庫別",
                            var CellvalJ = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(9));  //"J": "廠內品號",
                            var CellvalK = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(10)); //"K": "操作"

                            if (string.IsNullOrWhiteSpace(CellvalA) && string.IsNullOrWhiteSpace(CellvalH))
                            {
                                continue;
                            }
                            if (!string.IsNullOrWhiteSpace(CellvalA))
                            {
                                TempBOM.A = CellvalA;
                                TempBOM.B = CellvalB;
                                TempBOM.C = CellvalC;
                                TempBOM.D = CellvalD;
                                TempBOM.E = CellvalE;
                                TempBOM.F = CellvalF;
                                TempBOM.G = CellvalG;
                                TempBOM.H = CellvalH;
                                TempBOM.I = CellvalI;
                                TempBOM.J = CellvalJ;
                                TempBOM.K = CellvalK;
                            }
                            else
                            {
                                TempBOM.H = CellvalH;
                            }

                            // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                            var ProductBasic = _context.MaterialBasics.AsQueryable().Where(x => x.MaterialNo == TempBOM.A).ToList();
                            var MaterialBasic = _context.MaterialBasics.AsQueryable().Where(x => x.MaterialNo == TempBOM.H).ToList();
                            if (ProductBasic.Any())//有成品NO
                            {
                                if (MaterialBasic.Any())
                                {
                                    foreach (var ProductBasicitem in ProductBasic)
                                    {
                                        foreach (var MaterialBasicitem in MaterialBasic)
                                        {
                                            if (!ProductBasicitem.BillOfMaterialMaterialBasics.Where(x => x.MaterialBasicId == MaterialBasicitem.Id).Any())
                                            {
                                                //加入BOM
                                                ProductBasicitem.BillOfMaterialProductBasics.Add(new BillOfMaterial
                                                {
                                                    MaterialBasicId = MaterialBasicitem.Id,
                                                    Quantity = 1,
                                                    Lv = 1,
                                                    Group = 1
                                                });
                                            }

                                        }
                                    }
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                //新增成品
                                if (MaterialBasic.Any())
                                {
                                    var nProductBasic = new MaterialBasic
                                    {
                                        MaterialNo = TempBOM.A,
                                        MaterialNumber = TempBOM.J,
                                        Name = TempBOM.B,
                                        Specification = TempBOM.E,
                                        Property = ""
                                    };
                                    foreach (var MaterialBasicitem in MaterialBasic)
                                    {
                                        //加入BOM
                                        nProductBasic.BillOfMaterialProductBasics.Add(new BillOfMaterial
                                        {
                                            MaterialBasicId = MaterialBasicitem.Id,
                                            Quantity = 1,
                                            Lv = 1,
                                            Group = 1
                                        });
                                    }
                                    _context.MaterialBasics.Add(nProductBasic);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        #endregion
                        #region 原料匯入
                        //    var starti = 1;
                        //    excelcount = sheet.LastRowNum;
                        //    for (var i = starti; i <= sheet.LastRowNum; i++)//筆數
                        //    {
                        //        var CellvalA = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(0));  //"A": "品號",
                        //        var CellvalB = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));  //"B": "品名",
                        //        var CellvalC = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(2));  //"C": "規格",
                        //        var CellvalD = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(3));  //"D": "屬性",
                        //        MaterialBasiclist.Add(new MaterialBasic
                        //        {
                        //            MaterialNo = CellvalA,
                        //            Name = CellvalB,
                        //            Specification = CellvalC,
                        //            Property = CellvalD
                        //        });
                        //    }
                        //}
                        //foreach (var listitem in MaterialBasiclist)
                        //{
                        //    _context.MaterialBasics.Add(listitem);
                        //}
                        //_context.SaveChanges();
                        #endregion

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " 請檢查資料格式");
                }

            }
            return Ok(MyFun.APIResponseOK(null, MaterialBasiclist.Count + ";" + excelcount));

        }
        /// <summary>
        /// 取產品列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <returns></returns>
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterials([FromQuery] FromQuery FromQuery)
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasics([FromQuery] DataSourceLoadOptions FromQuery)
        {

            //var MaterialBasict = _context.MaterialBasics.AsEnumerable();
            //var MaterialBasic = _context.MaterialBasics.AsQueryable();
            //var MaterialBasic = await DataSourceLoader.LoadAsync(_context.MaterialBasics.AsQueryable(), FromQuery);

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(_context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0), FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 有BOM資料的產品列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasicsHaveBom([FromQuery] DataSourceLoadOptions FromQuery)
        {

            var materailbasic = _context.MaterialBasics.Where(x => x.DeleteFlag == 0 && x.BillOfMaterialProductBasics.Where(y => y.DeleteFlag == 0).Any());


            // var materailbasic = _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0)
            // .Join(_context.BillOfMaterials.Where(x => x.Pid == null && x.DeleteFlag == 0),
            // x => x.Id, y => y.ProductBasicId, (MaterialBasics, BillOfMaterials) => new
            // {
            //     BillOfMaterials.Id,
            //     BillOfMaterials.MaterialBasicId,
            //     MaterialBasics
            // });
            // var a =materailbasic.ToSql();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(materailbasic, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 無BOM資料的產品列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasicsHaveAny([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var materailbasic = _context.MaterialBasics.Where(x => x.DeleteFlag == 0 && !x.BillOfMaterialProductBasics.Any(y => y.DeleteFlag == 0));
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(materailbasic, FromQuery); 
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 用性品ID取BOM的列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<BomListVM>>> GetBomlist(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var bomlist = new List<BomList>();

            // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
            var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == id && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
            if (BillOfMaterials != null)
            {
                bomlist.AddRange(MyFun.GetBomList(BillOfMaterials));
            }
            ///// 舊的回傳結果
            // return Ok(MyFun.APIResponseOK(bomlist));

            var bomlistVM = new List<BomListVM>();
            foreach (var item in bomlist)
            {
                bomlistVM.Add(new BomListVM
                {
                    Id = item.Id,
                    Pid = item.Pid,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Outsource = item.Outsource,
                    Group = item.Group,
                    Type = item.Type,
                    Remarks = item.Remarks,
                    MaterialBasicId = item.MaterialBasicId,
                    MaterialName = item.MaterialName,
                    MaterialNo = item.MaterialNo,
                    MaterialSpecification = item.MaterialSpecification,
                    MaterialPrice = item.MaterialPrice,
                    ProductBasicId = item.ProductBasicId,
                    ProductName = item.ProductName,
                    ProductNo = item.ProductNo,
                    ProductNumber = item.ProductNumber,
                    ProductSpecification = item.ProductSpecification,
                    ProductPrice = item.ProductPrice,
                    Lv = item.Lv,
                    ReceiveQty = item.ReceiveQty,
                    Ismaterial = item.Ismaterial,

                    LvS = "(" + item.Lv + ")　 " + item.ProductNo + item.MaterialNo,
                    LvName = item.ProductNo + item.MaterialNo,
                    BomType = !string.IsNullOrWhiteSpace(item.MaterialNo) ? "元件" : "成品",
                    Master = item.Master
                });
            }
            return Ok(MyFun.APIResponseOK(bomlistVM));
        }

        /// <summary>
        /// 更新BOM表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PutBomlist"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BomList>>> PutBomlist(int id, BomList PutBomlist)
        {
            var BillOfMaterial = _context.BillOfMaterials.Find(id);
            if (!string.IsNullOrWhiteSpace(PutBomlist.Name))
            {
                BillOfMaterial.Name = PutBomlist.Name;
            }
            if (PutBomlist.Quantity != 0)
            {
                BillOfMaterial.Quantity = PutBomlist.Quantity;
            }
            if (PutBomlist.Pid.HasValue)
            {
                BillOfMaterial.Pid = PutBomlist.Pid;
                BillOfMaterial.Master = 0;
                BillOfMaterial.Lv = 2;
            }
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(PutBomlist));
        }

        /// <summary>
        /// 更新BOM表主件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PutBomlist"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BomList>>> PutBomlistMaster(int id, BomList PutBomlist)
        {
            var BillOfMaterial = await _context.BillOfMaterials.FindAsync(id);
            if (BillOfMaterial != null)
            {

                // 原本方法! 任何BOM成員都可以設為主要用料
                // if (BillOfMaterial.Master == 0) {
                //     BillOfMaterial.Master = 1;
                // } else {
                //     BillOfMaterial.Master = 0;
                // }

                // 2020/11/05 確定BOM成員只能有一個主要用料
                var BillOfMaterialAll = _context.BillOfMaterials.Where(x => x.ProductBasicId == BillOfMaterial.ProductBasicId && x.DeleteFlag == 0).ToList();
                foreach (var item in BillOfMaterialAll)
                {
                    item.Master = 0;
                }
                BillOfMaterial.Master = 1;

                await _context.SaveChangesAsync();
            }

            return Ok(MyFun.APIResponseOK(BillOfMaterial));
        }

        /// <summary>
        /// 新增BOM表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PostBom"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<ActionResult<BomList>> PostBomlist(int id, [FromBody] PostBom PostBom)
        {
            if (string.IsNullOrWhiteSpace(PostBom.Name) && PostBom.BasicId == null)
            {
                return Ok(MyFun.APIResponseError("請輸入名稱或品號"));
            }
            var masterVal = _context.BillOfMaterials.Where(x => x.ProductBasicId == id && x.DeleteFlag == 0).Any() ? 0 : 1;
            var nBillOfMaterials = new BillOfMaterial
            {
                ProductBasicId = id,
                Name = PostBom.Name,
                Quantity = PostBom.Quantity,
                Master = masterVal
            };
            
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (PostBom.BasicType == 1)//原料直接加入
            {
                nBillOfMaterials.MaterialBasicId = PostBom.BasicId;
            }
            else if (PostBom.BasicType == null && PostBom.BasicId.HasValue)//成品連同組成加入
            {
                //複製BOM內容
                int BasicId = PostBom.BasicId.Value;
                // var BillOfMaterials = _context.ProductBasics.Find(BasicId).BillOfMaterials.ToList();

                // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                var BillOfMaterials = _context.MaterialBasics.Find(BasicId).BillOfMaterialProductBasics.Where(x => x.Pid == null).ToList();
                if (BillOfMaterials.Count() == 0)
                {
                    nBillOfMaterials.MaterialBasicId = PostBom.BasicId;
                }
                else
                {
                    foreach (var item1 in BillOfMaterials)
                    {
                        var nbom1 = new BillOfMaterial
                        {
                            ProductBasicId = item1.ProductBasicId,
                            MaterialBasicId = item1.MaterialBasicId,
                            Quantity = item1.Quantity
                        };
                        foreach (var item2 in item1.InverseP)
                        {
                            var nbom2 = new BillOfMaterial
                            {
                                ProductBasicId = item2.ProductBasicId,
                                MaterialBasicId = item2.MaterialBasicId,
                                Quantity = item2.Quantity
                            };
                            foreach (var item3 in item2.InverseP)
                            {
                                var nbom3 = new BillOfMaterial
                                {
                                    ProductBasicId = item3.ProductBasicId,
                                    MaterialBasicId = item3.MaterialBasicId,
                                    Quantity = item3.Quantity
                                };
                                nbom2.InverseP.Add(nbom3);
                            }
                            nbom1.InverseP.Add(nbom2);
                        }
                        nBillOfMaterials.InverseP.Add(nbom1);
                    }
                }
            }
            _context.BillOfMaterials.Add(nBillOfMaterials);
            var materialBasic = _context.MaterialBasics.Find(id);
            materialBasic.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();

            /////紀錄變更版本
            var bomlist = new List<BomList>();
            var BomData = _context.BillOfMaterials.Where(x => x.ProductBasicId == id && x.DeleteFlag == 0 && !x.Pid.HasValue).ToList();
            if (BomData.Count() != 0)
            {
                bomlist.AddRange(MyFun.GetBomList(BomData));
                var BomVerData = _context.BillOfMaterialVers.Where(x => x.ProductBasicId == id).OrderByDescending(x => x.Id).ToList();
                decimal VerNo = 1;
                if (BomVerData.Count() != 0)
                {
                    VerNo = BomVerData[0].Version + 1;
                }
                foreach (var item in bomlist)
                {
                    // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                    var MaterialBasicInfo = _context.MaterialBasics.Find(item.MaterialBasicId);
                    var ProductBasicInfo = _context.MaterialBasics.Find(item.ProductBasicId);
                    var nVer = new BillOfMaterialVer
                    {
                        ProductBasicId = id,
                        Version = VerNo,
                        Bomid = item.Id,
                        Bompid = item.Pid,
                        MaterialNo = MaterialBasicInfo?.MaterialNo ?? null,
                        MaterialName = MaterialBasicInfo?.Name ?? null,
                        ProductNo = ProductBasicInfo?.MaterialNo ?? null,
                        ProductName = ProductBasicInfo?.Name ?? null,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Lv = item.Lv,
                        Outsource = item.Outsource,
                        Group = item.Group,
                        Type = item.Type,
                        Remarks = item.Remarks,
                        CreateUser = MyFun.GetUserID(HttpContext)
                    };
                    _context.BillOfMaterialVers.Add(nVer);
                }
                await _context.SaveChangesAsync();
            }

            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(PostBom));
        }

        // DELETE: api/BillOfMaterials/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BillOfMaterial>> DeleteBomlist(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var TempProductBasicData = _context.BillOfMaterialVers.Where(x => x.Bomid == id).FirstOrDefault();
            var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.Id == id).ToListAsync();
            // var BillOfMaterial = await _context.BillOfMaterials.FindAsync(id);
            if (BillOfMaterials != null)
            {
                // 假設該刪除品號被設為主件，則另尋其他品號設為主件。
                if (BillOfMaterials.FirstOrDefault().Master == 1)
                {
                    var BillOfMaterial = _context.BillOfMaterials.Where(x => x.ProductBasicId == BillOfMaterials.FirstOrDefault().ProductBasicId && x.Lv == 1 && x.Master == 0 && x.DeleteFlag == 0).ToList();
                    if (BillOfMaterial.Count() != 0)
                    {
                        BillOfMaterial.FirstOrDefault().Master = 1;
                    }
                }
                else
                { // 檢查是否有主件存在，否則隨機選一個當主件。
                    var BillOfMaterial = _context.BillOfMaterials.Where(x => x.ProductBasicId == BillOfMaterials.FirstOrDefault().ProductBasicId && x.Lv == 1 && x.Id != BillOfMaterials.FirstOrDefault().Id && x.DeleteFlag == 0).ToList();
                    if (BillOfMaterial.Where(x => x.Master == 1).ToList().Count() == 0)
                    {
                        BillOfMaterial.FirstOrDefault().Master = 1;
                    }
                }

                // BillOfMaterial.DeleteFlag = 2;
                var result = MyFun.DeleteBomList(BillOfMaterials);
                foreach (var item in result)
                {
                    var MBillOfMaterials = _context.MBillOfMaterials.Where(x => x.BomId == item.Id).ToList();
                    foreach (var item2 in MBillOfMaterials)
                    {
                        _context.MBillOfMaterials.Remove(item2);
                    }
                    _context.BillOfMaterials.Remove(item);
                }
                await _context.SaveChangesAsync();
            }

            /////紀錄變更版本
            var bomlist = new List<BomList>();
            if (TempProductBasicData != null)
            {
                var BomData = _context.BillOfMaterials.Where(x => x.ProductBasicId == TempProductBasicData.ProductBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToList();
                if (BomData.Count() != 0)
                {
                    bomlist.AddRange(MyFun.GetBomList(BomData));
                    var BomVerData = _context.BillOfMaterialVers.Where(x => x.ProductBasicId == TempProductBasicData.ProductBasicId).OrderByDescending(x => x.Id).ToList();
                    decimal VerNo = 1;
                    if (BomVerData.Count() != 0)
                    {
                        VerNo = BomVerData[0].Version + 1;
                    }
                    foreach (var item in bomlist)
                    {
                        // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                        var MaterialBasicInfo = _context.MaterialBasics.Find(item.MaterialBasicId);
                        var ProductBasicInfo = _context.MaterialBasics.Find(item.ProductBasicId);
                        var nVer = new BillOfMaterialVer
                        {
                            ProductBasicId = TempProductBasicData.ProductBasicId,
                            Version = VerNo,
                            Bomid = item.Id,
                            Bompid = item.Pid,
                            MaterialNo = MaterialBasicInfo?.MaterialNo ?? null,
                            MaterialName = MaterialBasicInfo?.Name ?? null,
                            ProductNo = ProductBasicInfo?.MaterialNo ?? null,
                            ProductName = ProductBasicInfo?.Name ?? null,
                            Name = item.Name,
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            Lv = item.Lv,
                            Outsource = item.Outsource,
                            Group = item.Group,
                            Type = item.Type,
                            Remarks = item.Remarks,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        };
                        _context.BillOfMaterialVers.Add(nVer);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(BillOfMaterials));
        }

        /// <summary>
        /// 取可用的原料下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasicsDrowDown()
        {
            var materialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).Select(x => new
            {
                x.Id,
                Name = x.MaterialNo
            }).OrderBy(x => x.Name).ToListAsync();
            return Ok(MyFun.APIResponseOK(materialBasic));
        }
        /// <summary>
        /// 取可用的成品下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasic>>> GetProductBasicsDrowDown()
        {
            var productBasic = await _context.ProductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).Select(x => new
            {
                x.Id,
                Name = x.ProductNo + "_" + x.Name,
                Group = x.Name
            }).OrderBy(x => x.Name).ToListAsync();
            return Ok(MyFun.APIResponseOK(productBasic));
        }
        private bool BillOfMaterialExists(int id)
        {
            return _context.BillOfMaterials.Any(e => e.Id == id);
        }

        /// <summary>
        /// 用ProductBasicID取BOM表的製程資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetProcessByMaterialBasicId(int id)
        {
            if (id != 0)
            {
                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.MaterialBasicId == id).OrderBy(x => x.SerialNumber).ToListAsync();

                if (billOfMaterial == null)
                {
                    return NotFound();
                }
                return Ok(MyFun.APIResponseOK(billOfMaterial));

            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 用BomID取BOM表的製程資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetProcessByBomId(int id)
        {
            if (id != 0)
            {
                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.BomId == id).OrderBy(x => x.SerialNumber).ToListAsync();

                if (billOfMaterial == null)
                {
                    return NotFound();
                }
                return Ok(MyFun.APIResponseOK(billOfMaterial));

            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 新增MBOM表
        /// </summary>
        /// <param name="MbomData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BomList>> PostMbomlist(MbomData MbomData)
        {
            if (MbomData.BomId != 0 || MbomData.MaterialBasicId != 0)
            {
                var MbillOfMaterials = new List<MBillOfMaterial>();
                if (MbomData.MaterialBasicId != 0)
                {
                    MbillOfMaterials = await _context.MBillOfMaterials.Where(x => x.MaterialBasicId == MbomData.MaterialBasicId).ToListAsync();
                }
                else
                {
                    MbillOfMaterials = await _context.MBillOfMaterials.Where(x => x.BomId == MbomData.BomId).ToListAsync();
                }
                if (MbillOfMaterials != null)
                {
                    foreach (var item in MbillOfMaterials)
                    {
                        _context.MBillOfMaterials.Remove(item);
                    }
                    // await _context.SaveChangesAsync();
                }

                foreach (var item in MbomData.MBillOfMaterialList)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nMbom = new MBillOfMaterial
                    {
                        Pid = null,
                        Name = null,
                        MaterialBasicId = MbomData.MaterialBasicId,
                        BomId = MbomData.BomId,
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
                        Status = item.Status,
                        Type = item.Type,
                        Remarks = item.Remarks,
                        Version = item.Version,
                        CreateUser = MyFun.GetUserID(HttpContext),
                    };
                    _context.MBillOfMaterials.Add(nMbom);
                }
                await _context.SaveChangesAsync();
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 用MaterialID取BOM表的變更紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterialVer>> GetBomVerByProductId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var BillOfMaterialVers = await _context.BillOfMaterialVers.Where(x => x.ProductBasicId == id && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
            if (BillOfMaterialVers == null)
            {
                return NotFound();
            }
            //整理資料

            //資料轉型
            string strjsonData = JsonConvert.SerializeObject(BillOfMaterialVers);
            var BillOfMaterialVerLvList = JsonConvert.DeserializeObject<List<BillOfMaterialVerLv>>(strjsonData);
            //產出階層
            foreach (var item in BillOfMaterialVerLvList)
            {
                if (item.Bompid == 0)
                {
                    item.ShowPLV = Decimal.ToInt32(item.Version);
                }
                else
                {
                    item.ShowPLV = int.Parse(decimal.ToInt32(item.Version).ToString() + item.Bompid.ToString());
                }
                item.ShowLV = int.Parse(decimal.ToInt32(item.Version).ToString() + item.Bomid.ToString());

            }
            //補上表頭
            foreach (var item in BillOfMaterialVerLvList.GroupBy(x => x.Version).ToList())
            {
                BillOfMaterialVerLvList.Add(new BillOfMaterialVerLv
                {
                    ShowPLV = 0,
                    ShowLV = Decimal.ToInt32(item.Key),
                    Version = item.Key
                });
            }
            return Ok(MyFun.APIResponseOK(BillOfMaterialVerLvList.OrderByDescending(x => x.Version)));
        }


    }
}
