// -----------------------------------------------------------------------
// <copyright file="DeferredRenderSystemDebugDrawMode.cs" company="">
// Copyright (C) Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    /// <summary>
    /// Specify which (if any) map to draw directly to the screen for debugging. If not <c>Disabled</c>,
    /// the render targets will not be merged.
    /// </summary>
    public enum DeferredRenderSystemDebugDrawMode
    {
        /// <summary>
        /// Do not enable the debug shader -- merge render targets as normal
        /// </summary>
        Disabled,

        /// <summary>
        /// Render the color map directly to the screen
        /// </summary>
        ColorMap,

        /// <summary>
        /// Render the options map directly to the screen
        /// </summary>
        OptionsMap,

        /// <summary>
        /// Render the light map directly to the screen
        /// </summary>
        LightMap,

        /// <summary>
        /// Render the normal map directly to the screen
        /// </summary>
        NormalMap
    }
}
