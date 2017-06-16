// -----------------------------------------------------------------------
// <copyright file="RenderTargetTypes.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;

    /// <summary>
    /// Types of render targets that exist within the render system
    /// </summary>
    [Flags]
    public enum RenderTargetTypes : int
    {
        /// <summary>
        /// No type selected
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Color Map Render Target
        /// </summary>
        ColorMap = 0x1,

        /// <summary>
        /// Normal Map Render Target
        /// </summary>
        NormalMap = 0x2,

        /// <summary>
        /// Light Map Render Target
        /// </summary>
        LightMap = 0x4,

        /// <summary>
        /// Options Map Render Target
        /// </summary>
        OptionsMap = 0x8
    }
}