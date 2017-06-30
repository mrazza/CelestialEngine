// -----------------------------------------------------------------------
// <copyright file="DebugTargetsShader.cs" company="">
// Copyright (C) Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Shaders
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Internal deferred renderer shader that merges the render targets onto the back buffer
    /// </summary>
    internal sealed class DebugTargetsShader : Shader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugTargetsShader"/> class.
        /// </summary>
        public DebugTargetsShader()
            : base(Content.Shaders.Core.DebugTargets)
        {
        }
        #endregion
    }
}
