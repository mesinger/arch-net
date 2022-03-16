namespace ArchNet.Test.Domain.Logistics;

public class Delivery
{
    public string Id { get; set; } = null!;
    public PostalAddress Address { get; set; } = null!;
}

public record PostalAddress(string Address, string City);
