﻿using System;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageViewZoneElement
    {
        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }

        public string BeginRender { get; set; }
        public string EndRender { get; set; }
    }
}
