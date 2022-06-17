using HRMMVC.Manager;
using HRMMVC.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    [Authorize]
    public class HRMController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        //   private ExtraFunctions ExtraFunctions = new ExtraFunctions();
        // GET: HRM
        [Route("")]
        public ActionResult Index()  // Dashboard View only
        {
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.Identity.TimeZone()).Date;
            var tomorrow = today.AddDays(1).Date;
            var q1 = _context.Employee.ToList().Select(f => new
            {
                f.DateOfBirth,
                today = today.AddYears(-f.DateOfBirth.Year),
                tomorrow = tomorrow.AddYears(-f.DateOfBirth.Year),
                f
            });

            ViewBag.TodayBirthDay = q1.Where(f => f.today == today).Select(f => f.f).ToList();
            ViewBag.TomorrowBirthDay = q1.Where(f => f.tomorrow == tomorrow).Select(f => f.f).ToList();

            ViewBag.TodaysLeave = _context.Department.ToList().Select(f => new DashboarCounting
            {
                Title = f.Name,
                Count = _context.EmpLeaves.Where(m => m.LeaveFromDate <= today.Date && m.LeaveToDate >= today.Date).Where(s => s.Employees.Designation.DepartmentId == f.Id).Count(),
            }).ToList();

            return View();
        }


        public ActionResult EmployeeLastMonthPresents()
        {
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.Identity.TimeZone()).Date;
            var index = today.AddDays(-30).Date;
            var firstlist = new List<PresentViewmodel>();
            for (DateTime i = index; i <= today; i = i.AddDays(1))
            {
                var pres = _context.DailyAttendance.Where(f => f.Date == i).DistinctBy(f => f.EmployeeId).Count();
                var emple = _context.EmpLeaves.Where(f => f.LeaveFromDate <= i && f.LeaveToDate >= i).Count();
                var totalemp = _context.Employee.Where(f => f.LeaveDateTime.Value > i).Count();
                var ab = totalemp > (emple + pres) ? totalemp - emple - pres : 0;
                var list = new PresentViewmodel()
                {
                    Date = i.ToString("dd-MMM-yyyy"),
                    Absence = ab,
                    Present = pres,
                };
                firstlist.Add(list);
            }
            var output = firstlist.Select(f => new
            {
                Date = f.Date,
                Absence = f.Absence,
                Present = f.Present
            }).ToList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }  // Json Call from home chart

        public ActionResult GetPerformanceLastMonth()
        {
            var empp = _context.EmpPerformance.OrderByDescending(f => f.Month).DistinctBy(f => f.EmployeeId).ToList();
            var days = Enum.GetValues(typeof(Performance)).Cast<Performance>()
                        .Select(d => d)
                        .ToList();
            var da = new List<DashboarCounting>();
            foreach (var item in days)
            {
                var d = new DashboarCounting()
                {
                    Title = item.ToString(),
                    Count = empp.Where(f => f.Performance == item).Count()
                };
                da.Add(d);
            }
            var res = da.Select(f => new
            {
                f.Title,
                f.Count
            }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }



        [Route("MyProfile"), Authorize]
        public ActionResult MyProfile()
        {
            var userId = User.Identity.GetUserId();
            var me = _context.Users.Where(f => f.Id == userId).SingleOrDefault();
            return PartialView(me);
        }

        [Route("MyProfile"), Authorize, HttpPost]
        public ActionResult MyProfile(ApplicationUser model)
        {
            var jsonre = new { Status = "Faild", Message = "Something went wrong", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsonre = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsonre, JsonRequestBehavior.AllowGet);
            }

            var user = _context.Users.Where(f => f.Id == model.Id).SingleOrDefault();
            if (user.DisplayName != model.DisplayName)
            {
                user.DisplayName = model.DisplayName;
            }
            if (user.TimeZoneId != model.TimeZoneId)
            {
                user.TimeZoneId = model.TimeZoneId;
            }
            if (user.PhoneNumber != model.PhoneNumber)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            jsonre = new { Status = "OK", Message = "Profile Updated", url = "/MyProfile" };
            return Json(jsonre, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Information() {  return PartialView(); }

    }
}