using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HRMMVC.Models;
using Microsoft.AspNet.Identity;

namespace HRMMVC.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Employees

        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Employee()
        {
            var d = db.Department.ToList().Select(f => new
            {
                Id = f.Id,
                text = f.Name
            }).ToList();
            var shift = db.Shift.ToList().Select(f => new
            {
                Id = f.Id,
                text = f.Name
            }).ToList();
            ViewBag.DepartmentId = new SelectList(d, "Id", "text");
            ViewBag.ShiftId = new SelectList(shift, "Id", "Text");
            return PartialView(new Employee());
        }
        //post Employees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Employee(Employee model)
        {
            var Jsonres = new { Status = "", Message = "", url = "" };

            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                var message = string.Format("Something went wrong, Please check {0} and try again", string.Join(", ", error));
                Jsonres = new { Status = "Faild", Message = message, url = "" };
                return Json(Jsonres, JsonRequestBehavior.AllowGet);
            }
            using (var savedata = db.Database.BeginTransaction())
            {
                try
                {
                    var check = db.Employee.Where(s => s.NationalId == model.NationalId).Any();
                    if (check)
                    {
                        ModelState.AddModelError("", "National id is already exist.");
                        Jsonres = new { Status = "Faild", Message = "National id is already exist", url = "" };
                        return Json(Jsonres, JsonRequestBehavior.AllowGet);
                    }
                    model.Id = Guid.NewGuid();
                    if (model.ImageUpload != null)
                    {
                        string directoryPath = "~/Assets/Images/Employee/";
                        bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                        if (!folderExists)
                            Directory.CreateDirectory(Server.MapPath(directoryPath));

                        string fileName = Path.GetFileNameWithoutExtension(model.ImageUpload.FileName);
                        string extension = Path.GetExtension(model.ImageUpload.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        model.ImageUrl = directoryPath+ fileName;
                        fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                        model.ImageUpload.SaveAs(fileName);
                    }
                    model.EmployeeCode = 1000;
                    var rowcount = db.Employee.Count();
                    if (rowcount > 0)
                    {
                        var empcode = db.Employee.Max(x => x.EmployeeCode);
                        model.EmployeeCode = empcode + 1;
                    }

                    model.IsLeave = false;
                    model.LeaveDateTime = null;
                    db.Employee.Add(model);
                    await db.SaveChangesAsync();
                    savedata.Commit();
                    Jsonres = new { Status = "OK", Message = "Successfully added", url = "/Employees/AddressSetup?id=" + model.Id };
                    return Json(Jsonres, JsonRequestBehavior.AllowGet);

                }
                catch (DbEntityValidationException ex)
                {
                    savedata.Rollback();
                    Jsonres = new { Status = "Error", Message = "Something went wrong, please try again", url = "" };
                    return Json(Jsonres, JsonRequestBehavior.AllowGet);
                }

            }


        }

        [HttpGet]
        public ActionResult AddressSetup(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var validid = db.Employee.Where(s => s.Id == id).SingleOrDefault();
            if (validid != null)
            {
                ViewBag.Id = validid.Id;
                ViewBag.Employee = validid.Full_Name + ", F:" + validid.Father_Name + ", Code: " + validid.EmployeeCode;
                var already = validid.AddressBook.ToList();
                ViewBag.Address = already;
                return PartialView(new AddressBook());
            }
            return RedirectToAction(nameof(Index));

        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddressSetup(AddressBook model, Guid? Id)
        {
            var jsre = new { Status = "Faild", Message = "Something Went wrong", url = "/Employees/Index" };
            if (Id == null)
            {
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(s => s.ErrorMessage).ToList();
                jsre = new { Status = "Warning", Message = string.Join(", ", error), url = "" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    var validemp = db.Employee.Where(s => s.Id == Id).SingleOrDefault();
                    model.Id = Guid.NewGuid();
                    db.AddressBook.Add(model);
                    db.SaveChanges();
                    validemp.AddressBook.Add(model);
                    await db.SaveChangesAsync();
                    save.Commit();
                    ModelState.Clear();
                    return RedirectToAction(nameof(Details), validemp.Id);
                }
                catch (Exception ex)
                {
                    save.Rollback();
                    jsre = new { Status = "Error", Message = ex.Message, url = "" };
                    return Json(jsre, JsonRequestBehavior.AllowGet);
                }

            }
        }


        [HttpGet]
        public ActionResult EducationalBackg(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var validid = db.Employee.Where(s => s.Id == id).SingleOrDefault();
            if (validid != null)
            {
                ViewBag.Id = validid.Id;
                ViewBag.Employee = validid.Full_Name + ", F:" + validid.Father_Name + ", Code: " + validid.EmployeeCode;
                var already = validid.Educational_Background.ToList();
                ViewBag.Education = already;
                return PartialView(new Educational_Background());
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EducationalBackg(Educational_Background model, Guid? Id)
        {

            var jsre = new { Status = "Faild", Message = "Something Went wrong", url = "/Employees/Index" };
            if (Id == null)
            {
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(s => s.ErrorMessage).ToList();
                jsre = new { Status = "Warning", Message = string.Join(", ", error), url = "" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {

                try
                {
                    var validemp = db.Employee.Where(s => s.Id == Id).SingleOrDefault();
                    model.Id = Guid.NewGuid();
                    db.Educational_Background.Add(model);
                    db.SaveChanges();
                    validemp.Educational_Background.Add(model);
                    await db.SaveChangesAsync();
                    save.Commit();
                    ModelState.Clear();
                    return RedirectToAction(nameof(Details), validemp.Id);
                }
                catch (Exception ex)
                {
                    save.Rollback();
                    jsre = new { Status = "Error", Message = ex.Message, url = "" };
                    return Json(jsre, JsonRequestBehavior.AllowGet);
                }

            }

        }




        [HttpGet]
        public ActionResult JobExperience(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var validid = db.Employee.Where(s => s.Id == id).SingleOrDefault();
            if (validid != null)
            {
                ViewBag.Id = validid.Id;
                ViewBag.Employee = validid.Full_Name + ", F:" + validid.Father_Name + ", Code: " + validid.EmployeeCode;
                var already = validid.JobExperience.ToList();
                ViewBag.Jobs = already;
                return PartialView(new JobExperience());
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> JobExperience(JobExperience model, Guid? Id)
        {
            var jsre = new { Status = "Faild", Message = "Something Went wrong", url = "/Employees/Index" };
            if (Id == null)
            {
                return RedirectToAction("Employee", "Employees");
            }
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(s => s.ErrorMessage).ToList();
                jsre = new { Status = "Warning", Message = string.Join(", ", error), url = "" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    var validemp = db.Employee.Where(s => s.Id == Id).SingleOrDefault();
                    model.Id = Guid.NewGuid();
                    db.JobExperience.Add(model);
                    db.SaveChanges();
                    validemp.JobExperience.Add(model);
                    await db.SaveChangesAsync();
                    save.Commit();
                    ModelState.Clear();
                    return RedirectToAction(nameof(Details), validemp.Id);
                }
                catch (Exception ex)
                {
                    save.Rollback();
                    jsre = new { Status = "Error", Message = ex.Message, url = "" };
                    return Json(jsre, JsonRequestBehavior.AllowGet);
                }

            }


        }

        public ActionResult Attachments(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var validid = db.Employee.Where(s => s.Id == id).SingleOrDefault();
            if (validid != null)
            {
                ViewBag.Id = validid.Id;
                ViewBag.Employee = validid.Full_Name + ", F:" + validid.Father_Name + ", Code: " + validid.EmployeeCode;
                var already = validid.Document.OrderBy(s => s.ReOrder).ToList();
                ViewBag.Doc = already;
                return PartialView(new Document());
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Attachments(Document model, Guid? Id)
        {
            var jsre = new { Status = "Faild", Message = "Something Went wrong", url = "/Employees/Index" };
            if (Id == null)
            {
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            model.Id = Guid.NewGuid();
            model.UploadOn = DateTime.UtcNow.AddHours(6);
            model.UploadBy = User.Identity.GetUserName();
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(s => s.ErrorMessage).ToList();
                jsre = new { Status = "Warning", Message = string.Join(", ", error), url = "" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
                {
                    try
                    {
                        var validemp = db.Employee.Where(s => s.Id == Id).SingleOrDefault();

                        if (model.FileUpload != null)
                        {
                            int size = model.FileUpload.ContentLength;
                            int kb = size / 1024;
                            if (kb > 5120)
                            {
                                TempData["Message"] = "File Size cann't more than 5 MB";
                                return RedirectToAction(nameof(Details), new { id = Id });
                            }


                            model.FileSize = model.FileUpload.ContentLength / 1024 + "KB";

                            string directoryPath = "~/Assets/Documents/Employee/";
                            bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                            if (!folderExists)
                            {
                                Directory.CreateDirectory(Server.MapPath(directoryPath));
                            }
                            string fileName = Path.GetFileNameWithoutExtension(model.FileUpload.FileName);
                            string extension = Path.GetExtension(model.FileUpload.FileName);

                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            model.Url = directoryPath + fileName;
                            fileName = Path.Combine(Server.MapPath(directoryPath), fileName);


                            model.FileUpload.SaveAs(fileName);
                            db.Document.Add(model);
                            db.SaveChanges();
                            validemp.Document.Add(model);
                            await db.SaveChangesAsync();
                            save.Commit();
                            ModelState.Clear();
                        }
                        return RedirectToAction(nameof(Details), new { id = Id });
                    }
                    catch (Exception ex)
                    {
                        save.Rollback();
                        TempData["Message"] = "Something went wrong " + ex;
                        return RedirectToAction(nameof(Details), new { id = Id });
                    }
            }

           
        }


        /*To Remove Job, Education, Address item from Employee following action*/

        public ActionResult Removeitem(Guid EmpId, string type, Guid id)
        {
            if (EmpId == null || id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            Employee emp = db.Employee.Where(s => s.Id == EmpId).SingleOrDefault();
            if (emp == null)
            {
                return RedirectToAction(nameof(Index));
            }
            switch (type)
            {
                case "Address":
                    AddressBook item = db.AddressBook.Where(s => s.Id == id).SingleOrDefault();
                    db.AddressBook.Remove(item);
                    db.SaveChanges();
                    break;
                case "Edu":
                    Educational_Background item1 = db.Educational_Background.Where(s => s.Id == id).SingleOrDefault();
                    db.Educational_Background.Remove(item1);
                    db.SaveChanges();
                    break;
                case "Job":
                    JobExperience item2 = db.JobExperience.Where(s => s.Id == id).SingleOrDefault();
                    db.JobExperience.Remove(item2);
                    db.SaveChanges();
                    break;
                case "Doc":
                    Document item3 = db.Document.Where(s => s.Id == id).SingleOrDefault();

                    string fullpath = Request.MapPath(item3.Url);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    db.Document.Remove(item3);
                    db.SaveChanges();
                    break;
                default:
                    break;
            }
            return RedirectToAction(nameof(Details), new { id = EmpId });
        }





        // GET: Employees/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return PartialView(employee);
        }




        public ActionResult UpdateEmployee(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            var username = User.Identity.GetUserId();
            var d = db.Department.ToList().Select(f => new
            {
                Id = f.Id,
                text = f.Name
            }).ToList();
            var shift = db.Shift.ToList().Select(f => new
            {
                Id = f.Id,
                text = f.Name
            }).ToList();
            var des = db.Designation.Where(f => f.Id == employee.DesignationId).ToList().Select(f => new
            {
                Id = f.Id,
                text = f.Title
            }).ToList();
            ViewBag.DepartmentId = new SelectList(d, "Id", "text", employee.Designation.DepartmentId);
            ViewBag.ShiftId = new SelectList(shift, "Id", "Text", employee.ShiftId);
            ViewBag.DesignationId = new SelectList(des, "Id", "Text");
            return PartialView(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmployee(Employee emp)
        {
            var jsr = new { Status = "Faild", Message = "Something went wrong", url = "" };
            var username = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsr = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsr, JsonRequestBehavior.AllowGet);
            }
                Employee s = new Employee();
                s = db.Employee.FirstOrDefault(f => f.Id == emp.Id);
                //var oldfilepath = db.Company.Where(x => x.Id == emp.Id).Select(sd => sd.CompanyLogo).FirstOrDefault();
                if (emp.ImageUpload != null)
                {
                    string directoryPath = "~/Assets/Images/Employee/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.ImageUrl = directoryPath+ fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    emp.ImageUpload.SaveAs(fileName);
                }
                s.ImageUrl = emp.ImageUrl;
                s.Email = emp.Email;
                s.Designation = emp.Designation;
                s.EmpType = emp.EmpType;
                s.Phone2 = emp.Phone2;
                s.BloodGroup = emp.BloodGroup;
                s.DateOfBirth = emp.DateOfBirth;
                s.Phone = emp.Phone;
                s.Phone1 = emp.Phone1;
                s.Notes = emp.Notes;
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
            jsr = new { Status = "OK", Message ="Updated", url = "/Employees/Index" };
            return Json(jsr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Employee employee = db.Employee.Find(id);
            db.Employee.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        public ActionResult PrintIds(Guid[] Id)
        {

            if (Id != null && Id.Count() > 0)
            {
                var rec = db.Employee.Where(s => Id.Contains(s.Id)).ToList();
                return PartialView(rec);

            }
            var emp = db.Employee.ToList();
            return PartialView(emp);


        }

        [HttpGet, AjaxOnly]
        public ActionResult Resignation() 
        {
            var emp = db.Employee.Where(f => f.IsLeave == false).ToList().Select(f => new
            {
                id = f.Id,
                text = f.Full_Name + " Code -" + f.EmployeeCode,
            }).ToList();
            ViewBag.EmployeeId = new SelectList(emp, "id", "text");
            return PartialView();
        }
        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
        public async  Task<ActionResult> Resignation(EmpResignViewModel model)
        {
            var json = new { Status = "", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                json = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            using(var save = db.Database.BeginTransaction())
            {
                try
                {
                    var employee = db.Employee.Where(f => f.Id == model.EmployeeId && f.IsLeave == false).SingleOrDefault();
                    if (employee == null)
                    {
                        json = new { Status = "Faild", Message = "Employee Record Not Found", url = "/Employees/Index" };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }

                    if (model.FileUpload != null)
                    {
                        var doc = new Document();
                        doc.Id = Guid.NewGuid();
                        doc.Title = "Resign Letter";
                        doc.ReOrder = -1;
                        doc.UploadOn = DateTime.UtcNow;
                        int size = model.FileUpload.ContentLength;
                        int kb = size / 1024;
                        if (kb > 5120)
                        {
                            json = new { Status = "Faild", Message = "File Size greater than 5 MB", url = "" };
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }
                        doc.FileSize = kb / 1024 + " MB";
                        string directoryPath = "~/Assets/Documents/Employee/";
                        bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                        if (!folderExists)
                        {
                            Directory.CreateDirectory(Server.MapPath(directoryPath));
                        }
                        string fileName = Path.GetFileNameWithoutExtension(model.FileUpload.FileName);
                        string extension = Path.GetExtension(model.FileUpload.FileName);

                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        doc.Url = directoryPath + fileName;
                        fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                        model.FileUpload.SaveAs(fileName);
                        db.Document.Add(doc);
                        db.SaveChanges();
                        employee.Document.Add(doc);
                    }
                    employee.LeaveDateTime = model.LeaveTime;
                    employee.IsLeave = true;
                    employee.Notes += "Leave Note : " + model.Casues;
                    db.Entry(employee).State = EntityState.Modified;
                  await  db.SaveChangesAsync();
                    save.Commit();
                    json = new { Status = "OK", Message = "Resign Successfull", url = "/Employees/Index" };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex )
                {
                    save.Rollback();
                    json = new { Status = "Error", Message = "Error : "+ex.Message, url = "/Employees/Index" };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
           
        }



        //  Ajax Call goes here ===================================================================================
        public ActionResult GetEmployeeListData()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = db.Employee.Include(f => f.Designation).ToList().Select(s => new
                {
                    Id = s.Id,
                    Email = s.Email,
                    Code = s.EmployeeCode,
                    Name = s.Full_Name,
                    JoiningDate = s.JoiningDate.ToString("dd-MMM-yyyy hh:mm tt"),
                    LeaveDate = s.LeaveDateTime?.ToString("dd-MMM-yyyy hh:mm tt") ?? "",
                    Designation = s.Designation.Title,
                    Phone = s.Phone,
                    Phone1 = s.Phone1,
                    Phone2 = s.Phone2,
                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }

        }  // Datatable
        public ActionResult GetDesignation(int Id)
        {
            var des = db.Designation.Where(s => s.DepartmentId == Id).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Title,
            }).ToList();
            return Json(des, JsonRequestBehavior.AllowGet);
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
