using BancoDigital.Api.Domain.Abstractions;

namespace BancoDigital.Api.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublicarAsync(DomainEvent evento, CancellationToken cancellationToken = default);
    }
}
