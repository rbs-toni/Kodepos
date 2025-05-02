using Kodepos.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Kodepos.Tests")]
namespace Kodepos.Services;
class GeoJSService : IGeoJSService
{
    readonly HttpClient _httpClient;

    public GeoJSService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://get.geojs.io/v1/");
    }
    public async Task<Geo?> GetGeoAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ipAddress, nameof(ipAddress));
        return await _httpClient.GetFromJsonAsync<Geo>($"ip/geo/{ipAddress}.json", cancellationToken);
    }

    public async Task<string> GetIPAddressAsync(CancellationToken cancellationToken = default)
    {
        var ipAddress =  await _httpClient.GetStringAsync("ip", cancellationToken);
        return ipAddress.Trim();
    }
}
