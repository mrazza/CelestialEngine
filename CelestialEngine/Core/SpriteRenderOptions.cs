// -----------------------------------------------------------------------
// <copyright file="SpriteRenderOptions.cs" company="">
// Copyright (C) Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;

    /// <summary>
    /// Specifies any additional post-processing to be applied to the sprite.
    /// </summary>
    /// <remarks>
    /// This enum contains bit-flag options for the sprite that get rendered onto the option-map's RED channel.
    /// There are 8 bits available in this channel.
    /// </remarks>
    [Flags]
    public enum SpriteRenderOptions : int
    {
        /// <summary>
        /// No render options.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Indicates that the sprite should be lit using light shaders (if in scene).
        /// </summary>
        IsLit = 0x1,

        /// <summary>
        /// Indicates that the sprite should cast shadows (if lights are in scene).
        /// </summary>
        CastsShadows = 0x2
    }
}
