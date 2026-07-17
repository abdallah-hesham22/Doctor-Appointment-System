using ClinicManagementApi.Data;
using ClinicManagementApi.DTOs;
using ClinicManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public PatientsController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientReadDto>>> GetPatients()
        {
            var patients = await _context.Patients
                .Select(p => new PatientReadDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,
                    DateOfBirth = p.DateOfBirth,
                    PhoneNumber = p.PhoneNumber
                })
                .ToListAsync();

            return Ok(patients);
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientReadDto>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound($"Patient with id {id} was not found.");

            return Ok(new PatientReadDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Email = patient.Email,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber
            });
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<PatientReadDto>> CreatePatient(PatientCreateDto dto)
        {
            var patient = new Patient
            {
                FullName = dto.FullName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var result = new PatientReadDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Email = patient.Email,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber
            };

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, result);
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientCreateDto dto)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound($"Patient with id {id} was not found.");

            patient.FullName = dto.FullName;
            patient.Email = dto.Email;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound($"Patient with id {id} was not found.");

            var hasAppointments = await _context.Appointments.AnyAsync(a => a.PatientId == id);
            if (hasAppointments)
                return BadRequest("Cannot delete a patient who has existing appointments.");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
