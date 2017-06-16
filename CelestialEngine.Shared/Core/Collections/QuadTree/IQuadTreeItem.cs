// -----------------------------------------------------------------------
// <copyright file="IQuadTreeItem.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Collections.QuadTree
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for items that can be stored in a <see cref="QuadTree{T, C}"/>
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <typeparam name="C">The collection type the item is being stored in.</typeparam>
    internal interface IQuadTreeItem<T, C>
        where T : IQuadTreeItem<T, C>
        where C : ICollection<T>, new()
    {
        /// <summary>
        /// Gets the world bounds.
        /// </summary>
        /// <value>
        /// The world bounds.
        /// </value>
        RectangleF WorldBounds
        {
            get;
        }

        /// <summary>
        /// Gets or sets the quad tree node.
        /// </summary>
        /// <value>
        /// The quad tree node.
        /// </value>
        QuadTreeNode<T, C> QuadTreeNode
        {
            get;
            set;
        }
    }
}
