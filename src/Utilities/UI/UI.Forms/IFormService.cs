using System.Collections.Generic;

namespace Riverside.Cms.Utilities.UI.Forms
{
    public interface IFormService
    {
        IDictionary<string, IField> ListFields<T>();
    }
}
