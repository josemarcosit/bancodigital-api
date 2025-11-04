using BancoDigital.Api.Infrastructure;

namespace BancoDigital.Api.Application.UseCases
{
    public class TransferenciaUseCase
    {
        private readonly BancoDigitalDbContext _dbContext;

        public TransferenciaUseCase(BancoDigitalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task EfetuarTransferencia(Guid origemId, Guid destinoId, decimal valor)
        {
            var contaOrigem = await _dbContext.Contas.FindAsync(origemId)
                ?? throw new InvalidOperationException("Conta origem não encontrada");

            var contaDestino = await _dbContext.Contas.FindAsync(destinoId)
                ?? throw new InvalidOperationException("Conta destino não encontrada");

            contaOrigem.TransferirPara(contaDestino, valor);

            // aqui, as mudanças + evento irão para Outbox via interceptor
            await _dbContext.SaveChangesAsync();
        }
    }
}
