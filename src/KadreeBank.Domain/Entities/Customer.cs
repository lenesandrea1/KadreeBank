using KadreeBank.Domain.Enums;

namespace KadreeBank.Domain.Entities;

public class Customer
{
    private Customer()
    {
    }

    public Customer(Guid id, string name, string document, CustomerKind kind)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(document))
        {
            throw new ArgumentException("El documento es obligatorio.", nameof(document));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim();
        Document = document.Trim();
        Kind = kind;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public CustomerKind Kind { get; private set; }
}
