using ClinicManagementApi.Data;
using ClinicManagementApi.DTOs;
using ClinicManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public DoctorsController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorReadDto>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Email = d.Email,
                    Specialization = d.Specialization,
                    PhoneNumber = d.PhoneNumber
                })
                .ToListAsync();

            return Ok(doctors);
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorReadDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound($"Doctor with id {id} was not found.");

            return Ok(new DoctorReadDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialization = doctor.Specialization,
                PhoneNumber = doctor.PhoneNumber
            });
        }

        // GET: api/doctors/5/appointments
        [HttpGet("{id}/appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetDoctorAppointments(int id)
        {
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == id);
            if (!doctorExists) return NotFound($"Doctor with id {id} was not found.");

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == id)
                .Include(a => a.Patient)
                .Select(a => new AppointmentReadDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient!.FullName,
                    DoctorId = a.DoctorId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status,
                    Reason = a.Reason,
                    DoctorNotes = a.DoctorNotes
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // POST: api/doctors
        [HttpPost]
        public async Task<ActionResult<DoctorReadDto>> CreateDoctor(DoctorCreateDto dto)
        {
            var doctor = new Doctor
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Specialization = dto.Specialization,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var result = new DoctorReadDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialization = doctor.Specialization,
                PhoneNumber = doctor.PhoneNumber
            };

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, result);
        }

        // PUT: api/doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, DoctorCreateDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound($"Doctor with id {id} was not found.");

            doctor.FullName = dto.FullName;
            doctor.Email = dto.Email;
            doctor.Specialization = dto.Specialization;
            doctor.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound($"Doctor with id {id} was not found.");

            var hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
            if (hasAppointments)
                return BadRequest("Cannot delete a doctor who has existing appointments.");

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
