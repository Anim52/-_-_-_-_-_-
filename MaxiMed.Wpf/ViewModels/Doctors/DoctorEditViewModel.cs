using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Common;
using MaxiMed.Application.Doctors;
using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Doctors
{
    public partial class DoctorEditViewModel : ObservableObject
    {
        private readonly IDoctorService _service;

        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string title = "Врач";

        [ObservableProperty] private string fullName = "";
        [ObservableProperty] private int branchId;
        [ObservableProperty] private int specialtyId;

        [ObservableProperty] private string? room;
        [ObservableProperty] private string? phone;
        [ObservableProperty] private string? email;
        [ObservableProperty] private bool isActive = true;

        [ObservableProperty] private string? errorText;

        public List<LookupItemDto> Branches { get; private set; } = new();
        public List<LookupItemDto> Specialties { get; private set; } = new();

        public DoctorEditViewModel(IDoctorService service) => _service = service;

        public async Task InitLookupsAsync(CancellationToken ct = default)
        {
            Branches = (await _service.GetBranchesAsync(ct)).ToList();
            Specialties = (await _service.GetSpecialtiesAsync(ct)).ToList();

            OnPropertyChanged(nameof(Branches));
            OnPropertyChanged(nameof(Specialties));
        }

        public void LoadFrom(DoctorDto dto, string dialogTitle)
        {
            Title = dialogTitle;

            Id = dto.Id;
            FullName = dto.FullName;
            BranchId = dto.BranchId;
            SpecialtyId = dto.SpecialtyId;
            Room = dto.Room;
            Phone = dto.Phone;
            Email = dto.Email;
            IsActive = dto.IsActive;

            ErrorText = null;
        }

        public DoctorDto ToDto() => new()
        {
            Id = Id,
            FullName = FullName,
            BranchId = BranchId,
            SpecialtyId = SpecialtyId,
            Room = Room,
            Phone = Phone,
            Email = Email,
            IsActive = IsActive
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
    }
}
