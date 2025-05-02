using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace Kodepos.Components;
public abstract class PosComponentBase : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    [Parameter]
    public string? Class { get; set; }
    
    [Parameter]
    public ElementReference Ref { get; set; }
    
    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
