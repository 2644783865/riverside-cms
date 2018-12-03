using System;
using System.Collections.Generic;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class PageViewModel
    {
        public PageView View { get; set; }
        public Dictionary<long, ElementPartialView> Elements { get; set; }
    }
}
