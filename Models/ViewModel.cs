using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{
    public class DashboarCounting
    {
        public string Title { get; set; }
        public int Count { get; set; }
    }
    public class PermissionViewmOdel
    {
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public bool IsSelected { get; set; }
    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email"), EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Display Name"), StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string DisplayName { get; set; }
        [Required]
        [Display(Name = "Phone Number"), Phone]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class PresentViewmodel
    {
        public string Date { get; set; }
        public int Absence { get; set; }
        public int Present { get; set; }
    }
    public class PerformanceViewModel
    {
        public Guid EmployeeId { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

    }
    public class EmployeeAttendanceViewModel
    {
        [Display(Name = "Duty Hour")]
        public decimal Duty_Hour { get; set; }
        [Display(Name = "Over Time Hour")]
        public decimal OverTime_Hour { get; set; }
        [Display(Name = "Working Hour")]
        public decimal Working_Hour { get; set; }
        [Display(Name = "Duty Day"), Range(0, 31)]
        public int DutyDay { get; set; }
        [Display(Name = "Over Duty Day"), Range(0, 31)]
        public decimal OverTime_Day { get; set; }
        [Display(Name = "Working Day"), Range(0, 31)]
        public int Working_Day { get; set; }

    }
    public class EmpResignViewModel
    {
        [Display(Name = "Resign Case"), MaxLength(50), MinLength(3), Required]
        public string Casues { get; set; }
        [Required, Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }
        [Required]
        public DateTime LeaveTime { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }

    }
    public class TransactionViewModel
    {
        [Display(Name = "Voucher Type"), Required]
        public int VoucherType { get; set; }
        [Display(Name = "Transaction Date"), DataType(DataType.Date), Required]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Narration"), DataType(DataType.MultilineText)]
        public string Narration { get; set; }

    }
    public class TransactionDetailsView
    {
        public Ledger Ledger { get; set; }
        public Guid LedgerId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }


    public class ProfitAndLoss
    {
        public string Name { get; set; }
        public decimal? DrBalance { get; set; }
        public decimal? CrBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public string Data { get; set; }
    }
    public class LedgerDetailsReport
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Vouchertype { get; set; }
        public int VoucherNo { get; set; }
        public string Voucher { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Cradit { get; set; }
        public string Balance { get; set; }
        public string Narration { get; set; }
        public Guid id { get; set; }
    }

    public class LedgerListReport
    {
        public decimal OpeningDebit { get; set; }
        public decimal OpeningCredit { get; set; }
        public int LedgerNo { get; set; }
        public string LedgerName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal ClosinngDebit { get; set; }
        public decimal ClosinngCredit { get; set; }
        public Guid ledgerID { get; set; }
        public decimal DefaultData { get; set; }
    }



}