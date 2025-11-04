using System.Text.Json;

namespace BancoDigital.Api.Infrastructure.Serializers
{
    public static class SystemTextJsonEventSerializer
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static string Serialize(object @event)
        {
            return JsonSerializer.Serialize(@event, _options);
        }

        public static object Deserialize(string json, string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                throw new InvalidOperationException($"Tipo não encontrado: {typeName}");

            return JsonSerializer.Deserialize(json, type, _options)
                   ?? throw new InvalidOperationException($"Falha ao desserializar tipo {typeName}");
        }
    }
}