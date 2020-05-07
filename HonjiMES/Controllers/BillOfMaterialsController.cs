using System;
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

namespace HonjiMES.Controllers
{
    /// <summary>
    /// BOM API
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BillOfMaterialsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public BillOfMaterialsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/BillOfMaterials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillOfMaterial>>> GetBillOfMaterials()
        {
            return await _context.BillOfMaterials.ToListAsync();
        }

        // GET: api/BillOfMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterial(int id)
        {
            var billOfMaterial = await _context.BillOfMaterials.FindAsync(id);

            if (billOfMaterial == null)
            {
                return NotFound();
            }

            return billOfMaterial;
        }

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
        [HttpGet]
        public ActionResult<OrderHead> Bomexcel()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var MaterialBasiclist = new List<MaterialBasic>();
            string Fileitem = @"D:\project\鴻銡\給翔呈 成品料號匯入(BOM).xlsx";
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
                            var ProductBasic = _context.ProductBasics.Where(x => x.ProductNo == TempBOM.A).ToList();
                            var MaterialBasic = _context.MaterialBasics.Where(x => x.MaterialNo == TempBOM.H).ToList();
                            if (ProductBasic.Any())//有成品NO
                            {
                                if (MaterialBasic.Any())
                                {
                                    foreach (var ProductBasicitem in ProductBasic)
                                    {
                                        foreach (var MaterialBasicitem in MaterialBasic)
                                        {
                                            if (!ProductBasicitem.BillOfMaterials.Where(x => x.MaterialBasicId == MaterialBasicitem.Id).Any())
                                            {
                                                //加入BOM
                                                ProductBasicitem.BillOfMaterials.Add(new BillOfMaterial
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
                                    var nProductBasic = new ProductBasic
                                    {
                                        ProductNo = TempBOM.A,
                                        ProductNumber = TempBOM.J,
                                        Name = TempBOM.B,
                                        Specification = TempBOM.E,
                                        Property = ""
                                    };
                                    foreach (var MaterialBasicitem in MaterialBasic)
                                    {
                                        //加入BOM
                                        nProductBasic.BillOfMaterials.Add(new BillOfMaterial
                                        {
                                            MaterialBasicId = MaterialBasicitem.Id,
                                            Quantity = 1,
                                            Lv = 1,
                                            Group = 1
                                        });
                                    }
                                    _context.ProductBasics.Add(nProductBasic);
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
        private bool BillOfMaterialExists(int id)
        {
            return _context.BillOfMaterials.Any(e => e.Id == id);
        }
    }
}
