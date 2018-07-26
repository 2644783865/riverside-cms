﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IElementContent
    {
        long TenantId { get; set; }
        Guid ElementTypeId { get; set; }
        long ElementId { get; set; }
    }
}
