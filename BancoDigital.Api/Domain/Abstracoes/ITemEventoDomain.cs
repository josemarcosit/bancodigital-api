namespace BancoDigital.Api.Domain.Abstracoes
{
    public interface ITemEventoDomain
    {
        IReadOnlyCollection<EventoDomain> EventosDomain { get; }
        void LimparEventosDomain();
    }
}
