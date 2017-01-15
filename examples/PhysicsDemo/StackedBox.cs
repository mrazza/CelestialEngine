// -----------------------------------------------------------------------
// <copyright file="StackedBox.cs" company="">
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
    /// The physics object used in the stacked pyramid of boxes
    /// </summary>
    /// <seealso cref="CelestialEngine.Game.SimpleSprite" />
    public class StackedBox : SimpleSprite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackedBox"/> class.
        /// </summary>
        /// <param name="world">The world object to use.</param>
        /// <param name="position">The position of this stacked box.</param>
        public StackedBox(World world, Vector2 position)
            : base(world, "Content/panel", "Content/panel_bump", false)
        {
            this.Position = position;
            RenderOptions = SpriteRenderOptions.IsLit;
            SpecularReflectivity = 0.3f;

            this.Body.CollidesWith = Category.Cat1;
            this.Body.CollisionCategories = Category.Cat1;
            this.Body.CreateFixture(new PolygonShape(new Vertices(new Vector2[] {
                        new Vector2(0, 0),
                        new Vector2(4, 0),
                        new Vector2(4, 4),
                        new Vector2(0, 4)
                    }), 0.5f)).Friction = 0.5f;

            this.Body.BodyType = BodyType.Dynamic;
        }
    }
}
