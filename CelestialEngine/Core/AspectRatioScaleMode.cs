// -----------------------------------------------------------------------
// <copyright file="AspectRatioScaleMode.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;

    /// <summary>
    /// Different methods to handle rendering the same data on targets of various aspect ratios
    /// </summary>
    public enum AspectRatioScaleMode
    {
        /// <summary>
        /// This is the default method whereby the only requirement is that the entire area specified is guarenteed to be visible on the screen.
        /// </summary>
        None,

        /// <summary>
        /// This is the veritcal deformation scale mode; if the screen aspect ratio differs from the target, the horizontal (width) is maintained
        /// between aspect ratios while the vertical portion scales with the screen aspect ratio.
        /// </summary>
        VerticalDeformation,

        /// <summary>
        /// This is the horizontal deformation scale mode; if the screen aspect ratio differs from the target, the vertical (height) is maintained
        /// between aspect ratios while the vertical portion scales with the screen aspect ratio.
        /// </summary>
        /// <remarks>This is generally considured the preferred mode over VerticalDeformation.</remarks>
        HorizontalDeformation
    }
}
