// -----------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Collections.QuadTree
{
    using System;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Represents a QuadTree which stores object type T
    /// </summary>
    /// <typeparam name="T">Type of object to store in the quad tree</typeparam>
    /// <typeparam name="C">Type of collection to use when storing items in each node</typeparam>
    internal class QuadTree<T, C> : ICollection<T>
        where T : IQuadTreeItem<T, C>
        where C : ICollection<T>, new()
    {
        /// <summary>
        /// The world bounds
        /// </summary>
        private RectangleF worldBounds;

        /// <summary>
        /// The preferred node size
        /// </summary>
        private uint preferredNodeSize;

        /// <summary>
        /// The root node of this quad tree
        /// </summary>
        private QuadTreeNode<T, C> rootNode;

        /// <summary>
        /// The object count
        /// </summary>
        private int objectCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{T}"/> class.
        /// </summary>
        /// <param name="worldBounds">The world bounds.</param>
        /// <param name="preferredNodeSize">Size of the preferred node.</param>
        public QuadTree(RectangleF worldBounds, uint preferredNodeSize)
        {
            this.worldBounds = worldBounds;
            this.preferredNodeSize = preferredNodeSize;
            this.rootNode = new QuadTreeNode<T, C>(null, this.worldBounds, this.preferredNodeSize);
            this.objectCount = 0;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                return this.objectCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            if (this.rootNode.Add(item))
            {
                this.objectCount++;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            this.rootNode.Clear();
            this.objectCount = 0;
        }

        /// <summary>
        /// Determines whether the specified item is contained in this QuadTree.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the item exists; otherwise <c>false</c></returns>
        public bool Contains(T item)
        {
            return this.rootNode.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var curr in this)
            {
                array.SetValue(curr, arrayIndex++);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c></returns>
        public bool Remove(T item)
        {
            var ret = this.rootNode.Remove(item);

            if (ret)
            {
                this.objectCount--;
            }

            return ret;
        }

        /// <summary>
        /// Repositions the specified item if it moved.
        /// </summary>
        /// <param name="item">The item to reposition.</param>
        /// <returns><c>true</c> if found and repositioned; otherwise, false</returns>
        public bool Reposition(T item)
        {
            var containingNode = item.QuadTreeNode;

            if (containingNode != null)
            {
                var newBounds = item.WorldBounds;

                if (containingNode.Contains(newBounds))
                {
                    // It still fits where it is
                    containingNode.AttemptImprovement(item);
                    return true;
                }
                else
                {
                    // It doesn't fit where it is
                    containingNode.RemoveFromSelf(item);
                    var containingParent = containingNode;
                    while ((containingParent = containingParent.Parent) != null)
                    {
                        if (containingParent.Contains(newBounds))
                        {
                            containingParent.Add(newBounds, item);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the items in specified bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>An enumerator that can be used to iterate through the items in the bounds.</returns>
        public IEnumerable<T> GetItemsInBounds(RectangleF bounds)
        {
            var node = this.rootNode.FindNode(bounds);
            foreach (var curr in node)
            {
                if (bounds.Intersects(curr.WorldBounds))
                    yield return curr;
            }

            while (node != null)
            {
                foreach (var curr in node.GetBackingCollection())
                {
                    if (bounds.Intersects(curr.WorldBounds))
                        yield return curr;
                }

                node = node.Parent;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.rootNode.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
