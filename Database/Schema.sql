/* ============================================================
   Clinic Management System - Database Schema (SQL Server)
   Matches the EF Core model exactly (table/column names use
   EF Core's default conventions), so you can use EITHER:
     A) this script directly, or
     B) `dotnet ef database update` (see API README)
   Use ONE of the two methods, not both, on the same database.
   ============================================================ */

IF DB_ID('ClinicManagementDb') IS NULL
BEGIN
    CREATE DATABASE ClinicManagementDb;
END
GO

USE ClinicManagementDb;
GO

-- ============================================================
-- Table: Patients
-- ============================================================
IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL DROP TABLE dbo.Patients;
GO
CREATE TABLE dbo.Patients (
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName      NVARCHAR(100)     NOT NULL,
    Email         NVARCHAR(150)     NOT NULL,
    DateOfBirth   DATETIME2         NOT NULL,
    PhoneNumber   NVARCHAR(20)      NULL
);
GO

-- ============================================================
-- Table: Doctors
-- ============================================================
IF OBJECT_ID('dbo.Doctors', 'U') IS NOT NULL DROP TABLE dbo.Doctors;
GO
CREATE TABLE dbo.Doctors (
    Id             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName       NVARCHAR(100)     NOT NULL,
    Email          NVARCHAR(150)     NOT NULL,
    Specialization NVARCHAR(100)     NOT NULL,
    PhoneNumber    NVARCHAR(20)      NULL
);
GO

-- ============================================================
-- Table: Appointments  (join entity between Patient and Doctor)
-- Status: 0 = Pending, 1 = Approved, 2 = Rejected, 3 = Completed
-- ============================================================
IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL DROP TABLE dbo.Appointments;
GO
CREATE TABLE dbo.Appointments (
    Id               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PatientId        INT               NOT NULL,
    DoctorId         INT               NOT NULL,
    AppointmentDate  DATETIME2         NOT NULL,
    Status           INT               NOT NULL DEFAULT 0,
    Reason           NVARCHAR(500)     NULL,
    DoctorNotes      NVARCHAR(500)     NULL,

    CONSTRAINT FK_Appointments_Patients
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_Appointments_Doctors
        FOREIGN KEY (DoctorId) REFERENCES dbo.Doctors(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Appointments_PatientId ON dbo.Appointments(PatientId);
CREATE INDEX IX_Appointments_DoctorId  ON dbo.Appointments(DoctorId);
GO

-- ============================================================
-- Seed data
-- ============================================================
SET IDENTITY_INSERT dbo.Doctors ON;

INSERT INTO dbo.Doctors (Id, FullName, Email, Specialization, PhoneNumber) VALUES
(1, N'Dr. Amina Salah', N'amina.salah@clinic.com', N'Cardiology',  N'0100000001'),
(2, N'Dr. Omar Farid',  N'omar.farid@clinic.com',  N'Dermatology', N'0100000002');

SET IDENTITY_INSERT dbo.Doctors OFF;
GO

SET IDENTITY_INSERT dbo.Patients ON;

INSERT INTO dbo.Patients (Id, FullName, Email, DateOfBirth, PhoneNumber) VALUES
(1, N'Sara Ahmed', N'sara.ahmed@example.com', '1998-04-12', N'01000000000');

SET IDENTITY_INSERT dbo.Patients OFF;
GO

PRINT 'ClinicManagementDb schema created and seeded successfully.';
