using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XpenseTrackerWebApp.Models;

namespace XpenseTrackerWebApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);

            DateTime StardDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);

            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StardDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .Sum(x => x.Amount);
            ViewBag.TotalExpenseMonth = TotalExpense.ToString("C0");

            int ExpensesLimit = await _context.Settings
                .Select(s => s.ExpensesLimit)
                .FirstOrDefaultAsync();
            ViewBag.ExpensesLimit = ExpensesLimit;

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            if (id == 0)
            {
                return View(new Transaction());
            }
            else
            {
                return View(_context.Transactions.Find(id));
            }
            
            return View(new Transaction());
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    _context.Add(transaction);
                }
                else
                {
                    _context.Update(transaction);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new()
            {
                CategoryId = 0,
                Title = "Choose a Category"
            };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}
