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
using MaxiMed.Wpf.Views.Themes;
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
        #region Propertyes
        public bool IsAdmin => _session.IsInRole("Admin");
        public bool IsRegistrar => _session.IsInRole("Registrar");
        public bool IsDoctor => _session.IsInRole("Doctor");
        public bool CanSeePatients => IsAdmin || IsRegistrar;
        public bool CanSeeAppointments => IsAdmin || IsRegistrar || IsDoctor;
        public bool CanSeeWeekSchedule => IsAdmin || IsDoctor;
        public bool CanSeeFreeSlots => IsAdmin || IsRegistrar; 
        public bool CanSeeAdminPanel => IsAdmin;
        public bool CanSeeDoctors => IsAdmin;
        public bool CanSeeScheduleDay => IsAdmin || IsDoctor;
        public bool CanSeeDiagnoses => IsAdmin || IsDoctor;
        public bool CanSeeServices => IsAdmin || IsRegistrar;
        public bool CanOpenBranches => IsAdmin;
        public bool CanSeeSpecialties => IsAdmin;
        #endregion
        private readonly IServiceProvider _sp;

        public MainWindowViewModel(INavigationService nav, ISessionService session, IServiceProvider sp,IAuthFlowService authFlow)
        {
            _nav = nav;
            _session = session;
            _sp = sp;
            _authFlow = authFlow;
        }
        #region PropertyesOpen
        public bool CanSeeReports =>_session.IsInRole("Admin") || _session.IsInRole("Manager");
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
        #endregion
        #region Command

        [RelayCommand(CanExecute = nameof(CanSeeReports))]
        private void OpenReports()
        {
            _nav.NavigateTo<ReportsPage>();
            SetActive("reports");
        }


        [RelayCommand(CanExecute = nameof(CanOpenPatients))]
        private void OpenPatients()
        {
            _nav.NavigateTo<PatientsPage>();
            SetActive("patients");
        }


        [RelayCommand(CanExecute = nameof(CanOpenDoctors))]
        private void OpenDoctors()
        {
            _nav.NavigateTo<DoctorsPage>();
            SetActive("doctors");
        }


        [RelayCommand(CanExecute = nameof(CanOpenAppointments))]
        private void OpenAppointments()
        {
            _nav.NavigateTo<AppointmentsPage>();
            SetActive("appointments");
        }


        [RelayCommand(CanExecute = nameof(CanOpenSchedule))]
        private void OpenSchedule()
        {
            _nav.NavigateTo<DoctorSchedulePage>();
            SetActive("schedule");
        }


        [RelayCommand(CanExecute = nameof(CanOpenWeekSchedule))]
        private void OpenWeekSchedule()
        {
            _nav.NavigateTo<WeekSchedulePage>();
            SetActive("week");
        }


        [RelayCommand(CanExecute = nameof(CanOpenFreeSlots))]
        private void OpenFreeSlots()
        {
            _nav.NavigateTo<FreeSlotsPage>();
            SetActive("freeSlots");
        }


        [RelayCommand(CanExecute = nameof(CanOpenSpecialties))]
        private void OpenSpecialties()
        {
            _nav.NavigateTo<SpecialtiesPage>();
            SetActive("specialties");
        }


        [RelayCommand(CanExecute = nameof(CanOpenBranches))]
        private void OpenBranches()
        {
            _nav.NavigateTo<BranchesPage>();
            SetActive("branches");
        }


        [RelayCommand(CanExecute = nameof(CanOpenDiagnoses))]
        private void OpenDiagnoses()
        {
            _nav.NavigateTo<DiagnosesPage>();
            SetActive("diagnoses");
        }


        [RelayCommand(CanExecute = nameof(CanOpenServices))]
        private void OpenServices()
        {
            _nav.NavigateTo<ServicesPage>();
            SetActive("services");
        }



        [RelayCommand(CanExecute = nameof(CanOpenAdmin))]
        private void OpenAdmin()
        {
            _nav.NavigateTo<AdminPage>();
            SetActive("admin");
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _nav.NavigateTo<SettingsPage>();
            SetActive("settings");
        }

        [RelayCommand]
        private async Task LogoutUserAsync()
        {
            await _authFlow.LogoutToLoginAsync();
        }

        [RelayCommand]
        private void Logout()
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion
        #region Active
        private bool _isPatientsActive;
        public bool IsPatientsActive { get => _isPatientsActive; set => SetProperty(ref _isPatientsActive, value); }

        private bool _isDoctorsActive;
        public bool IsDoctorsActive { get => _isDoctorsActive; set => SetProperty(ref _isDoctorsActive, value); }

        private bool _isSpecialtiesActive;
        public bool IsSpecialtiesActive { get => _isSpecialtiesActive; set => SetProperty(ref _isSpecialtiesActive, value); }

        private bool _isBranchesActive;
        public bool IsBranchesActive { get => _isBranchesActive; set => SetProperty(ref _isBranchesActive, value); }

        private bool _isAppointmentsActive;
        public bool IsAppointmentsActive { get => _isAppointmentsActive; set => SetProperty(ref _isAppointmentsActive, value); }

        private bool _isScheduleDayActive;
        public bool IsScheduleDayActive { get => _isScheduleDayActive; set => SetProperty(ref _isScheduleDayActive, value); }

        private bool _isWeekScheduleActive;
        public bool IsWeekScheduleActive { get => _isWeekScheduleActive; set => SetProperty(ref _isWeekScheduleActive, value); }

        private bool _isFreeSlotsActive;
        public bool IsFreeSlotsActive { get => _isFreeSlotsActive; set => SetProperty(ref _isFreeSlotsActive, value); }

        private bool _isDiagnosesActive;
        public bool IsDiagnosesActive { get => _isDiagnosesActive; set => SetProperty(ref _isDiagnosesActive, value); }

        private bool _isServicesActive;
        public bool IsServicesActive { get => _isServicesActive; set => SetProperty(ref _isServicesActive, value); }

        private bool _isReportsActive;
        public bool IsReportsActive { get => _isReportsActive; set => SetProperty(ref _isReportsActive, value); }

        private bool _isAdminActive;
        public bool IsAdminActive { get => _isAdminActive; set => SetProperty(ref _isAdminActive, value); }

        private bool _isSettingsActive;
        public bool IsSettingsActive { get => _isSettingsActive; set => SetProperty(ref _isSettingsActive, value); }
        #endregion
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
        private void ResetActive()
        {
            IsReportsActive = false;
            IsPatientsActive = false;
            IsDoctorsActive = false;
            IsAppointmentsActive = false;
            IsScheduleDayActive = false;
            IsWeekScheduleActive = false;
            IsFreeSlotsActive = false;
            IsSpecialtiesActive = false;
            IsBranchesActive = false;
            IsDiagnosesActive = false;
            IsServicesActive = false;
            IsAdminActive = false;
            IsSettingsActive = false;
        }

        private void SetActive(string key)
        {
            ResetActive();

            switch (key)
            {
                case "reports": IsReportsActive = true; break;
                case "patients": IsPatientsActive = true; break;
                case "doctors": IsDoctorsActive = true; break;
                case "appointments": IsAppointmentsActive = true; break;
                case "schedule": IsScheduleDayActive = true; break;
                case "week": IsWeekScheduleActive = true; break;
                case "freeSlots": IsFreeSlotsActive = true; break;
                case "specialties": IsSpecialtiesActive = true; break;
                case "branches": IsBranchesActive = true; break;
                case "diagnoses": IsDiagnosesActive = true; break;
                case "services": IsServicesActive = true; break;
                case "admin": IsAdminActive = true; break;
                case "settings": IsSettingsActive = true; break;
            }
        }


    }
}
