using MedikalSenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedikalSenter.Data
{
    public class MedikalSenterContext : DbContext
    {
        public MedikalSenterContext(DbContextOptions<MedikalSenterContext> options)
            : base(options) { }

        //entities are singular, collectiona are plural
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientCondition> PatientConditions { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        //medicaltrial was nullable, makes ondelete.restricted
        public DbSet<MedicalTrial> MedicalTrials { get; set; }

       

        //we do database work here while dbset is crated
        //fluent api to restrict/configure relations
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //lep//schema helps things organized in proj..MO here, means MedikalSenter
            //created dbsets of this dbcontext will get this schema
            //but..sqlite doesnt support schemas

            //modelBuilder.HasDefaultSchema("MS");

            //lep//many to many intersection
            modelBuilder.Entity<PatientCondition>()
                .HasKey(pc => new { pc.ConditionID, pc.PatientID });

            //lep//Fluent API from the Parent view..
            //Prevent Cascade Delete from Doctor to Patient
            //meaning:we prevent deleting a Doctor with Patients assigned
            modelBuilder.Entity<Doctor>()
                .HasMany<Patient>(d => d.Patients)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);//behave on doctor!

            //OHIP Number should be unique in database
            //to add a unique index for database,
            //we need to create unique index
            //Fluent API helps
            modelBuilder.Entity<Patient>()
            .HasIndex(p => p.OHIP)
            .IsUnique();

            //lep//m:m but delete restricted one side,
            //lep//Allow Cascade Delete from Patient to PatientCondition
            modelBuilder.Entity<PatientCondition>()
                .HasOne<Condition>(pc => pc.Condition)
                .WithMany(c => c.PatientConditions)
                .HasForeignKey(pc => pc.ConditionID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
