using HRMMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult BalanceSheet(DateTime? To)
        {
            var ToDate = To ?? DateTime.UtcNow.AddHours(6).AddSeconds(-5).Date;
            var com = _context.Company.FirstOrDefault();
            var id = com.Id;
            ViewBag.CompanyName = _context.Company.Where(x => x.Id == id).Select(s => s.Name).SingleOrDefault();
            var cid_assets = _context.ChartOfAccount.Where(x => x.Name == "Assets").Select(s => s.Id).SingleOrDefault();
            var cid_income = _context.ChartOfAccount.Where(x => x.Name == "Income").Select(s => s.Id).SingleOrDefault();
            var cid_expense = _context.ChartOfAccount.Where(x => x.Name == "Expenses").Select(s => s.Id).SingleOrDefault();
            var cid_liabilities = _context.ChartOfAccount.Where(x => x.Name == "Liabilities").Select(s => s.Id).SingleOrDefault();
            var cid_capital = _context.ChartOfAccount.Where(x => x.Name == "Capital").Select(s => s.Id).SingleOrDefault();

            var Assets = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_assets).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
                ClosingBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit - x.Credit)).FirstOrDefault()
            }).ToList();
            ViewBag.Assets = Assets;

            ViewBag.AssetsSum = Assets.Sum(s => s.DrBalance) - Assets.Sum(s => s.CrBalance);
            var Liabilities = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_liabilities).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
                ClosingBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit - x.Credit)).FirstOrDefault()
            }).ToList();
            ViewBag.Liabilities = Liabilities;
            ViewBag.LiabilitiesSum = Liabilities.Sum(s => s.CrBalance) - Liabilities.Sum(s => s.DrBalance);

            var Capital = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_capital).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
                ClosingBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit - x.Credit)).FirstOrDefault()
            }).ToList();
            ViewBag.Capital = Capital;

            ViewBag.CapitalSum = Capital.Sum(s => s.CrBalance) - Capital.Sum(s => s.DrBalance);

            var Income = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_income).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();
            ViewBag.Income = Income;
            ViewBag.IncomeSum = Income.Sum(s => s.CrBalance) - Income.Sum(s => s.DrBalance);
            var totalIncome = Income.Sum(s => s.CrBalance) - Income.Sum(s => s.DrBalance); ;
            var Expenses = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_expense).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= ToDate).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();
            ViewBag.Expenses = Expenses;
            ViewBag.ExpensesSum = Expenses.Sum(s => s.DrBalance) - Expenses.Sum(s => s.CrBalance);
            var totalExpense = Expenses.Sum(s => s.DrBalance) - Expenses.Sum(s => s.CrBalance);
            ViewBag.GrossIncome = totalIncome - totalExpense;

            ViewBag.PageTitle = "Balance Sheet";
            ViewBag.Preiod = "Till " + ToDate.ToString("dd-MM-yyyy");

            return PartialView();
        }
        public ActionResult ProfitAndLoss(DateTime From, DateTime To)
        {
            var com = _context.Company.FirstOrDefault();

            var cid_income = _context.ChartOfAccount.Where(x => x.Name == "Income").Select(s => s.Id).SingleOrDefault();
            var cid_expense = _context.ChartOfAccount.Where(x => x.Name == "Expenses").Select(s => s.Id).SingleOrDefault();




            var Income = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_income && x.Name != "Sales Accounts").Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();


            var Expenses = _context.LedgerCategory.Where(x => x.ChartOfAccountId == cid_expense && x.Name != "Purchase Accounts").Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();


            var SaleAccount = _context.LedgerCategory.Where(x => x.Name == "Sales Accounts").Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();

            var Purchase = _context.LedgerCategory.Where(x => x.Name == "Purchase Accounts").Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(a => a.TransDate >= From && a.TransDate <= To).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
            }).ToList();

            var PurchaseAccounts = (Purchase.Sum(s => s.DrBalance) - Purchase.Sum(s => s.CrBalance));
            var SalesAccounts = SaleAccount.Sum(s => s.CrBalance) - SaleAccount.Sum(s => s.DrBalance);
            var Other_IncomeSum = Income.Sum(s => s.CrBalance) - Income.Sum(s => s.DrBalance);
            var Other_ExpensesSum = Expenses.Sum(s => s.DrBalance) - Expenses.Sum(s => s.CrBalance);
            ViewBag.Preiod = " from " + From.ToString("dd-MMM-yyyy") + " to " + To.ToString("dd-MMM-yyyy");
            ViewBag.PageTitle = "Profit & Loss Accounts";
            ViewBag.Expenses = Expenses;
            ViewBag.Income = Income;
            ViewBag.Purchase = Purchase;
            ViewBag.Sales = SaleAccount;
            ViewBag.Other_IncomeSum = Other_IncomeSum;
            ViewBag.Other_ExpenseSum = Other_ExpensesSum;
            ViewBag.GrossProfit = SalesAccounts - PurchaseAccounts;
            ViewBag.TotalExpense = PurchaseAccounts + Other_ExpensesSum;
            ViewBag.TotalIncome = SalesAccounts + Other_IncomeSum;
            ViewBag.NetProfit = ((SalesAccounts + Other_IncomeSum) - (PurchaseAccounts + Other_ExpensesSum));
            return PartialView();
        }




        public ActionResult LedgerReport(Guid? Id, DateTime From, DateTime To)
        {

            var ledger = _context.Ledger.Where(x => x.Id == Id).SingleOrDefault();
            var LedgerId = ledger.Id;

            var minimumDate = DateTime.MinValue;
            var list = _context.TransDetails.Where(s => s.LedgerId == LedgerId).ToList();
            if (list.Count > 0)
            {
                minimumDate = list.Min(x => x.TransDate);
            }
            else
            {
                return RedirectToAction("index", "Accounting");
            }

            var searchDateExist = _context.TransDetails.Where(x => x.TransDate == From && x.LedgerId == ledger.Id).FirstOrDefault();
            if (searchDateExist == null)
            {
                if (From < minimumDate)
                {
                    From = minimumDate;
                }
            }
            var all = _context.TransDetails.Where(x => x.TransDate >= From).Where(x => x.TransDate <= To);
            Guid[] transAllids = all.OrderBy(x => x.LedgerCode).Where(x => x.LedgerId == LedgerId).GroupBy(x => x.TransactionId).Select(s => s.FirstOrDefault().TransactionId).ToArray();

            var ledgerTranx = all.Where(f => transAllids.Contains(f.TransactionId)).
                              Select(x => new LedgerDetailsReport
                              {
                                  Date = x.TransDate,
                                  Name = x.LedgerName,
                                  Vouchertype = x.VoucherType,
                                  VoucherNo = x.VoucherNo,
                                  Voucher = x.VoucherType + " " + x.VoucherNo,
                                  Debit = x.Credit,
                                  Cradit = x.Debit,
                                  Narration = x.Transaction.Narration,
                                  id = x.LedgerId
                              }).ToList();
            var traxlist = ledgerTranx.Where(s => s.id != LedgerId).OrderBy(o => o.Date).ToList();
            var openingBlance = _context.TransDetails.Where(s => s.TransDate < From && s.LedgerId == LedgerId).ToList();
            decimal? opeD = 0;
            decimal? opeC = 0;
            string bl = "";
            if (openingBlance.Count > 0)
            {
                opeD = openingBlance.Sum(s => s.Debit);
                opeC = openingBlance.Sum(s => s.Credit);
                bl = opeD - opeC + "  Dr";
                if (opeC > opeD)
                {
                    bl = opeC - opeD + "  Cr";
                }

            }

            var balC = opeC;
            var balD = opeD;
            List<LedgerDetailsReport> lr = new List<LedgerDetailsReport>();
               
            lr.Add(new LedgerDetailsReport()
            {
                Date = From.AddDays(-1),
                Name = "Opening Balance",
                Debit = opeD,
                Cradit = opeC,
                Balance = bl
            });
         
            foreach (var date in traxlist)
            {
                balC += date.Cradit ?? 0;
                balD += date.Debit ?? 0;
                string balance = "";
                if (balC >= balD)
                {
                    balance = balC - balD + " Cr";
                }
                else if (balC <= balD)
                {
                    balance = balD - balC + " Dr";
                }
                date.Balance = balance;
            }
            lr.AddRange(traxlist);

            TempData["ledgerDetails"] = lr;
            ViewBag.DebitSum = (lr.Sum(x => x.Debit)).ToString().Replace('-', ' ');
            ViewBag.CreditSum = (lr.Sum(x => x.Cradit)).ToString().Replace('-', ' ');

            var debitsum = Convert.ToDecimal(ViewBag.DebitSum);
            var Creditsum = Convert.ToDecimal(ViewBag.CreditSum);


            if (debitsum >= Creditsum)
            {
                ViewBag.totalCD = debitsum - Creditsum;
                ViewBag.totalDB = 0;
                ViewBag.ClosingBalance = debitsum - Creditsum + " Dr";
                ViewBag.TotalSum = debitsum;
            }
            else
            {
                ViewBag.totalCD = 0;
                ViewBag.totalDB = Creditsum - debitsum;
                ViewBag.ClosingBalance = Creditsum - debitsum + " Cr";
                ViewBag.TotalSum = Creditsum;
            }


            ViewBag.Preiod = From.ToString("dd-MM-yyyy") + " To " + To.ToString("dd-MM-yyyy");
            ViewBag.PageTitle = "Ledger : " + ledger.Name;
            return PartialView();

        }
        public ActionResult LedgerDetailsReport() // ajax call 
        {
            var data = TempData["ledgerDetails"] as List<LedgerDetailsReport>;
            if (data == null)
            {
                data = new List<LedgerDetailsReport>();
            }
            var res = data.ToList().Select(f => new
            {
                Date = f.Date.ToString("dd-MM-yyyy"),
                Type = f.Vouchertype,
                VoucherNo = f.VoucherNo,
                Particular = f.Name,
                Narration = f.Narration,
                Debit = f.Debit,
                Credit = f.Cradit,
                Balance = f.Balance
            }).ToList();

            return Json(new { data = res }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult LedgerCategory(Guid LedCategoryId, DateTime From, DateTime To) 
        {
            var cat = _context.LedgerCategory.ToList();
            var ledc = cat.Where(f => f.Id == LedCategoryId).SingleOrDefault();
            var childids = Recursive(cat, LedCategoryId);


            var findDate = _context.TransDetails.ToList().Min(x => x.TransDate);
            var dateFound = _context.TransDetails.Where(x => x.TransDate == From).FirstOrDefault();
            if (dateFound == null)
            {
                if (From < findDate)
                {
                    From = findDate;
                }
            }

         
            var all = _context.TransDetails.OrderBy(a => a.TransDate).Where(a => childids.Contains(a.Ledger.LedgerCategoryId)).ToList();

            var openingBalance = all.Where(x => x.TransDate < From).OrderBy(x => x.LedgerCode).GroupBy(x => x.LedgerCode).Select(s => new
            {

                ledgerID = s.FirstOrDefault().LedgerId,
                OpeningDebit = (s.Sum(x => x.Debit)),
                OpeningCredit = (s.Sum(x => x.Credit)),
            }).ToList();

            var curentdata = all.Where(x => x.TransDate >= From).Where(x => x.TransDate <= To).OrderBy(x => x.LedgerCode).GroupBy(x => x.LedgerCode).Select(s => new
            {
                LedgerNo = s.FirstOrDefault().LedgerCode,
                LedgerName = s.FirstOrDefault().LedgerName,
                DebitAmount = s.Sum(x => x.Debit),
                CreditAmount = s.Sum(x => x.Credit),
                ledgerID = s.FirstOrDefault().LedgerId
            }).ToList();

            var data = curentdata
                .GroupJoin(
                    openingBalance, opening => opening.ledgerID, transaction => transaction.ledgerID,
                    (x, y) => new { Open = x, trans = y }
                )
                .SelectMany(
                    T => T.trans.DefaultIfEmpty(),
                    (x, y) => new LedgerListReport
                    {
                        OpeningDebit = y == null ? 0 : y.OpeningDebit,
                        OpeningCredit = y == null ? 0 : y.OpeningCredit,
                        LedgerNo = x.Open.LedgerNo,
                        LedgerName = x.Open.LedgerName,
                        DebitAmount = x.Open.DebitAmount,
                        CreditAmount = x.Open.CreditAmount,
                        ClosinngDebit = y == null ? (x.Open.DebitAmount - x.Open.CreditAmount) : ((y.OpeningDebit + x.Open.DebitAmount) - (y.OpeningCredit + x.Open.CreditAmount)),
                        ClosinngCredit = y == null ? (x.Open.CreditAmount - x.Open.DebitAmount) : ((y.OpeningCredit + x.Open.CreditAmount) - (y.OpeningDebit + x.Open.DebitAmount)),
                        ledgerID = x.Open.ledgerID,
                        DefaultData = 0
                    }
                ).ToList();
            ViewBag.Preiod = From.ToString("dd-MM-yyyy") + " to " + To.ToString("dd-MM-yyyy");
            ViewBag.PageTitle = "Ledger Category Details : ( " + ledc.Name + " )";
            return PartialView(data);
        }
        public List<Guid> Recursive(List<LedgerCategory> ctegory, Guid parentId)
        {
            List<Guid> inner = new List<Guid>();
            foreach (var t in ctegory.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                inner.Add(t.Id);
                inner = inner.Union(Recursive(ctegory, t.Id)).ToList();
            }

            return inner;
        }



        public ActionResult ChartofAccount(Guid Id, DateTime From, DateTime To) 
        {

            var chart = _context.ChartOfAccount.Where(s => s.Id == Id).SingleOrDefault();
            var Query = _context.LedgerCategory.Where(x => x.ChartOfAccountId == Id).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = _context.TransDetails.Where(s => s.TransDate <= To && s.TransDate >= From).Where(x=>x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit)).FirstOrDefault(),
                CrBalance = _context.TransDetails.Where(s => s.TransDate <= To && s.TransDate >= From).Where(x=>x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Credit)).FirstOrDefault(),
                ClosingBalance = _context.TransDetails.Where(s => s.TransDate <= To && s.TransDate >= From).Where(x=>x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.Debit - x.Credit)).FirstOrDefault()
            }).ToList();
            var Debit = Query.Sum(s => s.DrBalance);
            var Credit = Query.Sum(s => s.CrBalance);
            var closingbalance = "";
            if (Debit > Credit)
            {
                closingbalance = Debit - Credit + " Dr";
            }
            else if (Debit < Credit)
            {
                closingbalance = Credit - Debit + " Cr";
            }
            ViewBag.Preiod = From.ToString("dd-MM-yyyy") + " to " + To.ToString("dd-MM-yyyy");
            ViewBag.PageTitle = "Chart of Accounts Details : ( " + chart.Name + " )";

            ViewBag.ClosingBalance = closingbalance;
            ViewBag.TotalDebit = Debit;
            ViewBag.TotalCredit = Credit;
            return PartialView(Query);

        }


        public ActionResult All_Voucher()
        {
            ViewBag.Type = new SelectList(_context.Voucher.ToList(), "Name", "Name");
            return PartialView();
        }
        public ActionResult Details(Guid Id)
        {
            var re = _context.Transaction.Where(f => f.Id == Id).Include(f=>f.TransDetails).SingleOrDefault();
            if (re != null)
            {
                return PartialView(re);
            }
            return RedirectToAction("Index", "Accounting");
        }








        // All Voucher Datewise view

        public ActionResult Get_All_Voucher(string Type, DateTime from, DateTime to) // ajax call
        {
            var all = _context.Transaction.Where(s => s.TransactionDate >= from && s.TransactionDate <= to).OrderByDescending(f => f.TransactionDate).ToList();
            if (!string.IsNullOrEmpty(Type))
            {
                all = all.Where(f => f.VoucherType == Type).ToList();
            }
            var record = all.Select(f => new
            {
                Id = f.Id,
                No = f.VoucherNo,
                Type = f.VoucherType,
                Amount = f.Amount,
                Date = TimeZoneInfo.ConvertTimeFromUtc(f.TransactionDate, User.Identity.TimeZone()).ToString("dd-MMM-yyyy"),
                Narration = f.Narration
            }).ToList();
            return Json(new { data = record }, JsonRequestBehavior.AllowGet);
        }







    }
}