using ClinicManagementApi.Data;
using ClinicManagementApi.DTOs;
using ClinicManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public AppointmentsController(ClinicDbContext context)
        {
            _context = context;
        }

        private static AppointmentReadDto ToDto(Appointment a) => new()
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient?.FullName,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor?.FullName,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status,
            Reason = a.Reason,
            DoctorNotes = a.DoctorNotes
        };

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Select(a => ToDto(a))
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentReadDto>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound($"Appointment with id {id} was not found.");

            return Ok(ToDto(appointment));
        }

        // POST: api/appointments  (Patient books an appointment)
        [HttpPost]
        public async Task<ActionResult<AppointmentReadDto>> BookAppointment(AppointmentCreateDto dto)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists) return BadRequest($"Patient with id {dto.PatientId} does not exist.");

            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists) return BadRequest($"Doctor with id {dto.DoctorId} does not exist.");

            if (dto.AppointmentDate <= DateTime.Now)
                return BadRequest("Appointment date must be in the future.");

            // Business rule (mirrors the original project): a patient cannot have
            // more than one active (Pending/Approved) appointment at a time.
            var hasActiveAppointment = await _context.Appointments.AnyAsync(a =>
                a.PatientId == dto.PatientId &&
                (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved));

            if (hasActiveAppointment)
                return BadRequest("This patient already has an active appointment. Complete or cancel it before booking another.");

            // Prevent double-booking the same doctor at the exact same time.
            var slotTaken = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != AppointmentStatus.Rejected);

            if (slotTaken)
                return BadRequest("This doctor already has an appointment booked at that time.");

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                Reason = dto.Reason,
                Status = AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Doctor).LoadAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, ToDto(appointment));
        }

        // PUT: api/appointments/5/status  (Doctor approves/rejects/completes)
        [HttpPut("{id}/status")]
        public async Task<ActionResult<AppointmentReadDto>> UpdateStatus(int id, AppointmentStatusUpdateDto dto)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound($"Appointment with id {id} was not found.");

            appointment.Status = dto.Status;
            if (!string.IsNullOrWhiteSpace(dto.DoctorNotes))
                appointment.DoctorNotes = dto.DoctorNotes;

            await _context.SaveChangesAsync();
            return Ok(ToDto(appointment));
        }

        // GET: api/appointments/patient/3  (a patient's appointment/treatment history)
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetByPatient(int patientId)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == patientId);
            if (!patientExists) return NotFound($"Patient with id {patientId} was not found.");

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .Select(a => ToDto(a))
                .ToListAsync();

            return Ok(appointments);
        }

        // DELETE: api/appointments/5  (cancel an appointment)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound($"Appointment with id {id} was not found.");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
