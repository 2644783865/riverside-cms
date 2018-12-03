using System;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class ElementPartialView
    {
        public string Name { get; set; }
        public IElementViewModel Model { get; set; }
    }
}
