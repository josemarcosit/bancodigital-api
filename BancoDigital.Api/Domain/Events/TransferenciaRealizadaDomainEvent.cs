using BancoDigital.Api.Domain.Abstractions;

namespace BancoDigital.Api.Domain.Events
{
    public sealed record TransferenciaRealizadaDomainEvent(
        Guid ContaOrigemId,
        Guid ContaDestinoId,
        decimal Valor
    ) : DomainEvent(DateTime.UtcNow);
}
