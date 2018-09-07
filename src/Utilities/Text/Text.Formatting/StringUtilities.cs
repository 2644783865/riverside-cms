using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Text.Formatting
{
    public class StringUtilities : IStringUtilities
    {
        public string BlockReplace(string text, string blockStartText, string blockStopText, Func<string, string> replaceFunc)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            StringBuilder sb = new StringBuilder();
            int currentIndex = 0;
            int blockStartIndex = 0;
            while (blockStartIndex != -1)
            {
                blockStartIndex = text.IndexOf(blockStartText, currentIndex);
                if (blockStartIndex == -1)
                {
                    // Block not found
                    string appendText = text.Substring(currentIndex);
                    sb.Append(appendText);
                }
                else
                {
                    // Block found
                    string appendText = text.Substring(currentIndex, blockStartIndex - currentIndex);
                    sb.Append(appendText);
                    int blockStopIndex = text.IndexOf(blockStopText, blockStartIndex) + blockStopText.Length;
                    string blockText = text.Substring(blockStartIndex, blockStopIndex - blockStartIndex);
                    blockText = replaceFunc(blockText);
                    sb.Append(blockText);
                    currentIndex = blockStopIndex;
                }
            }
            return sb.ToString();
        }
    }
}