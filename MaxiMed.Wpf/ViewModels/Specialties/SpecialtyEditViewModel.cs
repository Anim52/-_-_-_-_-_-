using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Specialties
{
    public partial class SpecialtyEditViewModel : ObservableObject
    {
        private readonly ISpecialtyService _service;
        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string title = "Специализация";
        [ObservableProperty] private string name = "";
        [ObservableProperty] private bool isActive = true;
        [ObservableProperty] private string? errorText;

        public SpecialtyEditViewModel(ISpecialtyService service) => _service = service;

        public void LoadFrom(SpecialtyDto dto, string dialogTitle)
        {
            Title = dialogTitle;
            Id = dto.Id;
            Name = dto.Name;
            IsActive = dto.IsActive;
            ErrorText = null;
        }

        private SpecialtyDto ToDto() => new()
        {
            Id = Id,
            Name = Name,
            IsActive = IsActive
        };

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;
                var dto = ToDto();

                if (dto.Id == 0) Id = await _service.CreateAsync(dto);
                else await _service.UpdateAsync(dto);

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
