using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    public sealed class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
        { }
    }

    public sealed class DateOnlyComparer : ValueComparer<DateOnly>
    {
        public DateOnlyComparer() : base(
            (a, b) => a.DayNumber == b.DayNumber,
            d => d.GetHashCode(),
            d => d)
        { }
    }

    public sealed class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
            t => t.ToTimeSpan(),
            t => TimeOnly.FromTimeSpan(t))
        { }
    }

    public sealed class TimeOnlyComparer : ValueComparer<TimeOnly>
    {
        public TimeOnlyComparer() : base(
            (a, b) => a.Ticks == b.Ticks,
            t => t.GetHashCode(),
            t => t)
        { }
    }
}
