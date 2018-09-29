using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class PageViewModel
    {
        public PageView View { get; set; }
        public Dictionary<long, ElementPartialView> Elements { get; set; }
    }
}
