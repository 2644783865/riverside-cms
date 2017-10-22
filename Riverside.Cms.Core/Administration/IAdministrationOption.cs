﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Core.Administration
{
    public interface IAdministrationOption
    {
        AdministrationAction Action { get; }
        string Name { get; set; }
    }
}
