namespace HRMMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        Ip = c.String(),
                        Browser = c.String(),
                        Execution_Time = c.DateTime(nullable: false),
                        Activity = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AddressBooks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Type = c.String(),
                        Division = c.String(),
                        District = c.String(),
                        Police_Station = c.String(),
                        Postal_Code = c.String(),
                        Street_Address = c.String(),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.Advance_Salary",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Notes = c.String(),
                        CreatedON = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Full_Name = c.String(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        ShiftId = c.Int(nullable: false),
                        Father_Name = c.String(nullable: false),
                        Mother_Name = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        BloodGroup = c.Int(nullable: false),
                        Phone = c.String(maxLength: 20),
                        Phone1 = c.String(maxLength: 20),
                        Phone2 = c.String(nullable: false, maxLength: 20),
                        NationalId = c.String(),
                        EmployeeCode = c.Int(nullable: false),
                        Email = c.String(),
                        DesignationId = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Notes = c.String(),
                        EmpType = c.Int(nullable: false),
                        JoiningDate = c.DateTime(nullable: false),
                        IsLeave = c.Boolean(nullable: false),
                        LeaveDateTime = c.DateTime(),
                        Department_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .ForeignKey("dbo.Designations", t => t.DesignationId, cascadeDelete: true)
                .ForeignKey("dbo.Shifts", t => t.ShiftId, cascadeDelete: true)
                .Index(t => t.ShiftId)
                .Index(t => t.DesignationId)
                .Index(t => t.Department_Id);
            
            CreateTable(
                "dbo.BonusSheetDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        Basic_Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonuSheetId = c.Guid(nullable: false),
                        Net_Bonus = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPaid = c.Boolean(nullable: false),
                        HandOver_Date = c.DateTime(),
                        Hand_Over_By = c.String(),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BonusSheets", t => t.BonuSheetId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.BonuSheetId);
            
            CreateTable(
                "dbo.BonusSheets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Percentage = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        CreatedOn = c.DateTime(nullable: false),
                        Distribution = c.DateTime(nullable: false),
                        IsConfirm = c.Boolean(nullable: false),
                        Confirm_Date = c.DateTime(),
                        Confirm_By = c.String(),
                        Total_Bouns = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DailyAttendances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Entry = c.Time(nullable: false, precision: 7),
                        Out = c.Time(nullable: false, precision: 7),
                        IsLeave = c.Boolean(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        ApplicationRef = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Designations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 55),
                        Description = c.String(),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 55),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 30),
                        Url = c.String(),
                        FileSize = c.String(),
                        UploadOn = c.DateTime(nullable: false),
                        ReOrder = c.Int(nullable: false),
                        UploadBy = c.String(),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.Educational_Background",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Exam_Title = c.String(),
                        Passing_Year = c.Int(nullable: false),
                        Result = c.String(),
                        Institute_or_Board = c.String(),
                        Roll = c.String(),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.EmpAttendanceSummaryViewModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        Duty_Hour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Over_Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Working_Hour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Worked_Day = c.Int(nullable: false),
                        Over_Duty_Day = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Working_Day = c.Int(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.EmpLeaves",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LeaveTypeId = c.Int(nullable: false),
                        LeaveFromDate = c.DateTime(nullable: false),
                        LeaveToDate = c.DateTime(nullable: false),
                        ApplicationRef = c.String(),
                        EmployeeId = c.Guid(nullable: false),
                        UserId = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.LeaveTypes", t => t.LeaveTypeId, cascadeDelete: true)
                .Index(t => t.LeaveTypeId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.LeaveTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 55),
                        EmployeeCredit = c.Int(nullable: false),
                        LeavePeriod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmployeeNomineeInfoes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        NomineeName = c.String(nullable: false),
                        NomineeDetails = c.String(),
                        Signature = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Country = c.String(nullable: false),
                        State = c.String(),
                        City = c.String(nullable: false),
                        ImageUrl = c.String(),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        EmpId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpId, cascadeDelete: true)
                .Index(t => t.EmpId);
            
            CreateTable(
                "dbo.EmpPerformances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        Month = c.DateTime(nullable: false),
                        Performance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.JobExperiences",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Company_Name = c.String(nullable: false),
                        Designation = c.String(nullable: false),
                        Responsiblity = c.String(),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(),
                        Notes = c.String(),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.Payrolls",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        Basic_Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        SalaryType = c.Int(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Over_Time_Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SameAsBasic = c.Boolean(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Payroll_Allowence",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AllowenceId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PecentOf = c.Int(),
                        PayrollId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Allowances", t => t.AllowenceId, cascadeDelete: true)
                .ForeignKey("dbo.Payrolls", t => t.PayrollId, cascadeDelete: true)
                .Index(t => t.AllowenceId)
                .Index(t => t.PayrollId);
            
            CreateTable(
                "dbo.Allowances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 25),
                        IsActive = c.Boolean(nullable: false),
                        Deactivation_Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payroll_Deduction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DeductionId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PecentOf = c.Int(),
                        PayrollId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Deductions", t => t.DeductionId, cascadeDelete: true)
                .ForeignKey("dbo.Payrolls", t => t.PayrollId, cascadeDelete: true)
                .Index(t => t.DeductionId)
                .Index(t => t.PayrollId);
            
            CreateTable(
                "dbo.Deductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 25),
                        IsActive = c.Boolean(nullable: false),
                        Deactivation_Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Salarysheets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        Designation = c.String(),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        SalaryType = c.Int(nullable: false),
                        Absence = c.Int(),
                        Presents = c.Int(nullable: false),
                        Office_Holiday = c.Int(nullable: false),
                        Paid_Leave = c.Int(nullable: false),
                        UnPaid_Leave = c.Int(nullable: false),
                        TotalWorkDays = c.Int(nullable: false),
                        Overtime_day = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverTime_Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pay_Full_Month = c.Boolean(nullable: false),
                        BasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrossSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Advance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        Approved_By = c.String(),
                        Approved_Date = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        IsHandover = c.Boolean(nullable: false),
                        HandOverDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.AdvanceSalaryDeductions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Month = c.DateTime(nullable: false),
                        Notes = c.String(),
                        EmployeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Salarysheets", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.AllowanceSalaries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Benifited_Field = c.String(maxLength: 55),
                        Benifited_Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        SalarysheetId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Salarysheets", t => t.SalarysheetId, cascadeDelete: true)
                .Index(t => t.SalarysheetId);
            
            CreateTable(
                "dbo.DeductionsSalaries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Deduction_Field = c.String(maxLength: 55),
                        Deducted_Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        SalarysheetId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Salarysheets", t => t.SalarysheetId, cascadeDelete: true)
                .Index(t => t.SalarysheetId);
            
            CreateTable(
                "dbo.Hourly_Duty",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Overtime_hour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RatePerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayforHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total_Working_Hour = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Salarysheets", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 55),
                        Start = c.Time(nullable: false, precision: 7),
                        End = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChartOfAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChartTrees",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Text = c.String(),
                        Type = c.String(),
                        Parent = c.String(),
                        icon = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Circulars",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Deadline = c.DateTime(nullable: false),
                        StartFrom = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobApplications",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CircularId = c.Guid(nullable: false),
                        ApplicationTime = c.DateTime(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        Key = c.String(),
                        Full_Name = c.String(nullable: false, maxLength: 55),
                        Email = c.String(nullable: false),
                        Mobile = c.String(nullable: false),
                        Preliminary = c.Int(),
                        Viva = c.Int(),
                        IsClosed = c.Boolean(nullable: false),
                        ResumeUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circulars", t => t.CircularId, cascadeDelete: true)
                .Index(t => t.CircularId);
            
            CreateTable(
                "dbo.Requirments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Circular_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circulars", t => t.Circular_Id)
                .Index(t => t.Circular_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Website = c.String(),
                        Fax = c.String(),
                        Address = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailConfigs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Protocol = c.String(nullable: false),
                        SmtpHost = c.String(nullable: false),
                        SmtpPort = c.Int(nullable: false),
                        SenderMail = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        MailType = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HoliDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveFrom = c.DateTime(nullable: false),
                        LeaveTo = c.DateTime(nullable: false),
                        Title = c.String(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ledgers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 55),
                        Code = c.Int(nullable: false),
                        LedgerCategoryId = c.Guid(nullable: false),
                        Email = c.String(),
                        Phone = c.String(),
                        RefNo = c.Guid(),
                        Ref_Type = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LedgerCategories", t => t.LedgerCategoryId, cascadeDelete: true)
                .Index(t => t.LedgerCategoryId);
            
            CreateTable(
                "dbo.LedgerCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ChartOfAccountId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 55),
                        ParentCategoryId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartOfAccounts", t => t.ChartOfAccountId, cascadeDelete: true)
                .Index(t => t.ChartOfAccountId);
            
            CreateTable(
                "dbo.Login_History",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(),
                        Browser = c.String(),
                        Os_Version = c.String(),
                        Device = c.String(),
                        UserName = c.String(),
                        Ip = c.String(),
                        Mac_Address = c.String(),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Other_setting",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Minimum_Approval = c.Int(nullable: false),
                        LayoutVersion = c.Int(nullable: false),
                        RequireAttendance = c.Boolean(nullable: false),
                        WorkingHour = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Custom_Date_Selection = c.Boolean(nullable: false),
                        ChangedBy = c.String(),
                        ChangedOn = c.DateTime(nullable: false),
                        OfficeStartTime = c.Time(nullable: false, precision: 7),
                        OfficeEndTime = c.Time(nullable: false, precision: 7),
                        ComWeekEnd = c.Int(),
                        WeekEnd = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PerformanceBonus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Performance = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        IsPercent = c.Boolean(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SMSConfigs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProviderName = c.String(nullable: false),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        SenderName = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Narration = c.String(),
                        VoucherType = c.String(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        EntryTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LedgerId = c.Guid(nullable: false),
                        LedgerCode = c.Int(nullable: false),
                        LedgerName = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VoucherNo = c.Int(nullable: false),
                        VoucherType = c.String(),
                        TransDate = c.DateTime(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.LedgerId)
                .Index(t => t.TransactionId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                        Lock = c.Boolean(nullable: false),
                        IsMember = c.Boolean(nullable: false),
                        TimeZoneId = c.String(),
                        ProfilePicture = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 15),
                        VoucherNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransDetails", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.TransDetails", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Ledgers", "LedgerCategoryId", "dbo.LedgerCategories");
            DropForeignKey("dbo.LedgerCategories", "ChartOfAccountId", "dbo.ChartOfAccounts");
            DropForeignKey("dbo.Requirments", "Circular_Id", "dbo.Circulars");
            DropForeignKey("dbo.JobApplications", "CircularId", "dbo.Circulars");
            DropForeignKey("dbo.Advance_Salary", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.Hourly_Duty", "Id", "dbo.Salarysheets");
            DropForeignKey("dbo.Salarysheets", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.DeductionsSalaries", "SalarysheetId", "dbo.Salarysheets");
            DropForeignKey("dbo.AllowanceSalaries", "SalarysheetId", "dbo.Salarysheets");
            DropForeignKey("dbo.AdvanceSalaryDeductions", "Id", "dbo.Salarysheets");
            DropForeignKey("dbo.AdvanceSalaryDeductions", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Payroll_Deduction", "PayrollId", "dbo.Payrolls");
            DropForeignKey("dbo.Payroll_Deduction", "DeductionId", "dbo.Deductions");
            DropForeignKey("dbo.Payroll_Allowence", "PayrollId", "dbo.Payrolls");
            DropForeignKey("dbo.Payroll_Allowence", "AllowenceId", "dbo.Allowances");
            DropForeignKey("dbo.Payrolls", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.JobExperiences", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.EmpPerformances", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeNomineeInfoes", "EmpId", "dbo.Employees");
            DropForeignKey("dbo.EmpLeaves", "LeaveTypeId", "dbo.LeaveTypes");
            DropForeignKey("dbo.EmpLeaves", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmpAttendanceSummaryViewModels", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Educational_Background", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.Documents", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.Employees", "DesignationId", "dbo.Designations");
            DropForeignKey("dbo.Designations", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Employees", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.DailyAttendances", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.BonusSheetDetails", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.BonusSheetDetails", "BonuSheetId", "dbo.BonusSheets");
            DropForeignKey("dbo.AddressBooks", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TransDetails", new[] { "TransactionId" });
            DropIndex("dbo.TransDetails", new[] { "LedgerId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LedgerCategories", new[] { "ChartOfAccountId" });
            DropIndex("dbo.Ledgers", new[] { "LedgerCategoryId" });
            DropIndex("dbo.Requirments", new[] { "Circular_Id" });
            DropIndex("dbo.JobApplications", new[] { "CircularId" });
            DropIndex("dbo.Hourly_Duty", new[] { "Id" });
            DropIndex("dbo.DeductionsSalaries", new[] { "SalarysheetId" });
            DropIndex("dbo.AllowanceSalaries", new[] { "SalarysheetId" });
            DropIndex("dbo.AdvanceSalaryDeductions", new[] { "EmployeeId" });
            DropIndex("dbo.AdvanceSalaryDeductions", new[] { "Id" });
            DropIndex("dbo.Salarysheets", new[] { "EmployeeId" });
            DropIndex("dbo.Payroll_Deduction", new[] { "PayrollId" });
            DropIndex("dbo.Payroll_Deduction", new[] { "DeductionId" });
            DropIndex("dbo.Payroll_Allowence", new[] { "PayrollId" });
            DropIndex("dbo.Payroll_Allowence", new[] { "AllowenceId" });
            DropIndex("dbo.Payrolls", new[] { "EmployeeId" });
            DropIndex("dbo.JobExperiences", new[] { "Employee_Id" });
            DropIndex("dbo.EmpPerformances", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeNomineeInfoes", new[] { "EmpId" });
            DropIndex("dbo.EmpLeaves", new[] { "EmployeeId" });
            DropIndex("dbo.EmpLeaves", new[] { "LeaveTypeId" });
            DropIndex("dbo.EmpAttendanceSummaryViewModels", new[] { "EmployeeId" });
            DropIndex("dbo.Educational_Background", new[] { "Employee_Id" });
            DropIndex("dbo.Documents", new[] { "Employee_Id" });
            DropIndex("dbo.Designations", new[] { "DepartmentId" });
            DropIndex("dbo.DailyAttendances", new[] { "EmployeeId" });
            DropIndex("dbo.BonusSheetDetails", new[] { "BonuSheetId" });
            DropIndex("dbo.BonusSheetDetails", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "Department_Id" });
            DropIndex("dbo.Employees", new[] { "DesignationId" });
            DropIndex("dbo.Employees", new[] { "ShiftId" });
            DropIndex("dbo.Advance_Salary", new[] { "EmployeeId" });
            DropIndex("dbo.AddressBooks", new[] { "Employee_Id" });
            DropTable("dbo.Vouchers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TransDetails");
            DropTable("dbo.Transactions");
            DropTable("dbo.SMSConfigs");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PerformanceBonus");
            DropTable("dbo.Other_setting");
            DropTable("dbo.Login_History");
            DropTable("dbo.LedgerCategories");
            DropTable("dbo.Ledgers");
            DropTable("dbo.HoliDays");
            DropTable("dbo.EmailConfigs");
            DropTable("dbo.Companies");
            DropTable("dbo.Requirments");
            DropTable("dbo.JobApplications");
            DropTable("dbo.Circulars");
            DropTable("dbo.ChartTrees");
            DropTable("dbo.ChartOfAccounts");
            DropTable("dbo.Shifts");
            DropTable("dbo.Hourly_Duty");
            DropTable("dbo.DeductionsSalaries");
            DropTable("dbo.AllowanceSalaries");
            DropTable("dbo.AdvanceSalaryDeductions");
            DropTable("dbo.Salarysheets");
            DropTable("dbo.Deductions");
            DropTable("dbo.Payroll_Deduction");
            DropTable("dbo.Allowances");
            DropTable("dbo.Payroll_Allowence");
            DropTable("dbo.Payrolls");
            DropTable("dbo.JobExperiences");
            DropTable("dbo.EmpPerformances");
            DropTable("dbo.EmployeeNomineeInfoes");
            DropTable("dbo.LeaveTypes");
            DropTable("dbo.EmpLeaves");
            DropTable("dbo.EmpAttendanceSummaryViewModels");
            DropTable("dbo.Educational_Background");
            DropTable("dbo.Documents");
            DropTable("dbo.Departments");
            DropTable("dbo.Designations");
            DropTable("dbo.DailyAttendances");
            DropTable("dbo.BonusSheets");
            DropTable("dbo.BonusSheetDetails");
            DropTable("dbo.Employees");
            DropTable("dbo.Advance_Salary");
            DropTable("dbo.AddressBooks");
            DropTable("dbo.ActivityLogs");
        }
    }
}
