using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Diagnoses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Diagnoses
{
    public partial class DiagnosisPickerViewModel : ObservableObject
    {
        private readonly IDiagnosisService _service;

        public ObservableCollection<DiagnosisDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private DiagnosisDto? selected;

        public event Action<DiagnosisDto>? Picked;

        public DiagnosisPickerViewModel(IDiagnosisService service) => _service = service;

        [RelayCommand]
        public async Task SearchAsync()
        {
            Items.Clear();
            var list = await _service.SearchAsync(SearchText);
            foreach (var x in list) Items.Add(x);
        }

        [RelayCommand]
        private void Pick()
        {
            if (Selected is null) return;
            Picked?.Invoke(Selected);
        }
    }
}
