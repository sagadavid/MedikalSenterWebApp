using MedikalSenter.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MedikalSenter.Data
{//lep//call this class with seed method under program.cs before app.run
    public static class MSInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder) 
        {
            //lep//dependency injection
            MedikalSenterContext context =
                applicationBuilder.ApplicationServices
                .CreateScope()//will be cleaned up when we are done with it
                .ServiceProvider.GetService<MedikalSenterContext>();

            //lep//database jobs, done with try catch
            try
            {
                //Add some Medical Trials
                if (!context.MedicalTrials.Any())
                {
                    context.MedicalTrials.AddRange(
                     new MedicalTrial
                     {
                         TrialName = "UOT - Lukemia Treatment"
                     }, new MedicalTrial
                     {
                         TrialName = "HyGIeaCare Center -  Microbiome Analysis of Constipated Versus Non-constipation Patients"
                     }, new MedicalTrial
                     {
                         TrialName = "Sunnybrook -  Trial of BNT162b2 versus mRNA-1273 COVID-19 Vaccine Boosters in Chronic Kidney Disease and Dialysis Patients With Poor Humoral Response following COVID-19 Vaccination"
                     }, new MedicalTrial
                     {
                         TrialName = "Altimmune -  Evaluate the Effect of Position and Duration on the Safety and Immunogenicity of Intranasal AdCOVID Administration"
                     }, new MedicalTrial
                     {
                         TrialName = "TUK - Hair Loss Treatment"
                     });
                    context.SaveChanges();
                }

                //we need to have doctors before patients.
                //order is important in database. so Look/find for any Doctors.
                if (!context.Doctors.Any())//if.any, prebuilt check method of Iqueryable<>!!
                {
                    context.Doctors.AddRange//add to db set !! with addrange..
                                            //but if any records fails fx mismatches spelling, non gets saved
                        (
                    new Doctor
                    {
                        FirstName = "Georg",
                        MiddleName = "Archild",
                        LastName = "House"
                    },
                    new Doctor
                    {
                        FirstName = "Frenrike",
                        MiddleName = "Reebok",
                        LastName = "Danser"
                    },
                    new Doctor
                    {
                        //nullable middle name
                        FirstName = "Charles",
                        LastName = "Xavier"
                    });
                    context.SaveChanges();
                }

                //Add patients after Doctors for startup
                //(nullable/enable deleted in projfile??)
                if (!context.Patients.Any())
                {
                    context.Patients.AddRange(
                    new Patient
                    {
                        FirstName = "Fred",
                        MiddleName = "Reginald",
                        LastName = "Flintstone",
                        OHIP = "1231231234",
                        DOB = DateTime.Parse("1955-09-01"),
                        ExpYrVisits = 6,
                        Phone = "9055551212",
                        EMail = "fflintstone@outlook.com",
                        MedicalTrialID = context.MedicalTrials.FirstOrDefault(m => m.TrialName.Contains("UOT")).ID,
                        DoctorID = context.Doctors.FirstOrDefault(d =>
                        d.FirstName == "Georg" && d.LastName == "House").ID
                    },
                    new Patient
                    {
                        FirstName = "Wilma",
                        MiddleName = "Jane",
                        LastName = "Flintstone",
                        OHIP = "1321321324",//should be unique
                        DOB = DateTime.Parse("1964-04-23"),//notice !
                        ExpYrVisits = 2,
                        Phone = "9055551212",//notice 10 digit without blank
                        EMail = "wflintstone@outlook.com",
                        //to assign doctors by id only is bad idea.
                        //Assign by query is good practice
                        DoctorID = context.Doctors
                        .FirstOrDefault(d => d.FirstName == "Frenrike"
                        && d.LastName == "Danser").ID
                    },
                    new Patient
                    {
                        FirstName = "Barney",
                        LastName = "Rubble",
                        OHIP = "3213213214",
                        DOB = DateTime.Parse("1964-02-22"),
                        ExpYrVisits = 2,
                        Phone = "9055551213",
                        EMail = "brubble@outlook.com",
                        MedicalTrialID = context.MedicalTrials.FirstOrDefault(d => d.TrialName.Contains("UOT")).ID,
                        DoctorID = context.Doctors
                        .FirstOrDefault(d => d.FirstName == "Charles" && d.LastName == "Xavier").ID
                    },
                    new Patient
                    {
                        FirstName = "Jane",
                        MiddleName = "Samantha",
                        LastName = "Doe",
                        OHIP = "4124124123",
                        ExpYrVisits = 2,
                        Phone = "9055551234",
                        EMail = "jdoe@outlook.com",
                        DoctorID = context.Doctors
                        .FirstOrDefault(d => d.FirstName == "Charles" && d.LastName == "Xavier").ID
                    });
                    context.SaveChanges();
                }
                //lep//we need conditions with patiens.. order is important at seeding
                if (!context.Conditions.Any())
                {
                    string[] conditions = new string[] {
                    "Asthma", "Cancer", "Cardiac disease", "Diabetes",
                        "Hypertension", "Siezure disdorder", "Covid",
                        "Circulation problems", "Bleeding disorder", 
                        "Thyroid condition", "Liver Disease", "Measles", "Mumps"
                    };
                    foreach (string conditus in conditions)
                    {
                        Condition conditionToAdd = new Condition
                        {
                            ConditionName = conditus
                        };

                        //lep//The Add method DetectChanges after every record added.
                        context.Conditions.Add(conditionToAdd);
                        //lep//The AddRange method DetectChanges after all records are added.
                        //context.Conditions.AddRange(conditionToAdd);
                    }

                    context.SaveChanges();
                }

                //add PatientConditions
                //lep//lack of (!) cost 10 mins to decipher :| be careful
                if (!context.PatientConditions.Any())
                {
                    context.PatientConditions.AddRange(
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Cancer").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone"
                            && p.FirstName == "Fred").ID
                        },
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Cardiac disease").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone" 
                            && p.FirstName == "Fred").ID
                        },
                        new PatientCondition
                        {
                            ConditionID = context.Conditions.FirstOrDefault(c => c.ConditionName == "Diabetes").ID,
                            PatientID = context.Patients.FirstOrDefault(p => p.LastName == "Flintstone" 
                            && p.FirstName == "Wilma").ID
                        });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //lep//this exception gets the most accurate here
                Debug.WriteLine(ex.GetBaseException().Message);
            }

         
        }
    }
}
