using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedikalSenter.Data;
using MedikalSenter.Models;
using MedikalSenter.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;

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
            //var medikalSenterContext 
              var patients  = _context.Patients
                .Include(p => p.Doctor)
                .Include(m=>m.MedicalTrial)
                //lep//patientcondition is combination of pc+c. so call both to materialize it
                .Include(pc=>pc.PatientConditions).ThenInclude(pc=>pc.Condition)
                //LEP//DONT NEED TO TRACK I AM JUST READING DATABASE NOT CHANGING IT, RELAX
                .AsNoTracking();
            return View(await patients.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                //LEP//get those data vha include
                .Include(p => p.Doctor)
                .Include(m => m.MedicalTrial)
                .Include(pc => pc.PatientConditions).ThenInclude(pc => pc.Condition)
                .AsNoTracking()
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
            //lep//add all unchecked conditions to viewbag
            var patient = new Patient();
            PopulateAssignedConditionData(patient);
          
            //ViewData["DoctorID"] = new SelectList(_context.Doctors
            //    //LEP//LINQ QUERY IN DATABASE-CONTEXT- BY ORDERBY, THENBY
            //   .OrderBy(d => d.LastName)
            //   .ThenBy(d => d.FirstName)
            //   , "ID", "FormalName");
            PopulateDropDownLists();
            
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]//lep//this post attrib is mistakenly deleted,
                  //lost hours to get it back after multiple enpoints error!!!
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create
            ([Bind("ID,OHIP,FirstName,MiddleName,LastName," +
            "DOB,ExpYrVisits,Phone,EMail,DoctorID,MedicalTrialID")] 
                        Patient patient, string[] selectedOptions)
        {
            //LEP//TRY, CATH IS A GOOD PRACTICE FOR DATABASE POSTINGS
            try
            {
                //Add the selected conditions
                if (selectedOptions != null)
                {
                    foreach (var condition in selectedOptions)
                    {
                        var conditionToAdd = new PatientCondition
                        { PatientID = patient.ID, ConditionID = int.Parse(condition) };
                        patient.PatientConditions.Add(conditionToAdd);
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            //transactions are good to try again  
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }

            catch (DbUpdateException dex)
            {//lep//get base excepiton is more omfattende depending on error location
                //lep//these are db related exeptions.. if we dont try cath error is seen as web site crash report

                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Patients.OHIP"))
                {
                    ModelState.AddModelError("OHIP", "Unable to save changes. Remember, you cannot have duplicate OHIP numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }


            PopulateAssignedConditionData(patient);
            //LEP//POPULATE DROPDOWN LIST INHOLD FOR VIEW
            PopulateDropDownLists(patient);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            //var patient = await _context.Patients.FindAsync(id);
            var patient = await _context.Patients
                .Include(p => p.PatientConditions).ThenInclude(p => p.Condition)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (patient == null)
            {
                return NotFound();
            }
            PopulateAssignedConditionData(patient);
            PopulateDropDownLists(patient);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //LEP//ID IS ALREADY AUTHO IN DATABSE, NO NEED TO KEEP IN WHITELIST OF BIND WHICH IS
        //PROTECTION AGAINST OVERPOSTING
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        //    Edit(int id, 
        //    [Bind("ID, OHIP,FirstName,MiddleName,LastName," +
        //    "DOB,ExpYrVisits,Phone,EMail,DoctorID, MedicalTrialID")]
        //Patient patient)
        //LEP//above, in the standart version of editing we lose auditing (who/when created/changed)of the object
        //'cause we replace patient object with the whitelisted one when we update(patient) it.
        //thats why we try to get around by TWO industry approved ways. TRYUPDATEASYNC AND DATATRANSFEROBJECT
        //which brings more tractable way/auditable-ity and
        //keeps secret properties if available --> TryUpdateModelAsync method :
        {
            //LEP//GO GET THE PATIENT TO UPDATE
            //var patientToUpdate = await _context.Patients.FirstOrDefaultAsync(p => p.ID == id);
            var patientToUpdate = await _context.Patients
                .Include(p => p.PatientConditions).ThenInclude(p => p.Condition)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (patientToUpdate == null)
            {
                System.Console.WriteLine("no patient object fetched");
                return NotFound();
            }

            //Update the medical history
            UpdatePatientConditions(selectedOptions, patientToUpdate);


            //if (id != patient.ID)
            //{return NotFound();}
            

            //LEP//try to update the patienttoupdate object with values posted/whitelist, change tracker will now know it
            //if (ModelState.IsValid)

            if (await TryUpdateModelAsync<Patient>(patientToUpdate, "",
                p => p.OHIP, p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.DOB,
                p => p.ExpYrVisits, p => p.Phone, p => p.EMail, p => p.DoctorID, p => p.MedicalTrialID))
            {
                try
                {
                    //lep//we replace patient object.. and we loose auditing (who/when created/cahnged)
                    //lep//after tryupdate method we dont neet this below.its already updated
                    //_context.Update(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patientToUpdate.ID))
                    //lep//if (!PatientExists(patient.ID))

                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //lep//we can have multiple catches and
                //we need to cath any error related to dbupdate
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Patients.OHIP"))
                    {
                        ModelState.AddModelError("OHIP", "Unable to save changes. " +
                            "Remember, you cannot have duplicate OHIP numbers.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists see your system administrator.");
                    }
                }
                //lep//if there is database error, we dont want want to return this.. move to try block
               // return RedirectToAction(nameof(Index));
            }
            PopulateAssignedConditionData(patientToUpdate);
            PopulateDropDownLists(patientToUpdate);
            return View(patientToUpdate);
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
                //LEP//CALL DATA OF MEDICAL INTO QUERY
                .Include(m => m.MedicalTrial)
                .Include(pc=>pc.PatientConditions).ThenInclude(pc=>pc.Condition)
                .AsNoTracking()
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
            
            //var patient = await _context.Patients.FindAsync(id);
            var patient = await _context.Patients
             .Include(p => p.Doctor)
             //LEP//CALL DATA OF MEDICAL INTO QUERY
             .Include(m => m.MedicalTrial)
            //lep//call data of conditions in two step
             .Include(pc => pc.PatientConditions).ThenInclude(pc => pc.Condition)
             .AsNoTracking()
             .FirstOrDefaultAsync(m => m.ID == id);

            //lep//do it in try
            try
            {
                if (patient != null)
                {
                    _context.Patients.Remove(patient);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            //lep//redefine catch here 
            //lep//those error messages is the result of database/server side warnings.. be aware of it !
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            //lep//if cant delete, then return patient view
            //lep//and we need a line in the delete view to show error.. copy from edit validation line and paste delete view
            return View(patient);

        }
        //video 22// 
        //lep//checklist view model, phase 1
        //this is gonna be a population mehtod to all possible checkbox data
        private void PopulateAssignedConditionData(Patient patient)
        {
            //For this to work, you must alleready have Included the PatientConditions 
            //in the Patient
            //conditions are medical history, nemlig sykdom
            var allOptions = _context.Conditions;
            var currentOptionIDs = new HashSet<int>(patient.PatientConditions.Select(b => b.ConditionID));
            
            //lep 1//notice.. a list of checkbox class..get skeleton
            var checkBoxes = new List<CheckOptionVM>();
            
            //lep 2//get parts from whole
            foreach (var option in allOptions)
            {
                //lep 3// benifit list ability to sew skeleton newly
                checkBoxes.Add(new CheckOptionVM
                {
                    //lep 4//sew parts to corresponding body organs
                    ID = option.ID,
                    DisplayText = option.ConditionName,
                    Assigned = currentOptionIDs.Contains(option.ID)
                });
            }
            //lep// 'cause checkboxes are filled by id..
            ViewData["ConditionOptions"] = checkBoxes;
        }
        //lep//cehck box phase 2
        //lep//selectoptions will contain all ids that is checked, string array of ids/ints
        private void UpdatePatientConditions(string[] selectedOptions, Patient patientToUpdate)
        {
            if (selectedOptions == null)
            {
                patientToUpdate.PatientConditions = new List<PatientCondition>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var patientOptionsHS = new HashSet<int>
                (patientToUpdate.PatientConditions.Select(c => c.ConditionID));//IDs of the currently selected conditions
            foreach (var option in _context.Conditions)
            {
                if (selectedOptionsHS.Contains(option.ID.ToString())) //It is checked
                {
                    if (!patientOptionsHS.Contains(option.ID))  //but not currently in the history
                    {
                        //.add requires preloading, otherwise you'll get nullreference error
                        patientToUpdate.PatientConditions.Add(new PatientCondition 
                        {PatientID = patientToUpdate.ID, ConditionID = option.ID });
                    }
                }
                else
                {
                    //Checkbox Not checked
                    if (patientOptionsHS.Contains(option.ID))//but it is currently in the history - so remove it
                    {
                        PatientCondition conditionToRemove = patientToUpdate.PatientConditions.SingleOrDefault(c => c.ConditionID == option.ID);
                        _context.Remove(conditionToRemove);
                    }
                }
            }
        }

        //LEP//DROPDOWN METHOD SUBSTITUDE DEPENDING ON SELECTING ENTITY
        private SelectList DoctorSelectList(int? selectedId)
        {
            return new SelectList(_context.Doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName), "ID", "FormalName", selectedId);
        }

        private SelectList MedicalTrialList(int? selectedId)
        {
            return new SelectList(_context
                .MedicalTrials
                .OrderBy(m => m.TrialName), "ID", "TrialName", selectedId);
        }

        //LEP// DROPDOWN LIST TO SELECT
        //This is a twist on the PopulateDropDownLists approach
        //  Create methods that return each SelectList separately
        //  and one method to put them all into ViewData.
        //This approach allows for AJAX requests to refresh
        //DDL Data at a later date.
        private void PopulateDropDownLists(Patient patient = null)
        {
            ViewData["DoctorID"] = DoctorSelectList(patient?.DoctorID);
            ViewData["MedicalTrialID"] = MedicalTrialList(patient?.MedicalTrialID);
        }

        //LEP// DROPDOWN LIST METHOD FORMATION
        //=null 'cause not to enforce parameter
        //already selected id brings the id and first name into select box
        //lets order drobpox by name ordering from database (fullname summary is not in database!)
        //private void PopulateDropDownLists(Patient patient = null)
        //{
        //    ViewData["DoctorID"] = new SelectList(_context
        //        .Doctors
        //        .OrderBy(d => d.LastName)
        //        .ThenBy(d => d.FirstName)
        //        ,"ID", "FormalName", patient.DoctorID);

        //    ViewData["MedicalTrialID"] = new SelectList(_context
        //        .MedicalTrials
        //        .OrderBy(m => m.TrialName)
        //        ,"ID", "TrialName", patient.MedicalTrialID);
        //}
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }
    }
}
