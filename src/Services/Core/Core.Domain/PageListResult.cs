﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageListResult
    {
        public IEnumerable<Page> Pages { get; set; }
        public int Total { get; set; }
    }
}
