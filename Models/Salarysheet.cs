using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMMVC.Models
{
    public class Salarysheet
    {
        public Guid Id { get; set; }
        [Required, DisplayName("Employee Name"), ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        [Display(Name="Designation")]
        public string Designation { get; set; }
        [DisplayName("Salary From"), DataType(DataType.Date)]
        public DateTime FromDate { get; set; }
        [DisplayName("Salary To"), DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
        public SalaryTypes SalaryType { get; set; }
        [Range(0, 31, ErrorMessage = "Please enter a value as 1-31")]
        public int? Absence { get; set; }
        [Range(0, 31, ErrorMessage = "Please enter a value as 1-31")]
        public int Presents { get; set; }
        [Range(0, 31, ErrorMessage = "Please enter a value as 1-31"), DisplayName("Office Holiday")]
        public int Office_Holiday { get; set; }
        [Range(0, 31, ErrorMessage = "Please enter a value as 1-31"), DisplayName("Paid Leave")]
        public int Paid_Leave { get; set; }
        [Range(0, 31, ErrorMessage = "Please enter a value as 1-31"), DisplayName("Unpaid Leave")]
        public int UnPaid_Leave { get; set; }
        [Display(Name = "Total leave (Day)")]
        public int? TotalLeave { get { return (Convert.ToInt32(Paid_Leave) + Convert.ToInt32(UnPaid_Leave) + Convert.ToInt32(Office_Holiday)); } }
        [Display(Name = "Pay For (Days)")]
        public int PayForDays { get { return (Convert.ToInt32(Paid_Leave) + Presents + Convert.ToInt32(Office_Holiday)); } }
       
        [DisplayName("Working Days"), Range(0, 31)]
        public int TotalWorkDays { get; set; }
        public decimal Overtime_day { get; set; }
        [Display(Name="Overtime Rate"), Range(0, 10000000)]
        public decimal OverTime_Rate { get; set; }
        [DisplayName("Pay Full Month")]
        public bool Pay_Full_Month { get; set; }
        [DisplayName("Basic Salary"), Range(0, 100000000)]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Gross Salary"), Range(0, 100000000)]
        public decimal GrossSalary  { get; set; }
        [Range(0, 100000000)]
        public decimal Loan { get; set; }
        [Range(0, 100000000)]
        public decimal Advance { get; set; }

        [Display(Name = "Net Salary"), Range(0, 100000000)]
        public decimal NetSalary   { get; set; }
        public bool IsApproved { get; set; }
        [Display(Name = "Approved By")]
        public string Approved_By { get; set; }
        [Display(Name = "Approved Date")]
        public DateTime? Approved_Date { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Employee Employee { get; set; }

        public Hourly_Duty Hourly_Duty { get; set; }
        public virtual ICollection<AllowanceSalary> AllowancSalary { get; set; }
        public virtual ICollection<DeductionsSalary> DeductionsSalary { get; set; }
        public AdvanceSalaryDeduction AdvanceSalaryDeduction { get; set; }
        public bool IsHandover { get; set; }
        public DateTime? HandOverDate { get; set; }
        public Salarysheet()
        {
            IsApproved = false;
            IsHandover = false;
            Pay_Full_Month = false;
        }

    }

    public class Hourly_Duty
    {
        [Key, ForeignKey("Salarysheet")]
        public Guid Id { get; set; }
        [DisplayName("Overtime Hour")]
        public decimal Overtime_hour { get; set; }
        [DisplayName("Hourly Rate")]
        public decimal RatePerHour { get; set; }
        [Display(Name = "Present (Hour)")]
        public decimal PayforHour { get; set; }
        [Display(Name = "Total Working (Hour)")]
        public decimal Total_Working_Hour { get; set; }
        public virtual Salarysheet Salarysheet { get; set; }
    }

    public class AllowanceSalary
    {
        public Guid Id { get; set; }
        [Display(Name="Benifited On"), MaxLength(55), MinLength(2)]
        public string Benifited_Field { get; set; }
        [Display(Name = "Benifited Amount"), Range(1, 100000000)]
        public decimal Benifited_Amount { get; set; }
        public string Notes { get; set; }
        [ForeignKey("Salarysheet"), Required, Display(Name="Salary Sheet")]
        public Guid SalarysheetId { get; set; }
        public virtual Salarysheet Salarysheet { get; set; }
    }
    public class DeductionsSalary
    {
        public Guid Id { get; set; }
        [Display(Name = "Deducted For"), MaxLength(55), MinLength(2)]
        public string Deduction_Field { get; set; }
        [Display(Name = "Deducted Amount"), Range(1, 100000000)]
        public decimal Deducted_Amount { get; set; }
        public string Notes { get; set; }
        [ForeignKey("Salarysheet"), Required, Display(Name = "Salary Sheet")]
        public Guid SalarysheetId { get; set; }
        public virtual Salarysheet Salarysheet { get; set; }
    }

    public class BonusSheet
    {
        public Guid Id { get; set; }
        [Range(1, 100), Required]
        public int Percentage { get; set; }
        [MaxLength(50), MinLength(5), Required]
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name="Distribution Date"), DataType(DataType.Date), Required]
        public DateTime Distribution { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime? Confirm_Date { get; set; }
        public string Confirm_By { get; set; }
        public decimal Total_Bouns { get; set; }
        public virtual ICollection <BonusSheetDetails> BonusSheetDetails { get; set; }
    }
    public class BonusSheetDetails
    {
        public Guid Id { get; set; }
        [ForeignKey("Employee"), Display(Name="Employee")]
        public Guid EmployeeId { get; set; }
        [Display(Name = "Employee")]
        public decimal  Basic_Salary { get; set; }
        [ForeignKey("BonusSheet"), Display(Name = "Bonus Title")]
        public Guid BonuSheetId { get; set; }
        public decimal Net_Bonus { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? HandOver_Date { get; set; }
        public string Hand_Over_By { get; set; }
        public string Comments { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual BonusSheet BonusSheet { get; set; }

    }
 
    
    public class Advance_Salary
    {
        public Guid Id { get; set; }
        [ForeignKey("Employee"), Required]
        public Guid EmployeeId { get; set; }
        [Range(1, 1000000), Required]
        public decimal Amount { get; set; }
        [DataType(DataType.Date), Required]
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime CreatedON { get; set; }
    }
    public class AdvanceSalaryDeduction
    {
        [Key, ForeignKey("Salarysheet")]
        public Guid Id { get; set; }
        [Range(0, 1000000000), Display(Name = "Deduction Amount")]
        public decimal Amount { get; set; }
        [DataType(DataType.Date), Display(Name = "Deducted Month")]
        public DateTime Month { get; set; }
        [Display(Name = "Deduction Notes")]
        public string Notes { get; set; }
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Salarysheet Salarysheet { get; set; }
    }





    public class Allowance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Allowence"), MinLength(5), MaxLength(25), Required]
        public string Title { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Deactivation Date")]
        public DateTime? Deactivation_Date { get; set; }
        public virtual ICollection<Payroll_Allowence> Payroll_Allowence { get; set; }
        public Allowance()
        {
            IsActive = true;
        }
    }

    public class Deduction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Deduction"), MinLength(5), MaxLength(25), Required]
        public string Title { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Deactivation Date")]
        public DateTime? Deactivation_Date { get; set; }
        public virtual ICollection<Payroll_Deduction> Payroll_Deduction { get; set; }
        public Deduction()
        {
            IsActive = true;
        }
    }






    /*============================================================================================================================
                                         Salrysheet's View Model
    ============================================================================================================================*/

    public class SalarySheetViewModel 
    {
        public DateTime Month { get; set; }
        public IEnumerable<Salarysheet_property> Salarysheet_property { get; set; }
    }
    public class Salarysheet_property
    {
        public Guid Id { get; set; }
        public string Full_Name { get; set; }
        public string Designation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public SalaryTypes SalaryType { get; set; }

        public int Absence { get; set; }

        public int Presents { get; set; }

        public int Office_Holiday { get; set; }

        public int Paid_Leave { get; set; }

        public int UnPaid_Leave { get; set; }

        public int? TotalLeave { get; set; }
        public int PayForDays { get; set; } 
        public int TotalWorkDays { get; set; }
        public decimal Rate_Per_Hour { get; set; }
        public decimal OverTime_Hour { get; set; }
        public decimal PayForHour { get; set; }
        public decimal Total_Working_Hour { get; set; }
        public bool Pay_Full_Month { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Loan { get; set; }
        public decimal Advance { get; set; }
        public decimal Deduction { get; set; }
        public decimal Bonus { get; set; }
        public decimal NetSalary { get; set; }
        public string Approved_By { get; set; }
        public bool IshandOver { get; set; }
        public DateTime? HandOverDate { get; set; }
    }



   
}