using HRMMVC.Manager;
using HRMMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    [RoutePrefix("Attendance")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Attendace
        [Route("")]
        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Entry()
        {

            var department = db.Department.ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Name
            }).ToList();
            var shift = db.Shift.ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Name
            }).ToList();
            ViewBag.DepartmentId = new SelectList(department, "Id", "Text");
            ViewBag.ShiftId = new SelectList(shift, "Id", "Text");

            return PartialView();
        }

        public ActionResult GetEmployee(int? DepartmentId, int? ShiftId, int? DesignationId)
        {
            var all = db.Employee.Where(s => s.IsLeave == false).Include(f=>f.Designation).ToList();
            if (DepartmentId > 0)
            {
                all = all.Where(s => s.Designation.DepartmentId == DepartmentId).ToList();
            }
            if (ShiftId > 0)
            {
                all = all.Where(s => s.ShiftId == ShiftId).ToList();
            }
            if (DesignationId > 0)
            {
                all = all.Where(s => s.DesignationId == DesignationId).ToList();
            }

            var jsondata = all.Select(f => new DailyAttendance
            {
                EmployeeId = f.Id,
                Out = Settings.OfficeEnd,
                Entry = Settings.OfficeStart,
                Employee = f
            }).ToList();

            return PartialView("_SearchEmployee", jsondata);
        }


        [HttpPost]
        public ActionResult Entry(DateTime Date, Guid[] EmployeeId, TimeSpan[] Entry, TimeSpan[] Out, bool[] isLeave, string[] ApplicationRef)
        {
            var Jsres = new {Status = "", Message = "", url ="" };
            if (EmployeeId.Length != Entry.Length && EmployeeId.Length != Out.Length && EmployeeId.Length != isLeave.Length && EmployeeId.Length != ApplicationRef.Length)
            {
                Jsres = new { Status = "Faild", Message = "Something Went Wrong, Please try again", url = "" };
                return Json(Jsres, JsonRequestBehavior.AllowGet);
            }

            List<DailyAttendance> da = new List<DailyAttendance>();
            for (int i = 0; i < EmployeeId.Length; i++)
            {
                var d = new DailyAttendance();
                d.EmployeeId = EmployeeId[i];
                d.Entry = Entry[i];
                d.Out = Out[i];
                d.Date = Date;
                d.ApplicationRef = null;
                d.IsLeave = isLeave[i];
                if (d.IsLeave == true)
                {
                    if (CheckValid(EmployeeId[i], d.Date, ApplicationRef[i]))
                    {
                        d.ApplicationRef = ApplicationRef[i];
                    }
                    else
                    {
                        d.ApplicationRef = "Not Found";
                    };
                }
                da.Add(d);
            }
            var checkdate = da.Where(f => f.Entry >= f.Out).FirstOrDefault();
            if (checkdate != null)
            {
                Jsres = new { Status = "Faild", Message = "Entry Time can't be greater than Out Time", url = "" };
                return Json(Jsres, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in da)
            {
                item.Id = Guid.NewGuid();
                db.DailyAttendance.Add(item);
            }
            db.SaveChanges();
            Jsres = new { Status = "OK", Message = "Added Successfully", url = "/Attendance/" };
            return Json(Jsres, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttendacesDate(DateTime? Date) 
        {
            var date = DateTime.UtcNow.Date;
            if (Date != null)
            {
                date = Date.Value.Date;
            }
            var at = db.DailyAttendance.Where(s => s.Date == date).ToList().Select(f => new
            {
                Employee = f.Employee,
                EmployeeId = f.EmployeeId,
                Entry = f.Entry,
                Out = f.Out,
                Duration = (f.Out - f.Entry).TotalMinutes,

            }).ToList();
            var listed = at.GroupBy(f => f.EmployeeId, (key, val) => new { Employee = key, Data = val }).ToList();

            var finalize = listed.Select(f => new
            {
                Employee = f.Data.FirstOrDefault().Employee.Full_Name,
                Entry = f.Data.Min(m => m.Entry).ToString(@"hh\:mm\:ss"),
                Out = f.Data.Max(m => m.Out).ToString(@"hh\:mm\:ss"),
                Duration = f.Data.Sum(l => l.Duration) / 60,
                EntryCount = f.Data.Count()
            }).ToList();
            return Json(new { data = finalize }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckValid(Guid empId, DateTime date, string refno)
        {
            return db.EmpLeaves.Where(s => s.EmployeeId == empId && s.LeaveFromDate <= date && s.LeaveToDate >= date && s.ApplicationRef == refno).Any();

        }
    }
}