
using MaxiMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(MaxiMedDbContext db)
        {
            // --- Роли ---
            if (!await db.Roles.AnyAsync())
            {
                db.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Registrar" },
                    new Role { Name = "Doctor" },
                    new Role { Name = "Manager" }
                );
                await db.SaveChangesAsync();
            }
            var doctorRole = await db.Roles.FirstAsync(r => r.Name == "Doctor");
            var registrarRole = await db.Roles.FirstAsync(r => r.Name == "Registrar");
            // --- Администратор ---
            if (!await db.Users.AnyAsync())
            {
                var admin = new User
                {
                    Login = "admin",
                    FullName = "Администратор",
                    PasswordHash = PasswordHasher.Hash("admin123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.Users.Add(admin);
                await db.SaveChangesAsync();

                var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");

                db.UserRoles.Add(new UserRole
                {
                    UserId = admin.Id,
                    RoleId = adminRole.Id
                });

                await db.SaveChangesAsync();
            }
            if (!await db.Users.AnyAsync(u => u.Login == "doctor"))
            {
                var doctor = new User
                {
                    Login = "doctor",
                    PasswordHash = PasswordHasher.Hash("doctor"),
                    FullName = "Врач по умолчанию",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                doctor.UserRoles.Add(new UserRole { Role = doctorRole });

                db.Users.Add(doctor);
            }

            // --- Registrar ---
            if (!await db.Users.AnyAsync(u => u.Login == "registrar"))
            {
                var reg = new User
                {
                    Login = "registrar",
                    PasswordHash = PasswordHasher.Hash("registrar"),
                    FullName = "Регистратор",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                reg.UserRoles.Add(new UserRole { Role = registrarRole });

                db.Users.Add(reg);
            }

            await db.SaveChangesAsync();
            await SeedDemoData(db);
        }
        private static async Task SeedDemoData(MaxiMedDbContext db)
        {
            var rnd = new Random();

            if (!await db.Specialties.AnyAsync())
            {
                var specialties = Enumerable.Range(1, 20)
                    .Select(i => new Specialty
                    {
                        Name = $"Специальность {i}",
                        IsActive = true
                    }).ToList();

                db.Specialties.AddRange(specialties);
                await db.SaveChangesAsync();
            }

            if (!await db.ClinicBranches.AnyAsync())
            {
                var branches = Enumerable.Range(1, 20)
                    .Select(i => new ClinicBranch
                    {
                        Name = $"Филиал {i}",
                        Address = $"Улица {i}",
                        Phone = $"+7999000{i:000}",
                        IsActive = true
                    }).ToList();

                db.ClinicBranches.AddRange(branches);
                await db.SaveChangesAsync();
            }

            var specialtiesList = await db.Specialties.ToListAsync();
            var branchesList = await db.ClinicBranches.ToListAsync();

            if (!await db.Doctors.AnyAsync())
            {
                var doctors = Enumerable.Range(1, 20)
                    .Select(i => new Doctor
                    {
                        FullName = $"Доктор {i}",
                        SpecialtyId = specialtiesList[rnd.Next(specialtiesList.Count)].Id,
                        BranchId = branchesList[rnd.Next(branchesList.Count)].Id,
                        Room = $"{100 + i}",
                        Phone = $"+7888000{i:000}",
                        Email = $"doctor{i}@mail.com",
                        IsActive = true,
                        WorkShift = i % 3 == 0 ? "AfterNoon" : i % 3 == 1 ? "BeforeNoon" : "AllDay"
                    }).ToList();

                db.Doctors.AddRange(doctors);
                await db.SaveChangesAsync();
            }

            if (!await db.Patients.AnyAsync())
            {
                var patients = Enumerable.Range(1, 20)
                    .Select(i => new Patient
                    {
                        LastName = $"Фамилия{i}",
                        FirstName = $"Имя{i}",
                        MiddleName = $"Отчество{i}",
                        BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-20 - i)),
                        Phone = $"+7777000{i:000}",
                        Email = $"patient{i}@mail.com",
                        Address = $"Адрес {i}",
                        Notes = $"Тестовый пациент {i}"
                    }).ToList();

                db.Patients.AddRange(patients);
                await db.SaveChangesAsync();
            }

            if (!await db.Services.AnyAsync())
            {
                var services = Enumerable.Range(1, 20)
                    .Select(i => new Service
                    {
                        Name = $"Услуга {i}",
                        DurationMinutes = rnd.Next(15, 90),
                        BasePrice = rnd.Next(1000, 10000),
                        IsActive = true
                    }).ToList();

                db.Services.AddRange(services);
                await db.SaveChangesAsync();
            }

            if (!await db.Diagnoses.AnyAsync())
            {
                var diagnoses = Enumerable.Range(1, 20)
                    .Select(i => new Diagnosis
                    {
                        Code = $"A{i:000}",
                        Name = $"Диагноз {i}"
                    }).ToList();

                db.Diagnoses.AddRange(diagnoses);
                await db.SaveChangesAsync();
            }

            var doctorsList = await db.Doctors.ToListAsync();
            var patientsList = await db.Patients.ToListAsync();

            if (!await db.Appointments.AnyAsync())
            {
                var appointments = Enumerable.Range(1, 20)
                    .Select(i =>
                    {
                        var start = DateTime.Now.AddDays(rnd.Next(-15, 15));

                        return new Appointment
                        {
                            BranchId = branchesList[rnd.Next(branchesList.Count)].Id,
                            DoctorId = doctorsList[rnd.Next(doctorsList.Count)].Id,
                            PatientId = patientsList[rnd.Next(patientsList.Count)].Id,
                            StartAt = start,
                            EndAt = start.AddMinutes(30),
                            CreatedAt = DateTime.UtcNow
                        };
                    }).ToList();

                db.Appointments.AddRange(appointments);
                await db.SaveChangesAsync();
            }

            var appointmentsList = await db.Appointments.ToListAsync();

            if (!await db.Visits.AnyAsync())
            {
                var appointmentsForVisits = await db.Appointments
                    .Include(x => x.Visit)
                    .Where(x => x.Visit == null)
                    .Take(20)
                    .ToListAsync();

                var visits = appointmentsForVisits
                    .Select((appointment, i) => new Visit
                    {
                        Appointment = appointment,
                        DoctorId = appointment.DoctorId,
                        Complaints = $"Жалобы пациента {i + 1}",
                        Anamnesis = $"Анамнез пациента {i + 1}",
                        Examination = $"Осмотр пациента {i + 1}",
                        Recommendations = $"Рекомендации пациенту {i + 1}"
                    })
                    .ToList();

                db.Visits.AddRange(visits);
                await db.SaveChangesAsync();
            }

            var visitsList = await db.Visits.ToListAsync();
            var diagnosesList = await db.Diagnoses.ToListAsync();

            if (!await db.VisitDiagnoses.AnyAsync() && visitsList.Count > 0 && diagnosesList.Count > 0)
            {
                var visitDiagnoses = visitsList
                    .Select((visit, i) => new VisitDiagnosis
                    {
                        VisitId = visit.Id,
                        DiagnosisId = diagnosesList[i % diagnosesList.Count].Id,
                        IsPrimary = true
                    })
                    .ToList();

                db.VisitDiagnoses.AddRange(visitDiagnoses);
                await db.SaveChangesAsync();
            }

            if (!await db.Invoices.AnyAsync())
            {
                var invoices = appointmentsList.Take(20)
                    .Select(a => new Invoice
                    {
                        AppointmentId = a.Id,
                        PatientId = a.PatientId,
                        TotalAmount = rnd.Next(1000, 15000),
                        DiscountAmount = rnd.Next(0, 1000),
                        PaidAmount = rnd.Next(500, 10000)
                    }).ToList();

                db.Invoices.AddRange(invoices);
                await db.SaveChangesAsync();
            }

            var firstDoctor = await db.Doctors.OrderBy(x => x.Id).FirstOrDefaultAsync();
            var doctorUser = await db.Users.FirstOrDefaultAsync(x => x.Login == "doctor");
            if (firstDoctor is not null && doctorUser is not null && doctorUser.DoctorId is null)
            {
                doctorUser.DoctorId = firstDoctor.Id;
                await db.SaveChangesAsync();
            }
        }
    }

}

