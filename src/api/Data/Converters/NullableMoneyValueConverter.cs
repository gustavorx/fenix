using api.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace api.Data.Converters;

public sealed class NullableMoneyValueConverter() : ValueConverter<Money?, decimal?>(
    money => money.HasValue ? money.Value.Value : null,
    value => value.HasValue ? Money.Create(value.Value) : null);
