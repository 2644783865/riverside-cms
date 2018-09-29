using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class ElementPartialView
    {
        public string Name { get; set; }
        public IElementViewModel Model { get; set; }
    }
}
