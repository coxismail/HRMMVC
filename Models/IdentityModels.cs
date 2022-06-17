using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HRMMVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("DisplayName", this.DisplayName.ToString()));
            userIdentity.AddClaim(new Claim("ProfilePicture", this.ProfilePicture?.ToString()?? "~/Assets/images/person.jpg"));
            userIdentity.AddClaim(new Claim("Lock", this.Lock.ToString(), ClaimValueTypes.Boolean));
            userIdentity.AddClaim(new Claim("IsMember", this.IsMember.ToString(), ClaimValueTypes.Boolean));
            userIdentity.AddClaim(new Claim("TimeZoneId", this.TimeZoneId?.ToString() ?? TimeZoneInfo.Local.Id));

            return userIdentity;
        }
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        public bool Lock { get; set; }
        public bool IsMember { get; set; }
        [Display(Name="Time Zone")]
        public string TimeZoneId { get; set; }
        [Display(Name="Profile Picture")]
        public string ProfilePicture { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Company> Company { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<HoliDays> HoliDays { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Employee> Employee { get; set; }

        public DbSet<Allowance> Allowance { get; set; }
        public DbSet<Deduction> Deduction { get; set; }

        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<Payroll_Allowence> Payroll_Allowence { get; set; }
        public DbSet<Payroll_Deduction> Payroll_Deduction { get; set; }
        public DbSet<PerformanceBonus> PerformanceBonus { get; set; } /// Settings

        public DbSet<Salarysheet> Salarysheet { get; set; }
        public DbSet<EmpLeave> EmpLeaves { get; set; }
       
        public DbSet<Hourly_Duty> Hourly_Duty { get; set; }

        public DbSet<AllowanceSalary> AllowanceSalary { get; set; } // payroll
        public DbSet<DeductionsSalary> DeductionsSalary { get; set; } // payroll
        public DbSet<Advance_Salary> Advance_Salary { get; set; }
        public DbSet<AdvanceSalaryDeduction> AdvanceSalaryDeductions { get; set; }

        public DbSet<BonusSheet> BonusSheet { get; set; }
        public DbSet<BonusSheetDetails> BonusSheetDetails { get; set; }




        public DbSet<AddressBook> AddressBook { get; set; }
        public DbSet<Educational_Background> Educational_Background { get; set; }
        public DbSet<JobExperience> JobExperience { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DailyAttendance> DailyAttendance { get; set; }
        public DbSet<EmpPerformance> EmpPerformance { get; set; }


        public DbSet<Circular> Circular { get; set; }
        public DbSet<Requirment> Requirment { get; set; }
        public DbSet<JobApplication> JobApplication { get; set; }




        public DbSet<ChartOfAccount> ChartOfAccount { get; set; }
        public DbSet<LedgerCategory> LedgerCategory { get; set; }
        public DbSet<Ledger> Ledger { get; set; }
        public DbSet<ChartTree> ChartTree { get; set; }



        public DbSet<Voucher> Voucher { get; set; }

        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransDetails> TransDetails { get; set; }

        public DbSet<Login_History> Login_History { get; set; }
        public DbSet<ActivityLogs> ActivityLogs { get; set; }
        public DbSet<Other_setting> Other_setting { get; set; }
        public DbSet<EmailConfig> EmailConfig { get; set; }
        public DbSet<SMSConfig> SMSConfig { get; set; }

      
    }
}