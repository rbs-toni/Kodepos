using Kodepos.Utilities;
using Microsoft.AspNetCore.Components;

// Remember to replace the namespace below with your own project's namespace
namespace Kodepos;

public partial class Highlighter : PosComponentBase
{
    Memory<string> _fragments;
    string _regex = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the highlighted text is case sensitive.
    /// </summary>
    [Parameter]
    public bool CaseSensitive { get; set; }

    /// <summary>
    /// Gets or sets the fragment of text to be highlighted.
    /// </summary>
    [Parameter]
    public string HighlightedText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the whole text in which a fragment will be highlighted.
    /// </summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of delimiters chars. Example: " ,;".
    /// </summary>
    [Parameter]
    public string Delimiters { get; set; } = string.Empty;

    /// <summary>
    /// If true, highlights the text until the next regex boundary.
    /// </summary>
    [Parameter]
    public bool UntilNextBoundary { get; set; }

    protected override void OnParametersSet()
    {
        var highlightedTexts = string.IsNullOrEmpty(Delimiters)
                             ? [HighlightedText]
                             : HighlightedText.Split(Delimiters.ToCharArray());

        _fragments = Splitter.GetFragments(Text, highlightedTexts, out _regex, CaseSensitive, UntilNextBoundary);
    }
}
