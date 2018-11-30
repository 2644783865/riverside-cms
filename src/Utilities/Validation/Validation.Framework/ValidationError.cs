﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Validation.Framework
{
    /// <summary>
    /// Holds information about a single validation failure.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Constructor sets validation failure key and message.
        /// </summary>
        /// <param name="key">Key uniquely identifying validation failure.</param>
        /// <param name="message">Description of validation failure.</param>
        /// <param name="keyPrefix">Validation key prefix.</param>
        public ValidationError(string key, string message, string keyPrefix = null)
        {
            if (key != string.Empty && keyPrefix != null)
                Key = string.Format("{0}.{1}", keyPrefix, key);
            else
                Key = key;
            Message = message;
        }

        /// <summary>
        /// Key that uniquely identifies validation that has failed.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Description of validation failure.
        /// </summary>
        public string Message { get; }
    }
}
