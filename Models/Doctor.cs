using System.ComponentModel.DataAnnotations;

namespace ClinicManagementApi.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Navigation property
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
