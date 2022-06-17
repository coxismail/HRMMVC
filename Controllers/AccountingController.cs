using HRMMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HRMMVC.Controllers
{
    public class AccountingController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Accounting
        [AjaxOnly]
        public ActionResult Index()
        {
            var led = db.Ledger.Include(f=>f.LedgerCategory).ToList().Select(f => new
            {
                id = f.Id,
                text = f.Name + " (" + f.Code + ")   <- " + f.LedgerCategory.Name + " <- " + f.LedgerCategory.ChartOfAccount.Name,
            }).ToList();
            var ch = db.ChartOfAccount.ToList();
            var ledc = db.LedgerCategory.ToList().Select(f => new
            {
                id = f.Id,
                text = f.Name + " <- " + f.ChartOfAccount.Name
            }).ToList();
            ViewBag.Ledger = new SelectList(led, "id", "text");
            ViewBag.LedgerCategory = new SelectList(ledc, "id", "text");
            ViewBag.ChartofAccount = new SelectList(ch, "Id", "Name");

            return PartialView();
        }

        // Mange Ledger Category start here
        [HttpPost, AjaxOnly]
        public ActionResult GetCategory(Guid CategoryId)
        {
            var js = new { Status = "Faild", Message = "Something Went wrong", url = "" };
            if (CategoryId != null)
            {
                js = new { Status = "", Message = "You can update now", url = "/Accounting/Category?Id=" + CategoryId };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [AjaxOnly]
        public ActionResult Category(Guid? Id)
        {
            var list = db.LedgerCategory.OrderBy(f => f.ChartOfAccount.Name).OrderBy(f => f.Name).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.ChartOfAccount.Name + " -> " + f.Name,
            }).ToList();

            var chd = db.ChartOfAccount.OrderBy(f => f.Name).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Name
            }).ToList();
            ViewBag.ChartOfAccountId = new SelectList(chd, "Id", "Text");
            ViewBag.ParentCategoryId = new SelectList(list, "Id", "Text");
            ViewBag.CategoryId = new SelectList(list, "Id", "Text");
            if (Id != Guid.Empty && Id != null)
            {
                var r = db.LedgerCategory.Where(f => f.Id == Id).SingleOrDefault();
                ViewBag.ChartOfAccountId = new SelectList(chd, "Id", "Text", r.ChartOfAccountId);
                ViewBag.ParentCategoryId = new SelectList(list, "Id", "Text", r.ParentCategoryId);
                return PartialView(r);
            }

            return PartialView(new LedgerCategory());
        }
        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
        public ActionResult Category(Guid? Id, LedgerCategory model)
        {
            var jsr = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsr = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            var parent = model.ParentCategoryId;
            if (parent != null)
            {
                var pca = db.LedgerCategory.Where(f => f.Id == model.ParentCategoryId).SingleOrDefault();
                if (pca.ChartOfAccountId != model.ChartOfAccountId)
                {
                    jsr = new { Status = "Faild", Message = "Chart of Account & Parent Category mismatch", url = "" };
                    return Json(jsr, JsonRequestBehavior.AllowGet);
                }
            }
            var check = db.LedgerCategory.Where(f => f.Name.ToUpper() == model.Name.ToUpper()).Any();
            if (check)
            {
                jsr = new { Status = "Faild", Message = "Category already Exist", url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }

            if (Id != null && Id != Guid.Empty)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                jsr = new { Status = "OK", Message = "Category Updated", url = "/Accounting/index" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            model.Id = Guid.NewGuid();
            db.LedgerCategory.Add(model);
            db.SaveChanges();
            jsr = new { Status = "OK", Message = "Category Added Successfully", url = "/Accounting/index" };
            return Json(jsr, JsonRequestBehavior.AllowGet);
        }

        // Mange Ledger Start Here
        [HttpPost, AjaxOnly]
        public ActionResult GetLedger(Guid LedgerId)
        {
            var js = new { Status = "Faild", Message = "Something Went wrong", url = "" };
            if (LedgerId != null)
            {
                js = new { Status = "", Message = "You can update now", url = "/Accounting/Ledger?Id=" + LedgerId };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [AjaxOnly]
        public ActionResult Ledger(Guid? Id)
        {
            var cat = db.LedgerCategory.OrderBy(f => f.ChartOfAccount.Name).OrderBy(f => f.Name).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.ChartOfAccount.Name + " -> " + f.Name,
            }).ToList();

            var led = db.Ledger.OrderBy(f => f.Name).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Name
            }).ToList();
            ViewBag.LedgerCategoryId = new SelectList(cat, "Id", "Text");
            ViewBag.LedgerId = new SelectList(led, "Id", "Text");
            if (Id != Guid.Empty && Id != null)
            {
                var r = db.Ledger.Where(f => f.Id == Id).SingleOrDefault();
                ViewBag.LedgerCategoryId = new SelectList(cat, "Id", "Text", r.LedgerCategoryId);
                return PartialView(r);
            }

            return PartialView(new Ledger());
        }
        [HttpPost, AjaxOnly, ValidateAntiForgeryToken]
        public ActionResult Ledger(Guid? Id, Ledger model)
        {
            var jsr = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsr = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            var check = db.Ledger.Where(f => f.Name.ToUpper() == model.Name.ToUpper()).Any();
            var codecheck = db.Ledger.Where(f => f.Code == model.Code).Any();
            var esit = "";
            if (check)
            {
                esit = "Ledger Name already Exist, ";
            }
            if (codecheck)
            {
                esit += "Ledger Code already Exist";
            }
            if (check || codecheck)
            {
                jsr = new { Status = "Faild", Message = esit, url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            if (Id != null && Id != Guid.Empty)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();
                jsr = new { Status = "OK", Message = "Ledger Updated", url = "/Accounting/index" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            model.Id = Guid.NewGuid();
            db.Ledger.Add(model);
            db.SaveChanges();
            ModelState.Clear();
            jsr = new { Status = "OK", Message = "Ledger Added Successfully", url = "/Accounting/index" };
            return Json(jsr, JsonRequestBehavior.AllowGet);
        }


        // Voucher Type
        [HttpPost, AjaxOnly]
        public ActionResult GetVoucher(int VoucherId)
        {
            var js = new { Status = "Faild", Message = "Something Went wrong", url = "" };
            if (VoucherId > 0)
            {
                js = new { Status = "", Message = "You can update now", url = "/Accounting/Voucher?Id=" + VoucherId };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [AjaxOnly]
        public ActionResult Voucher(int? Id)
        {
            var cat = db.Voucher.OrderBy(f => f.Name).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Name,
            }).ToList();
            ViewBag.VoucherId = new SelectList(cat, "Id", "Text");
            if (Id > 0 && Id != null)
            {
                var r = db.Voucher.Where(f => f.Id == Id).SingleOrDefault();
                return PartialView(r);
            }

            return PartialView(new Voucher());
        }
        [HttpPost, AjaxOnly, ValidateAntiForgeryToken]
        public ActionResult Voucher(int? Id, Voucher model)
        {
            var jsr = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsr = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            var check = db.Voucher.Where(f => f.Name.ToUpper() == model.Name.ToUpper()).Any();
            if (check)
            {
                jsr = new { Status = "Faild", Message = "Voucher Name already Exist", url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            if (Id != null && Id > 0)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();
                jsr = new { Status = "OK", Message = "Voucher Type Updated", url = "/Accounting/index" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            db.Voucher.Add(model);
            db.SaveChanges();
            ModelState.Clear();
            jsr = new { Status = "OK", Message = "Voucher Type Added Successfully", url = "/Accounting/index" };
            return Json(jsr, JsonRequestBehavior.AllowGet);
        }



        // Transaction voucher 
        [AjaxOnly]
        public ActionResult NewVoucher() {
            Session["transVoucher"] = null;
            var ledger = db.Ledger.Include(f => f.LedgerCategory).Include(f=>f.LedgerCategory.ChartOfAccount).Select(f => new
            {
                id = f.Id,
                text = f.LedgerCategory.ChartOfAccount.Name +" -> "+f.LedgerCategory.Name +" -> ("+ f.Code +") "+f.Name
            }).ToList();
            var vtype = db.Voucher.Select(f => new
            {
                id = f.Id,
                text = f.Name
            }).ToList();
            ViewBag.LedgerId = new SelectList(ledger, "id", "text");
            ViewBag.VoucherType = new SelectList(vtype, "id", "text");
            return PartialView();
        }

        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
        public ActionResult NewVoucher(TransactionViewModel model) 
        {
            var jsr = new { Status = "", Message = "", url = "" };
            var list = Session["transVoucher"] as List<TransactionDetailsView>;
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsr = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            if (list.Count < 2)
            {
                jsr = new { Status = "Faild", Message = "Please Added Item First", url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            if (list.Sum(f=>f.DebitAmount) != list.Sum(f=>f.CreditAmount))
            {
                jsr = new { Status = "Faild", Message = "Debit & Credit Amount Must be equal", url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    var trsdate = TimeZoneInfo.ConvertTimeToUtc(model.TransactionDate, User.Identity.TimeZone());
                    var vou = GetVoucherNo(model.VoucherType);
                    var tn = new Transaction();
                    tn.Id = Guid.NewGuid();
                    tn.EntryTime = DateTime.UtcNow;
                    tn.Amount = list.Sum(f => f.DebitAmount);
                    tn.VoucherType = vou.Name;
                    tn.VoucherNo = vou.VoucherNo;
                    tn.TransactionDate = trsdate;
                    tn.Narration = model.Narration;

                    db.Transaction.Add(tn);

                    db.SaveChanges();
                    var tsd = new List<TransDetails>();
                    foreach (var item in list)
                    {
                        var td = new TransDetails();
                        td.LedgerId = item.LedgerId;
                        td.LedgerCode = item.Ledger.Code;
                        td.LedgerName = item.Ledger.Name;
                        td.Debit = item.DebitAmount;
                        td.Credit = item.CreditAmount;
                        tsd.Add(td);
                    }
                    foreach (var item in tsd)
                    {
                        item.Id = Guid.NewGuid();
                        item.TransactionId = tn.Id;
                        item.VoucherNo = tn.VoucherNo;
                        item.VoucherType = tn.VoucherType;
                        item.TransDate = tn.TransactionDate;
                        db.TransDetails.Add(item);
                    }
                    db.SaveChanges();
                    save.Commit();
                    jsr = new { Status = "OK", Message = "Transaction Successfull", url = "/Accounting/NewVoucher" };
                    return Json(jsr, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    save.Rollback();
                    jsr = new { Status = "Error", Message = ex.Message, url = "/Accounting/NewVoucher" };
                    return Json(jsr, JsonRequestBehavior.AllowGet);
                }
            }

        }




        [HttpPost]
        public ActionResult StoreTrans(Guid LedgerId, decimal Amount, string Type) 
        {

            var list =  Session["transVoucher"] as List<TransactionDetailsView>;
            if (list == null)
            {
                list = new List<TransactionDetailsView>();
            }
            var led = db.Ledger.Where(f => f.Id == LedgerId).SingleOrDefault();

            if (led !=null && Amount > 0 && Type != "")
            {
                if (list.Where(f => f.LedgerId == LedgerId).Any())
                {
                    var rr = list.Where(f => f.LedgerId == LedgerId).SingleOrDefault();
                    list.Remove(rr);
                }
                var d = new TransactionDetailsView();
                d.Ledger = led;
                d.LedgerId = led.Id;
                if (Type == "Dr")
                {
                    d.DebitAmount = Amount;
                    d.CreditAmount = 0;
                }
                else
                {
                    d.CreditAmount = Amount;
                    d.DebitAmount = 0;
                }
                list.Add(d);
            }
            Session["transVoucher"] = list;
            var data = list.Select(f => new
            {
                Id = f.LedgerId,
                Name = f.Ledger.Name + " (" + f.Ledger.Code + ") ",
                Debit = f.DebitAmount,
                Credit = f.CreditAmount,
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RemoveTrans(Guid LedgerId) {
            var list = Session["transVoucher"] as List<TransactionDetailsView>;
            if (list == null)
            {
                list = new List<TransactionDetailsView>();
            }
            if (list.Where(f => f.LedgerId == LedgerId).Any())
            {
                var rr = list.Where(f => f.LedgerId == LedgerId).SingleOrDefault();
                list.Remove(rr);
            }
            Session["transVoucher"] = list;
            var data = list.Select(f => new
            {
                Id = f.LedgerId,
                Name = f.Ledger.Name + " (" + f.Ledger.Code + ") ",
                Debit = f.DebitAmount,
                Credit = f.CreditAmount,
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        // Voucher type & no
        public Voucher GetVoucherNo (int Id) 
        {
            var voucher = db.Voucher.Where(f => f.Id == Id).SingleOrDefault();
            if (voucher != null)
            {
              var pr =  db.Transaction.Where(f => f.VoucherType.ToUpper() == voucher.Name.ToUpper()).ToList();
                if (pr.Count > 0)
                {
                    var da = new Voucher()
                    {
                        Name = voucher.Name,
                        VoucherNo = pr.Max(f => f.VoucherNo) + 1,
                    };
                    voucher = da;
                }
            }
            return voucher;
        }
    }
}