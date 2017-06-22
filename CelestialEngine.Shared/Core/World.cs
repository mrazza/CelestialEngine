// -----------------------------------------------------------------------
// <copyright file="World.cs" company="">
// Copyright (C) 2011-2013 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System.Collections.Generic;
    using CelestialEngine.Core.Collections;
    using Microsoft.Xna.Framework;
    using Collections.QuadTree;

    /// <summary>
    /// Represents a singular instance of the world; contains and manages all
    /// objects within the current world.
    /// </summary>
    public sealed class World : GameComponent
    {
        #region Members
        /// <summary>
        /// A List of all simulated objects in the world.
        /// </summary>
        private ThrottledCollection<SortedSet<SimBase>, SimBase> worldSimObjects;

        /// <summary>
        /// A QuadTree of all sprite objects in the world.
        /// </summary>
        private ThrottledCollection<QuadTree<SpriteBase, List<SpriteBase>>, SpriteBase> worldSpriteObjects;

        /// <summary>
        /// A SortedSet of all screen drawable components in the world.
        /// </summary>
        private ThrottledCollection<SortedSet<ScreenDrawableComponent>, ScreenDrawableComponent> screenDrawableComponents;

        /// <summary>
        /// The Farseer Physics World instance used to simulate all physics objects.
        /// </summary>
        private FarseerPhysics.Dynamics.World physicsWorld;

        /// <summary>
        /// The ratio of world-units to pixel-space. The number of world units for each pixel.
        /// </summary>
        private float worldPerPixelRatio;

        /// <summary>
        /// Parent <see cref="BaseGame"/> instance.
        /// </summary>
        private BaseGame parentGame;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class with the defaults.
        /// Defaults: No gravity, 64 pixels units per world unit.
        /// </summary>
        /// <param name="parentGame">The Game in which the instance lives.</param>
        /// <param name="bounds">The world's bounds.</param>
        internal World(BaseGame parentGame, RectangleF bounds)
            : this(parentGame, bounds, 1 / 64.0f, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="parentGame">The Game in which the instance lives.</param>
        /// <param name="worldPerPixelRatio">The number of world units for each pixel.</param>
        /// <param name="gravity">The gravity that will be applied to world objects.</param>
        /// <param name="bounds">The world's bounds.</param>
        internal World(BaseGame parentGame, RectangleF bounds, float worldPerPixelRatio, Vector2 gravity)
            : base(parentGame)
        {
            this.Bounds = bounds;
            this.worldSpriteObjects = new ThrottledCollection<QuadTree<SpriteBase, List<SpriteBase>>, SpriteBase>(new QuadTree<SpriteBase, List<SpriteBase>>(bounds, 32));
            this.worldSimObjects = new ThrottledCollection<SortedSet<SimBase>, SimBase>(new SortedSet<SimBase>());
            this.screenDrawableComponents = new ThrottledCollection<SortedSet<ScreenDrawableComponent>, ScreenDrawableComponent>(new SortedSet<ScreenDrawableComponent>());
            this.physicsWorld = new FarseerPhysics.Dynamics.World(gravity);
            this.worldPerPixelRatio = worldPerPixelRatio;
            this.parentGame = parentGame;
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets the bounds of the world.
        /// </summary>
        /// <value>
        /// The world bounds.
        /// </value>
        public RectangleF Bounds
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the physics simulator world associated with this world
        /// </summary>
        public FarseerPhysics.Dynamics.World PhysicsWorld
        {
            get
            {
                return this.physicsWorld;
            }
        }

        /// <summary>
        /// Gets or sets the number of pixels per world unit.
        /// </summary>
        /// <value>The ratio of pixels to world units.</value>
        public float PixelPerWorldRatio
        {
            get
            {
                return 1 / this.worldPerPixelRatio;
            }

            set
            {
                this.worldPerPixelRatio = 1 / value;
            }
        }

        /// <summary>
        /// Gets or sets the number of world units per pixel.
        /// </summary>
        /// <value>The ratio of world units to pixels.</value>
        public float WorldPerPixelRatio
        {
            get
            {
                return this.worldPerPixelRatio;
            }

            set
            {
                this.worldPerPixelRatio = value;
            }
        }

        /// <summary>
        /// Gets the total number of <see cref="SimBase"/>s in the world.
        /// </summary>
        public int SimObjectCount
        {
            get
            {
                return this.worldSimObjects.Count;
            }
        }

        /// <summary>
        /// Gets the total number of <see cref="SpriteBase"/>s in the world.
        /// </summary>
        public int SpriteObjectCount
        {
            get
            {
                return this.worldSpriteObjects.Count;
            }
        }
        #endregion

        #region Collection Functions
        /// <summary>
        /// Gets all of the sprite objects in the scene in the specified area.
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>An IEnumerable of sprites in the specified area.</returns>
        public IEnumerable<SpriteBase> GetSpriteObjectsInArea(RectangleF area)
        {
            return this.worldSpriteObjects.GetBackingCollection().GetItemsInBounds(area);
        }

        /// <summary>
        /// Requests for the specified object's content to be loaded.
        /// </summary>
        /// <param name="obj">Object to load content for.</param>
        public void LoadDrawableContent(IDrawableComponent obj)
        {
            obj.LoadContent(this.parentGame.Content);
        }

        /// <summary>
        /// Adds a <see cref="SimBase"/> object to the simulation cluster.
        /// </summary>
        /// <param name="simObject">The <see cref="SimBase"/> object to add.</param>
        internal void AddSimObject(SimBase simObject)
        {
            this.worldSimObjects.Add(simObject);
        }

        /// <summary>
        /// Removes a <see cref="SimBase"/> object to the simulation cluster.
        /// </summary>
        /// <param name="simObject">The <see cref="SimBase"/> object to remove.</param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c></returns>
        internal bool RemoveSimObject(SimBase simObject)
        {
            return this.worldSimObjects.Remove(simObject);
        }

        /// <summary>
        /// Adds a <see cref="SpriteBase"/> derived object to the render pool.
        /// </summary>
        /// <param name="spriteObject">The sprite object to add.</param>
        internal void AddSpriteObject(SpriteBase spriteObject)
        {
            this.worldSpriteObjects.Add(spriteObject, (obj) => this.LoadDrawableContent(obj), null);
            //this.AddSimObject(spriteObject);
        }

        /// <summary>
        /// Removes a <see cref="SpriteBase"/> derived object to the render pool.
        /// </summary>
        /// <param name="spriteObject">The sprite object to remove.</param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c></returns>
        internal bool RemoveSpriteObject(SpriteBase spriteObject)
        {
            return this.worldSpriteObjects.Remove(spriteObject);
        }

        /// <summary>
        /// Adds a <see cref="ScreenDrawableComponent"/> derived object to the render pool.
        /// </summary>
        /// <param name="component">The component object to add.</param>
        internal void AddScreenDrawableComponent(ScreenDrawableComponent component)
        {
            this.screenDrawableComponents.Add(component, (obj) => obj.LoadContent(this.parentGame.Content), null);
        }

        /// <summary>
        /// Removes a <see cref="ScreenDrawableComponent"/> derived object to the render pool.
        /// </summary>
        /// <param name="component">The component object to remove.</param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c></returns>
        internal bool RemoveScreenDrawableComponent(ScreenDrawableComponent component)
        {
            return this.screenDrawableComponents.Remove(component);
        }
        #endregion

        #region World Space Functions
        /// <summary>
        /// Converts pixels to world units.
        /// </summary>
        /// <param name="pixels">The pixel value.</param>
        /// <returns>The world unit version of the pixel value.</returns>
        public float GetWorldFromPixel(float pixels)
        {
            return pixels / this.PixelPerWorldRatio;
        }

        /// <summary>
        /// Converts pixels to world units.
        /// </summary>
        /// <param name="pixels">The pixel value.</param>
        /// <returns>The world unit version of the pixel value.</returns>
        public Vector2 GetWorldFromPixel(Vector2 pixels)
        {
            return pixels / this.PixelPerWorldRatio;
        }

        /// <summary>
        /// Converts pixels to world units.
        /// </summary>
        /// <param name="pixel">The pixel value.</param>
        /// <returns>The world unit version of the pixel value.</returns>
        public Vector3 GetWorldFromPixel(Vector3 pixel)
        {
            return pixel / this.PixelPerWorldRatio;
        }

        /// <summary>
        /// Converts pixels to world units.
        /// </summary>
        /// <param name="pixel">The pixel value.</param>
        /// <returns>The world unit version of the pixel value.</returns>
        public RectangleF GetWorldFromPixel(Rectangle pixel)
        {
            return new RectangleF(pixel.X / this.PixelPerWorldRatio, pixel.Y / this.PixelPerWorldRatio, pixel.Width / this.PixelPerWorldRatio, pixel.Height / this.PixelPerWorldRatio);
        }

        /// <summary>
        /// Converts pixels to world units.
        /// </summary>
        /// <param name="pixel">The pixel value.</param>
        /// <returns>The world unit version of the pixel value.</returns>
        public RectangleF GetWorldFromPixel(RectangleF pixel)
        {
            return new RectangleF(pixel.X / this.PixelPerWorldRatio, pixel.Y / this.PixelPerWorldRatio, pixel.Width / this.PixelPerWorldRatio, pixel.Height / this.PixelPerWorldRatio);
        }

        /// <summary>
        /// Converts world units to pixels.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public float GetPixelFromWorld(float world)
        {
            return world / this.WorldPerPixelRatio;
        }

        /// <summary>
        /// Converts world units to pixels.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public Vector2 GetPixelFromWorld(Vector2 world)
        {
            return world / this.WorldPerPixelRatio;
        }

        /// <summary>
        /// Converts world units to pixels.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public Vector3 GetPixelFromWorld(Vector3 world)
        {
            return world / this.WorldPerPixelRatio;
        }

        /// <summary>
        /// Converts world units to pixels.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public RectangleF GetPixelFromWorld(Rectangle world)
        {
            return new RectangleF(world.X / this.WorldPerPixelRatio, world.Y / this.WorldPerPixelRatio, world.Width / this.WorldPerPixelRatio, world.Height / this.WorldPerPixelRatio);
        }

        /// <summary>
        /// Converts world units to pixels.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public RectangleF GetPixelFromWorld(RectangleF world)
        {
            return new RectangleF(world.X / this.WorldPerPixelRatio, world.Y / this.WorldPerPixelRatio, world.Width / this.WorldPerPixelRatio, world.Height / this.WorldPerPixelRatio);
        }

        /// <summary>
        /// Converts world units to pixel units.
        /// </summary>
        /// <param name="world">The world value.</param>
        /// <returns>The pixel version of the world value.</returns>
        public VertexPrimitive GetPixelFromWorld(VertexPrimitive world)
        {
            return new VertexPrimitive(world, this.PixelPerWorldRatio);
        }
        #endregion

        #region Event Functions
        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            // Process throttled collection updates
            this.worldSimObjects.ProcessUpdates();
            this.worldSpriteObjects.ProcessUpdates();
            this.screenDrawableComponents.ProcessUpdates();

            // Update all physics objects
            this.physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update all the simulated objects
            foreach (SimBase curr in this.worldSimObjects)
            {
                if (curr.Enabled)
                {
                    curr.Update(gameTime);

                    if (curr is SpriteBase)
                    {
                        // Make sure this object is correctly placed in the quad tree
                        this.worldSpriteObjects.GetBackingCollection().Reposition((SpriteBase)curr);
                    }
                }
            }

            // Update all the screen components
            foreach (var curr in this.screenDrawableComponents)
            {
                if (curr.Enabled)
                {
                    curr.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Requests that each sprite render its color map.
        /// </summary>
        /// <param name="sprites">The sprites to render.</param>
        /// <param name="gameTime">The game time.</param>
        /// <param name="renderSystem">The render system to render with.</param>
        internal void DrawSpriteColor(IEnumerable<SpriteBase> sprites, GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            foreach (SpriteBase curr in sprites)
            {
                if (curr.IsVisible)
                {
                    curr.DrawColorMap(gameTime, renderSystem);
                }
            }
        }

        /// <summary>
        /// Requests that each sprite render its normal map.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="renderSystem">The render system to render with.</param>
        internal void DrawSpriteNormal(IEnumerable<SpriteBase> sprites, GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            foreach (SpriteBase curr in sprites)
            {
                if (curr.IsVisible)
                {
                    curr.DrawNormalMap(gameTime, renderSystem);
                }
            }
        }

        /// <summary>
        /// Requests that each sprite render its options map.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="renderSystem">The render system to render with.</param>
        internal void DrawSpriteOptions(IEnumerable<SpriteBase> sprites, GameTime gameTime, DeferredRenderSystem renderSystem)
        {
            foreach (SpriteBase curr in sprites)
            {
                if (curr.IsVisible)
                {
                    curr.DrawOptionsMap(gameTime, renderSystem);
                }
            }
        }

        /// <summary>
        /// Requests that each screen drawable component render.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The <see cref="ScreenSpriteBatch"/> to render with.</param>
        internal void DrawScreenDrawableComponents(GameTime gameTime, ScreenSpriteBatch spriteBatch)
        {
            foreach (var curr in this.screenDrawableComponents)
            {
                curr.Draw(gameTime, spriteBatch);
            }
        }
        #endregion
    }
}
