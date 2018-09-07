using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Text.Formatting
{
    public interface IStringUtilities
    {
        string BlockReplace(string text, string blockStartText, string blockStopText, Func<string, string> replaceFunc);
    }
}
