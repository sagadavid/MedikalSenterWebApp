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

        //we do database work here while dbset is crated
        //fluent api to restrict/configure relations
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //schema helps things organized in proj..MO here, means MedikalSenter
            //created dbsets of this dbcontext will get this schema
            //but..sqlite doesnt support schemas

            //modelBuilder.HasDefaultSchema("MS");

            //Fluent API from the Parent view..
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

        }
    }
}
