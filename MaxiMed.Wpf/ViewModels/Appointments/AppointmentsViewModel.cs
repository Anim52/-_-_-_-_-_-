using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Visits;
using MaxiMed.Wpf.Views.Appointments;
using MaxiMed.Wpf.Views.Visits;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace MaxiMed.Wpf.ViewModels.Appointments
{
    public partial class AppointmentsViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<AppointmentDto> Items { get; } = new();

        public List<LookupItemDto> Doctors { get; private set; } = new();

        [ObservableProperty] private DateTime? selectedDate = DateTime.Today;
        [ObservableProperty] private int? selectedDoctorId; 

        [ObservableProperty] private AppointmentDto? selected;
        [ObservableProperty] private bool isBusy;
        private bool CanChangeStatus =>
    Selected is not null && Selected.Status == AppointmentStatus.Planned;


        public AppointmentsViewModel(IAppointmentService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        public async Task InitAsync()
        {
            Doctors = (await _service.GetActiveDoctorsAsync()).ToList();
            OnPropertyChanged(nameof(Doctors));
            await LoadAsync();
        }
        private readonly ISessionService _session;

        public bool CanEdit => _session.IsInRole("Admin") || _session.IsInRole("Registrar");
        public bool CanCancel => _session.IsInRole("Admin") || _session.IsInRole("Registrar");
        public bool CanComplete => _session.IsInRole("Admin") || _session.IsInRole("Doctor");
        public bool CanOpenVisit => _session.IsInRole("Admin") || _session.IsInRole("Doctor");

        private bool CanWork() => !IsBusy;
        private bool CanEditOrDelete() => !IsBusy && Selected is not null;

        partial void OnIsBusyChanged(bool value)
        {
            LoadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedChanged(AppointmentDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CompleteCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();

        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                Items.Clear();
                var day = (SelectedDate ?? DateTime.Today).Date;

                var list = await _service.GetDayAsync(day, SelectedDoctorId);
                foreach (var x in list) Items.Add(x);
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand(CanExecute = nameof(CanChangeStatus))]
        private async Task CompleteAsync()
        {
            if (Selected is null) return;

            var apptId = (long)Selected.Id;     
            var doctorId = Selected.DoctorId;
            var patientId = Selected!.PatientId;

            await _service.CompleteAsync(Selected.Id);

            await LoadAsync();


            var win = _sp.GetRequiredService<VisitEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (VisitEditViewModel)win.DataContext;
            await vm.LoadOrCreateAsync(apptId, doctorId,patientId);

            win.ShowDialog();
        }

        [RelayCommand(CanExecute = nameof(CanChangeStatus))]
        private async Task CancelAsync()
        {
            await _service.CancelAsync(Selected!.Id);
            await LoadAsync();
        }


        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task AddAsync()
        {
            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            vm.LoadFrom(new AppointmentDto
            {
                DoctorId = SelectedDoctorId ?? 0,
                StartAt = (SelectedDate ?? DateTime.Today).Date.AddHours(10),
                EndAt = (SelectedDate ?? DateTime.Today).Date.AddHours(10).AddMinutes(30),
            }, "Новая запись");

            // дефолты, чтобы не было 0
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

            var id = Selected.Id;

            if (System.Windows.MessageBox.Show(
                    $"Удалить запись ID={id}?",
                    "Подтверждение",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
                return;

            await _service.DeleteAsync(id);
            await LoadAsync();
            Selected = null;
        }
    }
}
