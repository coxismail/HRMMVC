using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Models
{

    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required, Display(Name = "Name")]
        public string Full_Name { get; set; }
        [Display(Name="Employee Id")]
        public int EmployeeId { get; set; }
        [ForeignKey("Shift")]
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }

        [Required, Display(Name = "Father's Name")]
        public string Father_Name { get; set; }
        [Required, Display(Name = "Mother's Name")]
        public string Mother_Name { get; set; }

        [Required, Display(Name = "Date Of Birth"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Display(Name = "Blood Group")]
        public BloodGroup BloodGroup { get; set; }
        [Display(Name = "Phone"), StringLength(20, MinimumLength = 6)]
        public string Phone { get; set; }
        [Display(Name = "Phone 1"), StringLength(20, MinimumLength = 6)]
        public string Phone1 { get; set; }
        [Required, Display(Name = "Phone 2"), StringLength(20, MinimumLength = 6)]
        public string Phone2 { get; set; }
      
        public string NationalId { get; set; }
        [Display(Name = "Employee Code")]
        public int EmployeeCode { get; set; }
        [Display(Name = "Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, Display(Name = "Designation"), ForeignKey("Designation")]
        public int DesignationId { get; set; }
        public Designation Designation { get; set; }
        [Display(Name = "Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public EmpType EmpType { get; set; }
        [Display(Name = "Joining Date"), DataType(DataType.DateTime)]
        public DateTime JoiningDate { get; set; }
        [Display(Name = "Is Leave")]
        public bool IsLeave { get; set; }
        [Display(Name = "Leaving Date & Name")]
        public DateTime? LeaveDateTime { get; set; }


        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public virtual ICollection<EmployeeNomineeInfo> EmployeeNomineeInfo { get; set; }
        public virtual ICollection<BonusSheetDetails> BonusSheetDetails { get; set; }
        public virtual ICollection<Salarysheet> Salarysheet { get; set; }
        public virtual ICollection<EmpAttendanceSummaryViewModel> EmpAttendance { get; set; }
        public virtual ICollection<EmpLeave> EmpLeave { get; set; }
        public virtual ICollection<Advance_Salary> Employee_Advance_Salary { get; set; }
        public virtual ICollection<Payroll> Payroll { get; set; }
        public virtual ICollection<Educational_Background> Educational_Background { get; set; }
        public virtual ICollection<AddressBook> AddressBook { get; set; }
        public virtual ICollection<JobExperience> JobExperience { get; set; }
        public virtual ICollection<Document> Document { get; set; }
        public virtual ICollection<DailyAttendance> DailyAttendance { get; set; }
        public virtual ICollection<EmpPerformance> EmpPerformance { get; set; }
        public Employee()
        {
            this.JoiningDate = DateTime.UtcNow;
            this.AddressBook = new HashSet<AddressBook>();
            this.Educational_Background = new HashSet<Educational_Background>();
            this.DailyAttendance = new HashSet<DailyAttendance>();
            this.JobExperience = new HashSet<JobExperience>();
            this.Document = new HashSet<Document>();
            this.ImageUrl = "~/Assets/Images/Employee/person.jpg";
            this.EmpPerformance = new HashSet<EmpPerformance>();

        }
    }


    public class PerformanceBonus 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Performance Performance { get; set; }
        public int Amount { get; set; }
        public bool IsPercent { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
    }
    public class EmpPerformance 
    {
        public Guid Id { get; set; }
        [ForeignKey("Employee"), Required]
        public Guid EmployeeId { get; set; }
        [DataType(DataType.Date), Required]
        public DateTime Month { get; set; }
        public Performance Performance { get; set; }
        public virtual Employee Employee { get; set; }
    }
    public class EmployeeType : IEnumerable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class EmployeeNomineeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required(), Display(Name = "Nominee Name")]
        public string NomineeName { get; set; }
        [Display(Name = "Nominee Details"), DataType(DataType.MultilineText)]
        public string NomineeDetails { get; set; }
        [Display(Name = "Signature"), DataType(DataType.ImageUrl)]
        public string Signature { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required, Display(Name = "Country/Region")]
        public string Country { get; set; }
        [Display(Name = "State/Province")]
        public string State { get; set; }
        [Required, Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [Display(Name = "Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [Display(Name = "Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        private DateTime _createdOn = DateTime.MinValue;
        //public DateTime CreatedOn;
        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        [NotMapped]
        public HttpPostedFileBase SignUpload { get; set; }

        public EmployeeNomineeInfo()
        {
            ImageUrl = "~/Image/EmpNominee/Image/user.png";
            Signature = "~/Image/EmpNominee/Signature/signature.png";
        }
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }
        public virtual Employee Employee { get; set; }
    }
    public class Document 
    {
        public Guid Id { get; set; }
        [Display(Name="Title"), Required, MaxLength(30, ErrorMessage ="30 letter limited")]
        public string Title { get; set; }
        [Display(Name="Url")]
        public string Url { get; set; }
        [Display(Name="File Size")]
        public string FileSize { get; set; }
        [Display(Name="Upload On")]
        public DateTime UploadOn { get; set; }
        [Display(Name = "Serial")]
        public int ReOrder { get; set; }
        public string UploadBy { get; set; }
        [NotMapped]
        public HttpPostedFileBase  FileUpload { get; set; } // allowed format pdf, jpg, png, doc, 
    }
    public class EmpAttendanceSummaryViewModel
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date), Display(Name = "Month")]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
        [Display(Name = "Duty Hour")]
        public decimal Duty_Hour { get; set; }
        [Display(Name = "Over Time")]
        public decimal Over_Time { get; set; }
        [Display(Name = "Working Hour")]
        public decimal Working_Hour { get; set; }
        [Display(Name = "Worked Day"), Range(0, 31)]
        public int Worked_Day { get; set; }
        [Display(Name = "Over Duty Day"), Range(0, 31)]
        public decimal Over_Duty_Day { get; set; }
        [Display(Name = "Working Day"), Range(0, 31)]
        public int Working_Day { get; set; }
        [ForeignKey("Employee"), Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }




    public class Educational_Background
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Exam Title")]
        public string Exam_Title { get; set; }
        [Display(Name = "Passing Year")]
        public int Passing_Year { get; set; }
        public string Result { get; set; }
        [Display(Name = "Institute/Board")]
        public string Institute_or_Board { get; set; }
        public string Roll { get; set; }
    }

    public class JobExperience
    {
        public Guid Id { get; set; }
        [Display(Name="Company / Organization"), Required]
        public string Company_Name { get; set; }
        [Display(Name="Position"), Required]
        public string Designation { get; set; }
        public string Responsiblity { get; set; }
        [Display(Name="From"), DataType(DataType.Date), Required]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date), Display(Name="End Date")]
        public DateTime? ToDate { get; set; }
        public string Notes { get; set; }
    }
    //=============================================== Employee Attendance Daily =============================================//
    public class DailyAttendance
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Entry { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Out { get; set; }
        public bool IsLeave { get; set; }
        [Display(Name = "Employee"), Required, ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        [Display(Name = "Application Reference No")]
        public string ApplicationRef { get; set; }
        public DailyAttendance()
        {
            IsLeave = false;
        }
    }

    //=============================================== Employee Attendance Daily  End =============================================//






    /*============================================================================================================================
                                           Employee's Payroll section
    ============================================================================================================================*/
    public class Payroll
    {
        public Guid Id { get; set; }
        [ForeignKey("Employee"), Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal Basic_Salary { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Required, Display(Name="Salary Mode")]
        public SalaryTypes SalaryType { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime Updated { get; set; }
        [Display(Name = "Over Time/Daily Rate"), Range(0, 10000000)]
        public decimal Over_Time_Rate { get; set; }
        [Display(Name = "Same as Basic Salary")]
        public bool SameAsBasic { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public virtual ICollection<Payroll_Allowence> Payroll_Allowance { get; set; }
        public virtual ICollection<Payroll_Deduction> Payroll_Deduction { get; set; }
        public Payroll()
        {
            Over_Time_Rate = 0;
            IsActive = true;
            SameAsBasic = false;
        }
    }




    public class Payroll_Allowence
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Allowance"), ForeignKey("Allowance")]
        public int AllowenceId { get; set; }
        public decimal Amount { get; set; }
        [Display(Name="% percent of Bsic")]
        public PecentOf? PecentOf { get; set; }
        [Display(Name = "Payroll"), ForeignKey("Payroll")]
        public Guid PayrollId { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual Allowance Allowance { get; set; }
    }
    public class Payroll_Deduction 
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Deduction"), ForeignKey("Deduction")]
        public int DeductionId { get; set; }
        public decimal Amount { get; set; }
        public PecentOf? PecentOf { get; set; }
        [Display(Name = "Payroll"), ForeignKey("Payroll")]
        public Guid PayrollId { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual Deduction Deduction { get; set; }

    }

    /*============================================================================================================================
                                            Employee's View Model
     ============================================================================================================================*/

    public class EmployeeViewModel
    {
        // No database just collect record
        [Required(), Display(Name = "Employee Code")]
        public int EmployeeCode { get; set; }
        [Required]
        public EmpType EmpType { get; set; }
        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(), Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Bussiness Phone"), StringLength(20, MinimumLength = 6)]
        public string BussinessPhone { get; set; }
        [Display(Name = "National ID"), Required]
        public string NationalId { get; set; }
        //[Display(Name="TIN Number")]
        //public string TinNumber { get; set; }

        [Display(Name = "Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required, Display(Name = "Full Name")]
        public string Full_Name { get; set; }

        [Required, Display(Name = "Father Name")]
        public string Father_Name { get; set; }

        [Required, Display(Name = "Mother Name")]
        public string Mother_Name { get; set; }

        [Required(), Display(Name = "Date Of Birth"), DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Branch Name")]
        public Guid BranchId { get; set; }
        [Required()]
        public Gender Genders { get; set; }

        [Display(Name = "Blood Group")]
        public BloodGroup BloodGroup { get; set; }

        [Display(Name = "Home Phone"), StringLength(20, MinimumLength = 6)]
        public string HomePhone { get; set; }

        [Required(), Display(Name = "Mobile Phone"), StringLength(20, MinimumLength = 6)]
        public string MobilePhone { get; set; }

        //[Required, Display(Name="Country/Region")]
        //public string Country { get; set; }

        //[Display(Name="State/Province")]
        //public string State { get; set; }

        //[Required, Display(Name="City")]
        //public string City { get; set; }

        //[Display(Name="Address Line1"), DataType(DataType.MultilineText)]
        //public string AddressLineOne { get; set; }

        //[Display(Name="Zip/Postal Code")]
        //public string ZipOrPostalCode { get; set; }

        [Display(Name = "Employee Joining Date"), DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }


        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        [NotMapped]
        public string ImageShow { get; set; }
        public EmployeeViewModel()
        {
            ImageUrl = "~/Assets/Images/person.jpg";
        }
    }
    public class PaySlips
    {
        public Guid Id { get; set; }
        public Guid EmpDepartmentId { get; set; }
        public Decimal AmountPaid { get; set; }
        public string DescriptionOfNeed { get; set; }
        [ForeignKey("Employees")]
        public Guid ReceivedBy { get; set; }
        [ForeignKey("Employees")]
        public Guid? ApprovedBy { get; set; }
        [ForeignKey("Employees")]
        public Guid? VerifiedBy { get; set; }
        private DateTime _createdOn = DateTime.MinValue;
        //public DateTime CreatedOn;
        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        public virtual Employee Employees { get; set; }
    }






    public class EmployeeImporterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime JoiningDate { get; set; }
        public Gender Genders { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public string HomePhone { get; set; }
        public string BussinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ZipOrPostalCode { get; set; }
        public string NationalId { get; set; }
        public string TinNumber { get; set; }
        public int Code { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }


    }
}
