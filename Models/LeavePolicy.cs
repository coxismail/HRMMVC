using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMMVC.Models
{

    public class LeaveType 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Leave title is required"), MinLength(5), MaxLength(55)]
        public string Title { get; set; }
        [Display(Name="Employee Credit (Days)"), Range(0, 100)]
        public int EmployeeCredit { get; set; }
        [Display(Name="Emloyee will credit within")]
        public LeavePeriod LeavePeriod { get; set; }
        public virtual ICollection<EmpLeave> Empleave { get; set; }
        public LeaveType()
        {
            this.Empleave = new HashSet<EmpLeave>();
        }
    }

    public class HoliDays
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, DataType(DataType.Date), Display(Name="Leave From")]
        public DateTime LeaveFrom { get; set; }
        [Required, DataType(DataType.Date), Display(Name="Leave To (Including)")]
        public DateTime LeaveTo { get; set; }
        [Required]
        public string Title { get; set; }
        public int TotalDays { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class EmpLeave
    {
        public Guid Id { get; set; }
        [Required, Display(Name = "Leave Type"), ForeignKey("LeaveType")]
        public int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        [DataType(DataType.Date), Required, Display(Name = "Leave From")]
        public DateTime LeaveFromDate { get; set; }
        [DataType(DataType.Date), Required, Display(Name = "Leave To")]
        public DateTime LeaveToDate { get; set; }
        [Display(Name = "Application Ref")]
        public string ApplicationRef { get; set; }
        [Display(Name = "Employee"), ForeignKey("Employees")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employees { get; set; }
        public string UserId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        public EmpLeave()
        {
            CreatedOn = DateTime.Now;
        }


    }

    // No Database
    public class LeaveDateRangeVM
    {
        
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public LeaveType leaveType { get; set; }
        public int days { get; set; }
    }
}