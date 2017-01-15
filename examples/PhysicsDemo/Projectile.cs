// -----------------------------------------------------------------------
// <copyright file="Projectile.cs" company="">
// Copyright (C) Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace PhysicsDemo
{
    using CelestialEngine.Core;
    using CelestialEngine.Game;
    using FarseerPhysics.Collision.Shapes;
    using FarseerPhysics.Common;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using World = CelestialEngine.Core.World;

    /// <summary>
    /// The projectile used to knock down the pyramid
    /// </summary>
    public class Projectile : SimpleSprite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class.
        /// </summary>
        /// <param name="world">The world to use.</param>
        /// <param name="position">The starting position for this object.</param>
        /// <param name="velocity">The velocity of the object.</param>
        /// <param name="angularVelocity">The angular velocity for the object.</param>
        public Projectile(World world, Vector2 position, Vector2 velocity, float angularVelocity)
            : base(world, "Content/floor", "Content/floor_bump", false)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.AngularVelocity = angularVelocity;
            this.RenderOptions = SpriteRenderOptions.IsLit | SpriteRenderOptions.CastsShadows;
            this.LayerDepth = 1;
            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat1;
            this.Body.CreateFixture(new PolygonShape(new Vertices(new Vector2[] {
                        new Vector2(0, 0),
                        new Vector2(4, 0),
                        new Vector2(4, 4),
                        new Vector2(0, 4)
                    }), 4f)).Friction = 0.5f;

            this.Body.BodyType = BodyType.Dynamic;
        }
    }
}
