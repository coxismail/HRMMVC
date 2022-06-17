using HRMMVC.Manager;
using HRMMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    [RoutePrefix("Setup")]
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Basic
        [Route("")]
        public ActionResult BasicSetup()
        {
            return PartialView();
        }

        public ActionResult Performance()
        {
            var days = Enum.GetValues(typeof(Performance))
            .Cast<Performance>()
            .Select(d => d)
            .ToList();
            List<PerformanceBonus> pbl = new List<PerformanceBonus>();
            var ec = db.PerformanceBonus.ToList();
            foreach (var item in days)
            {
                var pr = ec.Where(f => f.Performance == item)?.SingleOrDefault() ?? new PerformanceBonus();
                var pb = new PerformanceBonus();
                pb.Amount = pr.Amount;
                pb.IsPercent = pr.IsPercent;
                pb.LastUpdated = pr.LastUpdated;
                pb.UpdatedBy = pr.UpdatedBy;
                pb.Performance = item;
                pb.Id = pr.Id;
                pbl.Add(pb);
            }
            return PartialView(pbl);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Performance(Performance[] Performance, int[] Amount, bool[] IsPercent)
        {
            var JsonMes = new { Status = "Error", Message = "", url = "/setup/Performance" };
            if (Performance.Length != Amount.Length || Performance.Length != IsPercent.Length)
            {
                JsonMes = new { Status = "Faild", Message = "Something went wrong please try again", url = "" };
                return Json(JsonMes, JsonRequestBehavior.AllowGet);
            }
            List<PerformanceBonus> pbl = new List<PerformanceBonus>();
            for (int i = 0; i < Performance.Length; i++)
            {
                var pb = new PerformanceBonus();

                pb.Amount = Amount[i];
                pb.IsPercent = IsPercent[i];
                pb.LastUpdated = DateTime.UtcNow;
                pb.UpdatedBy = User.Identity.DisplayName();
                pb.Performance = Performance[i];
                pbl.Add(pb);
            }
            var ehckper = pbl.Where(f => f.IsPercent == true && f.Amount > 100).Any();
            if (pbl.Where(f => f.Amount < 0).Any())
            {
                JsonMes = new { Status = "Faild", Message = "Amount can't be less than zero", url = "" };
                return Json(JsonMes, JsonRequestBehavior.AllowGet);
            }
            if (ehckper)
            {
                JsonMes = new { Status = "Faild", Message = "If you checked % Amount would not be greater than 100", url = "" };
                return Json(JsonMes, JsonRequestBehavior.AllowGet);
            }

            foreach (var item in pbl)
            {
                var exist = db.PerformanceBonus.Where(s => s.Performance == item.Performance).SingleOrDefault();
                if (exist != null)
                {
                    exist.Amount = item.Amount;
                    exist.IsPercent = item.IsPercent;
                    exist.UpdatedBy = item.UpdatedBy;
                    db.SaveChanges();
                }
                else
                {
                    db.PerformanceBonus.Add(item);
                    db.SaveChanges();
                }
            }
            JsonMes = new { Status = "OK", Message = "Something went wrong please try again", url = "" };
            return Json(JsonMes, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Company()
        {

            var c = db.Company.FirstOrDefault();
            if (c != null)
            {
                return PartialView(c);
            }
            else
            {
                return PartialView(new Company());
            }

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Company(Company model)
        {
            var JsonResult = new { Status = "Faild", Message = "Something Went wrong", url = "" };
            if (!ModelState.IsValid)
            {
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            var c = db.Company.FirstOrDefault();
            if (c != null)
            {
                c.Address = model.Address;
                c.City = model.City;
                c.Country = model.Country;
                c.State = model.State;
                c.Phone = model.Phone;
                c.Email = model.Email;
                c.Fax = model.Fax;
                c.Name = model.Name;
                c.Website = model.Website;
                db.SaveChanges();
                JsonResult = new { Status = "OK", Message = "Updated", url = "" };
            }
            else
            {
                db.Company.Add(model);
                db.SaveChanges();
                JsonResult = new { Status = "OK", Message = "Successfully Saved", url = "" };
            }
            Settings.initialized = false;
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Department(int Id, Department model)
        {

            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }

            var exist = db.Department.Where(s => s.Name.ToUpper() == model.Name.ToUpper()).Any();
            if (exist)
            {
                JsonResult = new { Status = "Warning", Message = "Already Exist", url = "/setup/" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            if (Id > 0)
            {
                if (Id == model.Id)
                {
                    Department obj = db.Department.Where(s => s.Id == Id).SingleOrDefault();
                    obj.Name = model.Name;
                    db.SaveChanges();
                    JsonResult = new { Status = "OK", Message = "Updated Successfully", url = "/setup/" };
                    return Json(JsonResult, JsonRequestBehavior.AllowGet);
                }
            }

            db.Department.Add(model);
            db.SaveChanges();
            JsonResult = new { Status = "OK", Message = "Add Successfully", url = "/setup/" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Shift(int Id, Shift model)
        {
            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            var exist = db.Shift.Where(s => s.Name.ToUpper() == model.Name.ToUpper()).Any();
            if (exist)
            {
                JsonResult = new { Status = "Warning", Message = "Already Exist", url = "/setup/" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            if (Id > 0)
            {
                if (Id == model.Id)
                {
                    Shift obj = db.Shift.Where(s => s.Id == Id).SingleOrDefault();
                    if (!exist)
                    {
                        obj.Name = model.Name;
                    }
                    obj.Start = model.Start;
                    obj.End = model.End;
                    db.SaveChanges();
                    JsonResult = new { Status = "OK", Message = "Updated Successfully", url = "/setup/" };
                    return Json(JsonResult, JsonRequestBehavior.AllowGet);
                }
            }

            db.Shift.Add(model);
            db.SaveChanges();
            JsonResult = new { Status = "OK", Message = "Add Successfully", url = "/setup/" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Designation(int Id, Designation model)
        {
            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }

            var exist = db.Designation.Where(s => s.Title.ToUpper() == model.Title.ToUpper() && s.DepartmentId == model.DepartmentId).Any();
            if (exist)
            {
                JsonResult = new { Status = "Warning", Message = "Already Exist", url = "/setup/" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }

            db.Designation.Add(model);
            db.SaveChanges();
            JsonResult = new { Status = "OK", Message = "Add Successfully", url = "/setup/" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Allowance(int Id, Allowance model)
        {
            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            var exist = db.Allowance.Where(s => s.Title.ToUpper() == model.Title.ToUpper()).Any();
            if (Id > 0)
            {
                if (Id == model.Id)
                {
                    Allowance obj = db.Allowance.Where(s => s.Id == Id).SingleOrDefault();
                    obj.Title = model.Title;
                    db.SaveChanges();
                    JsonResult = new { Status = "OK", Message = "Updated", url = "/setup/" };
                    return Json(JsonResult, JsonRequestBehavior.AllowGet);
                }

            }
            if (!exist)
            {
                db.Allowance.Add(model);
                db.SaveChanges();
                JsonResult = new { Status = "OK", Message = "Successfully Added", url = "/setup/" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Deduction(int Id, Deduction model)
        {
            var JsonResult = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                JsonResult = new { Status = "Error", Message = string.Join(", ", error), url = "" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            var exist = db.Deduction.Where(s => s.Title.ToUpper() == model.Title.ToUpper()).Any();
            if (Id > 0)
            {
                if (Id == model.Id)
                {
                    Deduction obj = db.Deduction.Where(s => s.Id == Id).SingleOrDefault();
                    obj.Title = model.Title;
                    db.SaveChanges();
                    JsonResult = new { Status = "OK", Message = "Updated", url = "/setup/" };
                    return Json(JsonResult, JsonRequestBehavior.AllowGet);
                }

            }
            if (!exist)
            {
                db.Deduction.Add(model);
                db.SaveChanges();
                JsonResult = new { Status = "OK", Message = "Successfully Added", url = "/Setup/" };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }




        // List Data  ============================================

        public ActionResult GetData(string type)
        {

            switch (type)
            {
                case "Department":
                    var rec = db.Department.ToList().Select(f => new
                    {
                        Id = f.Id,
                        Name = f.Name
                    }).ToList();
                    return Json(new { data = rec }, JsonRequestBehavior.AllowGet);
                case "Designation":
                    var recd = db.Department.ToList().Select(f => new
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Designation = f.Designation.Select(s => s.Title).ToList()
                    }).ToList();
                    return Json(new { data = recd }, JsonRequestBehavior.AllowGet);
                case "Shift":
                    var recs = db.Shift.ToList().Select(f => new
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Start = f.Start.ToString(@"hh\:mm"),
                        End = f.End.ToString(@"hh\:mm")
                    }).ToList();
                    return Json(new { data = recs }, JsonRequestBehavior.AllowGet);
                case "Allowance":
                    var allow = db.Allowance.ToList().Select(f => new
                    {
                        Id = f.Id,
                        Title = f.Title
                    }).ToList();
                    return Json(new { data = allow }, JsonRequestBehavior.AllowGet);

                case "Deduction":
                    var ded = db.Deduction.ToList().Select(f => new
                    {
                        Id = f.Id,
                        Title = f.Title
                    }).ToList();
                    return Json(new { data = ded }, JsonRequestBehavior.AllowGet);
                default:
                    break;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }



}