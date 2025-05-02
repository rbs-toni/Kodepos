using System.Text.RegularExpressions;

namespace Kodepos;
public readonly partial struct CssBuilder
{
    readonly HashSet<string> _classes;
    readonly string[]? _userClasses;

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBuilder"/> class.
    /// </summary>
    public CssBuilder()
    {
        _classes = new HashSet<string>();
        _userClasses = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBuilder"/> class.
    /// </summary>
    /// <param name="userClasses">The user classes to include at the end.</param>
    public CssBuilder(string? userClasses)
    {
        _classes = new HashSet<string>();
        _userClasses = string.IsNullOrWhiteSpace(userClasses) ? null : userClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBuilder"/> class.
    /// </summary>
    internal CssBuilder(bool validateClassNames, string? userClasses) : this(userClasses)
    {
        // Removed _validateClassNames initialization, no longer needed
    }

    /// <summary>
    /// Adds one or more CSS Classes to the builder with space separator.
    /// </summary>
    /// <param name="values">Space-separated CSS Classes to add</param>
    /// <returns>CssBuilder</returns>
    public CssBuilder AddClass(string? values)
    {
        if (!string.IsNullOrWhiteSpace(values))
        {
            _classes.UnionWith(values.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        return this;
    }

    /// <summary>
    /// Adds one or more CSS Classes to the builder with space separator, based on a condition.
    /// </summary>
    /// <param name="value">Space-separated CSS Classes to add</param>
    /// <param name="when">Condition in which the CSS Classes are added.</param>
    /// <returns>CssBuilder</returns>
    public CssBuilder AddClass(string? value, bool when) => when ? AddClass(value) : this;

    /// <summary>
    /// Adds one or more CSS Classes to the builder with space separator, based on a condition.
    /// </summary>
    /// <param name="value">Space-separated CSS Classes to add</param>
    /// <param name="when">Function that returns a condition in which the CSS Classes are added.</param>
    /// <returns>CssBuilder</returns>
    public CssBuilder AddClass(string? value, Func<bool> when) => when() ? AddClass(value) : this;

    public CssBuilder AddClass(string? value, string alternateValue, bool when)
    {
        if (when)
        {
            AddClass(value);
        }
        else
        {
            AddClass(alternateValue);
        }
        return this;
    }

    /// <summary>
    /// Finalize the completed CSS Classes as a string.
    /// </summary>
    /// <returns>string</returns>
    public string? Build()
    {
        var allClasses = _userClasses == null ? _classes : _userClasses.Union(_classes);
        var result = string.Join(" ", allClasses);
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    /// <summary>
    /// ToString should only and always call Build to finalize the rendered string.
    /// </summary>
    /// <returns>string</returns>
    public override string? ToString() => Build();
}
