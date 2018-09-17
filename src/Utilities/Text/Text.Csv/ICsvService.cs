using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Text.Csv
{
    public interface ICsvService
    {
        /// <summary>
        /// Retrieve CSV values from single line of text.
        /// </summary>
        /// <param name="text">Line from CSV file.</param>
        /// <returns>Individual values.</returns>
        IEnumerable<string> GetCsvValues(string text);
    }
}
