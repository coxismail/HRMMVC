using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRMMVC.Models
{
    public class ChartOfAccount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LedgerCategory 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("ChartOfAccount"), Display(Name="Chart of Account")]
        public Guid ChartOfAccountId { get; set; }
        [Display(Name="Name"), Required, MinLength(3), MaxLength(55)]
        public string Name { get; set; }
        [Display(Name="Parent Category")]
        public Guid? ParentCategoryId { get; set; }
        public virtual ChartOfAccount ChartOfAccount { get; set; }
    }
    public class Ledger 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Ledger Name is required"), MinLength(3), MaxLength(55)]
        public string Name { get; set; }
        public int Code { get; set; }
        [ForeignKey("LedgerCategory"), Required, Display(Name="Category")]
        public Guid LedgerCategoryId { get; set; }
        public virtual LedgerCategory LedgerCategory { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public Guid? RefNo { get; set; }
        public string Ref_Type { get; set; }

    }

    public class ChartTree
    {
        [Key]
        public string Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Parent { get; set; }
        public string icon { get; set; }
    }


    public class Voucher 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name="Voucher Name"), MinLength(3), MaxLength(15), Required]
        public string Name { get; set; }
        [Display(Name="Voucher No Start From"), Range(1, 1000000)]
        public int VoucherNo { get; set; }
    }





    // Transaction
    public class Transaction 
    {
        public Guid Id { get; set; }
        public string Narration { get; set; }
        [Required(ErrorMessage ="Voucher Type is required"), Display(Name="Voucher")]
        public string VoucherType { get; set; }
        [Display(Name="Voucher No")]
        public int VoucherNo { get; set; }
        [Range(1, 100000000000000000)]
        public decimal Amount { get; set; }
        [Display(Name="Trans. Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name="Entry Time")]
        public DateTime EntryTime { get; set; }
        public virtual ICollection<TransDetails> TransDetails { get; set; }
        public Transaction()
        {
            this.TransDetails = new HashSet<TransDetails> ();
            this.EntryTime = DateTime.UtcNow;
        }
    }
    public class TransDetails 
    {
        public Guid Id { get; set; }
        [ForeignKey("Ledger")]
        public Guid LedgerId { get; set; }
        public int LedgerCode { get; set; }
        public string LedgerName { get; set; }
        public Ledger Ledger { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        [Display(Name="Voucher No")]
        public int VoucherNo { get; set; }
        [Display(Name="Voucher")]
        public string VoucherType { get; set; }
        [Display(Name = "Trans. Date")]
        public DateTime TransDate { get; set; }
        [Required, ForeignKey("Transaction")]
        public Guid TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
    // TransactionDetails
}