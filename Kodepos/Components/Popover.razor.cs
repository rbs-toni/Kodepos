using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Kodepos;
public partial class Popover
{
    const string JSFile = "/Components/Popover.razor.js";

    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    IJSObjectReference _jsModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", JSFile);
            if (_jsModule != null)
            {
                await _jsModule.InvokeVoidAsync("init", Target, Id);
            }
        }
    }
}
