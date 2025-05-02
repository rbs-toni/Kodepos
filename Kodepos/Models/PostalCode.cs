using Kodepos.Components;
using System.Collections;
using System.Text.Json.Serialization;

namespace Kodepos.Models;
public class PostalCode
{
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Regency { get; set; }
    public string? Village { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public int Elevation { get; set; }
    public string? Timezone { get; set; }
    public int Code { get; set; }

    public async Task OnClickAsync(PosLeaflet posLeaflet)
    {
        Console.WriteLine("OnClickAsync Called");
        await posLeaflet.SetViewAsync(new LatLng(Latitude, Longitude));
    }
}