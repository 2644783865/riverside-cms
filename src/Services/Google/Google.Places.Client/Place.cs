﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Google.Places.Client
{
    public class Place
    {
        public List<string> HtmlAttributions { get; set; }
        public PlaceResult Result { get; set; }
        public string Status { get; set; }
    }
}
