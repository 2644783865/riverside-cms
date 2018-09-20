using System;
using System.Collections.Generic;
using System.Text;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class NavigationBarTabDto : NavigationBarTab
    {
        public long ParentTabId { get; set; }
    }
}
