using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kodepos.Models;
using Microsoft.JSInterop;

namespace Kodepos.Services;
public interface IDataConverter
{
    Task GenerateXLSAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default);
    Task GenerateCSVAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default);
    Task GeneratePDFAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default);
}

class DataConverter : IDataConverter, IAsyncDisposable
{
    readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    public DataConverter(IJSRuntime jSRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jSRuntime.InvokeAsync<IJSObjectReference>("import", "/lib/converters/index.js").AsTask());
    }

    public async Task GenerateCSVAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default)
    {
        var module = await _moduleTask.Value;
        var jsonData = JsonSerializer.Serialize(postalCodes);
        await module.InvokeVoidAsync("exportToCSV", jsonData, Guid.NewGuid().ToString("N"));
    }

    public async Task GeneratePDFAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default)
    {
        var module = await _moduleTask.Value;
        var jsonData = JsonSerializer.Serialize(postalCodes);
        await module.InvokeVoidAsync("exportToPDF", jsonData, Guid.NewGuid().ToString("N"));
    }

    public async Task GenerateXLSAsync(IReadOnlyCollection<PostalCode> postalCodes, CancellationToken cancellationToken = default)
    {
        var module = await _moduleTask.Value;
        var jsonData = JsonSerializer.Serialize(postalCodes);
        await module.InvokeVoidAsync("exportToXLS", jsonData, Guid.NewGuid().ToString("N"));
    }
    private bool _isDisposed = false;
    public async ValueTask DisposeAsync()
    {
        await DisposeCoreAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
    protected virtual async ValueTask DisposeCoreAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        if (_moduleTask.IsValueCreated && !_moduleTask.Value.IsFaulted)
        {
            try
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                // This can be called too early when using prerendering
            }
            catch (Exception ex) when (ex is JSDisconnectedException ||
                                       ex is OperationCanceledException)
            {
                // The JSRuntime side may routinely be gone already if the reason we're disposing is that
                // the client disconnected. This is not an error.
            }
        }
        _isDisposed = true;
    }
}
