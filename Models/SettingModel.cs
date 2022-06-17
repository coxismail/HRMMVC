using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{

    public class Other_setting
    {
        public Guid Id { get; set; }
        [Display(Name = "Minimum Approval for Requisition"), Range(0, 100)]
        public int Minimum_Approval { get; set; }
        [Display(Name="Template")]
        public int LayoutVersion { get; set; }
        [Display(Name="Payslip based on Attendance")]
        public bool RequireAttendance { get; set; }
        public int WorkingHour { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Custom Date Selection (Voucher)")]
        public bool Custom_Date_Selection { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }

        [Display(Name="Office Start Time"), DataType(DataType.Time)]
        public TimeSpan OfficeStartTime { get; set; }
        [Display(Name = "Office End Time"), DataType(DataType.Time)]
        public TimeSpan OfficeEndTime { get; set; }
        [Display(Name = "Week End (Comapny)")]
        public DayOfWeek? ComWeekEnd { get; set; }
        [Display(Name="Week End"), Required]
        public DayOfWeek WeekEnd { get; set; }
        public Other_setting()
        {
            Custom_Date_Selection = false;
            Active = true;
            ChangedOn = DateTime.UtcNow.AddHours(6);
            WorkingHour = 8;
            OfficeStartTime = new TimeSpan(5,00, 00);
            OfficeEndTime = new TimeSpan(17, 00, 00);
            WeekEnd = DayOfWeek.Friday;
        }


    }


    public class Upload_Signature
    {
        // ViewModel
        [Required, Display(Name = "User")]
        public string UserId { get; set; }
        [Display(Name = "Sign Order"), Range(0, 10000)]
        public int Sign_Order { get; set; }
        public HttpPostedFileBase ImageUpload { get; set; }
    }
    public class ActivityLogs
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public string Browser { get; set; }
        public DateTime Execution_Time { get; set; }
        public string Activity { get; set; }
    }
    public class Login_History
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Browser { get; set; }
        public string Os_Version { get; set; }
        public string Device { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public string Mac_Address { get; set; }
        public DateTime Time { get; set; }
    }




    // SMS Configuration
    public class SMSConfig
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Provider Name"), Required, DataType(DataType.Text)] public string ProviderName { get; set; }
        [Display(Name = "User Name (API Key)"), Required, DataType(DataType.Text)] public string UserName { get; set; }
        [Display(Name = "Password (API Secret)"), Required, DataType(DataType.Text)] public string Password { get; set; }
        [Display(Name = "Number"), Required, DataType(DataType.PhoneNumber)] public string Phone { get; set; }
        [Display(Name = "Sender Name"), Required, DataType(DataType.Text)] public string SenderName { get; set; }


        public bool Active { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public SMSConfig()
        {
            Active = true;
        }
    }
    // Email Configuration

    public class EmailConfig
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Protocol"), DataType(DataType.Text), Required] public string Protocol { get; set; }
        [Display(Name = "SMTP Host"), DataType(DataType.Text), Required] public string SmtpHost { get; set; }
        [Display(Name = "SMTP Port"),  Required] public int SmtpPort { get; set; }
        [Display(Name = "Sender Mail"), DataType(DataType.EmailAddress), Required] public string SenderMail { get; set; }
        [Display(Name = "Password"), DataType(DataType.Text), Required] public string Password { get; set; }
        [Display(Name = "Mail Type"), DataType(DataType.Text), Required] public string MailType { get; set; }



        public bool Active { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public EmailConfig()
        {
            Active = true;
        }
    }

    // PayRoll Sections




}