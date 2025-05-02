using Kodepos.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;

namespace Kodepos;
public partial class PosLeaflet : IAsyncDisposable
{
    const string JSFile = "/Components/PosLeaflet.razor.js";

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;
    IJSObjectReference? JSModule { get; set; }
    public Geo Geo { get; set; } = new Geo();

    public IJSObjectReference? Leaflet { get; set; }
    async Task<Geo> GetGeoAsync()
    {
        if (JSModule != null)
        {
            return await JSModule.InvokeAsync<Geo>("getGeo");
        }

        return Geo;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", JSFile);
            Geo = await GetGeoAsync();
            Leaflet = await InitLeafletAsync();
            StateHasChanged();
        }
    }
    public LatLng LatLng { get; set; } = new();

    async Task<IJSObjectReference?> InitLeafletAsync()
    {
        if (JSModule != null)
        {
            var lat = float.Parse(Geo.Latitude ?? string.Empty);
            var lng = float.Parse(Geo.Longitude ?? string.Empty);
            LatLng = LatLng with { Lat = lat, Lng = lng };
            var loc = new float[] { lat, lng };
            return await JSModule.InvokeAsync<IJSObjectReference>("initMap", loc);
        }
        return default;
    }

    public async Task SetViewAsync(LatLng latLng)
    {
        if (JSModule != null)
        {
            await JSModule.InvokeVoidAsync("setView", Leaflet, latLng);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (JSModule != null)
        {
            await JSModule.DisposeAsync();
        }
    }
}
