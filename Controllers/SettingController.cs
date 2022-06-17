using HRMMVC.Manager;
using HRMMVC.Models;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{

    public class SettingController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        //   private readonly ExtraFunctions ExtraFunctions = new ExtraFunctions();
        public ActionResult Activity_log()
        {
            return PartialView();
        }
        public ActionResult Get_Activity_logs()
        {
            int take = 10000;
            if (User.IsInRole("System Admin"))
            {
                take = 500000;
            }
            var rec = db.ActivityLogs.OrderByDescending(s => s.Execution_Time).ToList().Take(take).Select(m => new
            {
                date = TimeZoneInfo.ConvertTimeFromUtc(m.Execution_Time, User.Identity.TimeZone()).ToString("yyyy-MM-dd hh:mm tt"),
                browser = m.Browser,
                ip = m.Ip,
                activity = m.Activity,
                user = m.UserName
            }).ToList();
            return Json(new { data = rec }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login_History()
        {
            return PartialView();
        }
        public ActionResult Get_Login_History()
        {

            var beforeSixMonth = DateTime.UtcNow.AddHours(6).AddMonths(-6);
            var his = db.Login_History.OrderByDescending(s => s.Time).Where(s => s.Time >= beforeSixMonth).ToList().Select(s => new
            {
                Ip = s.Ip,
                User = s.UserName,
                Device = s.Device + " - " + s.Os_Version,
                Browser = s.Browser,
                Date = s.Time.ToString("yyyy-MM-dd hh:mm tt")
            }).ToList();
            return Json(new { data = his }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Other()
        {
            var model = new Other_setting();
            var result = db.Other_setting.Where(s => s.Active == true).OrderByDescending(f => f.ChangedOn).FirstOrDefault();
            if (result != null)
            {
                model = result;
            }
            return PartialView(model);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult Other(Other_setting setting)
        {
            var Jsonres = new { Status = "Error", Message = "", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                var message = string.Join(", ", error);
                Jsonres = new { Status = "Faild", Message = message, url = "" };
                return Json(Jsonres, JsonRequestBehavior.AllowGet);
            }
            Other_setting result = db.Other_setting.Where(s => s.Active == true).OrderByDescending(f => f.ChangedOn).FirstOrDefault();
            if (result != null)
            {
                result.Active = false;
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
            }

            setting.Id = Guid.NewGuid();
            setting.Active = true;
            setting.ChangedBy = User.Identity.GetUserName();
            setting.ChangedOn = DateTime.UtcNow.AddHours(6);
            db.Other_setting.Add(setting);
            db.SaveChanges();

            Settings.initialized = false;
            //   Session["setting"] = db.Other_setting.Where(s => s.Active == true).OrderByDescending(f => f.ChangedOn).FirstOrDefault() ?? new Other_setting();
            Jsonres = new { Status = "OK", Message = "Successfully Updated", url = "/Setting/Other" };
            return Json(Jsonres, JsonRequestBehavior.AllowGet);
        }



        public ActionResult BackupDb()
        {
            return View();
        }





        public ActionResult ServiceConfiguration()
        {
            var sms = db.SMSConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault() ?? new SMSConfig();
            var email = db.EmailConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault() ?? new EmailConfig();
            return View(sms);
        }

        public ActionResult SMSConfiguration()
        {
            var sms = db.SMSConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault() ?? new SMSConfig();
            return PartialView(sms);
        }

        public ActionResult MailConfiguration()
        {
            var email = db.EmailConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault() ?? new EmailConfig();
            return PartialView(email);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SMSConfiguration(SMSConfig smsconfig)
        {

            SMSConfig sms = db.SMSConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault();
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid Data found";
            }
            smsconfig.Id = Guid.NewGuid();
            smsconfig.UpdatedBy = User.Identity.GetUserName();
            smsconfig.UpdatedOn = DateTime.UtcNow.AddHours(6);
            if (sms != null)
            {
                sms.Active = false;
                db.SaveChanges();
            }
            db.SMSConfig.Add(smsconfig);
            db.SaveChanges();
            TempData["Message"] = "Successfully Saved";
            return PartialView(smsconfig);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MailConfiguration(EmailConfig emailConf)
        {
            EmailConfig email = db.EmailConfig.OrderByDescending(s => s.UpdatedOn).Where(s => s.Active == true).FirstOrDefault();
            if (ModelState.IsValid)
            {
                TempData["Message"] = "Invalid Data found";
            }
            emailConf.Id = Guid.NewGuid();
            emailConf.UpdatedBy = User.Identity.GetUserName();
            emailConf.UpdatedOn = DateTime.UtcNow.AddHours(6);

            if (email != null)
            {
                email.Active = false;
                db.SaveChanges();
            }
            db.EmailConfig.Add(emailConf);
            db.SaveChanges();
            TempData["Message"] = "Successfully Saved";
            return PartialView(emailConf);
        }






        // IMport Master =========================================================== Start here


        public ActionResult ImportMaster() // Excel Importer Customer, Supplier, Employee
        {
            return View();
        }












        /*============================================================ 
                       Employee
        ============================================================*/

        [HttpPost]
        public ActionResult ReadExcelAsEmployee()
        {
            List<EmployeeImporterModel> lstStudent = new List<EmployeeImporterModel>();
            if (ModelState.IsValid)
            {

                string filePath = string.Empty;
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["file"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {

                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        filePath = path + Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        file.SaveAs(filePath);
                        Stream stream = file.InputStream;
                        // We return the interface, so that  
                        IExcelDataReader reader = null;
                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return RedirectToAction("ExcelUpload");
                        }
                        // reader.IsFirstRowAsColumnNames = true;
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        reader.Close();
                        //delete the file from physical path after reading   
                        string filedetails = path + fileName;
                        FileInfo fileinfo = new FileInfo(filedetails);
                        if (fileinfo.Exists)
                        {
                            fileinfo.Delete();
                        }
                        DataTable dt = result.Tables[0];
                        lstStudent = ConvertDataTable<EmployeeImporterModel>(dt);
                        TempData["ExcelEmployee"] = lstStudent;
                    }
                }

            }
            // var files = Request.Files;  

            return new JsonResult { Data = lstStudent, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public async Task<ActionResult> UpdateEmployeeTable()
        {
            int length = 0;

            if (TempData["ExcelEmployee"] == null)
            {
                return new JsonResult { Data = length, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            List<EmployeeImporterModel> empdata = (List<EmployeeImporterModel>)TempData["ExcelEmployee"];
            IEnumerable<int> duplicate1 = empdata.GroupBy(x => x.Code).Where(g => g.Count() > 1).Select(x => x.Key);
            IEnumerable<string> duplicate2 = empdata.GroupBy(x => x.NationalId).Where(g => g.Count() > 1).Select(x => x.Key);
            IEnumerable<string> duplicate3 = empdata.GroupBy(x => x.BussinessPhone).Where(g => g.Count() > 1).Select(x => x.Key);
            if (duplicate1.Count() > 0 || duplicate2.Count() > 0 || duplicate3.Count() > 0)
            {
                return new JsonResult { Data = "Dplicate Data Found, It may Code, Phone or National Id", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var s in empdata)
                    {
                        var already = db.Employee.Where(f => f.EmployeeCode == s.Code || f.NationalId == s.NationalId || f.Phone2 == s.BussinessPhone).Any();
                        if (!already)
                        {
                            Employee e = new Employee();
                            e.Phone2 = s.BussinessPhone;
                            //  e.AddressLineOne = s.Address;
                            e.BloodGroup = s.BloodGroup;


                            e.Email = s.Email;
                            e.EmployeeCode = s.Code;
                            e.JoiningDate = s.JoiningDate;

                            e.DateOfBirth = s.DateOfBirth;
                            e.Id = Guid.NewGuid();
                            e.Full_Name = s.FirstName;
                            e.Full_Name = s.LastName;
                            e.Gender = s.Genders;
                            e.Phone = s.MobilePhone;
                            e.Phone1 = s.HomePhone;
                            e.Notes = s.Notes;
                            e.NationalId = s.NationalId;
                            e.IsLeave = false;
                            db.Employee.Add(e);
                            db.SaveChanges();
                            length += 1;
                        }
                    }
                    await db.SaveChangesAsync();
                    save.Commit();

                }
                catch (Exception ex)
                {

                    save.Rollback();
                }


                return new JsonResult { Data = length, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        /*===============================================================
                            Common Function for above excel
            =============================================================*/

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.Trim().ToUpper() == column.ColumnName.Trim().ToUpper())
                        switch (pro.PropertyType.Name)
                        {
                            case "Gender":
                                var value = Enum.Parse(typeof(Gender), dr[column.ColumnName].ToString(), false);
                                pro.SetValue(obj, value ?? null, null);
                                break;
                            case "BloodGroup":
                                var val = Enum.Parse(typeof(BloodGroup), dr[column.ColumnName].ToString(), false);
                                pro.SetValue(obj, val ?? null, null);
                                break;
                            default:
                                pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType) ?? null, null);
                                break;
                        }


                    else
                        continue;
                }
            }
            return obj;
        }







        // IMport Master =========================================================== Start here



        /*===============================================================================================
                                             Ajax Coll goes here
        ==================================================================================================*/
        [HttpPost]
        public ActionResult MakeDbBackUpCopy()
        {
            var Message = "";
            var strFileName = "C:\\Users\\Administrator\\Assets\\BackupDb";
            var connectionString = ConfigurationManager.ConnectionStrings["DrtDbConnection"].ConnectionString;
            if (!Directory.Exists(strFileName))
            {
                Directory.CreateDirectory(strFileName);
            }
            var filename = strFileName + "\\dbBackup.bak";
            try
            {
                string Query = @"BACKUP DATABASE chandrim_drt TO DISK='" + filename + "'";
                SqlCommand Command = null;
                SqlConnection connection = null;
                connection = new SqlConnection(connectionString);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                Command = new SqlCommand(Query, connection);

                var res = Command.ExecuteNonQuery();

                connection.Close();

                Message = "Executed with " + res;

            }
            catch (Exception ex)
            {
                Message = "Falid try again" + ex;
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

    }
}