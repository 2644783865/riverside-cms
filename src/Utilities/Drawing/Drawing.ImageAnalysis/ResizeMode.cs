﻿namespace Riverside.Cms.Utilities.Drawing.ImageAnalysis
{
    public enum ResizeMode
    {
        // Resize image to the width and height specified, aspect ratio could change
        Simple,

        // Resize image so that it fits inside the width and height specified, aspect ratio maintained
        MaintainAspect,

        // Resize image to the width and height specified, cropping image if required to maintain aspect ratio
        Crop
    }
}
