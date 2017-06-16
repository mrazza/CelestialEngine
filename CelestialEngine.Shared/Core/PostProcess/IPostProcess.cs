// -----------------------------------------------------------------------
// <copyright file="IPostProcess.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.PostProcess
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Interface inhereted by all post process effects.
    /// </summary>
    public interface IPostProcess : IComparable<IPostProcess>
    {
        /// <summary>
        /// Gets the render priority.
        /// </summary>
        /// <value>
        /// The render priority.
        /// </value>
        float RenderPriority
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether or not this instance is enabled.
        /// </summary>
        bool IsEnabled
        {
            get;
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        void LoadContent(ExtendedContentManager contentManager);

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="StaticPostProcess"/> component needs render itself.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        void Draw(GameTime gameTime, DeferredRenderSystem renderSystem);
    }
}
