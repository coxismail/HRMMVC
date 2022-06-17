using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{

    public enum Gender
    {
        Male, Female, Other
    }
    public enum LeavePeriod
    {
        Month = 30, [Display(Name = "Half Year")] HalfYear = 180, Year = 365
    }
    public enum SalaryTypes { Hourly, Monthly }
    public enum BloodGroup
    {
        [Display(Name = "A+")]
        APositive,
        [Display(Name = "A-")]
        ANegative,
        [Display(Name = "B+")]
        BPositive,
        [Display(Name = "B-")]
        BNegative,
        [Display(Name = "AB+")]
        AbPositive,
        [Display(Name = "AB-")]
        AbNegative,
        [Display(Name = "O+")]
        OPositive,
        [Display(Name = "O-")]
        ONegative
    }

    public enum PecentOf
    {
        [Display(Name = " % Basic Salary")] Basic = 1, [Display(Name = " % Gross Salary")] Gross = 3
    }

    public enum EmpType
    {
        [Display(Name = "Full Time")]
        FullTime,
        [Display(Name = "Part Time")]
        PartTime,
        [Display(Name = "Contractual")]
        Contractual
    }

    public enum Performance
    {
        [Display(Name = "Very Low")] VeryLow = 0, Low = 1,
        Good = 2, [Display(Name = "Very Good")] VeryGood = 3,
        Nice = 4, [Display(Name = "Very Nice")] VeryNice = 5,
        Excellent = 6, [Display(Name = "Very Excellent")] VeryExcellent = 7
    }

    public enum Amount_Type
    {
        Amount, Percent
    }

    public enum ApplicationStatus
    {
        Faild = 0, Passed = 100, Cancelled = 300, NotPublished = 600
    }
}