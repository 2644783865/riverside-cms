﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Google.Places.Client
{
    public interface IPlaceService
    {
        Task<Place> ReadPlaceAsync(string placeId);
    }
}
