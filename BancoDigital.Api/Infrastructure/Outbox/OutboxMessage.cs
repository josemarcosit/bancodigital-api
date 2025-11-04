namespace BancoDigital.Api.Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public DateTime OccurredOnUtc { get; set; }
        public string Type { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }
    }
}
