using System.Text.Json.Serialization;

namespace AspNetInAction;


[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, Fruit>))]
[JsonSerializable(typeof(Fruit))]
public partial class ApiSerializerContext : JsonSerializerContext
{
}