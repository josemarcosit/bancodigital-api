using BancoDigital.Api.Domain.Abstracoes;
using BancoDigital.Api.Domain.Eventos;

namespace BancoDigital.Api.Domain.Entidades
{
    public class Conta : ITemEventoDomain
    {
        private readonly List<EventoDomain> _eventosDomain = new();
        public IReadOnlyCollection<EventoDomain> EventosDomain => _eventosDomain.AsReadOnly();

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

            _eventosDomain.Add(new TransferenciaRealizadaDomainEvent(
                ContaOrigemId: this.Id,
                ContaDestinoId: contaDestino.Id,
                Valor: valor
            ));
        }

        public void LimparEventosDomain() => _eventosDomain.Clear();
    }
}
