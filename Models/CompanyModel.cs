using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{
    public class Company
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name="Company Name *")]
        public string Name { get; set; }
        [Display(Name="Phone *")]
        public string Phone { get; set; }
        [Display(Name="Email *")]
        public string Email { get; set; }
        [Display(Name="Website")]
        public string Website { get; set; }
        [Display(Name="Fax")]
        public string Fax { get; set; }
        [Display(Name="Address")]
        public string Address { get; set; }
        [Display(Name="Country *")]
        public string Country { get; set; }
        [Display(Name="State *")]
        public string State { get; set; }
        [Display(Name="City *")]
        public string City  { get; set; }


    }
    public class Department 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Depart name is Required"), MinLength(5), MaxLength(55)]
        public string Name { get; set; }
        public virtual ICollection<Designation> Designation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public Department()
        {
            this.Designation = new HashSet<Designation>();
            this.Employees = new HashSet<Employee>();
        }
    }
    public class Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Designation Title is Required"), MinLength(5), MaxLength(55)]
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        [ForeignKey("Department"),Required]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public Designation()
        {
            this.Employee = new HashSet<Employee>();
        }
    }
    public class Shift 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Shift Name is Required"), MinLength(3), MaxLength(55)]
        public string Name { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public Shift()
        {
            this.Employee = new HashSet<Employee>();
        }
    }
    public class Year 
    { 

    }
}