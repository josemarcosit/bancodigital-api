using BancoDigital.Api.Domain.Abstracoes;
using BancoDigital.Api.Domain.Eventos;
using BancoDigital.Api.Infrastructure.Serializers;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api.Infrastructure.Outbox
{
    public class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<OutboxDispatcher> _logger;
        private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);

        public OutboxDispatcher(IServiceProvider sp, ILogger<OutboxDispatcher> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox dispatcher iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BancoDigitalDbContext>();

                var pendentes = await db.OutboxMessages
                    .Where(o => o.ProcessedOnUtc == null)
                    .OrderBy(o => o.Id)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var msg in pendentes)
                {
                    try
                    {
                        var evt = SystemTextJsonEventSerializer.Deserialize(msg.Payload, msg.Type) as EventoDomain;
                        if (evt != null)
                        {
                            //TODO: IEventPublisher
                            await SimulatedPublish(evt);

                            // Marca como processado
                            msg.ProcessedOnUtc = DateTime.UtcNow;
                        }
                    }
                    catch (Exception ex)
                    {
                        msg.Error = ex.Message;
                        _logger.LogError(ex, "Erro processando mensagem outbox {Id}", msg.Id);
                    }
                }

                if (pendentes.Any())
                {
                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(_pollInterval, stoppingToken);
            }
        }

        private Task SimulatedPublish(EventoDomain evt)
        {
            if (evt is TransferenciaRealizadaDomainEvent e)
            {
                _logger.LogInformation("Evento TransferenciaRealizada publicado: {Origem} -> {Destino} : Valor {Valor}",
                    e.ContaOrigemId, e.ContaDestinoId, e.Valor);
            }
            return Task.CompletedTask;
        }
    }
}
