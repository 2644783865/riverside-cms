using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class ElementRender
    {
        public string PartialViewName { get; set; }
        public IElementView ElementView { get; set; }
    }
}
