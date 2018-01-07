﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
{
    public interface IElementService
    {
        Task<ElementSettings> ReadElementAsync(long tenantId, long elementId);
    }

    public interface IElementService<T> where T : ElementSettings
    {
        Task<ElementSettings> ReadElementAsync(long tenantId, long elementId);
    }
}
