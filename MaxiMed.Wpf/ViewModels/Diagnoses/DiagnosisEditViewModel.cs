using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MaxiMed.Application.Diagnoses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Diagnoses
{
    public partial class DiagnosisEditViewModel : ObservableObject
    {
        private readonly IDiagnosisService _service;

        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string code = "";
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string? errorText;

        public DiagnosisEditViewModel(IDiagnosisService service) => _service = service;

        public void LoadFrom(DiagnosisDto? dto)
        {
            ErrorText = null;

            if (dto is null)
            {
                Id = 0; Code = ""; Name = "";
                return;
            }

            Id = dto.Id;
            Code = dto.Code;
            Name = dto.Name;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;

                var dto = new DiagnosisDto { Id = Id, Code = Code, Name = Name };

                if (Id == 0)
                    Id = await _service.CreateAsync(dto);
                else
                    await _service.UpdateAsync(dto);

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
