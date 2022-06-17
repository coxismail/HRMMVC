using HRMMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using UAParser;

namespace HRMMVC.Manager
{
    public static class ExtraFunctions
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        public static void Set_Activity(string doc, string userName)
        {
            string UserAgent = HttpContext.Current.Request.UserAgent.ToString();
            var parse = Parser.GetDefault();
            ClientInfo c = parse.Parse(UserAgent);
            string browser = c.UA.ToString();
            string system = c.OS.Family.ToString();
            string version = c.OS.Major.ToString();
            var date = DateTime.UtcNow;
            string browsing = system + " - " + version + " - " + browser;

            ActivityLogs al = new ActivityLogs();
            al.Id = Guid.NewGuid();
            al.Activity = doc;
            al.Ip = HttpContext.Current.Request.UserHostAddress;
            al.UserName = userName;
            al.Browser = browsing;
            al.Execution_Time = date;
            db.ActivityLogs.Add(al);
            db.SaveChanges();
        }
        public static void StoreError(string doc, string userName)
        {
            string UserAgent = HttpContext.Current.Request.UserAgent.ToString();
            var parse = Parser.GetDefault();
            ClientInfo c = parse.Parse(UserAgent);
            string browser = c.UA.ToString();
            string system = c.OS.Family.ToString();
            string version = c.OS.Major.ToString();
            var date = DateTime.UtcNow.AddHours(6);
            string browsing = system + " - " + version + " - " + browser;

            ActivityLogs al = new ActivityLogs();
            al.Id = Guid.NewGuid();
            al.Activity = doc;
            al.Ip = HttpContext.Current.Request.UserHostAddress;
            al.UserName = userName;
            al.Browser = browsing;
            al.Execution_Time = date;
            db.ActivityLogs.Add(al);
            db.SaveChanges();
        }
        public static void Set_Login_Details(string userid, string userName)
        {
            string UserAgent = HttpContext.Current.Request.UserAgent.ToString();
            var parse = Parser.GetDefault();
            ClientInfo c = parse.Parse(UserAgent);
            string browser = c.UA.ToString();
            string system = c.OS.Family.ToString();
            string version = c.OS.Major.ToString();
            var date = DateTime.UtcNow.AddHours(6);

            Login_History hl = new Login_History();
            hl.Id = Guid.NewGuid();
            hl.Ip = HttpContext.Current.Request.UserHostAddress;
            hl.Browser = browser;
            hl.Os_Version = version;
            hl.UserId = userid;
            hl.Device = system;
            hl.UserName = userName;
            hl.Time = date;
            db.Login_History.Add(hl);
            db.SaveChanges();

        }

        public static bool isNotNewDevice(string userid)
        {
            var ip = HttpContext.Current.Request.UserHostAddress;

            string UserAgent = HttpContext.Current.Request.UserAgent.ToString();
            var parse = Parser.GetDefault();
            ClientInfo c = parse.Parse(UserAgent);
            string browser = c.UA.ToString();
            return db.Login_History.Where(s => s.UserId == userid && s.Ip == ip && s.Browser == browser).Any();
        }


        // Setup Accounting
        public static void SetDefaultAccounting()
        {
            var check = db.ChartOfAccount.ToList();
            if (check.Count == 0)
            {
                List<ChartTree> Charttree = new List<ChartTree>();
                var root = Guid.NewGuid().ToString();
                Charttree.Add(new ChartTree()
                {
                    Id = root,
                    Parent = "#",
                    Type = "root",
                    Text = "Accounts",
                });
                List<ChartOfAccount> ch = new List<ChartOfAccount>()
                {
                new ChartOfAccount() {  Name = "Assets" },
                new ChartOfAccount() {  Name = "Liabilities" },
                new ChartOfAccount() {  Name = "Income" },
                new ChartOfAccount() {  Name = "Capital" },
                new ChartOfAccount() {  Name = "Expenses" }
                };

                foreach (var item1 in ch)
                {
                    item1.Id = Guid.NewGuid();
                    db.ChartOfAccount.Add(item1);
                    db.SaveChanges();
                    var tree = new ChartTree()
                    {
                        Id = item1.Id.ToString(),
                        Text = item1.Name,
                        Type = "chart",
                        Parent = root,
                    };
                    Charttree.Add(tree);
                }
                var lcatId = ch.Where(x => x.Name == "Assets").Select(s => s.Id).SingleOrDefault();
                var lcatId2 = ch.Where(x => x.Name == "Liabilities").Select(s => s.Id).SingleOrDefault();
                var lcatId3 = ch.Where(x => x.Name == "Income").Select(s => s.Id).SingleOrDefault();
                var lcatId4 = ch.Where(x => x.Name == "Capital").Select(s => s.Id).SingleOrDefault();
                var lcatId5 = ch.Where(x => x.Name == "Expenses").Select(s => s.Id).SingleOrDefault();


                List<LedgerCategory> SubCat1 = new List<LedgerCategory>()
                {
                    new LedgerCategory() { Name = "Purchase Accounts", ChartOfAccountId = lcatId5 },
                    new LedgerCategory() { Name = "Indirect Expense",ChartOfAccountId = lcatId5 },
                    new LedgerCategory() {  Name = "Direct Expense",ChartOfAccountId = lcatId5 },

                    new LedgerCategory() { Name = "Current Liabilities", ChartOfAccountId = lcatId2 },
                    new LedgerCategory() { Name = "Loan Accounts",ChartOfAccountId = lcatId2 },
                    new LedgerCategory() { Name = "Sales Representative",ChartOfAccountId = lcatId2 },

                    new LedgerCategory() { Name = "Current Assets", ChartOfAccountId = lcatId },
                    new LedgerCategory() { Name = "Fixed Assets",ChartOfAccountId = lcatId },

                    new LedgerCategory() {  Name = "Paid Up Capital",ChartOfAccountId = lcatId4 },
                    new LedgerCategory() {  Name = "Owner's Equity",ChartOfAccountId = lcatId4 },

                    new LedgerCategory() {  Name = "Sales Accounts",ChartOfAccountId = lcatId3 },
                    new LedgerCategory() {  Name = "Indirect Income",ChartOfAccountId = lcatId3 },
                };

                foreach (var item0 in SubCat1)
                {
                    item0.Id = Guid.NewGuid();
                    db.LedgerCategory.Add(item0);
                    db.SaveChanges();

                    var tree = new ChartTree()
                    {
                        Id = item0.Id.ToString(),
                        Text = item0.Name,
                        Type = "cat",
                        Parent = item0.ChartOfAccountId.ToString(),
                    };
                    Charttree.Add(tree);
                }

                var lgCat1 = SubCat1.Where(x => x.Name == "Purchase Accounts").Select(s => s.Id).SingleOrDefault();
                var lgCat2 = SubCat1.Where(x => x.Name == "Current Liabilities").Select(s => s.Id).SingleOrDefault();
                var lgCat3 = SubCat1.Where(x => x.Name == "Current Assets").Select(s => s.Id).SingleOrDefault();
                var lgCat4 = SubCat1.Where(x => x.Name == "Indirect Expense").Select(s => s.Id).SingleOrDefault();
                var lgCat5 = SubCat1.Where(x => x.Name == "Fixed Assets").Select(s => s.Id).SingleOrDefault();
               
                
                List<LedgerCategory> lcat = new List<LedgerCategory>()
            {
                new LedgerCategory() {  Name = "Cash A/C", ChartOfAccountId = lcatId  ,ParentCategoryId = lgCat3},
                new LedgerCategory() {  Name = "Bank A/C", ChartOfAccountId = lcatId  ,ParentCategoryId = lgCat3},
                new LedgerCategory() {  Name = "Advance & Deposit", ChartOfAccountId = lcatId ,ParentCategoryId = lgCat3},

                new LedgerCategory() {  Name = "Accounts Receivable", ChartOfAccountId = lcatId  ,ParentCategoryId = lgCat3},
                new LedgerCategory() {  Name = "Accounts Receivable (Customer)", ChartOfAccountId = lcatId  ,ParentCategoryId = lgCat3},

                new LedgerCategory() {  Name = "Accounts Payable", ChartOfAccountId = lcatId2, ParentCategoryId = lgCat2},
                new LedgerCategory() {  Name = "Accounts Payable (Supplier)", ChartOfAccountId = lcatId2 ,ParentCategoryId = lgCat2},

                new LedgerCategory() {  Name = "Discount", ChartOfAccountId = lcatId5 ,ParentCategoryId = lgCat4 },
                new LedgerCategory() {  Name = "Staff Salary", ChartOfAccountId = lcatId5  ,ParentCategoryId = lgCat4},

            };

                foreach (var item2 in lcat)
                {
                    item2.Id = Guid.NewGuid();
                    db.LedgerCategory.Add(item2);
                    db.SaveChanges();
                    var tree = new ChartTree()
                    {
                        Id = item2.Id.ToString(),
                        Text = item2.Name,
                        Type = "cat",
                        Parent = item2.ParentCategoryId.ToString(),
                    };
                    Charttree.Add(tree);
                }
                foreach (var item in Charttree)
                {
                    db.ChartTree.Add(item);
                }

                db.SaveChanges();
            }
        }
        public static void CreateDefults() // Custom for defult
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string[] roleData = {
                "System Admin", "Employee", "Managment",
            };
            foreach (var items in roleData)
            {
                var role = new IdentityRole();
                if (!roleManager.RoleExists(items))
                {
                    role.Id = Guid.NewGuid().ToString();
                    role.Name = items;
                    roleManager.Create(role);
                }
            }
            var user = new ApplicationUser();
            if (UserManager.FindByName("chandrimsoft24@gmail.com") == null)
            {
                user.UserName = "coxismail.bd@gmail.com";
                user.Email = "coxismail.bd@gmail.com";
                user.Lock = false;
                user.EmailConfirmed = true;
                user.IsMember = false;
                user.DisplayName = "Mohammad Ismail";
                string userPWD = "Aa@12345";
                var chkUser = UserManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "System Admin");
                }
            }
        }
    }

    public partial class Settings
    {
        public static bool Custom_Date_Selection { get; set; }
        public static  int LayoutVersion { get; set; }
        public static bool RequireAttendance { get; set; }
        public static int WorkingHour { get; set; }
        public static TimeSpan OfficeStart { get; set; }
        public static TimeSpan OfficeEnd { get; set; }
        public static DayOfWeek WeekEnd { get; set; }
        public static DayOfWeek ComWeekEnd { get; set; }
        public static bool initialized;
        public static  Company Company { get; set; }
        /// Constructor
        public Settings()
        {
            initialized = false;
        }

        /// Initializes the class
        public void Init()
        {
            if (!initialized)
                RetrieveAllSettings();
            initialized = true;
        }

        /// Loads all the settings from the db
        private void RetrieveAllSettings()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var settingval = db.Other_setting.Where(s => s.Active == true).OrderByDescending(f => f.ChangedOn).SingleOrDefault() ?? new Other_setting();
            var com = db.Company.FirstOrDefault() ?? new Company();
            Company = com;
            if (settingval != null)
            {
                Custom_Date_Selection = settingval.Custom_Date_Selection;
                LayoutVersion = settingval.LayoutVersion;
                OfficeStart = settingval.OfficeStartTime;
                OfficeEnd = settingval.OfficeEndTime;
                WeekEnd = settingval.WeekEnd;
                ComWeekEnd = settingval.ComWeekEnd ?? WeekEnd;
                RequireAttendance = settingval.RequireAttendance;
                WorkingHour = settingval.WorkingHour;
            }

        }


    }
    public class EmailTemplate
    {
        public static string ConfrimEmail()
        {
            string path = "~/Assets/Templates/ConfirmEmail.html";
            var ppath = HttpContext.Current.Server.MapPath(path);
            using (StreamReader SourceReader = System.IO.File.OpenText(ppath))
            {
                return SourceReader.ReadToEnd();
            }
        }


        public static string ForgetPassword()
        {
            string path = "~/Assets/Templates/ForgetPassword.html";
            var ppath = HttpContext.Current.Server.MapPath(path);
            using (StreamReader SourceReader = System.IO.File.OpenText(ppath))
            {
                return SourceReader.ReadToEnd();
            }
        }

    }
}