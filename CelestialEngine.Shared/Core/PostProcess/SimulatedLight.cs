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

        /// <summary>
        /// Gets a collection of <see cref="VertexPrimitive"/>s that make up the shadow mask from the light to the specified casting sprite.
        /// </summary>
        /// <param name="castingSprite">The sprite casting the shadow.</param>
        /// <param name="lightDrawBounds">The draw bounds of the light.</param>
        /// <returns>A collection of <see cref="VertexPrimitive"/>s that make up the shadow mask.</returns>
        public List<VertexPrimitive> GetAllShadowPrimitives()
        {
            RectangleF lightDrawBounds = this.GetWorldDrawBounds();
            List<VertexPrimitive> result = new List<VertexPrimitive>();
            List<SpriteBase> objectList = this.World.GetSpriteObjectsInArea(lightDrawBounds).Where(currObj => (currObj.RenderOptions & SpriteRenderOptions.CastsShadows) != 0).OrderBy(s => s.LayerDepth).ToList();

            for (int objectIndex = 0; objectIndex < objectList.Count; objectIndex++)
            {
                result.AddRange(this.GetShadowPrimitives(objectList[objectIndex], lightDrawBounds));
            }

            return result;
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
                RectangleF lightDrawBounds = this.GetWorldDrawBounds();
                // Render the shadow caused by each object within the lights range
                List<SpriteBase> objectList = this.World.GetSpriteObjectsInArea(lightDrawBounds).Where(currObj => (currObj.RenderOptions & SpriteRenderOptions.CastsShadows) != 0).OrderBy(s => s.LayerDepth).ToList();
                
                for (int objectIndex = 0; objectIndex < objectList.Count; objectIndex++)
                {
                    foreach (var primitives in this.GetShadowPrimitives(objectList[objectIndex], lightDrawBounds))
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
        protected VertexPrimitive MakeShadowShape(Vector2[] extrema, RectangleF lightDrawBounds)
        {
            Vector2 widthVector = Vector2.Normalize(extrema[0] - base.Position); // Get the vector to the first extrema
            Vector2 heightVector = Vector2.Normalize(extrema[1] - base.Position); // Get the vector to the second extrema

            VertexPrimitive shadowArea = new VertexPrimitive(PrimitiveType.TriangleStrip, 4);
            shadowArea.Add(extrema[0]);
            shadowArea.Add(extrema[1]);

            // Let's extend the shadow vector until it hits the edge of the draw bounds
            shadowArea.Add(lightDrawBounds.CastInternalRay(extrema[0], widthVector));
            shadowArea.Add(lightDrawBounds.CastInternalRay(extrema[1], heightVector));

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
        public List<VertexPrimitive> GetShadowPrimitives(SpriteBase castingSprite, RectangleF lightDrawBounds)
        {
            List<VertexPrimitive> result = new List<VertexPrimitive>();

            if (castingSprite.SpriteWorldVertices != null)
            {
                if (castingSprite.SpriteWorldVertices.IsConvex())
                {
                    result.Add(this.MakeShadowShape(castingSprite.SpriteWorldVertices.GetRelativeExtrema(base.Position), lightDrawBounds));
                }
                else
                {
                    // For concave shapes, we need to render a shadow for each convex part.
                    foreach (var curr in castingSprite.SpriteWorldShapes)
                    {
                        result.Add(this.MakeShadowShape(curr.GetRelativeExtrema(base.Position), lightDrawBounds));
                    }
                }
            }
            else
            {
                result.Add(this.MakeShadowShape(castingSprite.SpriteWorldBounds.GetRelativeExtrema(base.Position), lightDrawBounds));
            }

            return result;
        }
        #endregion
    }
}