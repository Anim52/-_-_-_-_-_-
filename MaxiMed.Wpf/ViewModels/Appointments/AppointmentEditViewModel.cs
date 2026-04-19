using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MaxiMed.Wpf.ViewModels.Appointments
{
    public partial class AppointmentEditViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string title = "Запись";
        private int _patientSearchVersion;
        private bool _isInitializingDefaultSlot;

        public List<LookupItemDto> Doctors { get; private set; } = new();
        public List<LookupItemDto> Branches { get; private set; } = new();

        [ObservableProperty] private int doctorId; // ✅ int
        [ObservableProperty] private int branchId; // ✅ int

        [ObservableProperty] private DateTime? date;

        [ObservableProperty] private string startTimeText = "10:00";
        [ObservableProperty] private string endTimeText = "10:30";

        [ObservableProperty] private string? patientSearch;
        public List<LookupItemDto> PatientCandidates { get; private set; } = new();
        [ObservableProperty] private LookupItemDto? selectedPatientCandidate;

        [ObservableProperty] private string? errorText;

        public AppointmentEditViewModel(IAppointmentService service) => _service = service;

        public async Task InitAsync(CancellationToken ct = default)
        {
            Doctors = (await _service.GetActiveDoctorsAsync(ct)).ToList();
            Branches = (await _service.GetActiveBranchesAsync(ct)).ToList();

            OnPropertyChanged(nameof(Doctors));
            OnPropertyChanged(nameof(Branches));
        }

        public void LoadFrom(AppointmentDto dto, string dialogTitle)
        {
            Title = dialogTitle;

            Id = dto.Id;
            DoctorId = dto.DoctorId;
            BranchId = dto.BranchId;

            Date = dto.StartAt.Date;
            StartTimeText = dto.StartAt.ToString("HH:mm");
            EndTimeText = dto.EndAt.ToString("HH:mm");

            ErrorText = null;
            PatientSearch = null;
            PatientCandidates = new();
            OnPropertyChanged(nameof(PatientCandidates));
            SelectedPatientCandidate = null;
            PatientCandidates = new();


        }
        public async Task SetPatientByIdAsync(int patientId)
        {
            var item = await _service.GetPatientLookupAsync(patientId);
            if (item is null) return;

            PatientCandidates = new List<LookupItemDto> { item };
            OnPropertyChanged(nameof(PatientCandidates));
            SelectedPatientCandidate = item;
        }

        public AppointmentDto ToDto()
        {
            if (Date is null) throw new ArgumentException("Выберите дату");

            if (!TimeOnly.TryParse(StartTimeText, out var st))
                throw new ArgumentException("Неверное время начала (HH:mm)");

            if (!TimeOnly.TryParse(EndTimeText, out var en))
                throw new ArgumentException("Неверное время окончания (HH:mm)");

            var start = Date.Value.Date.Add(st.ToTimeSpan());
            var end = Date.Value.Date.Add(en.ToTimeSpan());

            return new AppointmentDto
            {
                Id = Id,
                DoctorId = DoctorId,
                BranchId = BranchId,
                PatientId = SelectedPatientCandidate?.Id ?? 0,
                StartAt = start,
                EndAt = end
            };
        }

        partial void OnPatientSearchChanged(string? value)
        {
            _ = ReloadPatientsAsync();
        }

        private async Task ReloadPatientsAsync()
        {
            var version = ++_patientSearchVersion;

            var list = await _service.SearchPatientsAsync(PatientSearch);

            // если за время запроса пользователь уже ввёл другое — игнорируем старый результат
            if (version != _patientSearchVersion) return;

            PatientCandidates = list.ToList();
            OnPropertyChanged(nameof(PatientCandidates));

            // НЕ перетирай выбор, если пользователь уже выбрал
            if (SelectedPatientCandidate == null && PatientCandidates.Count > 0)
                SelectedPatientCandidate = PatientCandidates[0];
        }


        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;

                var dto = ToDto();

                try
                {
                    if (dto.Id == 0)
                        Id = await _service.CreateAsync(dto);
                    else
                        await _service.UpdateAsync(dto);

                    RequestClose?.Invoke(true);
                }
                catch (Exception ex) when (
                    ex.Message.Contains("У врача уже есть запись") ||
                    ex.Message.Contains("Пациент уже записан"))
                {
                    await TryMoveToNearestFreeSlotAsync(dto);

                    ErrorText = $"Выбранное время занято. Подставлено ближайшее свободное: {dto.StartAt:dd.MM.yyyy HH:mm} - {dto.EndAt:HH:mm}";
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
        private async Task TryMoveToNearestFreeSlotAsync(AppointmentDto dto)
        {
            var duration = dto.EndAt - dto.StartAt;
            if (duration <= TimeSpan.Zero)
                duration = TimeSpan.FromMinutes(30);

            var slots = await _service.FindFreeSlotsAsync(
                dto.DoctorId,
                dto.StartAt.Date,
                dto.StartAt.Date.AddDays(7),
                maxResults: 100);

            var free = slots
                .Where(s => s.StartAt >= dto.StartAt)
                .FirstOrDefault(s => (s.EndAt - s.StartAt) >= duration);

            if (free is null)
                throw new InvalidOperationException("Нет свободного времени для записи.");

            dto.StartAt = free.StartAt;
            dto.EndAt = free.StartAt.Add(duration);

            Date = dto.StartAt.Date;
            StartTimeText = dto.StartAt.ToString("HH:mm");
            EndTimeText = dto.EndAt.ToString("HH:mm");
        }
        private async Task ApplyNearestFreeSlotAsync(CancellationToken ct = default)
        {
            if (DoctorId <= 0 || Date is null)
                return;

            var day = Date.Value.Date;

            var slots = await _service.FindFreeSlotsAsync(
                DoctorId,
                day,
                day,
                maxResults: 50,
                ct);

            var free = slots
                .Where(s => s.StartAt >= day)
                .OrderBy(s => s.StartAt)
                .FirstOrDefault();

            if (free is null)
            {
                ErrorText = "На выбранную дату нет свободного времени.";
                return;
            }

            StartTimeText = free.StartAt.ToString("HH:mm");
            EndTimeText = free.EndAt.ToString("HH:mm");
            ErrorText = null;
        }

        public async Task InitDefaultSlotAsync(CancellationToken ct = default)
        {
            if (_isInitializingDefaultSlot)
                return;

            try
            {
                _isInitializingDefaultSlot = true;
                await ApplyNearestFreeSlotAsync(ct);
            }
            finally
            {
                _isInitializingDefaultSlot = false;
            }
        }
        partial void OnDoctorIdChanged(int value)
        {
            if (Id == 0 && !_isInitializingDefaultSlot)
                _ = InitDefaultSlotAsync();
        }

        partial void OnDateChanged(DateTime? value)
        {
            if (Id == 0 && !_isInitializingDefaultSlot)
                _ = InitDefaultSlotAsync();
        }
    }
}
