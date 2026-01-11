using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MaxiMed.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Services
{
    public partial class ServiceEditViewModel : ObservableObject
    {
        private readonly IServiceService _service;
        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string name = "";
        [ObservableProperty] private int durationMinutes = 30;
        [ObservableProperty] private decimal basePrice;
        [ObservableProperty] private bool isActive = true;
        [ObservableProperty] private string? errorText;

        public ServiceEditViewModel(IServiceService service) => _service = service;

        public void LoadFrom(ServiceDto? dto)
        {
            ErrorText = null;

            if (dto is null)
            {
                Id = 0;
                Name = "";
                DurationMinutes = 30;
                BasePrice = 0;
                IsActive = true;
                return;
            }

            Id = dto.Id;
            Name = dto.Name;
            DurationMinutes = dto.DurationMinutes;
            BasePrice = dto.BasePrice;
            IsActive = dto.IsActive;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;

                var dto = new ServiceDto
                {
                    Id = Id,
                    Name = Name,
                    DurationMinutes = DurationMinutes,
                    BasePrice = BasePrice,
                    IsActive = IsActive
                };

                if (Id == 0) Id = await _service.CreateAsync(dto);
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
