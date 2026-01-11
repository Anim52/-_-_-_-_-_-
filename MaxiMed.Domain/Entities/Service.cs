using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Service : Entity<int>
    {
        public string Name { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();
    }
}
