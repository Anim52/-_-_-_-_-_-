using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Services
{
    public partial class ServicePickerViewModel : ObservableObject
    {
        private readonly IServiceService _service;

        public ObservableCollection<ServiceDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private ServiceDto? selected;

        public event Action<ServiceDto>? Picked;

        public ServicePickerViewModel(IServiceService service) => _service = service;

        [RelayCommand]
        public async Task SearchAsync()
        {
            Items.Clear();
            var list = await _service.SearchAsync(SearchText);
            foreach (var x in list.Where(s => s.IsActive)) Items.Add(x);
        }

        [RelayCommand]
        private void Pick()
        {
            if (Selected is null) return;
            Picked?.Invoke(Selected);
        }
    }
}
