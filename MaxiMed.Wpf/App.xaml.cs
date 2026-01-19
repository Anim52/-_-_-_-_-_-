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
using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels;
using MaxiMed.Wpf.ViewModels.Admin;
using MaxiMed.Wpf.ViewModels.Appointments;
using MaxiMed.Wpf.ViewModels.Auth;
using MaxiMed.Wpf.ViewModels.Branches;
using MaxiMed.Wpf.ViewModels.Diagnoses;
using MaxiMed.Wpf.ViewModels.Doctors;
using MaxiMed.Wpf.ViewModels.Invoices;
using MaxiMed.Wpf.ViewModels.Patients;
using MaxiMed.Wpf.ViewModels.Reports;
using MaxiMed.Wpf.ViewModels.Schedule;
using MaxiMed.Wpf.ViewModels.Services;
using MaxiMed.Wpf.ViewModels.Specialties;
using MaxiMed.Wpf.ViewModels.Visits;
using MaxiMed.Wpf.Views;
using MaxiMed.Wpf.Views.Admin;
using MaxiMed.Wpf.Views.Appointments;
using MaxiMed.Wpf.Views.Auth;
using MaxiMed.Wpf.Views.Branches;
using MaxiMed.Wpf.Views.Diagnoses;
using MaxiMed.Wpf.Views.Doctors;
using MaxiMed.Wpf.Views.Invoices;
using MaxiMed.Wpf.Views.Patients;
using MaxiMed.Wpf.Views.Reports;
using MaxiMed.Wpf.Views.Schedule;
using MaxiMed.Wpf.Views.Services;
using MaxiMed.Wpf.Views.Specialties;
using MaxiMed.Wpf.Views.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace MaxiMed.Wpf;

public partial class App : System.Windows.Application
{
    public static IHost AppHost { get; private set; } = null!;

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContextFactory<MaxiMedDbContext>(options =>
       options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

                services.AddScoped<IPatientService, PatientService>();

                services.AddScoped<IPatientHistoryService, PatientHistoryService>();

                services.AddScoped<IPrescriptionService, PrescriptionService>();

                services.AddScoped<IInvoiceService, InvoiceService>();

                services.AddScoped<IUserAdminService, UserAdminService>();

                services.AddScoped<IInvoiceService, InvoiceService>();

                services.AddScoped<IServiceService, ServiceService>();

                services.AddSingleton<ISessionService, SessionService>();

                services.AddScoped<IAuthService, AuthService>();

                services.AddScoped<IReportService, ReportService>();

                services.AddSingleton<IAppointmentService, AppointmentsService>();

                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IDoctorService, DoctorService>();

                services.AddScoped<IDiagnosisService, DiagnosisService>();

                services.AddSingleton<IPatientService, PatientService>();

                services.AddSingleton<IExcelExportService, ExcelExportService>();

                services.AddSingleton<IAuthFlowService, AuthFlowService>();

                services.AddScoped<IVisitDiagnosisService, VisitDiagnosisService>();

                services.AddScoped<IPaymentService, PaymentService>();

                services.AddScoped<IAuditService, AuditService>();

                services.AddScoped<IAppointmentServiceItemService, AppointmentServiceItemService>();

                services.AddScoped<IAppointmentServicesService, AppointmentServicesService>();

                services.AddScoped<IAttachmentService, AttachmentService>();

                services.AddScoped<IAppointmentServiceItemService, AppointmentServiceItemService>();
                
                services.AddScoped<IUserService, UserService>();

                services.AddScoped<IDoctorDayOffService, DoctorDayOffService>();

                services.AddScoped<IVisitService, VisitService>();

                services.AddSingleton<IBranchService, BranchService>();
                services.AddSingleton<ISpecialtyService, SpecialtyService>();

                services.AddTransient<PatientsViewModel>();
                services.AddTransient<PatientsPage>();

                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<MainWindow>();


                services.AddTransient<PatientEditViewModel>();
                services.AddTransient<PatientEditWindow>();

                services.AddTransient<DoctorsViewModel>();
                services.AddTransient<DoctorsPage>();

                services.AddTransient<DoctorEditViewModel>();
                services.AddTransient<DoctorEditWindow>();

                services.AddTransient<BranchesViewModel>();
                services.AddTransient<BranchesPage>();
                services.AddTransient<BranchEditViewModel>();
                services.AddTransient<BranchEditWindow>();


                services.AddTransient<SpecialtiesViewModel>();
                services.AddTransient<SpecialtiesPage>();
                services.AddTransient<SpecialtyEditViewModel>();
                services.AddTransient<SpecialtyEditWindow>();

                services.AddTransient<AppointmentsViewModel>();
                services.AddTransient<AppointmentsPage>();

                services.AddTransient<AppointmentEditViewModel>();
                services.AddTransient<AppointmentEditWindow>();

                services.AddTransient<DoctorScheduleViewModel>();
                services.AddTransient<DoctorSchedulePage>();

                services.AddTransient<PatientHistoryViewModel>();
                services.AddTransient<PatientHistoryPage>();

                services.AddTransient<FreeSlotsViewModel>();
                services.AddTransient<FreeSlotsPage>();

                services.AddTransient<WeekScheduleViewModel>();
                services.AddTransient<WeekSchedulePage>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<LoginWindow>();

                services.AddTransient<VisitEditViewModel>();
                services.AddTransient<VisitEditWindow>();

                services.AddTransient<DiagnosisPickerViewModel>();
                services.AddTransient<DiagnosisPickerWindow>();

                services.AddTransient<DiagnosisEditViewModel>();
                services.AddTransient<DiagnosisEditWindow>();

                services.AddTransient<DiagnosesViewModel>();
                services.AddTransient<DiagnosesPage>();

                services.AddTransient<ServicesViewModel>();
                services.AddTransient<ServicesPage>();

                services.AddTransient<ServiceEditViewModel>();
                services.AddTransient<ServiceEditWindow>();

                services.AddTransient<ServicePickerViewModel>();
                services.AddTransient<ServicePickerWindow>();

                services.AddTransient<InvoiceViewModel>();
                services.AddTransient<InvoiceWindow>();

                services.AddTransient<PatientsHistoryViewModel>();
                services.AddTransient<PatientHistoryWindow>();

                services.AddTransient<ReportsViewModel>();
                services.AddTransient<ReportsPage>();

                services.AddTransient<AdminViewModel>();
                services.AddTransient<AdminPage>();

                services.AddTransient<UserListViewModel>();
                services.AddTransient<UserListPage>();

                services.AddTransient<UserEditViewModel>();
                services.AddTransient<UserEditWindow>();

                services.AddTransient<AuditViewModel>();
                services.AddTransient<AuditPage>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        using (var scope = AppHost.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MaxiMedDbContext>();
            db.Database.Migrate();
            await DbInitializer.SeedAsync(db);
        }

        var authFlow = AppHost.Services.GetRequiredService<IAuthFlowService>();
        var ok = await authFlow.LoginAndShowMainAsync();
        if (!ok)
        {
            Shutdown();
            return;
        }

        Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        base.OnStartup(e);
    }



    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
