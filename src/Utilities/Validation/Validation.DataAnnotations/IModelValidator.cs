using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Validation.DataAnnotations
{
    /// <summary>
    /// Validates an object's data annotations.
    /// </summary>
    public interface IModelValidator
    {
        /// <summary>
        /// Validates model. Throws validation error exception if any data annotations fail to validate.
        /// </summary>
        /// <param name="model">The model to validate.</param>
        /// <param name="keyPrefix">Validation key prefix.</param>
        void Validate(object model, string keyPrefix = null);
    }
}
