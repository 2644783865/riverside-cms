using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Text.Csv
{
    public class CsvService : ICsvService
    {
        /// <summary>
        /// Retrieve CSV values from single line of text.
        /// </summary>
        /// <param name="text">Line from CSV file.</param>
        /// <returns>Individual values.</returns>
        public IEnumerable<string> GetCsvValues(string text)
        {
            List<string> values = new List<string>();
            while (!string.IsNullOrEmpty(text))
            {
                if (text[0] == '\"')
                {
                    // Value starts with double quote, so find closing double quote
                    int closeIndex = 1;
                    bool finished = false;
                    while (!finished)
                    {
                        closeIndex = text.IndexOf("\"", closeIndex);
                        if (closeIndex < 0)
                            throw new InvalidOperationException();
                        int nonCloseIndex = text.IndexOf("\"\"", closeIndex);
                        bool invalidClose = closeIndex == nonCloseIndex;
                        finished = !invalidClose;
                        if (!finished)
                            closeIndex = closeIndex + 2;
                    }

                    // Get value
                    values.Add(text.Substring(1, closeIndex - 1).Replace("\"\"", "\""));

                    // Start with next value
                    text = text.Substring(closeIndex + 1);

                    // Strip comma if it exists
                    if (text.StartsWith(","))
                    {
                        text = text.Substring(1);
                        if (text == string.Empty)
                            values.Add(string.Empty);
                    }
                }
                else
                {
                    // Value does not start with double quote, so a single comma must close this value
                    int closeIndex = text.IndexOf(",");
                    if (closeIndex < 0)
                    {
                        values.Add(text);
                        text = null;
                    }
                    else
                    {
                        values.Add(text.Substring(0, closeIndex));
                        text = text.Substring(closeIndex + 1);
                        if (text == string.Empty)
                            values.Add(string.Empty);
                    }
                }
            }
            return values;
        }
    }
}
