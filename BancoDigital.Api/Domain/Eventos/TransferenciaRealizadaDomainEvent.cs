using BancoDigital.Api.Domain.Abstracoes;

namespace BancoDigital.Api.Domain.Eventos
{
    public sealed record TransferenciaRealizadaDomainEvent(
        Guid ContaOrigemId,
        Guid ContaDestinoId,
        decimal Valor
    ) : EventoDomain(DateTime.UtcNow);
}
