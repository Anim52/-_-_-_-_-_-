using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Patients;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace MaxiMed.Wpf.ViewModels.Patients
{
    public sealed class NamedValue<T>
    {
        public string Name { get; set; } = "";
        public T Value { get; set; } = default!;
    }

    public partial class PatientEditViewModel : ObservableObject
    {
        private readonly IPatientService _service;

        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string title = "Пациент";

        [ObservableProperty] private string lastName = "";
        [ObservableProperty] private string firstName = "";
        [ObservableProperty] private string? middleName;

        [ObservableProperty] private DateTime? birthDate;
        [ObservableProperty] private Sex sex = Sex.Unknown;

        [ObservableProperty] private string? phone;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string? address;
        [ObservableProperty] private string? notes;
        [ObservableProperty] private string? snils;
        [ObservableProperty] private string? passportNumber;
        [ObservableProperty] private string? omsPolicyNumber;


        [ObservableProperty] private string? errorText;

        public List<NamedValue<Sex>> SexItems { get; } = new()
    {
        new() { Name = "Не указан", Value = Sex.Unknown },
        new() { Name = "Мужской", Value = Sex.Male },
        new() { Name = "Женский", Value = Sex.Female },
    };

        public PatientEditViewModel(IPatientService service)
        {
            _service = service;
        }

        public void LoadFrom(PatientDto dto, string dialogTitle)
        {
            Title = dialogTitle;

            Id = dto.Id;
            LastName = dto.LastName;
            FirstName = dto.FirstName;
            MiddleName = dto.MiddleName;
            BirthDate = dto.BirthDate;
            Sex = dto.Sex;
            Phone = dto.Phone;
            Email = dto.Email;
            Address = dto.Address;
            Notes = dto.Notes;
            Snils = dto.Snils;
            PassportNumber = dto.PassportNumber;
            OmsPolicyNumber = dto.OmsPolicyNumber;

            ErrorText = null;
        }

        public PatientDto ToDto() => new()
        {
            Id = Id,
            LastName = LastName,
            FirstName = FirstName,
            MiddleName = MiddleName,
            BirthDate = BirthDate,
            Sex = Sex,
            Phone = Phone,
            Email = Email,
            Address = Address,
            Notes = Notes,
            Snils = Snils,
            PassportNumber = PassportNumber,
            OmsPolicyNumber = OmsPolicyNumber
        };

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;

                var dto = ToDto();

                if (dto.Id == 0)
                {
                    var newId = await _service.CreateAsync(dto);
                    Id = newId;
                }
                else
                {
                    await _service.UpdateAsync(dto);
                }

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
        partial void OnSnilsChanged(string? value)
        {
            if (value == null) return;

            // оставить только цифры
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length > 11)
                digits = digits.Substring(0, 11);

            Snils = FormatSnils(digits);
        }

        private string FormatSnils(string digits)
        {
            if (digits.Length <= 3)
                return digits;

            if (digits.Length <= 6)
                return $"{digits.Substring(0, 3)}-{digits.Substring(3)}";

            if (digits.Length <= 9)
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 3)}-{digits.Substring(6)}";

            if (digits.Length <= 11)
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 3)}-{digits.Substring(6, 3)} {digits.Substring(9)}";

            return digits;
        }
    }
}
