using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Schedule
{
    public sealed class WeekRowVm
    {
        public string Time { get; set; } = "";
        public ObservableCollection<WeekSlotVm> Slots { get; } = new();
    }
}
