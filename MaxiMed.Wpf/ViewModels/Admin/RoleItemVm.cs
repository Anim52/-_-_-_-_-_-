using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Admin
{
    public partial class RoleItemVm : ObservableObject
    {
        public string Name { get; }

        [ObservableProperty] private bool isSelected;

        public RoleItemVm(string name) => Name = name;
    }
}
