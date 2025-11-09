using BancoDigital.Api.Domain.Entidades;
using BancoDigital.Api.Infrastructure;

namespace BancoDigital.Api.Application.UseCases
{
    public class CriarContaUseCase
    {
        private readonly BancoDigitalDbContext _dbContext;

        public CriarContaUseCase(BancoDigitalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Criar(Conta conta)
        {
            ArgumentNullException.ThrowIfNull(conta);

            var result = await _dbContext.Contas.FindAsync(conta.Id);
            if (result != null)
                throw new InvalidOperationException($"Conta {result.Id} já existe");

            await _dbContext.Contas.AddAsync(conta);

            await _dbContext.SaveChangesAsync();
        }
    }
}
