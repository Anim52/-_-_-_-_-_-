using MaxiMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    public class MaxiMedDbContext : DbContext
    {
        public MaxiMedDbContext(DbContextOptions<MaxiMedDbContext> options) : base(options) { }

        public DbSet<ClinicBranch> ClinicBranches => Set<ClinicBranch>();
        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<PriceList> PriceLists => Set<PriceList>();
        public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();
        public DbSet<VisitDiagnosis> VisitDiagnoses => Set<VisitDiagnosis>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
        public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();

        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();
        public DbSet<Visit> Visits => Set<Visit>();
        public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<Attachment> Attachments => Set<Attachment>();

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();   
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.HasDefaultSchema("dbo");

            b.Entity<UserRole>()
        .HasKey(x => new { x.UserId, x.RoleId });

            b.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId);

            b.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId);

            b.Entity<User>()
                .HasIndex(x => x.Login)
                .IsUnique();
            // --- ClinicBranch
            b.Entity<ClinicBranch>(e =>
            {
                e.ToTable("ClinicBranch");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Address).HasMaxLength(250);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.HasIndex(x => x.Name).IsUnique();
            });
            b.Entity<Visit>()
      .HasOne(v => v.Appointment)
      .WithOne(a => a.Visit)                 // в Appointment должен быть Visit? Visit {get;set;}
      .HasForeignKey<Visit>(v => v.AppointmentId)
      .OnDelete(DeleteBehavior.NoAction);    // чтобы не поймать multiple cascade paths

            // Visit: many-to-one к Doctor
            b.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany()                            // если у Doctor нет коллекции Visits
                .HasForeignKey(v => v.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // VisitDiagnosis: составной ключ
            b.Entity<VisitDiagnosis>(e =>
            {
                e.ToTable("VisitDiagnosis"); // 🔥 ВАЖНО

                e.HasKey(x => new { x.VisitId, x.DiagnosisId });

                e.HasOne(x => x.Visit)
                    .WithMany(v => v.Diagnoses)
                    .HasForeignKey(x => x.VisitId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Diagnosis)
                    .WithMany(d => d.Visits)
                    .HasForeignKey(x => x.DiagnosisId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Diagnosis: уникальность кода
            b.Entity<Diagnosis>()
                .HasIndex(x => x.Code)
                .IsUnique();

            // Prescription: Visit 1-N
            b.Entity<Prescription>()
                .HasOne(p => p.Visit)
                .WithMany(v => v.Prescriptions)
                .HasForeignKey(p => p.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
            // --- Specialty
            b.Entity<Specialty>(e =>
            {
                e.ToTable("Specialty");
                e.Property(x => x.Name).HasMaxLength(120).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();
            });

            // --- Doctor
            b.Entity<Doctor>(e =>
            {
                e.ToTable("Doctor");
                e.Property(x => x.FullName).HasMaxLength(150).IsRequired();
                e.Property(x => x.Room).HasMaxLength(20);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.Property(x => x.Email).HasMaxLength(120);

                e.HasOne(x => x.Specialty).WithMany(s => s.Doctors).HasForeignKey(x => x.SpecialtyId);
                e.HasOne(x => x.Branch).WithMany(bc => bc.Doctors).HasForeignKey(x => x.BranchId);

                e.HasIndex(x => x.BranchId);
                e.HasIndex(x => x.SpecialtyId);
            });

            // --- Service
            b.Entity<Service>(e =>
            {
                e.ToTable("Service");
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.BasePrice).HasColumnType("decimal(12,2)");
                e.HasIndex(x => x.Name).IsUnique();
            });

            // --- PriceList
            b.Entity<PriceList>(e =>
            {
                e.ToTable("PriceList");
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.ValidFrom).HasConversion<DateOnlyConverter, DateOnlyComparer>();
                e.Property(x => x.ValidTo).HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });

            // --- PriceListItem
            b.Entity<PriceListItem>(e =>
            {
                e.ToTable("PriceListItem");
                e.Property(x => x.Price).HasColumnType("decimal(12,2)");
                e.HasOne(x => x.PriceList).WithMany(p => p.Items).HasForeignKey(x => x.PriceListId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Service).WithMany(s => s.PriceListItems).HasForeignKey(x => x.ServiceId);
                e.HasIndex(x => new { x.PriceListId, x.ServiceId }).IsUnique();
            });

            // --- Patient
            b.Entity<Patient>(e =>
            {
                e.ToTable("Patient");
                e.Property(x => x.LastName).HasMaxLength(80).IsRequired();
                e.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
                e.Property(x => x.MiddleName).HasMaxLength(80);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.Property(x => x.Email).HasMaxLength(120);
                e.Property(x => x.Address).HasMaxLength(250);
                e.Property(x => x.Notes).HasMaxLength(1000);

                e.Property(x => x.Sex).HasConversion<int>(); // enum -> int
                e.Property(x => x.BirthDate).HasConversion<DateOnlyConverter, DateOnlyComparer>();

                e.HasIndex(x => x.Phone);
                e.HasIndex(x => new { x.LastName, x.FirstName, x.MiddleName });
            });

            b.Entity<PatientDocument>(e =>
            {
                e.ToTable("PatientDocument");
                e.Property(x => x.DocumentType).HasConversion<int>();
                e.Property(x => x.Series).HasMaxLength(20);
                e.Property(x => x.Number).HasMaxLength(30).IsRequired();
                e.Property(x => x.IssuedBy).HasMaxLength(150);
                e.Property(x => x.IssuedAt).HasConversion<DateOnlyConverter, DateOnlyComparer>();

                e.HasOne(x => x.Patient).WithMany(p => p.Documents).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.PatientId);
            });

            b.Entity<InsurancePolicy>(e =>
            {
                e.ToTable("InsurancePolicy");
                e.Property(x => x.Company).HasMaxLength(120).IsRequired();
                e.Property(x => x.PolicyNumber).HasMaxLength(50).IsRequired();
                e.Property(x => x.ValidTo).HasConversion<DateOnlyConverter, DateOnlyComparer>();
                e.HasOne(x => x.Patient).WithMany(p => p.InsurancePolicies).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.PatientId);
            });

            // --- DoctorSchedule
            b.Entity<DoctorSchedule>(e =>
            {
                e.ToTable("DoctorSchedule");
                e.Property(x => x.DayOfWeek).IsRequired();
                e.Property(x => x.StartTime).HasConversion<TimeOnlyConverter, TimeOnlyComparer>();
                e.Property(x => x.EndTime).HasConversion<TimeOnlyConverter, TimeOnlyComparer>();
                e.HasOne(x => x.Doctor).WithMany(d => d.Schedules).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.DoctorId);
            });

            // --- Appointment
            b.Entity<Appointment>(e =>
            {
                e.ToTable("Appointment");
                e.Property(x => x.Status).HasConversion<int>();
                e.Property(x => x.CancelReason).HasMaxLength(250);

                e.HasOne(x => x.Branch)
                    .WithMany(bc => bc.Appointments)
                    .HasForeignKey(x => x.BranchId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(x => x.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => new { x.DoctorId, x.StartAt });
                e.HasIndex(x => new { x.PatientId, x.StartAt });
                e.HasIndex(x => new { x.BranchId, x.StartAt });
            });

            b.Entity<AppointmentService>(e =>
            {
                e.ToTable("AppointmentService");
                e.Property(x => x.Price).HasColumnType("decimal(12,2)");
                e.HasOne(x => x.Appointment).WithMany(a => a.Services).HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId);
                e.HasIndex(x => x.AppointmentId);
            });

           

            // --- Attachment
            b.Entity<Attachment>(e =>
            {
                e.ToTable("Attachment");
                e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                e.Property(x => x.ContentType).HasMaxLength(80);
                e.Property(x => x.StoragePath).HasMaxLength(400);

                e.HasOne(x => x.Patient).WithMany(p => p.Attachments).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Visit).WithMany(v => v.Attachments).HasForeignKey(x => x.VisitId).OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(x => x.PatientId);
                e.HasIndex(x => x.VisitId);
            });

            // --- Billing
            b.Entity<Invoice>(e =>
            {
                e.ToTable("Invoice");
                e.Property(x => x.TotalAmount).HasColumnType("decimal(12,2)");
                e.Property(x => x.DiscountAmount).HasColumnType("decimal(12,2)");
                e.Property(x => x.PaidAmount).HasColumnType("decimal(12,2)");

                e.HasOne(x => x.Appointment).WithOne(a => a.Invoice).HasForeignKey<Invoice>(x => x.AppointmentId);
                e.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId);

                e.HasIndex(x => x.AppointmentId).IsUnique();
            });

            b.Entity<InvoiceItem>(e =>
            {
                e.ToTable("InvoiceItem");
                e.Property(x => x.Price).HasColumnType("decimal(12,2)");
                e.HasOne(x => x.Invoice).WithMany(i => i.Items).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId);
                e.HasIndex(x => x.InvoiceId);
            });

            b.Entity<Payment>(e =>
            {
                e.ToTable("Payment");
                e.Property(x => x.Method).HasConversion<int>();
                e.Property(x => x.Amount).HasColumnType("decimal(12,2)");
                e.HasOne(x => x.Invoice).WithMany(i => i.Payments).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.InvoiceId);
            });

            // --- Auth
            b.Entity<User>(e =>
            {
                e.ToTable("User");
                e.Property(x => x.Login).HasMaxLength(60).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
                e.Property(x => x.FullName).HasMaxLength(150);
                e.HasIndex(x => x.Login).IsUnique();
            });

            b.Entity<Role>(e =>
            {
                e.ToTable("Role");
                e.Property(x => x.Name).HasMaxLength(60).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();
            });

            b.Entity<UserRole>(e =>
            {
                e.ToTable("UserRole");
                e.HasKey(x => new { x.UserId, x.RoleId });
                e.HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Role).WithMany(r => r.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<AuditLog>(e =>
            {
                e.ToTable("AuditLog");
                e.Property(x => x.Action).HasMaxLength(80).IsRequired();
                e.Property(x => x.Entity).HasMaxLength(80).IsRequired();
                e.Property(x => x.EntityId).HasMaxLength(60);
                e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
                e.HasIndex(x => x.At);
            });
        }
    }
}
