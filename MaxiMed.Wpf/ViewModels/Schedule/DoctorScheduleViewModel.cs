using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Appointments;
using MaxiMed.Wpf.Views.Appointments;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Schedule
{
    public partial class DoctorScheduleViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;
        private readonly ISessionService _session;

        public bool IsDoctorMode => _session.IsInRole("Doctor");
        public bool IsNotDoctorMode => !IsDoctorMode;

        public List<LookupItemDto> Doctors { get; private set; } = new();

        [ObservableProperty] private DateTime? selectedDate = DateTime.Today;
        [ObservableProperty] private int? selectedDoctorId;

        public ObservableCollection<TimeSlotVm> Slots { get; } = new();
        [ObservableProperty] private TimeSlotVm? selectedSlot;

        [ObservableProperty] private bool isBusy;

        public DoctorScheduleViewModel(IAppointmentService service, IServiceProvider sp, ISessionService session)
        {
            _service = service;
            _sp = sp;
            _session = session;
        }

        public async Task InitAsync()
        {
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
                if (SelectedDoctorId is null && Doctors.Count > 0)
                    SelectedDoctorId = Doctors[0].Id;
            }

            OnPropertyChanged(nameof(Doctors));
            OnPropertyChanged(nameof(IsDoctorMode));
            OnPropertyChanged(nameof(IsNotDoctorMode));

            await LoadAsync();
        }

        private bool CanLoad() => !IsBusy && SelectedDoctorId is not null && SelectedDoctorId > 0;

        [RelayCommand(CanExecute = nameof(CanLoad))]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                Slots.Clear();

                var day = (SelectedDate ?? DateTime.Today).Date;
                var did = _session.IsInRole("Doctor") ? _session.DoctorId : SelectedDoctorId;

                // берём записи врача за день
                var appts = await _service.GetDayAsync(day, did);

                // настройки расписания
                var workStart = day.AddHours(9);
                var workEnd = day.AddHours(18);
                var stepMinutes = 30;

                // генерим слоты
                for (var t = workStart; t < workEnd; t = t.AddMinutes(stepMinutes))
                {
                    var end = t.AddMinutes(stepMinutes);

                    var hit = appts.FirstOrDefault(a =>
                        a.StartAt < end &&
                        a.EndAt > t &&
                        a.Status != AppointmentStatus.Canceled); // отменённые считаем свободными

                    if (hit is null)
                    {
                        Slots.Add(new TimeSlotVm
                        {
                            StartAt = t,
                            EndAt = end,
                            IsFree = true
                        });
                    }
                    else
                    {
                        Slots.Add(new TimeSlotVm
                        {
                            StartAt = t,
                            EndAt = end,
                            IsFree = false,
                            AppointmentId = hit.Id,
                            PatientName = hit.PatientName,
                            BranchName = hit.BranchName
                        });
                    }
                }
            }
            finally
            {
                IsBusy = false;
                LoadCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private async Task AddFromSlotAsync(TimeSlotVm slot)
        {
            if (slot is null || !slot.IsFree) return;

            var currentDoctorId = _session.IsInRole("Doctor") ? _session.DoctorId : SelectedDoctorId;
            if (currentDoctorId is null || currentDoctorId <= 0) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            vm.LoadFrom(new AppointmentDto
            {
                DoctorId = currentDoctorId.Value,
                StartAt = slot.StartAt,
                EndAt = slot.EndAt
            }, "Новая запись");

            // дефолт филиала (если есть)
            if (vm.Branches.Count > 0 && vm.BranchId == 0)
                vm.BranchId = vm.Branches[0].Id;

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async Task OpenAppointmentAsync(TimeSlotVm slot)
        {
            if (slot?.AppointmentId is null) return;

            // Открываем через обычный диалог редактирования:
            // возьмём все записи дня и найдём нужную
            var day = (SelectedDate ?? DateTime.Today).Date;
            var currentDoctorId = _session.IsInRole("Doctor") ? _session.DoctorId : SelectedDoctorId;
            var appts = await _service.GetDayAsync(day, currentDoctorId);
            var dto = appts.FirstOrDefault(x => x.Id == slot.AppointmentId.Value);
            if (dto is null) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();
            vm.LoadFrom(dto, "Редактирование записи");

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
                await LoadAsync();
        }
    }
}
