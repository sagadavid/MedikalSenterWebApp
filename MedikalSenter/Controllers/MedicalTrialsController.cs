using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedikalSenter.Data;
using MedikalSenter.Models;

namespace MedikalSenter.Controllers
{
    public class MedicalTrialsController : Controller
    {
        private readonly MedikalSenterContext _context;

        public MedicalTrialsController(MedikalSenterContext context)
        {
            _context = context;
        }

        // GET: MedicalTrials
        public async Task<IActionResult> Index()
        {
            //LEP//ENLIST DATA BY TOLIST
              return View(await _context.MedicalTrials.ToListAsync());
        }

        // GET: MedicalTrials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalTrials == null)
            {
                return NotFound();
            }

            var medicalTrial = await _context.MedicalTrials
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicalTrial == null)
            {
                return NotFound();
            }

            return View(medicalTrial);
        }

        // GET: MedicalTrials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalTrials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TrialName")] MedicalTrial medicalTrial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalTrial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalTrial);
        }

        // GET: MedicalTrials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalTrials == null)
            {
                return NotFound();
            }

            var medicalTrial = await _context.MedicalTrials.FindAsync(id);
            if (medicalTrial == null)
            {
                return NotFound();
            }
            return View(medicalTrial);
        }

        // POST: MedicalTrials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TrialName")] MedicalTrial medicalTrial)
        {
            if (id != medicalTrial.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalTrial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalTrialExists(medicalTrial.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medicalTrial);
        }

        // GET: MedicalTrials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalTrials == null)
            {
                return NotFound();
            }

            var medicalTrial = await _context.MedicalTrials
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicalTrial == null)
            {
                return NotFound();
            }

            return View(medicalTrial);
        }

        // POST: MedicalTrials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalTrials == null)
            {
                return Problem("Entity set 'MedikalSenterContext.MedicalTrials'  is null.");
            }
            var medicalTrial = await _context.MedicalTrials.FindAsync(id);
            if (medicalTrial != null)
            {
                _context.MedicalTrials.Remove(medicalTrial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalTrialExists(int id)
        {
          return _context.MedicalTrials.Any(e => e.ID == id);
        }
    }
}
