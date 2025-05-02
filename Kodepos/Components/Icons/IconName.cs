using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kodepos.Components;
public enum IconName
{
    ArrowUpDown,
    ChevronDown,
    Loader,
    SignPost,
    GitHub
}

public static class IconNameExtensions
{
    public static string ToIconContent(this IconName iconName)
    {
        return iconName switch
        {
            IconName.ArrowUpDown => "<path d=\"m21 16-4 4-4-4\"></path>\r\n    <path d=\"M17 20V4\"></path>\r\n    <path d=\"m3 8 4-4 4 4\"></path>\r\n    <path d=\"M7 4v16\"></path>",
            IconName.Loader => "<path d=\"M12 2v4\"/><path d=\"m16.2 7.8 2.9-2.9\"/><path d=\"M18 12h4\"/><path d=\"m16.2 16.2 2.9 2.9\"/><path d=\"M12 18v4\"/><path d=\"m4.9 19.1 2.9-2.9\"/><path d=\"M2 12h4\"/><path d=\"m4.9 4.9 2.9 2.9\"/>",
            IconName.ChevronDown => "<path d=\"m6 9 6 6 6-6\"></path>",
            IconName.SignPost => "<path d=\"M12 13v8\" />\r\n                    <path d=\"M12 3v3\" />\r\n                    <path d=\"M18 6a2 2 0 0 1 1.387.56l2.307 2.22a1 1 0 0 1 0 1.44l-2.307 2.22A2 2 0 0 1 18 13H6a2 2 0 0 1-1.387-.56l-2.306-2.22a1 1 0 0 1 0-1.44l2.306-2.22A2 2 0 0 1 6 6z\" />",
            IconName.GitHub => "<path d=\"M15 22v-4a4.8 4.8 0 0 0-1-3.5c3 0 6-2 6-5.5.08-1.25-.27-2.48-1-3.5.28-1.15.28-2.35 0-3.5 0 0-1 0-3 1.5-2.64-.5-5.36-.5-8 0C6 2 5 2 5 2c-.3 1.15-.3 2.35 0 3.5A5.403 5.403 0 0 0 4 9c0 3.5 3 5.5 6 5.5-.39.49-.68 1.05-.85 1.65-.17.6-.22 1.23-.15 1.85v4\" />\r\n                        <path d=\"M9 18c-4.51 2-5-2-7-2\" />",
            _ => throw new ArgumentOutOfRangeException(nameof(iconName), iconName, null)
        };
    }
}
