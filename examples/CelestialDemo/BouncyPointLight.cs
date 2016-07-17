// -----------------------------------------------------------------------
// <copyright file="BouncyPointLight.cs" company="">
// Copyright (C) Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.TechDemo
{
    using CelestialEngine.Core;
    using CelestialEngine.Game.PostProcess.Lights;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A bouncy point light.
    /// </summary>
    /// <seealso cref="CelestialEngine.Game.PostProcess.Lights.PointLight" />
    public class BouncyPointLight : PointLight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BouncyPointLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="T:CelestialEngine.Core.World" /> in which the instance lives.</param>
        public BouncyPointLight(World world)
            : base(world)
        {
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            Vector3 newPosition = this.Position;
            Vector3 newVelocity = this.Velocity;
            RectangleF bounds = ((BaseGame)this.World.Game).RenderSystem.GetCameraRenderBounds(); // HACK: Fix this. But it's a tech demo so suck my nuts

            if (this.Position.X < bounds.X || this.Position.X > bounds.TopRight.X)
            {
                newVelocity.X *= -1;
                newPosition.X = this.Position.X < bounds.X ? bounds.X : bounds.TopRight.X;
            }
            if (this.Position.Y < bounds.Y || this.Position.Y > bounds.BottomLeft.Y)
            {
                newVelocity.Y *= -1;
                newPosition.Y = this.Position.Y < bounds.Y ? bounds.Y : bounds.BottomLeft.Y;
            }

            this.Velocity = newVelocity;
            this.Position = newPosition;
        }
    }
}
