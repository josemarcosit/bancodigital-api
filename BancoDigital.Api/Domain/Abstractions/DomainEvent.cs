namespace BancoDigital.Api.Domain.Abstractions
{
    public abstract record DomainEvent(DateTime OccurredOnUtc);
}
