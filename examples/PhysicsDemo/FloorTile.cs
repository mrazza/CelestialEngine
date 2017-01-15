// -----------------------------------------------------------------------
// <copyright file="FloorTile.cs" company="">
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
    /// Static floor tile object the pyramid rests on
    /// </summary>
    public class FloorTile : SimpleSprite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorTile"/> class.
        /// </summary>
        /// <param name="world">The world object to use.</param>
        /// <param name="position">The position of this florr tile.</param>
        public FloorTile(World world, Vector2 position)
            : base(world, "Content/floor", null, false)
        {
            this.Position = position;
            this.RenderOptions = SpriteRenderOptions.IsLit;
            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat1;
            this.Body.CreateFixture(new PolygonShape(new Vertices(new Vector2[] {
                        new Vector2(0, 0),
                        new Vector2(4, 0),
                        new Vector2(4, 4),
                        new Vector2(0, 4)
                    }), 0.5f)).Friction = 0.5f;
        }
    }
}
