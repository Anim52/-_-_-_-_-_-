using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.ExtendedProperties;
using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.Views;
using MaxiMed.Wpf.Views.Admin;
using MaxiMed.Wpf.Views.Appointments;
using MaxiMed.Wpf.Views.Auth;
using MaxiMed.Wpf.Views.Branches;
using MaxiMed.Wpf.Views.Diagnoses;
using MaxiMed.Wpf.Views.Doctors;
using MaxiMed.Wpf.Views.Patients;
using MaxiMed.Wpf.Views.Reports;
using MaxiMed.Wpf.Views.Schedule;
using MaxiMed.Wpf.Views.Services;
using MaxiMed.Wpf.Views.Specialties;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaxiMed.Wpf.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly ISessionService _session;
        private readonly IAuthFlowService _authFlow;

        public bool IsAdmin => _session.IsInRole("Admin");
        public bool IsRegistrar => _session.IsInRole("Registrar");
        public bool IsDoctor => _session.IsInRole("Doctor");
        public bool CanSeePatients => IsAdmin || IsRegistrar;
        public bool CanSeeAppointments => IsAdmin || IsRegistrar;
        public bool CanSeeWeekSchedule => IsAdmin || IsDoctor;
        public bool CanSeeFreeSlots => IsAdmin || IsRegistrar; 
        public bool CanSeeAdminPanel => IsAdmin;
        public bool CanSeeDoctors => IsAdmin;
        public bool CanSeeScheduleDay => IsAdmin || IsDoctor;
        public bool CanSeeDiagnoses => IsAdmin || IsDoctor;
        public bool CanSeeServices => IsAdmin || IsRegistrar;
        public bool CanOpenBranches => IsAdmin;
        public bool CanSeeSpecialties => IsAdmin;

        private readonly IServiceProvider _sp;

        public MainWindowViewModel(INavigationService nav, ISessionService session, IServiceProvider sp,IAuthFlowService authFlow)
        {
            _nav = nav;
            _session = session;
            _sp = sp;
            _authFlow = authFlow;
        }

        [RelayCommand]
        private async Task LogoutUserAsync()
        {
            await _authFlow.LogoutToLoginAsync();
        }


        public bool CanSeeReports =>
       _session.IsInRole("Admin") ||
       _session.IsInRole("Manager");

        [RelayCommand(CanExecute = nameof(CanSeeReports))]
        private void OpenReports()
            => _nav.NavigateTo<ReportsPage>();


        // --- Команды навигации
        private bool CanOpenPatients() => CanSeePatients;
        private bool CanOpenDoctors() => CanSeeDoctors;
        private bool CanOpenAppointments() => CanSeeAppointments;
        private bool CanOpenSchedule() => CanSeeScheduleDay;
        private bool CanOpenWeekSchedule() => CanSeeWeekSchedule;
        private bool CanOpenFreeSlots() => CanSeeFreeSlots;
        private bool CanOpenAdmin() => CanSeeAdminPanel;
        private bool CanOpenDiagnoses() => CanSeeDiagnoses;
        private bool CanOpenServices() => CanSeeServices;
        private bool CanOpenReports() => CanSeeReports;

        private bool CanOpenSpecialties() => CanSeeSpecialties;

        // --- Команды навигации
        [RelayCommand(CanExecute = nameof(CanOpenPatients))]
        private void OpenPatients() => _nav.NavigateTo<PatientsPage>();

        [RelayCommand(CanExecute = nameof(CanOpenDoctors))]
        private void OpenDoctors() => _nav.NavigateTo<DoctorsPage>();

        [RelayCommand(CanExecute = nameof(CanOpenAppointments))]
        private void OpenAppointments() => _nav.NavigateTo<AppointmentsPage>();

        [RelayCommand(CanExecute = nameof(CanOpenSchedule))]
        private void OpenSchedule() => _nav.NavigateTo<DoctorSchedulePage>();

        [RelayCommand(CanExecute = nameof(CanOpenWeekSchedule))]
        private void OpenWeekSchedule() => _nav.NavigateTo<WeekSchedulePage>();

        [RelayCommand(CanExecute = nameof(CanOpenFreeSlots))]
        private void OpenFreeSlots() => _nav.NavigateTo<FreeSlotsPage>();

        [RelayCommand(CanExecute = nameof(CanOpenSpecialties))]
        private void OpenSpecialties() => _nav.NavigateTo<SpecialtiesPage>();

        [RelayCommand(CanExecute = nameof(CanOpenBranches))]
        private void OpenBranches() => _nav.NavigateTo<BranchesPage>();

        [RelayCommand(CanExecute = nameof(CanOpenDiagnoses))]
        private void OpenDiagnoses() => _nav.NavigateTo<DiagnosesPage>();

        [RelayCommand(CanExecute = nameof(CanOpenServices))]
        private void OpenServices() => _nav.NavigateTo<ServicesPage>();



        [RelayCommand(CanExecute = nameof(CanOpenAdmin))]
        private void OpenAdmin() => _nav.NavigateTo<AdminPage>();


        // Выход — позже красиво сделаем “возврат на окно логина”
        [RelayCommand]
        private void Logout()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void RefreshPermissions()
        {
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsRegistrar));
            OnPropertyChanged(nameof(IsDoctor));

            OnPropertyChanged(nameof(CanSeePatients));
            OnPropertyChanged(nameof(CanSeeAppointments));
            OnPropertyChanged(nameof(CanSeeWeekSchedule));
            OnPropertyChanged(nameof(CanSeeFreeSlots));
            OnPropertyChanged(nameof(CanSeeAdminPanel));
            OnPropertyChanged(nameof(CanSeeDoctors));
            OnPropertyChanged(nameof(CanSeeScheduleDay));
            OnPropertyChanged(nameof(CanSeeDiagnoses));
            OnPropertyChanged(nameof(CanSeeServices));
            OnPropertyChanged(nameof(CanOpenBranches));
            OnPropertyChanged(nameof(CanSeeSpecialties));
            OnPropertyChanged(nameof(CanSeeReports));

            OpenPatientsCommand.NotifyCanExecuteChanged();
            OpenDoctorsCommand.NotifyCanExecuteChanged();
            OpenAppointmentsCommand.NotifyCanExecuteChanged();
            OpenScheduleCommand.NotifyCanExecuteChanged();
            OpenWeekScheduleCommand.NotifyCanExecuteChanged();
            OpenFreeSlotsCommand.NotifyCanExecuteChanged();
            OpenDiagnosesCommand.NotifyCanExecuteChanged();
            OpenServicesCommand.NotifyCanExecuteChanged();
            OpenBranchesCommand.NotifyCanExecuteChanged();
            OpenSpecialtiesCommand.NotifyCanExecuteChanged();
            OpenReportsCommand.NotifyCanExecuteChanged();
            OpenAdminCommand.NotifyCanExecuteChanged();

        }

    }
}
