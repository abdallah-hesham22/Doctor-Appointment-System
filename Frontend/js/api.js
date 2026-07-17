/* ============================================================
   Clinic Management System - API client
   Talks to the ASP.NET Core Web API (ClinicManagementApi).
   Change API_BASE_URL below if your API runs on a different port.
   ============================================================ */

const API_BASE_URL = "http://localhost:5100/api";

/**
 * Generic request helper. Throws with a readable message on failure
 * so calling code can just .catch(err => showAlert(err.message)).
 */
async function apiRequest(path, options = {}) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: { "Content-Type": "application/json" },
        ...options
    });

    if (response.status === 204) return null; // No Content

    let body = null;
    const text = await response.text();
    if (text) {
        try { body = JSON.parse(text); } catch { body = text; }
    }

    if (!response.ok) {
        const message = typeof body === "string" ? body : (body?.title || JSON.stringify(body));
        throw new Error(message || `Request failed with status ${response.status}`);
    }

    return body;
}

const Api = {
    // ---- Patients ----
    getPatients: () => apiRequest("/patients"),
    getPatient: (id) => apiRequest(`/patients/${id}`),
    createPatient: (dto) => apiRequest("/patients", { method: "POST", body: JSON.stringify(dto) }),
    updatePatient: (id, dto) => apiRequest(`/patients/${id}`, { method: "PUT", body: JSON.stringify(dto) }),

    // ---- Doctors ----
    getDoctors: () => apiRequest("/doctors"),
    getDoctor: (id) => apiRequest(`/doctors/${id}`),
    getDoctorAppointments: (id) => apiRequest(`/doctors/${id}/appointments`),

    // ---- Appointments ----
    getAppointments: () => apiRequest("/appointments"),
    getAppointment: (id) => apiRequest(`/appointments/${id}`),
    bookAppointment: (dto) => apiRequest("/appointments", { method: "POST", body: JSON.stringify(dto) }),
    updateAppointmentStatus: (id, dto) => apiRequest(`/appointments/${id}/status`, { method: "PUT", body: JSON.stringify(dto) }),
    getAppointmentsByPatient: (patientId) => apiRequest(`/appointments/patient/${patientId}`),
    cancelAppointment: (id) => apiRequest(`/appointments/${id}`, { method: "DELETE" })
};

const STATUS_LABELS = ["Pending", "Approved", "Rejected", "Completed"];

function statusBadge(status) {
    return `<span class="status-badge status-${status}">${STATUS_LABELS[status]}</span>`;
}

function showAlert(message, type = "danger") {
    const box = document.getElementById("alertBox");
    if (!box) { alert(message); return; }
    box.className = `alert alert-${type}`;
    box.textContent = message;
    box.style.display = "block";
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function hideAlert() {
    const box = document.getElementById("alertBox");
    if (box) box.style.display = "none";
}

/** Simple "session": store which patient/doctor is currently active in localStorage
    (fine for a graduation project demo; not a substitute for real authentication). */
const Session = {
    setPatient(patient) { localStorage.setItem("clinic_patient", JSON.stringify(patient)); },
    getPatient() { const v = localStorage.getItem("clinic_patient"); return v ? JSON.parse(v) : null; },
    clearPatient() { localStorage.removeItem("clinic_patient"); },

    setDoctor(doctor) { localStorage.setItem("clinic_doctor", JSON.stringify(doctor)); },
    getDoctor() { const v = localStorage.getItem("clinic_doctor"); return v ? JSON.parse(v) : null; },
    clearDoctor() { localStorage.removeItem("clinic_doctor"); }
};
