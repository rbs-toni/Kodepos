using Kodepos.Services;

namespace KodeposTests;

public class GeoJSServiceTests
{
    readonly IGeoJSService _geoService = new GeoJSService(new HttpClient());

    [Fact]
    public async Task GetIPAddressAsync_ReturnsValidIp()
    {
        var ip = await _geoService.GetIPAddressAsync();
        Assert.False(string.IsNullOrWhiteSpace(ip));
        Assert.Matches(@"\b(?:\d{1,3}\.){3}\d{1,3}\b", ip);
    }

    [Fact]
    public async Task GetGeoAsync_ReturnsGeoForValidIp()
    {
        var ip = await _geoService.GetIPAddressAsync();
        var geo = await _geoService.GetGeoAsync(ip);

        Assert.NotNull(geo);
        Assert.False(string.IsNullOrWhiteSpace(geo!.Country));
    }
}
