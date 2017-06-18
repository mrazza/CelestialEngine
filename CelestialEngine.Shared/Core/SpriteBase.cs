// -----------------------------------------------------------------------
// <copyright file="SpriteBase.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Collections.QuadTree;
    using FarseerPhysics.Common;
    using FarseerPhysics.Common.Decomposition;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for any object that requires simulation and 2D drawing that
    /// exists within the world space.
    /// </summary>
    /// <seealso cref="SimBase"/>
    public abstract class SpriteBase : SimBase, IDrawableComponent, IComparable<SpriteBase>, IQuadTreeItem<SpriteBase, List<SpriteBase>>
    {
        #region Members
        /// <summary>
        /// Vertices of the sprite in world units.
        /// </summary>
        protected Vertices spriteWorldVertices;

        /// <summary>
        /// The change in scale from the default sprite's size.
        /// </summary>
        private Vector2 renderScale;

        /// <summary>
        /// Specifies whether or not the object is visible in the scene.
        /// </summary>
        private bool isVisible;

        /// <summary>
        /// Specifies the <see cref="SpriteRenderOptions"/> flags that will be applied to the sprite on rendering.
        /// </summary>
        private SpriteRenderOptions renderOptions;

        /// <summary>
        /// A float value that ranges between [0, 1] that represents what percentage of specular effects cast on
        /// the sprite reflect off to the camera. Default is 0.5.
        /// </summary>
        private float specularReflectivity;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBase"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        public SpriteBase(World world)
            : this(world, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBase"/> class.
        /// </summary>
        /// <param name="world">The <see cref="World" /> in which the instance lives.</param>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The starting velocity of the object.</param>
        public SpriteBase(World world, Vector2 position, Vector2 velocity)
            : base(world, position, velocity)
        {
            this.renderScale = Vector2.One;
            this.isVisible = true;
            this.specularReflectivity = 0.5f;
            this.World.AddSpriteObject(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the render scale from the default shape's size.
        /// </summary>
        /// <value>
        /// The render scale.
        /// </value>
        public Vector2 RenderScale
        {
            get
            {
                return this.renderScale;
            }

            set
            {
                this.renderScale = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible in the scene.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets the layer depth.
        /// </summary>
        /// <value>
        /// The layer depth used when rendering.
        /// </value>
        public byte LayerDepth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpriteRenderOptions"/> flags that will be applied to the sprite on rendering.
        /// </summary>
        /// <value>
        /// The render options.
        /// </value>
        public SpriteRenderOptions RenderOptions
        {
            get
            {
                return this.renderOptions;
            }

            set
            {
                this.renderOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets a float value that ranges between [0, 1] that represents what percentage of specular effects cast
        /// on the sprite reflect off to the camera. Default is 0.5.
        /// </summary>
        /// <remarks>
        /// Attempts to set the value outside the bounds [0, 1] results in the value being clamped inside the bounds.
        /// </remarks>
        public float SpecularReflectivity
        {
            get
            {
                return this.specularReflectivity;
            }

            set
            {
                this.specularReflectivity = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Gets the sprite's image bounds in world units.
        /// </summary>
        /// <remarks>
        /// This is an approximate bounding rectangle for the area impacted by this sprite. The actual area impacted may
        /// be smaller than this bounding rectangle and must NEVER be greater. For more percise bounding shapes, use the
        /// <see cref="SpriteWorldShape"/> property.
        /// </remarks>
        public abstract RectangleF SpriteWorldBounds
        {
            get;
        }

        /// <summary>
        /// Represents the shape of the sprite.
        /// </summary>
        public Vertices SpriteWorldShape
        {
            get
            {
                // TODO: Optimize this
                return this.spriteWorldVertices == null ? null : new Vertices(this.spriteWorldVertices.Select(v => (v * this.RenderScale * this.World.WorldPerPixelRatio).Rotate(this.Rotation) + this.Position));
            }
        }

        /// <summary>
        /// Gets the triangulated primitives that make up the shape of the sprite.
        /// </summary>
        /// <value>
        public VertexPrimitive[] SpriteWorldPrimitives
        {
            get
            {
                if (this.spriteWorldVertices == null)
                {
                    return null;
                }

                // TODO: Optimize this
                return Triangulate.ConvexPartition(this.SpriteWorldShape, TriangulationAlgorithm.Flipcode).Select(shape => new VertexPrimitive(PrimitiveType.TriangleStrip, shape.ToArray())).ToArray();
            }
        }

        /// <summary>
        /// Gets the world bounds.
        /// </summary>
        /// <value>
        /// The world bounds.
        /// </value>
        RectangleF IQuadTreeItem<SpriteBase, List<SpriteBase>>.WorldBounds
        {
            get
            {
                return this.SpriteWorldBounds;
            }
        }

        /// <summary>
        /// Gets or sets the quad tree node.
        /// </summary>
        /// <value>
        /// The quad tree node.
        /// </value>
        QuadTreeNode<SpriteBase, List<SpriteBase>> IQuadTreeItem<SpriteBase, List<SpriteBase>>.QuadTreeNode
        {
            get;
            set;
        }
        #endregion

        #region IDrawableComponent Methods
        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        public abstract void LoadContent(ExtendedContentManager contentManager);

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its color map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public abstract void DrawColorMap(GameTime gameTime, DeferredRenderSystem renderSystem);

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its normal map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public abstract void DrawNormalMap(GameTime gameTime, DeferredRenderSystem renderSystem);

        /// <summary>
        /// Called by the <see cref="DeferredRenderSystem"/> when this <see cref="SpriteBase"/> component needs to draw its options map.
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to DrawColorMap.</param>
        /// <param name="renderSystem"><see cref="DeferredRenderSystem"/> to render with.</param>
        public abstract void DrawOptionsMap(GameTime gameTime, DeferredRenderSystem renderSystem);
        #endregion

        #region IComparable Methods
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(SpriteBase other)
        {
            float layerDifference = this.LayerDepth - other.LayerDepth;

            if (layerDifference == 0)
            {
                // Are these actually the same instance?
                if (this == other)
                {
                    return 0; // Yes; let them be equal!
                }
                else
                {
                    // They're different, compare update order
                    int updateDifference = this.UpdateOrder - other.UpdateOrder;

                    if (updateDifference == 0)
                    {
                        return this.GetHashCode() - other.GetHashCode(); // Update order is the same, find a way to compare them
                    }
                    else if (updateDifference < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else if (layerDifference < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        #region IDisposable Method
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.World.RemoveSpriteObject(this);

            base.Dispose();
        }
        #endregion
    }
}
