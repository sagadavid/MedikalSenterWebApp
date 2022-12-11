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
    public class PatientsController : Controller
    {
        private readonly MedikalSenterContext _context;

        public PatientsController(MedikalSenterContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var medikalSenterContext = _context.Patients.Include(p => p.Doctor);
            return View(await medikalSenterContext.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            //PopulateDropDownLists();
            ViewData["DoctorID"] = new SelectList(_context.Doctors
               .OrderBy(d => d.LastName)
               .ThenBy(d => d.FirstName)
               , "ID", "FormalName");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]//this post attrib is mistakenly deleted, lost hours to get it back after multiple enpoints error!!!
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create
            ([Bind("ID,OHIP,FirstName,MiddleName,LastName,DOB,ExpYrVisits,Phone,EMail,DoctorID")] 
        Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //this query is repeated 5 times in this controller  
            //DRY.. so lets generate a method for dropboxlists
            //PopulateDropDownLists(patient);
            ViewData["DoctorID"] = new SelectList(_context.Doctors
               .OrderBy(d => d.LastName)
               .ThenBy(d => d.FirstName)
               , "ID", "FormalName", patient.DoctorID);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            //PopulateDropDownLists(patient);
            ViewData["DoctorID"] = new SelectList(_context.Doctors
               .OrderBy(d => d.LastName)
               .ThenBy(d => d.FirstName)
               , "ID", "FormalName", patient.DoctorID);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OHIP,FirstName,MiddleName,LastName,DOB,ExpYrVisits,Phone,EMail,DoctorID")] Patient patient)
        {
            if (id != patient.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.ID))
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
            //PopulateDropDownLists(patient);
            ViewData["DoctorID"] = new SelectList(_context.Doctors
               .OrderBy(d => d.LastName)
               .ThenBy(d => d.FirstName)
               , "ID", "FormalName", patient.DoctorID);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'MedikalSenterContext.Patients'  is null.");
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //=null 'cause not to enforce parameter
        //already selected id brings the id and first name into select box
        //lets order drobpox by name ordering from database (fullname summary is not in database!)
        private void PopulateDropDownLists(Patient patient = null)
        {
            ViewData["DoctorID"] = new SelectList(_context.Doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                ,"ID", "FormalName", patient.DoctorID);
        }
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }
    }
}
