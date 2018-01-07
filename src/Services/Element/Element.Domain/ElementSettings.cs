﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Domain
{
    public class ElementSettings
    {
        public long TenantId { get; set; }
        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }
        public string Name { get; set; }
    }
}
