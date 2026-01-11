using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Branches
{
    public partial class BranchEditViewModel : ObservableObject
    {
        private readonly IBranchService _service;
        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string title = "Филиал";
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string? address;
        [ObservableProperty] private string? phone;
        [ObservableProperty] private bool isActive = true;
        [ObservableProperty] private string? errorText;

        public BranchEditViewModel(IBranchService service) => _service = service;

        public void LoadFrom(BranchDto dto, string dialogTitle)
        {
            Title = dialogTitle;
            Id = dto.Id;
            Name = dto.Name;
            Address = dto.Address;
            Phone = dto.Phone;
            IsActive = dto.IsActive;
            ErrorText = null;
        }

        private BranchDto ToDto() => new()
        {
            Id = Id,
            Name = Name,
            Address = Address,
            Phone = Phone,
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
