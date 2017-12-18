﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageZone
    {
        public long TenantId { get; set; }

        public long PageId { get; set; }
        public long PageZoneId { get; set; }

        public long MasterPageId { get; set; }
        public long MasterPageZoneId { get; set; }
    }
}
