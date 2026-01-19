using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Application.Services;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Appointments;
using MaxiMed.Wpf.Views.Appointments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Schedule
{
    public partial class WeekScheduleViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;
        private readonly IExcelExportService _excel;
        private readonly IDoctorDayOffService _dayOffService;
        private readonly ISessionService _session;

        public bool CanSetDayOff =>
            _session.IsInRole("Admin") || _session.IsInRole("Registrar");

        // 🔹 Выбранный день недели для пометки как нерабочий
        [ObservableProperty]
        private DateTime? selectedDayForDayOff;

        public List<LookupItemDto> Doctors { get; private set; } = new();

        [ObservableProperty] private int? selectedDoctorId;
        [ObservableProperty] private DateTime weekStart = DateTime.Today;

        [ObservableProperty] private string? weekTitle;
        [ObservableProperty] private string? doctorTitle;

        public ObservableCollection<WeekRowVm> Rows { get; } = new();
        public List<DateTime> Days { get; } = new();

        public WeekScheduleViewModel(
            IAppointmentService service,
            IServiceProvider sp,
            IExcelExportService excel,
            IDoctorDayOffService dayOffService,
            ISessionService session)
        {
            _service = service;
            _sp = sp;
            _excel = excel;
            _dayOffService = dayOffService;
            _session = session;
        }

        partial void OnSelectedDoctorIdChanged(int? value)
        {
            UpdateHeader();
        }

        public async Task InitAsync()
        {
            Doctors = (await _service.GetActiveDoctorsAsync()).ToList();
            OnPropertyChanged(nameof(Doctors));

            if (Doctors.Count > 0)
                SelectedDoctorId = Doctors[0].Id;
            else
                SelectedDoctorId = null;

            SetWeekStart(DateTime.Today);

            await LoadAsync();
        }

        public void SetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            WeekStart = date.Date.AddDays(-diff);
            UpdateHeader();
        }

        private void UpdateHeader()
        {
            var start = WeekStart.Date;
            var end = start.AddDays(6);
            WeekTitle = $"{start:dd.MM.yyyy} – {end:dd.MM.yyyy}";

            if (SelectedDoctorId is null || SelectedDoctorId == 0)
            {
                DoctorTitle = "Врач не выбран";
                return;
            }

            var doc = Doctors.FirstOrDefault(d => d.Id == SelectedDoctorId.Value);
            if (doc is null || string.IsNullOrWhiteSpace(doc.Name))
            {
                DoctorTitle = "Врач не выбран";
                return;
            }

            var name = doc.Name;
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                DoctorTitle = name;
            }
            else if (parts.Length == 1)
            {
                DoctorTitle = parts[0];
            }
            else
            {
                var last = parts[0];
                var firstInitial = parts.Length > 1 && parts[1].Length > 0
                    ? parts[1][0] + "."
                    : "";
                var middleInitial = parts.Length > 2 && parts[2].Length > 0
                    ? parts[2][0] + "."
                    : "";

                DoctorTitle = $"{last} {firstInitial}{middleInitial}";
            }

            OnPropertyChanged(nameof(DoctorTitle));
            OnPropertyChanged(nameof(WeekTitle));
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            Rows.Clear();
            Days.Clear();

            var start = WeekStart.Date;

            for (int i = 0; i < 7; i++)
                Days.Add(start.AddDays(i));

            OnPropertyChanged(nameof(Days));

            // по умолчанию выбран первый день недели, если ещё не выбран
            if (SelectedDayForDayOff is null && Days.Count > 0)
                SelectedDayForDayOff = Days[0];

            if (SelectedDoctorId is null || SelectedDoctorId == 0)
                return;

            var doctorId = SelectedDoctorId.Value;

            // какие дни нерабочие
            var dayOffSet = new HashSet<DateTime>();
            foreach (var d in Days)
            {
                if (await _dayOffService.IsDayOffAsync(doctorId, d, default))
                    dayOffSet.Add(d.Date);
            }

            // приёмы врача за неделю
            var all = new List<AppointmentDto>();
            foreach (var d in Days)
                all.AddRange(await _service.GetDayAsync(d, doctorId));

            var workDayStart = start.AddHours(9);
            var workDayEnd = start.AddHours(18);

            for (var t = workDayStart; t < workDayEnd; t = t.AddMinutes(30))
            {
                var row = new WeekRowVm { Time = t.ToString("HH:mm") };

                foreach (var day in Days)
                {
                    var slotStart = day.Date.Add(t.TimeOfDay);
                    var slotEnd = slotStart.AddMinutes(30);

                    if (dayOffSet.Contains(day.Date))
                    {
                        row.Slots.Add(new WeekSlotVm
                        {
                            StartAt = slotStart,
                            EndAt = slotEnd,
                            IsFree = false,
                            IsDayOff = true,
                            PatientName = "Нерабочий день"
                        });
                        continue;
                    }

                    var hit = all.FirstOrDefault(a =>
                        a.StartAt < slotEnd &&
                        a.EndAt > slotStart &&
                        a.Status != AppointmentStatus.Canceled);

                    if (hit == null)
                    {
                        row.Slots.Add(new WeekSlotVm
                        {
                            StartAt = slotStart,
                            EndAt = slotEnd,
                            IsFree = true,
                            IsDayOff = false
                        });
                    }
                    else
                    {
                        row.Slots.Add(new WeekSlotVm
                        {
                            StartAt = slotStart,
                            EndAt = slotEnd,
                            IsFree = false,
                            IsDayOff = false,
                            AppointmentId = hit.Id,
                            PatientName = hit.PatientName
                        });
                    }
                }

                Rows.Add(row);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSetDayOff))]
        private async Task SetDayOffAsync()
        {
            if (SelectedDoctorId is null || SelectedDoctorId == 0)
                return;

            var day = SelectedDayForDayOff ?? WeekStart.Date;

            await _dayOffService.AddDayOffAsync(SelectedDoctorId.Value, day, "Нерабочий день");
            await LoadAsync();
        }

        [RelayCommand]
        private async Task AddFromSlotAsync(WeekSlotVm slot)
        {
            if (slot is null || !slot.IsFree) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            vm.LoadFrom(new AppointmentDto
            {
                DoctorId = SelectedDoctorId ?? 0,
                StartAt = slot.StartAt,
                EndAt = slot.EndAt
            }, "Новая запись");

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private void ExportExcel()
        {
            var doctorName = Doctors.FirstOrDefault(x => x.Id == (SelectedDoctorId ?? 0))?.Name ?? "Doctor";

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Week_{doctorName}_{WeekStart:yyyyMMdd}.xlsx"
            };

            if (dlg.ShowDialog() != true) return;

            _excel.ExportWeekSchedule(dlg.FileName, doctorName, Days, Rows);
        }

        [RelayCommand]
        private async Task PrevWeekAsync()
        {
            SetWeekStart(WeekStart.AddDays(-7));
            await LoadAsync();
        }

        [RelayCommand]
        private async Task NextWeekAsync()
        {
            SetWeekStart(WeekStart.AddDays(7));
            await LoadAsync();
        }
    }
}
