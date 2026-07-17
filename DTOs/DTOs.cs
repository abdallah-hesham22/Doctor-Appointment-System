using ClinicManagementApi.Models;

namespace ClinicManagementApi.DTOs
{
    // ---- Patient ----
    public class PatientCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class PatientReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }

    // ---- Doctor ----
    public class DoctorCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class DoctorReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    // ---- Appointment ----
    public class AppointmentCreateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Reason { get; set; }
    }

    public class AppointmentStatusUpdateDto
    {
        public AppointmentStatus Status { get; set; }
        public string? DoctorNotes { get; set; }
    }

    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
