using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using XpenseTrackerWebApp.Models;

namespace XpenseTrackerWebApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SettingsController
        public async Task<IActionResult> SettingsView()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new Settings { ExpensesLimit = 0 };
            }

            return View(settings);
        }

        // GET: SettingsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SettingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SettingsController/SaveSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSettings(Settings setting)
        {
            if (ModelState.IsValid)
            {
                var existingSetting = await _context.Settings.FirstOrDefaultAsync();

                if (existingSetting != null)
                {
                    // Update the existing settings record
                    existingSetting.ExpensesLimit = setting.ExpensesLimit;
                    _context.Settings.Update(existingSetting);
                }
                else
                {
                    // If no settings exist, create a new one
                    await _context.Settings.AddAsync(setting);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SettingsView));
            }

            return View(setting);
        }


        // GET: SettingsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SettingsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SettingsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SettingsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
