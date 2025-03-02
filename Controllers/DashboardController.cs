using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XpenseTrackerWebApp.Models;
using System.Globalization;

namespace XpenseTrackerWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            DateTime StardDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StardDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .Sum(x => x.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .Sum(x => x.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            // Donut Chart - Expenses by Category
            var doughnutChartRadius = new string[] { "140", "135.8", "130.6", "125.5", "120.8", "107.5", "94.6", "88.7", "70" };

            var doughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount) // Sort by amount (highest first)
                .Select((item, index) => new
                {
                    item.categoryTitleWithIcon,
                    item.amount,
                    item.formattedAmount,
                    radius = doughnutChartRadius.Length > index ? doughnutChartRadius[index] : doughnutChartRadius.Last() // Assign radius from highest to lowest
                })
                .ToList();

            ViewBag.DougnutChartData = doughnutChartData;



            //Spline Chart - Income vs Expense
            //Income 
            List<SplineChartData>  IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount),
                })
                .ToList();

            //Expense 
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount),
                })
                .ToList();

            //Combined Income and Expenses
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StardDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();
            
            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined 
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined 
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new SplineChartData()
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            return View();
        }

        public async Task<IActionResult> TodayView()
        {
            DateTime today = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date == today)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .Sum(x => x.Amount);
            ViewBag.TotalIncomeToday = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .Sum(x => x.Amount);
            ViewBag.TotalExpenseToday = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.BalanceToday = String.Format(culture, "{0:C0}", Balance);

            //Income by category
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(t => t.Category.Type == "Income")
                .GroupBy(t => t.Category.Title)
                .Select(g => new SplineChartData()
                {
                    day = g.Key, // Category title as x-axis label
                    income = g.Sum(t => t.Amount), // Total income for that category
                })
                .ToList();
            //Expenses by category
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => new SplineChartData()
                {
                    day = g.Key, // Category title as x-axis label
                    expense = g.Sum(t => t.Amount), // Total expense for that category
                })
                .ToList();

            ViewBag.ColumnChartData = (from category in SelectedTransactions.Select(t => t.Category.Title).Distinct()
                                       join income in IncomeSummary on category equals income.day into incomeJoined
                                       from income in incomeJoined.DefaultIfEmpty()
                                       join expense in ExpenseSummary on category equals expense.day into expenseJoined
                                       from expense in expenseJoined.DefaultIfEmpty()
                                       select new SplineChartData()
                                       {
                                           day = category, // Category name
                                           income = income?.income ?? 0, // Income for that category
                                           expense = expense?.expense ?? 0 // Expense for that category
                                       }).ToList();

            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .Where(k => k.Date == today)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> MonthView()
        {
            DateTime StardDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);

            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StardDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .Sum(x => x.Amount);
            ViewBag.TotalIncomeMonth = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .Sum(x => x.Amount);
            ViewBag.TotalExpenseMonth = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.BalanceMonth = String.Format(culture, "{0:C0}", Balance);

            var doughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount) // Sort by amount (highest first)
                .ToList();

            ViewBag.DougnutChartDataMonth = doughnutChartData;


            //Spline Chart - Income vs Expense
            //Income 
            List<SplineChartData>  IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount),
                })
                .ToList();

            //Expense 
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount),
                })
                .ToList();

            //Combined Income and Expenses
            string[] AllDaysOfMonth = Enumerable.Range(0, (EndDate - StardDate).Days + 1)
                .Select(i => StardDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            // Combine Income and Expense Data
            ViewBag.ColumnChartDataMonth = from day in AllDaysOfMonth
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new SplineChartData()
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .Where(y => y.Date >= StardDate && y.Date <= EndDate)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
}
