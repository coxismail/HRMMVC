using HRMMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    [RoutePrefix("Career")]
    public class CareerController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Career
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Result() 
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Result(string Type, int Id, string Key) 
        {
            var rec = db.JobApplication.Where(f => f.ApplicationId == Id && f.Key == Key).SingleOrDefault();
            if (rec == null)
            {
                ViewBag.Message = "Not Found";
                return View();
            }
            if (Type == "Preliminary")
            {
                var pr = rec.Preliminary;
                switch (pr)
                {
                    case ApplicationStatus.Faild:
                        ViewBag.Message = "You have faild";
                            break;
                    case ApplicationStatus.Passed:
                        ViewBag.Message = "Congratulation! You have Passed";
                        break;
                    case ApplicationStatus.Cancelled:
                        ViewBag.Message = "Sorry! Your Application has been canceled";
                        break;
                    default:
                        ViewBag.Message = "Result is not published yet";
                        break;
                }
            }
            if (Type == "Viva")
            {
                var pr = rec.Viva;
                switch (pr)
                {
                    case ApplicationStatus.Faild:
                        ViewBag.Message = "You have faild";
                        break;
                    case ApplicationStatus.Passed:
                        ViewBag.Message = "Congratulation! You have Passed";
                        break;
                    case ApplicationStatus.Cancelled:
                        ViewBag.Message = "Sorry! Your Application has been canceled";
                        break;
                    default:
                        ViewBag.Message = "Result is not published yet";
                        break;
                }
            }
            return View();
        }
    }
}