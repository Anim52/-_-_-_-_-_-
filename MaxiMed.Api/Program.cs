using MaxiMed.Application.Appointments;
using MaxiMed.Application.Attachments;
using MaxiMed.Application.Audit;
using MaxiMed.Application.Auth;
using MaxiMed.Application.Common;
using MaxiMed.Application.Diagnoses;
using MaxiMed.Application.Doctors;
using MaxiMed.Application.Invoices;
using MaxiMed.Application.Patients;
using MaxiMed.Application.Reports;
using MaxiMed.Application.Services;
using MaxiMed.Application.Users;
using MaxiMed.Application.Visits;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContextFactory<MaxiMedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IPatientHistoryService, PatientHistoryService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAppointmentService, AppointmentsService>();
builder.Services.AddScoped<IAppointmentServiceItemService, AppointmentServiceItemService>();
builder.Services.AddScoped<IAppointmentServicesService, AppointmentServicesService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDoctorDayOffService, DoctorDayOffService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IVisitDiagnosisService, VisitDiagnosisService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MaxiMedDbContext>>().CreateDbContext();
    db.Database.Migrate();
    await DbInitializer.SeedAsync(db);
}

app.MapControllers();

app.Run();
