using api.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace api.Data.Converters;

public sealed class MoneyValueConverter() : ValueConverter<Money, decimal>(
    money => money.Value,
    value => Money.Create(value));
