using api.ValueObjects;

namespace api.Entities;

public class Income
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Money Amount { get; set; }
    public DateOnly ReceivedDate { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
