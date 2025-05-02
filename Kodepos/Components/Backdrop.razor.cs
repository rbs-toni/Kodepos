using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Kodepos;

public partial class Backdrop : IAsyncDisposable
{
    private readonly string _defaultId = Identifier.NewId();

    private const string JSFile = "/Components/Backdrop.razor.js";

    private DotNetObjectReference<Backdrop>? _dotNetHelper = null;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private IJSObjectReference? _jsModule { get; set; }

    protected string? ClassValue => new CssBuilder("cursor-auto fixed pointer-events-none inset-0").AddClass(Class)
        .Build();

    /// <summary>
    /// Gets or sets a value indicating whether the overlay is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = false;

    /// <summary>
    /// Callback for when overlay visibility changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Callback for when the overlay is closed.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClose { get; set; }

    /// <summary>
    /// Gets or sets the HTML identifier of the element that is not interactive when the overlay is shown.
    /// This property is ignored if <see cref="Interactive"/> is false.
    /// </summary>
    [Parameter]
    public string? ExceptId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = _defaultId;
        }

        if (Visible)
        {
            // Add a document.addEventListener when Visible is true
            await InvokeOverlayInitializeAsync();
        }
        else
        {
            // Remove a document.addEventListener when Visible is false
            await InvokeOverlayDisposeAsync();
        }
    }
    [JSInvokable]
    public async Task OnCloseInteractiveAsync(MouseEventArgs e)
    {
        if (!Visible)
        {
            return;
        }

        // Remove the document.removeEventListener
        await InvokeOverlayDisposeAsync();

        // Close the overlay
        await OnCloseInternalHandlerAsync(e);
    }

    public async Task OnCloseHandlerAsync(MouseEventArgs e)
    {
        if (!Visible)
        {
            return;
        }

        // Close the overlay
        await OnCloseInternalHandlerAsync(e);
    }

    private async Task OnCloseInternalHandlerAsync(MouseEventArgs e)
    {
        Visible = false;

        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(Visible);
        }

        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync(e);
        }
    }

    /// <summary>
    /// Disposes the overlay.
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await InvokeOverlayDisposeAsync();

        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }

    /// <summary />
    private async Task InvokeOverlayInitializeAsync()
    {
        _dotNetHelper ??= DotNetObjectReference.Create(this);
        _jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", JSFile);

        await _jsModule.InvokeVoidAsync("overlayInitialize", _dotNetHelper, null, ExceptId);
    }

    /// <summary />
    private async Task InvokeOverlayDisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("overlayDispose", ExceptId);
        }
    }
}
