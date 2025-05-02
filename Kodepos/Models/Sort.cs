using System.Text.Json.Serialization;

namespace Kodepos.Models;
public record Sort
{
    [JsonPropertyName("sortBy")]
    public string? Name { get; set; }
    [JsonPropertyName("dir")]
    public string Dir { get; set; } = "asc";

    public virtual bool Equals(Sort? other) =>
        other is not null && string.Equals(Name, other.Name, StringComparison.Ordinal);

    public override int GetHashCode() => Name?.GetHashCode() ?? 0;
}
