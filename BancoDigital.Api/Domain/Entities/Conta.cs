using BancoDigital.Api.Domain.Abstractions;
using BancoDigital.Api.Domain.Events;

namespace BancoDigital.Api.Domain.Entities
{
    public class Conta : IHasDomainEvents
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Guid Id { get; private set; }
        public decimal Saldo { get; private set; }
        public Conta()
        {
            
        }
        public Conta(Guid id, decimal saldoInicial = 0m)
        {
            Id = id;
            Saldo = saldoInicial;
        }

        public void Creditar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentException("Valor deve ser positivo", nameof(valor));
            Saldo += valor;            
        }

        public void Debitar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentException("Valor deve ser positivo", nameof(valor));
            if (Saldo < valor) throw new InvalidOperationException("Saldo insuficiente");
            Saldo -= valor;            
        }

        public void TransferirPara(Conta contaDestino, decimal valor)
        {
            Debitar(valor);
            contaDestino.Creditar(valor);
            
            _domainEvents.Add(new TransferenciaRealizadaDomainEvent(
                ContaOrigemId: this.Id,
                ContaDestinoId: contaDestino.Id,
                Valor: valor
            ));
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
