using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using HRMMVC.Models;
using HRMMVC.Manager;
using Microsoft.AspNet.Identity;

namespace Manager.Controllers
{
    [Authorize]
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        //  private ExtraFunctions ExtraFunctions. = new ExtraFunctions();

        // GET: Payrolls
        public async Task<ActionResult> Index()
        {
            var payroll = await db.Payroll.Where(s => s.IsActive == true).Include(p => p.Employee).ToListAsync();
            return PartialView(payroll);
        }

        // GET: Payrolls/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payroll payroll = db.Payroll.Where(s => s.Id == id).Include(f=>f.Employee).Include(f=>f.Employee.Designation).Include(f => f.Payroll_Allowance).Include(f => f.Payroll_Deduction).SingleOrDefault();
            if (payroll == null)
            {
                return HttpNotFound();
            }
            return PartialView(payroll);
        }

        // GET: Payrolls/Create
        public ActionResult Set_Up()
        {
            var emp = db.Employee.Where(s => s.IsLeave == false).Select(s => new
            {
                Text = s.Full_Name + ", F: " + s.Father_Name + ", " + s.Phone2 + " Code: " + s.EmployeeCode,
                Id = s.Id
            }).ToList();
            ViewBag.EmployeeId = new SelectList(emp, "Id", "Text");
            var all = db.Allowance.Where(f=>f.IsActive == true).ToList();
            ViewBag.AllowenceId = new SelectList(all, "Id", "Title");
            var dea = db.Deduction.Where(f=>f.IsActive == true).ToList();
            ViewBag.DeductionId = new SelectList(dea, "Id", "Title");


            return PartialView(new Payroll());
        }

        // POST: Payrolls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Set_Up([Bind(Include = "Id,EmployeeId,Basic_Salary,IsActive,Updated,Over_Time_Rate,SameAsBasic,Notes")] Payroll payroll)
        {
            var Jsonres = new { Status = "", Message = "", url = "" };

            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                var message = string.Join(", ", error);
                Jsonres = new { Status = "Faild", Message = message, url = "" };
                return Json(Jsonres, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    var empl = db.Employee.Where(s => s.Id == payroll.EmployeeId).SingleOrDefault();
                    var check = db.Payroll.Where(s => s.EmployeeId == payroll.EmployeeId && s.IsActive == true).SingleOrDefault();
                    if (check != null)
                    {
                        check.IsActive = false;
                        db.Entry(check).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    payroll.IsActive = true;
                    payroll.Updated = DateTime.UtcNow.AddHours(6);

                    List<Payroll_Allowence> allow_Data = Session["Allowence"] as List<Payroll_Allowence>;
                    payroll.Id = Guid.NewGuid();
                    db.Payroll.Add(payroll);
                    db.SaveChanges();
                    if (allow_Data != null)
                    {
                        foreach (var item in allow_Data)
                        {
                            item.Id = Guid.NewGuid();
                            item.PayrollId = payroll.Id;
                            // item.Payroll = payroll;
                            item.Allowance = null;
                            db.Payroll_Allowence.Add(item);
                            db.SaveChanges();
                        }
                        Session["Allowence"] = null;
                    }

                    List<Payroll_Deduction> pd = Session["deduction"] as List<Payroll_Deduction>;

                    if (pd != null)
                    {
                        foreach (var item in pd)
                        {
                            item.Id = Guid.NewGuid();
                            item.PayrollId = payroll.Id;
                            // item.Payroll = payroll;
                            item.Deduction = null;
                            db.Payroll_Deduction.Add(item);
                            db.SaveChanges();
                        }
                        Session["deduction"] = null;
                    }

                    var userName = User.Identity.GetUserName();
                    var doc = "Created a payroll for " + empl.EmployeeCode;
                    ExtraFunctions.Set_Activity(doc, userName);
                    await db.SaveChangesAsync();
                    save.Commit();
                    Jsonres = new { Status = "OK", Message = "Successfully Saved", url = "/Payrolls/Index" };
                    return Json(Jsonres, JsonRequestBehavior.AllowGet);
                }

                catch (Exception ex)
                {
                    
                    save.Rollback();
                    Jsonres = new { Status = "Error", Message = ex.Message, url = "" };
                    return Json(Jsonres, JsonRequestBehavior.AllowGet);

                }
            }
        }

 



        [HttpPost]
        public ActionResult Set_Allowence(int Id, decimal Amount, PecentOf? Percent)
        {
            List<Payroll_Allowence> allow_Data = Session["Allowence"] as List<Payroll_Allowence>;
            if (allow_Data == null)
            {
                allow_Data = new List<Payroll_Allowence>();
            }
            if (Id > 0 && Amount > 0)
            {
                var allowence = db.Allowance.Where(s => s.Id == Id && s.IsActive == true).SingleOrDefault();
                if (allowence != null)
                {
                    if (Amount > 0)
                    {
                        var already = allow_Data.Where(s => s.AllowenceId == Id).SingleOrDefault();
                        if (already != null)
                        {
                            allow_Data.Remove(already);
                            already.Amount = Amount;
                            already.PecentOf = Percent ?? null;
                            allow_Data.Add(already);
                        }
                        else
                        {
                            Payroll_Allowence pl = new Payroll_Allowence();
                            pl.Id = Guid.NewGuid();
                            pl.AllowenceId = Id;
                            pl.Amount = Amount;
                            pl.PecentOf = Percent ?? null;
                            pl.Allowance = allowence;
                            allow_Data.Add(pl);

                        }
                    }

                }
               
            }
            Session["Allowence"] = allow_Data;
            var data = allow_Data.ToList().Select(s => new
            {
                Title = s.Allowance.Title,
                Amount = s.Amount,
                Percent = s.PecentOf,
                Id = s.Id
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DeleteAllowence(Guid Id)
        {
            List<Payroll_Allowence> allow_Data = Session["Allowence"] as List<Payroll_Allowence>;
            try
            {
                var re = allow_Data.Where(s => s.Id == Id).SingleOrDefault();
                allow_Data.Remove(re);
            }
            catch (Exception ex)
            {

                TempData["Message"] = "Something went wrong" + ex;
            }
            Session["Allowence"] = allow_Data;
            var data = allow_Data.ToList().Select(s => new
            {
                Title = s.Allowance.Title,
                Amount = s.Amount,
                Percent = s.PecentOf,
                Id = s.Id
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Set_Deduction(int Id, decimal Amount, PecentOf? Percent)
        {
            List<Payroll_Deduction> allow_Data = Session["deduction"] as List<Payroll_Deduction>;
            if (allow_Data == null)
            {
                allow_Data = new List<Payroll_Deduction>();
            }
            if (Id > 0 && Amount > 0)
            {
                var deduction = db.Deduction.Where(s => s.Id == Id && s.IsActive == true).SingleOrDefault();
                if (deduction != null)
                {
                    if (Amount <= 100 && Amount > 0)
                    {
                        var already = allow_Data.Where(s => s.DeductionId == Id).SingleOrDefault();
                        if (already != null)
                        {
                            allow_Data.Remove(already);
                            already.Amount = Amount;
                            already.PecentOf = Percent ?? null;
                            allow_Data.Add(already);
                        }
                        else
                        {
                            Payroll_Deduction pl = new Payroll_Deduction();
                            pl.Id = Guid.NewGuid();
                            pl.DeductionId = Id;
                            pl.Amount = Amount;
                            pl.PecentOf = Percent ?? null;
                            pl.Deduction = deduction;
                            allow_Data.Add(pl);

                        }
                    }

                }


            }


            Session["deduction"] = allow_Data;
            var data = allow_Data.ToList().Select(s => new
            {
                Title = s.Deduction.Title,
                Amount = s.Amount,
                Percent = s.PecentOf,
                Id = s.Id
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DeleteDeduction(Guid Id)
        {
            List<Payroll_Deduction> allow_Data = Session["deduction"] as List<Payroll_Deduction>;
            try
            {
                var re = allow_Data.Where(s => s.Id == Id).SingleOrDefault();
                allow_Data.Remove(re);
            }
            catch (Exception ex)
            {

                TempData["Message"] = "Something went wrong" + ex;
            }
            Session["deduction"] = allow_Data;
            var data = allow_Data.ToList().Select(s => new
            {
                Title = s.Deduction.Title,
                Amount = s.Amount,
                Percent = s.PecentOf,
                Id = s.Id
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
