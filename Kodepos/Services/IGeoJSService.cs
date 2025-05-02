using Kodepos.Models;
using System;
using System.Linq;

namespace Kodepos.Services;
public interface IGeoJSService
{
    Task<string> GetIPAddressAsync(CancellationToken cancellationToken = default);
    Task<Geo?> GetGeoAsync(string ipAddress, CancellationToken cancellationToken = default);
}
