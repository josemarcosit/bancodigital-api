using BancoDigital.Api.Domain.Abstracoes;
using BancoDigital.Api.Infrastructure.Outbox;
using BancoDigital.Api.Infrastructure.Serializers;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BancoDigital.Api.Infrastructure.Interceptors
{
    public class OutboxSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ILogger<OutboxSaveChangesInterceptor> _logger;

        public OutboxSaveChangesInterceptor(ILogger<OutboxSaveChangesInterceptor> logger)
        {
            _logger = logger;
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return result;

            var aggregates = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is ITemEventoDomain temEventos && temEventos.EventosDomain.Any())
                .Select(e => (ITemEventoDomain)e.Entity)
                .ToList();

            if (!aggregates.Any()) return result;

            var outbox = context.Set<OutboxMessage>();

            foreach (var aggregate in aggregates)
            {
                foreach (var @event in aggregate.EventosDomain)
                {
                    outbox.Add(new OutboxMessage
                    {
                        OccurredOnUtc = @event.CriadoEmUtc,
                        Type = @event.GetType().AssemblyQualifiedName!,
                        Payload = SystemTextJsonEventSerializer.Serialize(@event)
                    });
                    _logger.LogInformation("Outbox message {Type} stored ", @event.GetType().Name);
                }
                aggregate.LimparEventosDomain();
            }

            return result;
        }
    }
}
