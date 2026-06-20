using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.Helpers;
using MaxiMed.Wpf.Api;
using MaxiMed.Wpf.ViewModels.Patients;
using MaxiMed.Wpf.Views.Patients;
using MaxiMed.Wpf.ViewModels.Visits;
using MaxiMed.Wpf.Views.Appointments;
using MaxiMed.Wpf.Views.Visits;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Appointments
{
    public partial class AppointmentsViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;
        private readonly ISessionService _session;
        private readonly ApiClient _api;

        public ObservableCollection<AppointmentDto> Items { get; } = new();

        public bool IsDoctorMode => _session.IsInRole("Doctor");
        public bool IsNotDoctorMode => !IsDoctorMode;

        public List<LookupItemDto> Doctors { get; private set; } = new();

        [ObservableProperty] private DateTime? selectedDate;
        [ObservableProperty] private int? selectedDoctorId;

        [ObservableProperty] private AppointmentDto? selected;
        [ObservableProperty] private bool isBusy;

        private int _loadRequestId;
        private bool _isInitialized;
        private bool _suppressAutoLoad;

        private bool CanWork() => !IsBusy;
        private bool IsCurrentDoctorAppointment() => !_session.IsInRole("Doctor") || (_session.DoctorId.HasValue && Selected?.DoctorId == _session.DoctorId.Value);
        private bool CanEditOrDelete() => !IsBusy && Selected is not null && (_session.IsInRole("Admin") || _session.IsInRole("Registrar"));
        private bool CanChangeStatus => !IsBusy && Selected is not null && Selected.Status == AppointmentStatus.Planned && IsCurrentDoctorAppointment() && (_session.IsInRole("Admin") || _session.IsInRole("Registrar") || _session.IsInRole("Doctor"));
        private bool CanOpenVisitInternal => !IsBusy && Selected is not null && IsCurrentDoctorAppointment() && (_session.IsInRole("Admin") || _session.IsInRole("Doctor"));
        private bool CanPrintOrHistory => !IsBusy && Selected is not null && IsCurrentDoctorAppointment();

        public bool CanEdit => _session.IsInRole("Admin") || _session.IsInRole("Registrar");
        public bool CanCancel => _session.IsInRole("Admin") || _session.IsInRole("Registrar");
        public bool CanComplete => _session.IsInRole("Admin") || _session.IsInRole("Doctor");
        public bool CanOpenVisit => _session.IsInRole("Admin") || _session.IsInRole("Doctor");

        public AppointmentsViewModel(IAppointmentService service, IServiceProvider sp, ISessionService session, ApiClient api)
        {
            _service = service;
            _sp = sp;
            _session = session;
            _api = api;
        }

        public async Task InitAsync()
        {
            _suppressAutoLoad = true;

            var doctors = (await _service.GetActiveDoctorsAsync()).ToList();
            if (_session.IsInRole("Doctor"))
            {
                Doctors = _session.DoctorId.HasValue
                    ? doctors.Where(d => d.Id == _session.DoctorId.Value).ToList()
                    : new List<LookupItemDto>();
                SelectedDoctorId = _session.DoctorId;
            }
            else
            {
                Doctors = doctors;
            }
            OnPropertyChanged(nameof(Doctors));
            OnPropertyChanged(nameof(IsDoctorMode));
            OnPropertyChanged(nameof(IsNotDoctorMode));

            SelectedDate = DateTime.Today;

            _isInitialized = true;
            _suppressAutoLoad = false;

            await LoadAsync();
        }

        partial void OnIsBusyChanged(bool value)
        {
            LoadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CompleteCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            OpenVisitCommand.NotifyCanExecuteChanged();
            PrintTicketCommand.NotifyCanExecuteChanged();
            OpenPatientHistoryCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedChanged(AppointmentDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CompleteCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            OpenVisitCommand.NotifyCanExecuteChanged();
            PrintTicketCommand.NotifyCanExecuteChanged();
            OpenPatientHistoryCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task LoadAsync()
        {
            var requestId = ++_loadRequestId;

            try
            {
                IsBusy = true;

                var day = (SelectedDate ?? DateTime.Today).Date;

                var doctorFilter = _session.IsInRole("Doctor")
                    ? (_session.DoctorId ?? -1)
                    : SelectedDoctorId;

                if (_session.IsInRole("Doctor") && doctorFilter <= 0)
                {
                    Items.Clear();
                    Selected = null;
                    return;
                }

                var list = await _service.GetDayAsync(day, doctorFilter);

                // Если пользователь быстро переключил день/врача, старый ответ не должен затирать новый список.
                if (requestId != _loadRequestId)
                    return;

                Items.Clear();
                foreach (var x in list)
                    Items.Add(x);

                Selected = null;
            }
            finally
            {
                if (requestId == _loadRequestId)
                    IsBusy = false;
            }
        }

        // ✅ открытие/редактирование карты приёма для ЛЮБОЙ записи (в т.ч. Completed)
        [RelayCommand(CanExecute = nameof(CanOpenVisitInternal))]
        private async Task OpenVisitAsync()
        {
            if (Selected is null) return;

            var apptId = (long)Selected.Id;
            var doctorId = Selected.DoctorId;
            var patientId = Selected.PatientId;

            var win = _sp.GetRequiredService<VisitEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (VisitEditViewModel)win.DataContext;
            await vm.LoadOrCreateAsync(apptId, doctorId, patientId);

            win.ShowDialog();
        }

        [RelayCommand(CanExecute = nameof(CanChangeStatus))]
        private async Task CompleteAsync()
        {
            if (Selected is null) return;

            var apptId = (long)Selected.Id;
            var doctorId = Selected.DoctorId;
            var patientId = Selected.PatientId;

            await _service.CompleteAsync(Selected.Id);
            await LoadAsync();

            // после завершения сразу открываем карту приёма
            var win = _sp.GetRequiredService<VisitEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (VisitEditViewModel)win.DataContext;
            await vm.LoadOrCreateAsync(apptId, doctorId, patientId);

            win.ShowDialog();
        }

        [RelayCommand(CanExecute = nameof(CanChangeStatus))]
        private async Task CancelAsync()
        {
            if (Selected is null) return;
            await _service.CancelAsync(Selected.Id);
            await LoadAsync();
        }

        [RelayCommand(CanExecute = nameof(CanPrintOrHistory))]
        private async Task PrintTicketAsync()
        {
            if (Selected is null)
                return;

            var data = await _api.GetAsync<AppointmentTicketPrintDto>($"api/appointments/{Selected.Id}/ticket");

            if (data is null)
                return;

            PrintHelper.PrintTicket(
                data.PatientName,
                data.DoctorName,
                data.BranchName,
                data.StartAt,
                data.EndAt,
                data.Room);
        }

        private sealed class AppointmentTicketPrintDto
        {
            public string PatientName { get; set; } = "";
            public string DoctorName { get; set; } = "";
            public string BranchName { get; set; } = "";
            public string? Room { get; set; }
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
        }

        [RelayCommand(CanExecute = nameof(CanPrintOrHistory))]
        private async Task OpenPatientHistoryAsync()
        {
            if (Selected is null) return;

            var win = _sp.GetRequiredService<PatientHistoryWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (PatientsHistoryViewModel)win.DataContext;
            await vm.LoadAsync(Selected.PatientId);

            win.ShowDialog();
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task AddAsync()
        {
            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            var day = (SelectedDate ?? DateTime.Today).Date;

            vm.LoadFrom(new AppointmentDto
            {
                DoctorId = SelectedDoctorId ?? 0,
                StartAt = day.AddHours(9),
                EndAt = day.AddHours(9).AddMinutes(30),
            }, "Новая запись");

            if (vm.Branches.Count > 0 && vm.BranchId == 0)
                vm.BranchId = vm.Branches[0].Id;

            if (vm.Doctors.Count > 0 && vm.DoctorId == 0)
                vm.DoctorId = vm.Doctors[0].Id;

            await vm.InitDefaultSlotAsync();

            if (vm.Branches.Count > 0 && vm.BranchId == 0) vm.BranchId = vm.Branches[0].Id;
            if (vm.Doctors.Count > 0 && vm.DoctorId == 0) vm.DoctorId = vm.Doctors[0].Id;

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task EditAsync()
        {
            if (Selected is null) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            vm.LoadFrom(Selected, "Редактирование записи");
            await vm.SetPatientByIdAsync(Selected.PatientId);

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task DeleteAsync()
        {
            if (Selected is null) return;

            await _service.DeleteAsync(Selected.Id);
            await LoadAsync();
        }
        partial void OnSelectedDateChanged(DateTime? value)
        {
            if (value is not null)
                _ = AutoLoadAsync();
        }

        partial void OnSelectedDoctorIdChanged(int? value)
        {
            _ = AutoLoadAsync();
        }

        private async Task AutoLoadAsync()
        {
            if (!_isInitialized || _suppressAutoLoad)
                return;

            await LoadAsync();
        }
    }
}
