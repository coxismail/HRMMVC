using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using HRMMVC.Manager;
using HRMMVC.Models;
using Microsoft.AspNet.Identity;

namespace HRMMVC.Controllers
{
    [Authorize]
    public class SalarysheetsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // private ExtraFunctions ExtraFunctions = new ExtraFunctions();

        public ActionResult Index()
        {
            var salarysheet = db.Salarysheet.Include(s => s.Employee).Include(f => f.AllowancSalary).Include(f => f.Hourly_Duty).Include(f => f.AdvanceSalaryDeduction).OrderByDescending(f => f.SalaryType).Where(x => x.IsApproved == false);
            return PartialView(salarysheet.ToList());
        }

        public ActionResult PreviousSheet(DateTime? Month)  // need to updated with stm latest
        {
         var  month = Month ?? DateTime.UtcNow.Date;


            var salarysheet22 = db.Salarysheet.Where(m => m.IsApproved == true && m.FromDate.Month == month.Month && m.FromDate.Year == month.Year).Include(s => s.Employee).ToList();
            SalarySheetViewModel sl = new SalarySheetViewModel();
            List<Salarysheet_property> ps = new List<Salarysheet_property>();
            sl.Month = month.Date;
            foreach (var sh in salarysheet22)
            {
                Salarysheet_property p = new Salarysheet_property();
                p.Loan = sh.Loan;
                p.NetSalary = sh.NetSalary;
                p.BasicSalary = sh.BasicSalary;
                p.GrossSalary = sh.GrossSalary;
                p.Office_Holiday = sh.Office_Holiday;
                p.Paid_Leave = sh.Paid_Leave;
                p.Presents = sh.Presents;
                p.Pay_Full_Month = sh.Pay_Full_Month;
                p.SalaryType = sh.SalaryType;
                p.ToDate = sh.ToDate;
                p.TotalLeave = sh.TotalLeave;
                p.UnPaid_Leave = sh.UnPaid_Leave;
                p.TotalWorkDays = sh.TotalWorkDays;
                p.Absence = (int)sh.Absence;
                p.Advance = sh.Advance;
                p.Approved_By = sh.Approved_By;
                p.Designation = sh.Designation;
                p.FromDate = sh.FromDate;
                p.Full_Name = sh.Employee.Full_Name + " " + sh.Employee.Father_Name;
                p.Id = sh.Id;
                p.IshandOver = sh.IsHandover;

                var br = db.AllowanceSalary.Where(s => s.Id == sh.Id).ToList();
                var dr = db.AdvanceSalaryDeductions.Where(s => s.Id == sh.Id).SingleOrDefault();
                if (sh.SalaryType == SalaryTypes.Hourly)
                {
                    var hr = db.Hourly_Duty.Where(s => s.Id == sh.Id).SingleOrDefault();
                    p.OverTime_Hour = hr.Overtime_hour;
                    p.PayForHour = hr.PayforHour;
                    p.Rate_Per_Hour = hr.RatePerHour;
                }

                p.Bonus = ((decimal?)br.Sum(s => s.Benifited_Amount) ?? 0);
                p.Deduction = dr.Amount;
                ps.Add(p);
            }
            sl.Salarysheet_property = ps;
            return PartialView(sl);
        }


        public ActionResult Performance()
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
        [HttpPost]
        public ActionResult Performance(Guid[] EmployeeId, Performance[] Performance)
        {
            var jsmess = new { Status = "Faild", Message = "Something went wrong please try again", url = "" };

            if (EmployeeId.Length != Performance.Length)
            {
                return Json(jsmess, JsonRequestBehavior.AllowGet);
            }
            List<EmpPerformance> emf = new List<EmpPerformance>();
            for (int i = 0; i < EmployeeId.Length; i++)
            {
                var ep = new EmpPerformance()
                {
                    EmployeeId = EmployeeId[i],
                    Performance = Performance[i],
                    Month = DateTime.UtcNow.Date
                };
                emf.Add(ep);
            }
            foreach (var item in emf)
            {
                    item.Id = Guid.NewGuid();
                    db.EmpPerformance.Add(item);
            }
            db.SaveChanges();
            jsmess = new { Status = "OK", Message = "Successfully Saved", url = "/Salarysheets/Perpormance" };
            return Json(jsmess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployee(int? DepartmentId, int? ShiftId, int? DesignationId)
        {
            var all = db.Employee.Where(s => s.IsLeave == false).Include(f => f.Designation).ToList();
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

            var jsondata = all.Select(f => new PerformanceViewModel
            {
                EmployeeId = f.Id,
                Name = f.Full_Name,
                Code = f.EmployeeCode,
            }).ToList();

            return PartialView("_SearchEmployee", jsondata);
        }

        public ActionResult Advance_History()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Get_Advance_Salary_Data() // Json Call
        {
            var emp = db.Employee.Include(f => f.Designation).ToList().Select(s => new
            {
                Name = s.Full_Name + " " + s.Father_Name,
                position = s.Designation.Title,
                Join_Date = s.JoiningDate.ToString("dd-MMM-yyyy hh:mm tt"),
                //  loan = (((decimal?)db.Employee_Loan.Where(m => m.EmployeeId == s.Id).ToList().Sum(a => a.Amount) ?? 0) - ((decimal?)db.Loan_Paid_history.Where(x => x.EmployeeId == s.Id).ToList().Sum(m => m.Amount)) ?? 0),
                Total_Advance = ((decimal?)db.Advance_Salary.Where(v => v.EmployeeId == s.Id).ToList()?.Sum(m => m.Amount) ?? 0),
                Paid = ((decimal?)db.AdvanceSalaryDeductions.Where(m => m.EmployeeId == s.Id).ToList()?.Sum(l => l.Amount) ?? 0),

            }).ToList();
            return Json(new { data = emp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewPayslip()
        {
            var Empl = db.Employee.Where(s => s.IsLeave == false).ToList().Select(f => new
            {
                Id = f.Id,
                Text = f.Full_Name + ", " + f.EmployeeCode
            }).ToList();
            ViewBag.EmployeeId = new SelectList(Empl, "Id", "Text");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPayslip(Guid EmployeeId, DateTime Month)
        {
            var jsre = new { Status = "", Message = "", url = "" };

            Session["record"] = null;
            Session["benefits"] = null;
            Session["loss"] = null;
            int day = 1;
            int mo = Month.Month;
            int year = Month.Year;
            var fdate = new DateTime(year, mo, day);
            var tdate = fdate.AddMonths(1).AddDays(-1);

            Salarysheet salarysheet = new Salarysheet();
            var payroll = db.Payroll.Where(s => s.IsActive == true && s.EmployeeId == EmployeeId).Include(f => f.Payroll_Allowance).Include(f => f.Payroll_Deduction).SingleOrDefault();
            var empProfile = db.Employee.Include(f => f.Designation).Where(s => s.Id == EmployeeId && s.IsLeave == false).SingleOrDefault();
            var attendanceSumary = GetAttendanceSummary(EmployeeId, fdate, tdate);
            var checkp = (empProfile.JoiningDate.Month > fdate.Month && empProfile.JoiningDate.Year >= fdate.Year);
            var LastPayDate = db.Salarysheet.Where(a => a.EmployeeId == EmployeeId).Where(s => s.FromDate.Month == fdate.Month && s.FromDate.Year == fdate.Year).Any();
            if (LastPayDate)
            {
                jsre = new { Status = "Warning", Message = "Salary Already made for this employee on this month", url = "/Salarysheets/newpayslip" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            if (checkp)
            {
                jsre = new { Status = "Warning", Message = "At your selected month This employee has not been started working on this company", url = "/Salarysheets/newpayslip" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            if (!db.DailyAttendance.Where(f => f.EmployeeId == EmployeeId && f.Date.Month == fdate.Month && f.Date.Year == fdate.Year).Any())
            {
                jsre = new { Status = "Warning", Message = "Attendance record has not been inserted yet", url = "/Salarysheets/newpayslip" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }
            if (payroll == null)
            {
                jsre = new { Status = "Warning", Message = "Payroll has not been setup yet", url = "/Salarysheets/newpayslip" };
                return Json(jsre, JsonRequestBehavior.AllowGet);
            }

            decimal As = GetAdvanceSalaryReport(EmployeeId);
            string dn = db.Advance_Salary.OrderByDescending(s => s.CreatedON).Select(s => s.Notes).FirstOrDefault();
            var EmpLeave = ((int?)CountEmployeeLeave(fdate, tdate, EmployeeId) ?? 0);

            salarysheet.EmployeeId = empProfile.Id;
            salarysheet.Employee = empProfile;
            salarysheet.BasicSalary = payroll.Basic_Salary;
            salarysheet.FromDate = fdate;
            salarysheet.TotalWorkDays = attendanceSumary.Working_Day;
            salarysheet.Presents = attendanceSumary.DutyDay;
            salarysheet.ToDate = tdate;
            salarysheet.Advance = As;
            salarysheet.SalaryType = payroll.SalaryType;
            salarysheet.Paid_Leave = EmpLeave;
            salarysheet.Pay_Full_Month = false;
            salarysheet.Designation = empProfile.Designation.Title;
            salarysheet.Absence = ((int?)(attendanceSumary.Working_Day - attendanceSumary.DutyDay) ?? 0);
            salarysheet.Presents = attendanceSumary.DutyDay;
            salarysheet.TotalWorkDays = attendanceSumary.Working_Day;
            salarysheet.Overtime_day = attendanceSumary.OverTime_Day;

            salarysheet.OverTime_Rate = payroll.Over_Time_Rate;
            if (payroll.SameAsBasic == true)
            {
                salarysheet.OverTime_Rate = payroll.Basic_Salary / attendanceSumary.Working_Day;
            }


            if (payroll.SalaryType == SalaryTypes.Monthly)
            {
                salarysheet.SalaryType = SalaryTypes.Monthly;
                salarysheet.Absence = ((int?)(attendanceSumary.Working_Day - attendanceSumary.DutyDay) ?? 0);
                salarysheet.Presents = attendanceSumary.DutyDay;
                salarysheet.TotalWorkDays = attendanceSumary.Working_Day;
                salarysheet.Overtime_day = attendanceSumary.OverTime_Day;
                salarysheet.OverTime_Rate = payroll.Over_Time_Rate;
                if (payroll.SameAsBasic == true)
                {
                    salarysheet.OverTime_Rate = payroll.Basic_Salary / attendanceSumary.Working_Day;
                }
                salarysheet.GrossSalary = (payroll.Basic_Salary / attendanceSumary.Working_Day * attendanceSumary.DutyDay) + (salarysheet.Overtime_day * salarysheet.OverTime_Rate);

            }
            if (payroll.SalaryType == SalaryTypes.Hourly)
            {
                salarysheet.OverTime_Rate = payroll.Over_Time_Rate;
                salarysheet.Absence = 0;
                if (payroll.SameAsBasic == true)
                {
                    salarysheet.OverTime_Rate = payroll.Basic_Salary / attendanceSumary.Working_Hour;
                }
                salarysheet.SalaryType = SalaryTypes.Hourly;
                Hourly_Duty h = new Hourly_Duty();
                h.Overtime_hour = attendanceSumary.OverTime_Hour;
                h.PayforHour = attendanceSumary.Duty_Hour;
                h.RatePerHour = payroll.Basic_Salary / attendanceSumary.Working_Hour;
                h.Total_Working_Hour = attendanceSumary.Working_Hour;
                salarysheet.Hourly_Duty = h;
                Session["hourly"] = h;
                salarysheet.GrossSalary = (payroll.Basic_Salary / attendanceSumary.Working_Hour * attendanceSumary.Duty_Hour) + (salarysheet.Hourly_Duty.Overtime_hour * salarysheet.OverTime_Rate);

            }
            salarysheet.GrossSalary = (payroll.Basic_Salary / attendanceSumary.Working_Day * attendanceSumary.DutyDay) + (salarysheet.Overtime_day * salarysheet.OverTime_Rate);


            var perb = db.PerformanceBonus.ToList();
            List<AllowanceSalary> allowanceSalary = new List<AllowanceSalary>();
            foreach (var item in payroll.Payroll_Allowance)
            {
                AllowanceSalary b = new AllowanceSalary();
                b.Benifited_Field = item.Allowance.Title;
                b.Benifited_Amount = item.Amount;
                b.Notes = item.Amount + " Fixed Benifit";
                if (item.PecentOf == PecentOf.Basic)
                {
                    var am = (payroll.Basic_Salary * item.Amount / 100);
                    b.Benifited_Amount = am;
                    b.Notes = item.Amount + " % of Basic Salary";
                }
                else if (item.PecentOf == PecentOf.Gross)
                {
                    var am = (salarysheet.GrossSalary * item.Amount / 100);
                    b.Benifited_Amount = am;
                    b.Notes = item.Amount + " % of Gross Salary";
                }
                allowanceSalary.Add(b);
            }
            var EmployePerformance = db.EmpPerformance.Where(s => s.EmployeeId == empProfile.Id && s.Month.Month == fdate.Month && s.Month.Year == fdate.Year).SingleOrDefault();
            if (EmployePerformance != null)
            {
                var p = perb.Where(f => f.Performance == EmployePerformance.Performance).SingleOrDefault();
                if (p != null && p.Amount > 0)
                {
                    var PerformBonus = new AllowanceSalary()
                    {
                        Benifited_Field = " Performance bonus",
                        Notes = " "
                    };
                    var PerformAmount = p.Amount;
                    PerformBonus.Benifited_Amount = PerformAmount;

                    if (p.IsPercent == true)
                    {
                        PerformAmount = Convert.ToInt32((payroll.Basic_Salary / 100) * p.Amount);
                        PerformBonus.Benifited_Amount = PerformAmount;
                        PerformBonus.Notes = p.Amount + "% Basic Salary";
                    }
                    allowanceSalary.Add(PerformBonus);

                }
            }
            salarysheet.AllowancSalary = allowanceSalary;
            List<DeductionsSalary> extraDeduction = new List<DeductionsSalary>();
            foreach (var item in payroll.Payroll_Deduction)
            {
                DeductionsSalary b = new DeductionsSalary();
                b.Deduction_Field = item.Deduction.Title;
                b.Deducted_Amount = item.Amount;
                b.Notes = item.Amount + "  ";
                if (item.PecentOf == PecentOf.Basic)
                {
                    var am = (payroll.Basic_Salary * item.Amount / 100);
                    b.Deducted_Amount = am;
                    b.Notes = item.Amount + " % of Basic Salary";
                }
                else if (item.PecentOf == PecentOf.Gross)
                {
                    var am = (salarysheet.GrossSalary * item.Amount / 100);
                    b.Deducted_Amount = am;
                    b.Notes = item.Amount + " % of Gross Salary";
                }
                extraDeduction.Add(b);
            }


            salarysheet.NetSalary = salarysheet.GrossSalary + ((decimal?)salarysheet.AllowancSalary.Sum(s => s.Benifited_Amount) ?? 0);
            salarysheet.AllowancSalary = allowanceSalary;
            salarysheet.DeductionsSalary = extraDeduction;
            Session["record"] = salarysheet;
            Session["benefits"] = allowanceSalary;
            Session["loss"] = extraDeduction;
            jsre = new { Status = "Info", Message = "Going to be confirmed", url = "/Salarysheets/ConfirmPayslip" };
            return Json(jsre, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPayslipAuto(Guid[] EmployeeId, DateTime Month)
        {
            string messgae = "";

            var jsre = new { Status = "", Message = "", url = "" };
            Session["record"] = null;
            int day = 1;
            int mo = Month.Month;
            int year = Month.Year;
            var fdate = new DateTime(year, mo, day);
            var tdate = fdate.AddMonths(1).AddDays(-1);
            int workingdays = 0;
            var perb = db.PerformanceBonus.ToList();

            List<Salarysheet> ssl = new List<Salarysheet>();
            for (int i = 0; i < EmployeeId.Length; i++)
            {

                var employeeId = EmployeeId[i];
                Salarysheet se = new Salarysheet();
                var employeeprofile = db.Employee.Where(s => s.Id == employeeId).Include(f => f.Designation).SingleOrDefault();
                var payroll = db.Payroll.Where(s => s.IsActive == true && s.EmployeeId == employeeId).Include(f => f.Payroll_Deduction).Include(f => f.Payroll_Allowance).SingleOrDefault();
                var checkp = (employeeprofile.JoiningDate.Month > fdate.Month && employeeprofile.JoiningDate.Year >= fdate.Year);
                var LastPayDate = db.Salarysheet.Where(a => a.EmployeeId == employeeId).Where(s => s.FromDate.Month == fdate.Month && s.FromDate.Year == fdate.Year).Any();
                if (LastPayDate)
                {
                    messgae += "Already Made for : " + employeeprofile.Full_Name;
                }
                if (checkp)
                {
                    jsre = new { Status = "Warning", Message = "At your selected month This employee has not been started working on this company", url = "/Salarysheets/newpayslip" };
                    return Json(jsre, JsonRequestBehavior.AllowGet);
                }

                if (payroll == null)
                {
                    jsre = new { Status = "Warning", Message = "Payroll has not been setup yet", url = "/Salarysheets/newpayslip" };
                    return Json(jsre, JsonRequestBehavior.AllowGet);
                }

                decimal AdvanceSalary = GetAdvanceSalaryReport(EmployeeId[i]);
                string dn = db.Advance_Salary.OrderByDescending(s => s.CreatedON).Select(s => s.Notes).FirstOrDefault();
                var EmpLeave = ((int?)CountEmployeeLeave(fdate, tdate, EmployeeId[i]) ?? 0);

                se.EmployeeId = employeeprofile.Id;
                se.Employee = employeeprofile;

                se.BasicSalary = payroll.Basic_Salary;
                se.FromDate = fdate;
                se.ToDate = tdate;
                se.Advance = AdvanceSalary;
                se.Paid_Leave = EmpLeave;
                se.Pay_Full_Month = false;
                List<AllowanceSalary> PayrollAllownaceList = new List<AllowanceSalary>();
                foreach (var item in payroll.Payroll_Allowance)
                {
                    AllowanceSalary b = new AllowanceSalary();
                    b.Benifited_Field = item.Allowance.Title;
                    b.Benifited_Amount = item.Amount;
                    b.Notes = item.Amount + "Fixed Benifit";
                    if (item.PecentOf == PecentOf.Basic)
                    {
                        var am = (payroll.Basic_Salary * item.Amount / 100);
                        b.Benifited_Amount = am;
                        b.Notes = item.Amount + " % of Basic Salary";
                    }
                    else if (item.PecentOf == PecentOf.Gross)
                    {
                        var am = (se.GrossSalary * item.Amount / 100);
                        b.Benifited_Amount = am;
                        b.Notes = item.Amount + " % of Gross Salary";
                    }
                    PayrollAllownaceList.Add(b);
                }
                var EmployePerformance = db.EmpPerformance.Where(s => s.EmployeeId == employeeId && s.Month.Month == fdate.Month && s.Month.Year == fdate.Year).SingleOrDefault();
                if (EmployePerformance != null)
                {
                    var p = perb.Where(f => f.Performance == EmployePerformance.Performance).SingleOrDefault();
                    if (p != null && p.Amount > 0)
                    {
                        var PerformBonus = new AllowanceSalary()
                        {
                            Benifited_Field = "Performance",
                            Notes = "Fixed Amount"
                        };
                        var PerformAmount = p.Amount;
                        PerformBonus.Benifited_Amount = PerformAmount;

                        if (p.IsPercent == true)
                        {
                            PerformAmount = Convert.ToInt32((payroll.Basic_Salary / 100) * p.Amount);
                            PerformBonus.Benifited_Amount = PerformAmount;
                            PerformBonus.Notes = p.Amount + "% Basic Salary";
                        }
                        PayrollAllownaceList.Add(PerformBonus);

                    }
                }
                List<DeductionsSalary> PayrollDeductionList = new List<DeductionsSalary>();
                foreach (var item in payroll.Payroll_Deduction)
                {
                    DeductionsSalary dedu = new DeductionsSalary();
                    dedu.Deduction_Field = item.Deduction.Title;
                    dedu.Deducted_Amount = item.Amount;
                    dedu.Notes = item.Amount + "Fixed Benifit";
                    if (item.PecentOf == PecentOf.Basic)
                    {
                        var am = (payroll.Basic_Salary * item.Amount / 100);
                        dedu.Deducted_Amount = am;
                        dedu.Notes = item.Amount + " % of Basic Salary";
                    }
                    else if (item.PecentOf == PecentOf.Gross)
                    {
                        var am = (se.GrossSalary * item.Amount / 100);
                        dedu.Deducted_Amount = am;
                        dedu.Notes = item.Amount + " % of Gross Salary";
                    }
                    PayrollDeductionList.Add(dedu);
                }

                se.AllowancSalary = PayrollAllownaceList;
                se.DeductionsSalary = PayrollDeductionList;
                se.Designation = employeeprofile.Designation.Title;
                se.Presents = 0;
                se.TotalWorkDays = 0;
                se.SalaryType = payroll.SalaryType;
                se.Absence = 0;
                se.Presents = 0;
                se.TotalWorkDays = workingdays;
                se.Overtime_day = 0;
                se.Pay_Full_Month = true;
                se.OverTime_Rate = payroll.Over_Time_Rate;
                if (payroll.SameAsBasic == true)
                {
                    se.OverTime_Rate = payroll.Basic_Salary / workingdays;
                }
                se.GrossSalary = payroll.Basic_Salary;
                se.NetSalary = se.GrossSalary + ((decimal?)se.AllowancSalary.Sum(s => s.Benifited_Amount) ?? 0);
                se.AllowancSalary = PayrollAllownaceList;
                Session["record"] = se;
                jsre = new { Status = "OK", Message = "Going to be confirmed", url = "/Salarysheets/ConfirmPayslip" };
            }
            return Json(jsre, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmPayslip()
        {
            var se = Session["record"];
            if (se == null)
            {
                return RedirectToAction(nameof(NewPayslip));
            }
            return PartialView(se);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmPayslip(Salarysheet se, decimal? deductionAmount, string Notes)
        {
            var jsmess = new { Status = "Faild", Message = "Something went wrong", url = "/salarysheet/Index" };
            decimal deduction = 0;
            if (deductionAmount != null)
            {
                deduction = deductionAmount ?? 0;
            }
            Salarysheet sen = Session["record"] as Salarysheet;
            List<AllowanceSalary> bn = Session["benefits"] as List<AllowanceSalary>;
            List<DeductionsSalary> dudu = Session["loss"] as List<DeductionsSalary>;
            Hourly_Duty h = Session["hourly"] as Hourly_Duty;
            AdvanceSalaryDeduction advanceSalaryDeduction = new AdvanceSalaryDeduction();

            //    Salarysheet se = new Salarysheet();
            //    se = sen;
            if (se != null)
            {
                se.Id = Guid.NewGuid();
                se.ToDate = sen.ToDate.Date;
                se.FromDate = sen.FromDate.Date;
                se.Hourly_Duty = h;
                se.CreatedOn = DateTime.UtcNow;
                var employee = db.Employee.Where(f => f.Id == se.EmployeeId).SingleOrDefault();
                if (employee == null)
                {
                    return Json(jsmess, JsonRequestBehavior.AllowGet);
                }
                using (var save = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (se.Pay_Full_Month == true)
                        {
                            se.Pay_Full_Month = true;
                            if (se.SalaryType == SalaryTypes.Hourly)
                            {
                                se.GrossSalary = se.BasicSalary + (se.Hourly_Duty.Overtime_hour * se.OverTime_Rate);
                            }
                            else
                            {
                                se.GrossSalary = se.BasicSalary + (se.TotalWorkDays * se.OverTime_Rate);
                            }
                        }


                        if (se.Hourly_Duty != null)
                        {
                            se.Hourly_Duty.Id = se.Id;
                        }
                        se.AdvanceSalaryDeduction = advanceSalaryDeduction;
                        se.AdvanceSalaryDeduction.Id = se.Id;
                        se.CreatedOn = DateTime.UtcNow.AddHours(6);
                        se.AdvanceSalaryDeduction.Month = se.FromDate;
                        se.AdvanceSalaryDeduction.EmployeeId = se.EmployeeId;
                        se.AdvanceSalaryDeduction.Amount = 0;
                        se.AdvanceSalaryDeduction.Notes = "";
                        if (deduction > 0)
                        {
                            se.AdvanceSalaryDeduction.Amount = deduction;
                            se.AdvanceSalaryDeduction.Notes = Notes;
                            se.NetSalary = se.GrossSalary + se.AllowancSalary.ToList().Sum(s => s.Benifited_Amount) - se.DeductionsSalary.ToList().Sum(s => s.Deducted_Amount) - deduction;
                        }

                        db.Salarysheet.Add(se);
                        db.SaveChanges();

                        foreach (AllowanceSalary item in bn)
                        {
                            item.Id = Guid.NewGuid();
                            item.SalarysheetId = se.Id;
                            db.AllowanceSalary.Add(item);
                        }
                        foreach (DeductionsSalary item in dudu)
                        {
                            item.Id = Guid.NewGuid();
                            item.SalarysheetId = se.Id;
                            db.DeductionsSalary.Add(item);
                        }


                        db.SaveChanges();

                        await db.SaveChangesAsync();
                        Session["searchrecord"] = null;
                        save.Commit();
                        jsmess = new { Status = "OK", Message = "Successfully Created", url = "/salarysheets/Index" };
                        var doc = "Created Pay slip for " + se.Employee.Full_Name + " (" + se.Employee.EmployeeCode + ")";
                        var userName = User.Identity.GetUserName();
                        ExtraFunctions.Set_Activity(doc, userName);
                        return Json(jsmess, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        save.Rollback();
                        jsmess = new { Status = "Error", Message = "Something went wrong please try again " + ex.Message, url = "/salarysheets/Index" };
                        return Json(jsmess, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            return RedirectToAction(nameof(NewPayslip));
        }

        public ActionResult Advance_Salary()
        {
            var employee = db.Employee.ToList().Select(s => new
            {
                Text = s.EmployeeCode + " " + s.Full_Name + ", F:" + s.Father_Name,
                Value = s.Id
            });
            ViewBag.EmployeeId = new SelectList(employee, "Value", "Text");
            return PartialView();

        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Advance_Salary(Advance_Salary As)
        {
            As.CreatedON = DateTime.UtcNow;
            var jsmess = new { Status = "Faild", Message = "Something went wrong please try again", url = "" };
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(f => f.Errors).Select(f => f.ErrorMessage).ToList();
                jsmess = new { Status = "Faild", Message = string.Join(", ", error), url = "" };
                return Json(jsmess, JsonRequestBehavior.AllowGet);
            }
            using (var save = db.Database.BeginTransaction())
            {
                try
                {
                    var emp = db.Employee.Where(f => f.Id == As.EmployeeId).SingleOrDefault();
                    db.Advance_Salary.Add(As);
                    var doc = "Created an advance salary to " + emp.Full_Name + " (Emp.Code " + emp.EmployeeCode + ")";
                    var userName = User.Identity.GetUserName();
                    ExtraFunctions.Set_Activity(doc, userName);
                    await db.SaveChangesAsync();
                    save.Commit();
                    ModelState.Clear();
                    jsmess = new { Status = "OK", Message = "Successfully Saved", url = "/Salarysheets/Advance_History" };
                    return Json(jsmess, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Something went wrong with " + ex;
                    save.Rollback();
                    jsmess = new { Status = "Error", Message = ex.Message, url = "" };
                    return Json(jsmess, JsonRequestBehavior.AllowGet);
                }
            }







        }
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salarysheet salarysheet = db.Salarysheet.Where(s => s.Id == id).Include(f => f.Employee).Include(f => f.AllowancSalary).Include(f => f.AdvanceSalaryDeduction).SingleOrDefault();
            if (salarysheet == null)
            {
                return HttpNotFound();
            }
            return View(salarysheet);
        }







        public ActionResult Salary_Distribution()
        {
            var record = db.Salarysheet.Where(s => s.IsApproved == true && s.IsHandover == false).Include(s => s.Employee).Include(f => f.AllowancSalary).Include(f => f.Hourly_Duty).Include(f => f.AdvanceSalaryDeduction).OrderByDescending(f => f.SalaryType).ToList();
            return View(record);
        }
        // Only Accounts can do this task

        public async Task<ActionResult> Salary_handOver(Guid id)
        {
            if (id != null)
            {
                var data = db.Salarysheet.Include(f => f.Employee).Include(f => f.AdvanceSalaryDeduction).Where(s => s.Id == id).SingleOrDefault();
                using (var save = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (data.IsHandover == false && data.IsApproved == true)
                        {
                            data.IsHandover = true;
                            data.HandOverDate = DateTime.UtcNow.AddHours(6);
                            //var CashL = db.Ledger.Where(s => s.Name == "Cash A/C").SingleOrDefault();// First time Credit // 
                            //var advsl = db.Ledger.Where(s => s.Name == "Advance Against Salary").SingleOrDefault();// First time Credit /
                            //var empid = data.EmployeeId;
                            //var Empl = db.Ledger.Where(s => s.RefNo == empid).SingleOrDefault();
                            //List<TransactionDetails> tsdl = new List<TransactionDetails>();
                            //Transaction ts = new Transaction();
                            //ts.Id = Guid.NewGuid();
                            //ts.Narration = data.Deduction_History.Notes;
                            //ts.TransactionDate = DateTime.UtcNow.AddHours(6).Date;
                            //// ts.UserId = User.Identity.GetUserId();
                            //ts.TrasactionalAmount = data.NetSalary + data.Deduction_History.Amount;
                            //ts.VoucherType = "Payment";
                            //ts.VoucherNo = ExtraFunctions.VoucherNo("Payment") + 1;
                            ////   ts.BranchID = db.Branch.Where(s => s.IsDefault == true).Select(f => f.Id).SingleOrDefault();
                            //db.Transaction.Add(ts);
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();

                            //if (data.Deduction_History.Amount > 0)
                            //{
                            //    var amount = data.Deduction_History.Amount;
                            //    tsdl.Add(new TransactionDetails()
                            //    {
                            //        Id = Guid.NewGuid(),
                            //        LedgerID = advsl.Id,
                            //        LedgerName = advsl.Name,
                            //        LedgerNo = advsl.LedgerCode.ToString(),
                            //        CreditAmount = amount,
                            //        DebitAmount = 0,
                            //    });

                            //}
                            //tsdl.Add(new TransactionDetails()
                            //{
                            //    Id = Guid.NewGuid(),
                            //    LedgerID = CashL.Id,
                            //    LedgerName = CashL.Name,
                            //    LedgerNo = CashL.LedgerCode.ToString(),
                            //    CreditAmount = data.NetSalary,
                            //    DebitAmount = 0,
                            //});
                            //tsdl.Add(new TransactionDetails()
                            //{
                            //    Id = Guid.NewGuid(),
                            //    LedgerID = Empl.Id,
                            //    LedgerName = Empl.Name,
                            //    LedgerNo = Empl.LedgerCode.ToString(),
                            //    CreditAmount = 0,
                            //    DebitAmount = data.NetSalary + data.Deduction_History.Amount,
                            //});

                            //foreach (var item in tsdl)
                            //{
                            //    item.Id = Guid.NewGuid();
                            //    // item.BranchID = ts.BranchID;
                            //    item.CreatedOn = DateTime.UtcNow.AddHours(6);
                            //    item.TransactionID = ts.Id;
                            //    item.TransactionDate = ts.TransactionDate;
                            //    item.VoucherNo = ts.VoucherNo;
                            //    item.VoucherType = ts.VoucherType;
                            //    db.TransactionDetails.Add(item);
                            //}
                            db.SaveChanges();
                            var doc = "Make a payment as salary to " + data.Employee.Full_Name + " " + data.Employee.Father_Name;
                            var userName = User.Identity.GetUserName();
                            ExtraFunctions.Set_Activity(doc, userName);
                            await db.SaveChangesAsync();
                            save.Commit();

                            return Json("Hand Over Successfull", JsonRequestBehavior.AllowGet);
                        }
                        return Json("Already Hand Over or is not approved yet", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Something went wrong with " + ex;
                        save.Rollback();

                    }
                }

            }
            return Json("something went wrong, try again or contact administrator", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Guid? id)
        {
            var result = new { Status = "Faild", Message = "Something went wrong", url = "/Salarysheets/index" };
            if (id == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Salarysheet salarysheet = db.Salarysheet.Where(s => s.Id == id).Include(f => f.AllowancSalary).Include(f => f.Hourly_Duty).Include(f => f.Employee).Include(f => f.AdvanceSalaryDeduction).SingleOrDefault();
            var name = salarysheet.Employee.Full_Name + ",  (" + salarysheet.Employee.EmployeeCode+")";

            if (salarysheet == null || salarysheet.IsApproved == true)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            int i = 0;
            var dd = salarysheet.AdvanceSalaryDeduction;
            var hr = salarysheet.Hourly_Duty;
            var bn = salarysheet.AllowancSalary.ToList();
            var dn = salarysheet.DeductionsSalary.ToList();
            if (dd != null) db.AdvanceSalaryDeductions.Remove(dd);
            if (hr != null) db.Hourly_Duty.Remove(hr);
            db.AllowanceSalary.RemoveRange(bn);
            db.DeductionsSalary.RemoveRange(dn);
            db.Salarysheet.Remove(salarysheet);

            var doc = "Cancelled a Pay slip of " + name;
            var userName = User.Identity.GetUserName();
            ExtraFunctions.Set_Activity(doc, userName);
            db.SaveChanges();
            result = new { Status = "OK", Message = "Successfully Cancelled", url = "/Salarysheets/index" };
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Approve(Guid id)
        {
            var data = db.Salarysheet.Where(s => s.Id == id).Include(f => f.AdvanceSalaryDeduction).SingleOrDefault();
            if (data.IsApproved == false)
            {
                data.Approved_Date = DateTime.UtcNow.AddHours(6);
                data.Approved_By = User.Identity.DisplayName();
                data.IsApproved = true;
                db.SaveChanges();
                string doc = "Approved Salarysheet for " + data.Employee.Full_Name + " (Emp. Code " + data.Employee.EmployeeCode + ")";
                var userName = User.Identity.GetUserName();
                ExtraFunctions.Set_Activity(doc, userName);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public ActionResult PaySlip(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salarysheet salarysheet = db.Salarysheet.Where(s => s.Id == id).Include(f => f.AllowancSalary).Include(f => f.Hourly_Duty).Include(m => m.DeductionsSalary).Include(f => f.AdvanceSalaryDeduction).SingleOrDefault();
            if (salarysheet != null)
            {
                return PartialView(salarysheet);
            }
            return PartialView();
        }


        public int CountWorkDays(DateTime fdate, DateTime tdate)
        {
            var holyda = GetHolyDaysList(fdate, tdate);
            var day = 0;
            for (DateTime date = fdate; date <= tdate; date = date.AddDays(1))
            {
                var checkisnotholydate = holyda.Where(f => f.From <= date && f.To >= date).Any();
                if (date.DayOfWeek != Settings.WeekEnd && date.DayOfWeek != Settings.ComWeekEnd && !checkisnotholydate)
                {
                    day += 1;
                }
            }
            return Convert.ToInt32(day);
        }
        public List<LeaveDateRangeVM> GetHolyDaysList(DateTime fdate, DateTime tdate)
        {
            var officialLeaves = db.HoliDays.Where(s => s.LeaveTo >= fdate).ToList();
            List<LeaveDateRangeVM> offleaveList = new List<LeaveDateRangeVM>();
            foreach (var item in officialLeaves)
            {
                LeaveDateRangeVM range = new LeaveDateRangeVM();
                range.To = item.LeaveTo;
                range.From = item.LeaveFrom;
                if (item.LeaveTo > tdate)
                {
                    range.To = tdate;
                }
                if (item.LeaveFrom < fdate)
                {
                    range.From = fdate;
                }

                offleaveList.Add(range);
            }
            return offleaveList;
        }
        public int CountHolyDays(DateTime fdate, DateTime tdate)
        {
            var holyDays = GetHolyDaysList(fdate, tdate);
            int TOL = 0;
            foreach (var item in holyDays)
            {
                // TOL += (item.To.AddDays(1) - item.From).TotalDays;
                for (DateTime index = item.From; index <= item.To; index = index.AddDays(1))
                {
                    if (index.DayOfWeek != DayOfWeek.Friday)
                    {
                        TOL += 1;
                    }
                }
            }
            return TOL;
        }
        public int CountEmployeeLeave(DateTime fdate, DateTime tdate, Guid EID)
        {
            var officialLeaves = GetHolyDaysList(fdate, tdate);
            var EmpLeaveDateRange = db.EmpLeaves.Where(s => s.EmployeeId == EID).Where(s => s.LeaveToDate >= fdate).ToList();
            List<LeaveDateRangeVM> EmployeeLeavesList = new List<LeaveDateRangeVM>();
            foreach (var item in EmpLeaveDateRange)
            {
                LeaveDateRangeVM range = new LeaveDateRangeVM();
                range.To = item.LeaveToDate;
                range.From = item.LeaveFromDate;
                if (item.LeaveToDate > tdate)
                {
                    range.To = tdate;
                }
                if (item.LeaveFromDate < fdate)
                {
                    range.From = fdate;
                }
                EmployeeLeavesList.Add(range);
            }
            int TotalEmpLeave = 0;
            foreach (var item in EmployeeLeavesList)
            {
                for (DateTime index = item.From; index <= item.To; index = index.AddDays(1))
                {
                    var isOfficeialBook = officialLeaves.Where(s => s.To >= index && s.From <= index).Any();
                    if (index.DayOfWeek != Settings.WeekEnd && index.DayOfWeek != Settings.ComWeekEnd && !isOfficeialBook)
                    {
                        TotalEmpLeave += 1;
                    }
                }
            }
            return TotalEmpLeave;

        }
        public EmployeeAttendanceViewModel GetAttendanceSummary(Guid EmployeeId, DateTime From, DateTime To)
        {
            var pay = db.Payroll.Where(f => f.EmployeeId == EmployeeId && f.IsActive == true).SingleOrDefault();
            var wd = CountWorkDays(From, To);
            var rec = db.DailyAttendance.Where(f => f.EmployeeId == EmployeeId && f.Date.Month == From.Month && f.Date.Year == From.Year).ToList().Select(f => new
            {
                Date = f.Date,
                Duty = Convert.ToDecimal((f.Out - f.Entry).TotalMinutes),
                OverTime = Convert.ToInt32((f.Out - f.Entry).TotalMinutes) - (Settings.WorkingHour * 60),
            }).ToList();
            var mo = rec.GroupBy(f => f.Date, (j, k) => new { date = j, data = k }).ToList();
            var sma = new EmployeeAttendanceViewModel()
            {
                Working_Day = wd,
                Working_Hour = wd * Settings.WorkingHour,
                DutyDay = mo.Count(),
                Duty_Hour = mo.Sum(f => f.data.Sum(s => s.Duty)) / 60,
                OverTime_Day = wd - mo.Count(),
                OverTime_Hour = mo.Sum(f => f.data.Sum(s => s.OverTime)),
            };


            return sma;
        }
        public ActionResult GetPerformanceList()
        {
            var days = Enum.GetValues(typeof(Performance))
                    .Cast<Performance>()
                    .Select(d => new { Text = d, id = (int)d })
                    .ToList();
            return Json(days, JsonRequestBehavior.AllowGet);
        }  // Json Call

        public decimal GetAdvanceSalaryReport(Guid empId)
        {
            var total = ((decimal?)db.Advance_Salary.Where(s => s.EmployeeId == empId).ToList().Sum(z => z.Amount) ?? 0);
            var paid = ((decimal?)db.AdvanceSalaryDeductions.Where(s => s.EmployeeId == empId).ToList().Sum(z => z.Amount) ?? 0);
            return total - paid;
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
