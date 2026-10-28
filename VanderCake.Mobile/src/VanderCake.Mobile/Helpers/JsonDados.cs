using System.Text.Json;

namespace VanderCake.Helpers;

public static class JsonDados
{
    public static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web);
    public static string Escrever<T>(T valor) => JsonSerializer.Serialize(valor, Opcoes);
    public static T Ler<T>(string json) => JsonSerializer.Deserialize<T>(json, Opcoes) ?? throw new InvalidDataException("O registro está vazio ou é inválido.");
    public static T Copiar<T>(T valor) => Ler<T>(Escrever(valor));
}
