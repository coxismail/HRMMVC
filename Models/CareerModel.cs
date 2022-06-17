using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{
    public class Circular
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required, Display(Name = "Application Title")]
        public string Title { get; set; }
        [Required, Display(Name = "Application End")]
        public DateTime Deadline { get; set; }
        [Required, Display(Name="Application Start")]
        public DateTime StartFrom { get; set; }
        [DataType(DataType.MultilineText), Required]
        public string Description { get; set; }
        public virtual ICollection<Requirment> Requirment { get; set; }
        public virtual ICollection<JobApplication> JobApplication { get; set; }
        public bool IsClosed { get; set; }
        public bool IsPublished { get; set; }
        public DateTime Created { get; set; }
        public Circular()
        {
            Requirment = new HashSet<Requirment>();
            JobApplication = new HashSet<JobApplication>();
        }
    }
    public class Requirment 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> RequirTerms { get; set; }
    }


    public class JobApplication 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Circular"), Required]
        public Guid CircularId { get; set; }
        public virtual Circular Circular { get; set; }
        [Display(Name="Application Time")]
        public DateTime ApplicationTime { get; set; }
        public int ApplicationId { get; set; }
        public string Key { get; set; }
        [Display(Name = "Applicant Name"), Required, MinLength(5), MaxLength(55)]
        public string Full_Name { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; }
        [Phone, Required]
        public string Mobile { get; set; }
        public ApplicationStatus? Preliminary { get; set; }
        public ApplicationStatus? Viva { get; set; }
        public bool IsClosed { get; set; }
        public string ResumeUrl { get; set; }
        [NotMapped]
        public HttpPostedFileBase  ResumeUpload { get; set; }

    }
}