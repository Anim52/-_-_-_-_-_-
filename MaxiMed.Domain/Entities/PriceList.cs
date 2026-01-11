using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class PriceList : Entity<int>
    {
        public string Name { get; set; } = null!;
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }

        public ICollection<PriceListItem> Items { get; set; } = new List<PriceListItem>();
    }

    public class PriceListItem : Entity<int>
    {
        public int PriceListId { get; set; }
        public int ServiceId { get; set; }
        public decimal Price { get; set; }

        public PriceList PriceList { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
