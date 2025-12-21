using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Converters;

public class DateOnlyNullableConverter : ValueConverter<DateOnly?, DateTime?>
{
    public DateOnlyNullableConverter() 
        : base(
            dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
            dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null)
    {
    }
}