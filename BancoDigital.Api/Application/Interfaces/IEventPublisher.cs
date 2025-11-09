using BancoDigital.Api.Domain.Abstracoes;

namespace BancoDigital.Api.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublicarAsync(EventoDomain evento, CancellationToken cancellationToken = default);
    }
}
