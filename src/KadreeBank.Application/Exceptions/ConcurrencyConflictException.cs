namespace KadreeBank.Application.Exceptions;

public class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException()
        : base("La cuenta fue modificada por otra operación. Intente de nuevo.")
    {
    }
}
