using MedikalSenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedikalSenter.Data
{
    public class MedikalSenterContext : DbContext
    {
        public MedikalSenterContext(DbContextOptions<MedikalSenterContext> options)
            : base(options) { }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //schema helps things organized in proj.. 
            //MO here, means MedikalSenter
            //created dbsets of this dbcontext will get this schema
            //but.. sqlite doesnt support schemas
            //modelBuilder.HasDefaultSchema("MS");

            //Fluent API from the Parent view..
            //Prevent Cascade Delete from Doctor to Patient
            //meaning, we are
            //prevented from deleting a Doctor with Patients assigned
            modelBuilder.Entity<Doctor>()
                .HasMany<Patient>(d => d.Patients)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            //OHIP Number should be unique
            //to add a unique;
            //'cause will be an addition to Primary Key, we need to create index
            //Fluent API helps
            modelBuilder.Entity<Patient>()
            .HasIndex(p => p.OHIP)
            .IsUnique();

        }
    }
}
