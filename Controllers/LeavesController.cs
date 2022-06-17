using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using HRMMVC.Models;
using Microsoft.AspNet.Identity;
using HRMMVC.Manager;

namespace HRMMVC.Controllers
{
    [Authorize]
    public class LeavesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // "leave_empl_index", "leave_ofil_index", "leave_ofil_entry", "leave_empl_entry","",

        public async Task<ActionResult> Index()
        {
            var empLeaves = db.EmpLeaves.OrderByDescending(s => s.CreatedOn).Include(e => e.Employees);
            return PartialView(await empLeaves.ToListAsync());
        }


        public ActionResult LeaveType(int Id)
        {
            var al = db.LeaveType.ToList();
            ViewBag.LeaveTyps = al;
            if (Id > 0)
            {
               var d = al.Where(f => f.Id == Id).SingleOrDefault();
                return PartialView(d);
            }
            return PartialView(new LeaveType());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LeaveType(int Id, LeaveType model)
        {
            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            var exist = db.LeaveType.Where(f => f.Title.ToUpper() == model.Title.ToUpper()).Any();
            if (Id > 0)
            {
                if (Id == model.Id)
                {
                    LeaveType obj = db.LeaveType.Where(s => s.Id == Id).SingleOrDefault();
                    if (exist == false)
                    {
                        obj.Title = model.Title;
                    }
                    obj.LeavePeriod = model.LeavePeriod;
                    obj.EmployeeCredit = model.EmployeeCredit;
                    
                    db.SaveChanges();
                    JsonResult = new { Status = "OK", Message = "Updated Successfully", url = "/Leaves/LeaveType?id=0" };
                    return Json(JsonResult, JsonRequestBehavior.AllowGet);
                }
            }
           
            if (exist)
            {
                JsonResult = new { Status = "Warning", Message = "Already Exist", url = "/Leaves/LeaveType" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            db.LeaveType.Add(model);
            db.SaveChanges();
            JsonResult = new { Status = "OK", Message = "Add Successfully", url = "/Leaves/LeaveType" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        // GET: Leaves/Create

        public ActionResult EmpLeaveEntry()
        {
            var employee = db.Employee.ToList().Select(s => new
            {
                Text = s.EmployeeCode + ", N: " + s.Full_Name + ", F:" + s.Father_Name,
                Value = s.Id
            });
            var leavetuype = db.LeaveType.ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Title
            }).ToList();
            ViewBag.EmployeeId = new SelectList(employee, "Value", "Text");
            ViewBag.LeaveTypeId = new SelectList(leavetuype, "Id", "Text");

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmpLeaveEntry(EmpLeave empLeave)
        {
            var jsonmess = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                var messa = string.Join(", ", error);
                jsonmess = new { Status = "Error", Message = messa, url = "" };
                return Json(jsonmess, JsonRequestBehavior.AllowGet);
            }
            if (IsAlready(empLeave))
            {
                jsonmess = new { Status = "Faild", Message = "Leave Already Assign", url = "" };
                return Json(jsonmess, JsonRequestBehavior.AllowGet);
            }
            empLeave.Id = Guid.NewGuid();
            empLeave.UserId = User.Identity.GetUserId();
            db.EmpLeaves.Add(empLeave);
            await db.SaveChangesAsync();
            jsonmess = new { Status = "OK", Message = "Successfull saved", url = "/Leaves/Index" };
            return Json(jsonmess, JsonRequestBehavior.AllowGet);
        }

        // GET: Leaves/Edit/5
        [Authorize(Roles = "Company Admin, leave_entry")]
        public async Task<ActionResult> EmpLeaveEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpLeave empLeave = await db.EmpLeaves.FindAsync(id);
            if (empLeave == null)
            {
                return HttpNotFound();
            }
            var employee = db.Employee.ToList().Select(s => new
            {
                Text = s.EmployeeCode + ", N: " + s.Full_Name + ", F:" + s.Father_Name,
                Value = s.Id
            });
            ViewBag.EmployeesId = new SelectList(employee, "Value", "Text", empLeave.EmployeeId);
            return View(empLeave);
        }

        // POST: Leaves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmpLeaveEdit([Bind(Include = "Id,LeaveType,LeaveFromDate,LeaveToDate,ApplicationRef,EmployeeId,UserId,CreatedOn")] EmpLeave empLeave)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empLeave).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employee, "Id", "EmailAddress", empLeave.EmployeeId);
            return View(empLeave);
        }

        // GET: Leaves/Delete/5

        public ActionResult AllHoliDays()
        {
            var rec = db.HoliDays.OrderBy(f => f.LeaveFrom).ToList();
            return PartialView(rec);
        }


        public ActionResult HoliDays()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult HoliDays(HoliDays holidas)
        {
            var jsonmess = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsonmess = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(jsonmess, JsonRequestBehavior.AllowGet);
            }
            var exist = db.HoliDays.Where(f =>
            (f.LeaveFrom <= holidas.LeaveFrom && f.LeaveTo >= holidas.LeaveTo) ||
            (f.LeaveFrom >= holidas.LeaveFrom && f.LeaveTo <= holidas.LeaveTo) ||
            (f.LeaveFrom >= holidas.LeaveFrom && f.LeaveFrom <= holidas.LeaveTo) ||
            (f.LeaveFrom <= holidas.LeaveFrom && f.LeaveTo >= holidas.LeaveFrom)
            ).Any();

            if (exist)
            {
                jsonmess = new { Status = "Warning", Message = "Already Exist", url = "" };
                return Json(jsonmess, JsonRequestBehavior.AllowGet);
            }
            holidas.UpdatedOn = DateTime.UtcNow;
            holidas.TotalDays = CountDays(holidas.LeaveFrom, holidas.LeaveTo);
            db.HoliDays.Add(holidas);
            db.SaveChanges();

            jsonmess = new { Status = "OK", Message = "Successfully Saved", url = "/Leaves/AllHoliDays" };
            return Json(jsonmess, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLeavesRec(Guid EmployeeId, int Type) 
        {
           
            var l = db.LeaveType.Where(f => f.Id == Type).SingleOrDefault();
            return Json("----", JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public bool IsAlready(EmpLeave eml)
        {
            var exist = db.EmpLeaves.Where(s => s.EmployeeId == eml.EmployeeId && (
            (s.LeaveFromDate <= eml.LeaveFromDate && s.LeaveToDate >= eml.LeaveFromDate) || 
            (s.LeaveFromDate > eml.LeaveFromDate && s.LeaveFromDate < eml.LeaveToDate))).ToList();
            if (exist.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int CountDays(DateTime fdate, DateTime tdate)
        {

            int day = 0;
            for (DateTime date = fdate; date <= tdate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != Settings.WeekEnd && date.DayOfWeek != Settings.ComWeekEnd)
                {
                    day += 1;
                }
            }
            return day;
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
