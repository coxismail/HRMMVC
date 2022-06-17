using HRMMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    [RoutePrefix("CareerIn"), Authorize]
    public class CareerInController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        [Route("")]
        public ActionResult Index()
        {
            var doc = db.Circular.Where(f => f.IsClosed == false).ToList();
            return PartialView(doc);
        }
        public ActionResult Circular(Guid? Id)
        {
            if (Id != null)
            {
                var res = db.Circular.Where(f => f.Id == Id).SingleOrDefault();
                ViewBag.PageTitle = "Update Circular";
                return PartialView(res);
            }
            ViewBag.PageTitle = "Mew Circular";
            return PartialView(new Circular());
        }
        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
        public ActionResult Circular(Guid? Id, Circular model) 
        {
            var js = new { Status = "", Message = "", url = "" };
            model.Id = Guid.NewGuid();
            model.IsPublished = false;
            model.IsClosed = false;
            model.Deadline = TimeZoneInfo.ConvertTimeToUtc(model.Deadline, User.Identity.TimeZone());
            model.StartFrom = TimeZoneInfo.ConvertTimeToUtc(model.StartFrom, User.Identity.TimeZone());
            model.Created = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                 js = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            if (model.StartFrom >= model.Deadline)
            {
                 js = new { Status = "Faild", Message = "Deadline should be greater than Start Date", url = "" };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Circular.Add(model);
                db.SaveChanges();
                js = new { Status = "OK", Message = "Save Successfully", url = "/Careerin/Requirmentsetup?Id="+model.Id };
                return Json(js, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult Requirmentsetup(Guid Id) 
        {
            var cir = db.Circular.Where(f => f.Id == Id).Include(f=>f.Requirment).SingleOrDefault();
            if (cir != null)
            {
                ViewBag.Circular = cir;
                return PartialView();
            }
            return PartialView();
        }


        public ActionResult Details(Guid? Id) 
        {
            if (Id != null)
            {
                var res = db.Circular.Where(f => f.Id == Id).SingleOrDefault();
                return PartialView(res);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Applications(Guid ? Id) 
        {
            ViewBag.Post = "";
            var Jobs = db.JobApplication.ToList();
            if (Id != null)
            {
                var circular = db.Circular.Where(f => f.Id == Id).SingleOrDefault();
                ViewBag.Post = circular.Title;
                Jobs = Jobs.Where(f => f.Circular.Id == Id).ToList();
            }
            return PartialView(Jobs);
        }
    }
}