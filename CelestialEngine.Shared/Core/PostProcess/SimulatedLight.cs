// -----------------------------------------------------------------------
// <copyright file="SimulatedLight.cs" company="">
// Copyright (C) 2011-2013 Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.PostProcess
{
    using System.Collections.Generic;
    using CelestialEngine.Core;
    using CelestialEngine.Core.Shaders;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;

    /// <summary>
    /// Abstract base class for all simulated light post processing effects.
    /// This class provides shared functionality among all simulated lights.
    /// </summary>
    public abstract class SimulatedLight : SimulatedPostProcess
    {
        #region Members
        /// <summary>
        /// The change in Height in game units/second
        /// </summary>
        private float lightHeightVelocity;

        /// <summary>
        /// The position of the light
        /// </summary>
        private Vector3 lightPosition;

        /// <summary>
        /// The shader used when generating the shadow map.
        /// </summary>
        private Shader shadowMapShader;

        /// <summary>
        /// If true this light will cast shadows; otherwise the light will ignore shadow casting sprites
        /// </summary>
        private bool castsShadows;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedLight"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="shader">The shader to use with the light.</param>
        public SimulatedLight(World world, Shader shader)
            : base(world, shader)
        {
            this.lightPosition = Vector3.Zero;
            this.shadowMapShader = new Shader(Content.Shaders.Core.ShadowMap);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="Vector3"/> calculated position of the object in absolute 2D world space.
        /// </summary>
        /// <value>
        /// The position of the object in absolute 2D world space.
        /// </value>
        public new Vector3 Position
        {
            get
            {
                return this.lightPosition;
            }

            set
            {
                this.lightPosition = value;
                base.Position = new Vector2(value.X, value.Y);
            }
        }

        /// <summary>
        /// Gets the <see cref="Vector3"/> calculated position of the object in absolute 2D pixel space.
        /// </summary>
        /// <value>The position of the object in absolute 2D pixel space.</value>
        public new Vector3 PixelPosition
        {
            get
            {
                return this.World.GetPixelFromWorld(this.lightPosition);
            }
        }

        /// <summary>
        /// Gets or sets the velocity of the object in world space units/sec.
        /// (delta position over time)
        /// </summary>
        /// <value>
        /// The velocity of the object in units/sec.
        /// </value>
        public new Vector3 Velocity
        {
            get
            {
                return new Vector3(base.Velocity, this.lightHeightVelocity);
            }

            set
            {
                base.Velocity = new Vector2(value.X, value.Y);
                this.lightHeightVelocity = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this light should casts shadows.
        /// </summary>
        /// <value><c>true</c> if this light will casts shadows; otherwise, <c>false</c>.</value>
        public bool CastsShadows
        {
            get
            {
                return this.castsShadows;
            }

            set
            {
                this.castsShadows = value;
            }
        }

        /// <summary>
        /// Gets or sets the layer depth for this light (used when calculating which objects are in shadow).
        /// </summary>
        /// <value>
        /// The layer depth.
        /// </value>
        public byte LayerDepth
        {
            get;
            set;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a collection of <see cref="VertexPrimitive"/>s that make up the shadow mask from the light to the specified casting sprite.
        /// </summary>
        /// <param name="castingSprite">The sprite casting the shadow.</param>
        /// <param name="lightDrawBounds">The draw bounds of the light.</param>
        /// <returns>A collection of <see cref="VertexPrimitive"/>s that make up the shadow mask.</returns>
        public List<VertexPrimitive> GetAllShadowPrimitives()
        {
            RectangleF lightDrawBounds = this.GetWorldDrawBounds();
            return this.GetShadowEligibleSprites(lightDrawBounds)
                .SelectMany(sprite => this.CalculateShadowPrimitives(sprite, lightDrawBounds)).ToList();
        }

        /// <summary>
        /// Determines whether or not the specified point is lit by this light source.
        /// </summary>
        /// <remarks>
        /// Note that a point is considered lit if it is within the draw bounds of this light and
        /// is not obscured by a shadow cast by this light.
        /// </remarks>
        /// <param name="point">The point (in world space) to check.</param>
        /// <returns>True if this point is lit by this light; otherwise false.</returns>
        public bool IsPointLit(Vector2 point)
        {
            return this.GetShadowEligibleSprites(this.GetWorldDrawBounds())
                .SelectMany(sprite => this.CalculateShapePartsRelativeExtrema(sprite))
                .All(extrema => !extrema.IsPointBeyond(point) || !extrema.IsPointBetween(point));

        }

        /// <summary>
        /// Determines whether or not the specified sprite is lit by this light source.
        /// </summary>
        /// <remarks>
        /// Note that a sprite is considered lit if it is within the draw bounds of this light and
        /// is not fully obscured by a shadow cast by this light.
        /// </remarks>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>True if this sprite is lit by this light; otherwise false.</returns>
        public bool IsSpriteLit(SpriteBase sprite)
        {
            IEnumerable<Vector2> spriteVerts;
            if (sprite.SpriteWorldVertices != null)
            {
                spriteVerts = sprite.SpriteWorldVertices;
            }
            else
            {
                spriteVerts = sprite.SpriteWorldBounds.Vertices;
            }

            var extremas = this.GetShadowEligibleSprites(this.GetWorldDrawBounds())
                .Where(eligibleSprite => eligibleSprite != sprite)
                .SelectMany(eligibleSprite => this.CalculateShapePartsRelativeExtrema(eligibleSprite)).ToArray();
            return spriteVerts.Any(vertex => !extremas.Any(extrema => extrema.IsPointBeyond(vertex) && extrema.IsPointBetween(vertex)));

        }
        #endregion

        #region SimulatedPostProcess Overrides
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public override void LoadContent(ExtendedContentManager contentManager)
        {
            this.shadowMapShader.LoadContent(contentManager);
            base.LoadContent(contentManager);
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update position
            this.lightPosition.X = base.Position.X;
            this.lightPosition.Y = base.Position.Y;
            this.lightPosition.Z += this.lightHeightVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Generates the shadow map to be used when rendering this light.
        /// </summary>
        /// <remarks>If this instance is not to cast shadows the shadow map is cleared.</remarks>
        /// <param name="renderSystem">The render system to render with.</param>
        /// <param name="shadowMap">The shadow map to render on</param>
        protected virtual void GenerateShadowMap(DeferredRenderSystem renderSystem, RenderTarget2D shadowMap)
        {
            renderSystem.GraphicsDevice.SetRenderTarget(shadowMap);
            renderSystem.ClearCurrentRenderTarget(Color.White); // Clear the core shadow map

            // Only render the shadows if we are to cast them
            if (this.CastsShadows)
            {
                // Render the shadow caused by each object within the lights range
                RectangleF lightDrawBounds = this.GetWorldDrawBounds();
                List<SpriteBase> objectList = this.GetShadowEligibleSprites(lightDrawBounds).ToList();
                
                for (int objectIndex = 0; objectIndex < objectList.Count; objectIndex++)
                {
                    foreach (var primitives in this.CalculateShadowPrimitives(objectList[objectIndex], lightDrawBounds))
                    {
                        this.RenderShadow(renderSystem, primitives, objectList[objectIndex].LayerDepth);
                    }
                }
            }

            renderSystem.SetRenderTargets(RenderTargetTypes.None, 0); // Resolve the render target
        }

        /// <summary>
        /// Renders a single shadow from the specified extrema to the end of the visible light.
        /// </summary>
        /// <param name="renderSystem">The render system to render with.</param>
        /// <param name="shadowShape">A VertexPrimitive representing the shape of the shadow.</param>
        /// <param name="spriteLayerDepth">The layer depth of the sprite casting the shadow.</param>
        protected void RenderShadow(DeferredRenderSystem renderSystem, VertexPrimitive shadowShape, byte spriteLayerDepth)
        {
            this.shadowMapShader.ConfigureShader(renderSystem); // Configure the shader
            this.shadowMapShader.GetParameter("viewProjection").SetValue(renderSystem.GameCamera.GetViewProjectionMatrix(renderSystem));
            this.shadowMapShader.GetParameter("cameraPosition").SetValue(renderSystem.GameCamera.PixelPosition);
            this.shadowMapShader.GetParameter("layerDepth").SetValue(spriteLayerDepth / 255.0f);

            // Create the shadow map caused by the current sprite
            this.shadowMapShader.ApplyPass(0);

            renderSystem.DirectScreenPaint(shadowShape); // Render the shadow
        }

        /// <summary>
        /// Makes a <see cref="VertexPrimitive"/> representing the shape of the shadow cast from this light onto the extrema.
        /// </summary>
        /// <param name="extrema">The extrema of the shape casting the shadow.</param>
        /// <param name="lightDrawBounds">The draw bounds of the light.</param>
        /// <returns>A <see cref="VertexPrimitive"/> for the shadow.</returns>
        protected VertexPrimitive MakeShadowShape(RelativeExtrema extrema, RectangleF lightDrawBounds)
        {
            Vector2 widthVector = Vector2.Normalize(extrema.Min - base.Position); // Get the vector to the first extrema
            Vector2 heightVector = Vector2.Normalize(extrema.Max - base.Position); // Get the vector to the second extrema

            VertexPrimitive shadowArea = new VertexPrimitive(PrimitiveType.TriangleStrip, 4);
            shadowArea.Add(extrema.Min);
            shadowArea.Add(extrema.Max);

            // Let's extend the shadow vector until it hits the edge of the draw bounds
            shadowArea.Add(lightDrawBounds.CastInternalRay(extrema.Min, widthVector));
            shadowArea.Add(lightDrawBounds.CastInternalRay(extrema.Max, heightVector));

            // Let's get the remaining verts that might exist to finish up the rect
            List<Vector2> interiorVerts = lightDrawBounds.GetInteriorVertices(base.Position, widthVector, heightVector);
            shadowArea.Add(interiorVerts); // Add the interior verts (if any exist)

            return shadowArea;
        }

        /// <summary>
        /// Gets a collection of <see cref="VertexPrimitive"/>s that make up the shadow mask from the light to the specified casting sprite.
        /// </summary>
        /// <param name="castingSprite">The sprite casting the shadow.</param>
        /// <param name="lightDrawBounds">The draw bounds of the light.</param>
        /// <returns>A collection of <see cref="VertexPrimitive"/>s that make up the shadow mask.</returns>
        protected List<VertexPrimitive> CalculateShadowPrimitives(SpriteBase castingSprite, RectangleF lightDrawBounds)
        {
            return this.CalculateShapePartsRelativeExtrema(castingSprite).Select(extrema => this.MakeShadowShape(extrema, lightDrawBounds)).ToList();
        }

        /// <summary>
        /// Gets the relative extrema for each concave part of a given sprite.
        /// </summary>
        /// <remarks>
        /// For concave shapes there is a single set of relative extrema. For convex shapes there will be multiple.
        /// </remarks>
        /// <param name="castingSprite">The sprite to resolve extrema for.</param>
        /// <returns>Enumerable of sets of relative extrema.</returns>
        protected IEnumerable<RelativeExtrema> CalculateShapePartsRelativeExtrema(SpriteBase castingSprite)
        {
            if (castingSprite.SpriteWorldVertices != null)
            {
                if (castingSprite.SpriteWorldVertices.IsConvex())
                {
                    return new[] { castingSprite.SpriteWorldVertices.GetRelativeExtrema(base.Position) };
                }
                else
                {
                    // For concave shapes, we need to render a shadow for each convex part.
                    return castingSprite.SpriteWorldShapes.Select(shape => shape.GetRelativeExtrema(base.Position));
                }
            }
            else
            {
                return new[] { castingSprite.SpriteWorldBounds.GetRelativeExtrema(base.Position) };
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the <see cref="SpriteBase"/> objects that are within the provided draw bounds and eligible to cast shadows.
        /// </summary>
        /// <param name="lightDrawBounds">Draw bounds used when searching for sprites.</param>
        /// <returns>Enumerable of sprites eligible for shadow casting.</returns>
        private IEnumerable<SpriteBase> GetShadowEligibleSprites(RectangleF lightDrawBounds)
        {
            return this.World.GetSpriteObjectsInArea(lightDrawBounds).Where(currObj => (currObj.RenderOptions & SpriteRenderOptions.CastsShadows) != 0).OrderBy(s => s.LayerDepth);
        }
        #endregion
    }
}