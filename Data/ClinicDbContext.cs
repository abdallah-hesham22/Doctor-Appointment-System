using ClinicManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementApi.Data
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prevent a Patient/Doctor being deleted from cascading-deleting unrelated data unexpectedly.
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed a couple of doctors so the API is testable immediately.
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FullName = "Dr. Amina Salah", Email = "amina.salah@clinic.com", Specialization = "Cardiology", PhoneNumber = "0100000001" },
                new Doctor { Id = 2, FullName = "Dr. Omar Farid", Email = "omar.farid@clinic.com", Specialization = "Dermatology", PhoneNumber = "0100000002" }
            );
        }
    }
}
