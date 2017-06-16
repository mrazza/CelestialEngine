// -----------------------------------------------------------------------
// <copyright file="QuadTreeNode.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Collections.QuadTree
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System;
    using System.Collections;

    /// <summary>
    /// Represents a single node within a quadtree
    /// </summary>
    internal class QuadTreeNode<T, C> : IEnumerable<T>
        where T : IQuadTreeItem<T, C>
        where C : ICollection<T>, new()
    {
        /// <summary>
        /// The node bounds
        /// </summary>
        private RectangleF nodeBounds;

        /// <summary>
        /// The backing collection
        /// </summary>
        private C backingCollection;

        /// <summary>
        /// The top left sub node
        /// </summary>
        private QuadTreeNode<T, C> topLeftSubNode;

        /// <summary>
        /// The top right sub node
        /// </summary>
        private QuadTreeNode<T, C> topRightSubNode;

        /// <summary>
        /// The bottom left sub node
        /// </summary>
        private QuadTreeNode<T, C> bottomLeftSubNode;

        /// <summary>
        /// The bottom right sub node
        /// </summary>
        private QuadTreeNode<T, C> bottomRightSubNode;

        /// <summary>
        /// The preferred node size
        /// </summary>
        private uint preferredNodeSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTreeNode{T, C}"/> class.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="nodeBounds">The node bounds.</param>
        /// <param name="preferredNodeSize">Size of the preferred node.</param>
        public QuadTreeNode(QuadTreeNode<T, C> parent, RectangleF nodeBounds, uint preferredNodeSize)
        {
            this.backingCollection = new C();
            this.nodeBounds = nodeBounds;
            this.preferredNodeSize = preferredNodeSize;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent nide.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal QuadTreeNode<T, C> Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the specified item bounds.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item fit in this node; otherwise, false</returns>
        public bool Add(T item)
        {
            return this.Add(item.WorldBounds, item);
        }

        /// <summary>
        /// Adds the specified item bounds.
        /// </summary>
        /// <param name="itemBounds">The item bounds.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item fit in this node; otherwise, false</returns>
        public bool Add(RectangleF itemBounds, T item)
        {
            if (!this.nodeBounds.Contains(itemBounds))
                return false;

            if (this.IsSubDivided())
            {
                if (this.topLeftSubNode.Add(itemBounds, item))
                    return true;

                if (this.topRightSubNode.Add(itemBounds, item))
                    return true;

                if (this.bottomLeftSubNode.Add(itemBounds, item))
                    return true;

                if (this.bottomRightSubNode.Add(itemBounds, item))
                    return true;
            }

            this.backingCollection.Add(item);
            item.QuadTreeNode = this;

            if (!this.IsSubDivided() && this.backingCollection.Count > this.preferredNodeSize)
                this.SubDivide();

            return true;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if found and removed; otherwise, false</returns>
        public bool Remove(T item)
        {
            return this.Remove(item.WorldBounds, item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="itemBounds">The item bounds.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if found and removed; otherwise, false</returns>
        public bool Remove(RectangleF itemBounds, T item)
        {
            if (!this.nodeBounds.Contains(itemBounds))
                return false;
            
            if (this.RemoveFromSelf(item))
            {
                return true;
            }

            if (this.IsSubDivided())
            {
                if (this.topLeftSubNode.Remove(itemBounds, item))
                    return true;

                if (this.topRightSubNode.Remove(itemBounds, item))
                    return true;

                if (this.bottomLeftSubNode.Remove(itemBounds, item))
                    return true;

                if (this.bottomRightSubNode.Remove(itemBounds, item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to improve the accuracy of storage for the specified item.
        /// </summary>
        /// <remarks>
        /// This function makes a best-effort attempt to improve the storage accuracy of the specified item.
        /// It attempts to move this item to one of the child nodes of this <see cref="QuadTreeNode{T, C}"/> if possible.
        /// These calculations ignore rotation is it is not guarenteed that the item will be optimally stored
        /// after this call completes. However, this does prevent moving items from always bubbling to the root node.
        /// </remarks>
        /// <param name="item">The item.</param>
        public void AttemptImprovement(T item)
        {
            if (!this.IsSubDivided())
                return;

            var bounds = item.WorldBounds;
            var halfWidth = this.nodeBounds.Width / 2.0f;
            var halfHeight = this.nodeBounds.Height / 2.0f;

            // Could it fit in a sub square?
            // NOTE: All these calculations ignore rotation and so are a best-guess
            if (bounds.Width < halfWidth && bounds.Height < halfHeight)
            {
                var bRight = bounds.BottomRight;

                // Make sure it's not crossing between two squares
                if ((bRight.X % halfWidth) + this.nodeBounds.X > bounds.X &&
                    (bRight.Y % halfHeight) + this.nodeBounds.Y > bounds.Y)
                {
                    this.RemoveFromSelf(item);
                    this.Add(bounds, item);
                }
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.topLeftSubNode = null;
            this.topRightSubNode = null;
            this.bottomLeftSubNode = null;
            this.bottomRightSubNode = null;
            this.backingCollection.Clear();
        }

        /// <summary>
        /// Determines whether this node fully contains the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns><c>true</c> if the point is contained; otherwise, <c>false</c></returns>
        public bool Contains(RectangleF rect)
        {
            return this.nodeBounds.Contains(rect);
        }

        /// <summary>
        /// Determines whether this node contains the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the point is contained; otherwise, <c>false</c></returns>
        public bool Contains(Vector2 point)
        {
            return this.nodeBounds.Contains(point);
        }

        /// <summary>
        /// Determines whether this node or its subnodes contain the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item is contained; otherwise, <c>false</c></returns>
        public bool Contains(T item)
        {
            return this.FindNode(item) != null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var curr in this.backingCollection)
                yield return curr;

            if (this.IsSubDivided())
            {
                foreach (var curr in this.topLeftSubNode)
                    yield return curr;

                foreach (var curr in this.topRightSubNode)
                    yield return curr;

                foreach (var curr in this.bottomLeftSubNode)
                    yield return curr;

                foreach (var curr in this.bottomRightSubNode)
                    yield return curr;
            }
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

        /// <summary>
        /// Gets the collection enumerator.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        public IEnumerable<T> GetBackingCollection()
        {
            return this.backingCollection;
        }

        /// <summary>
        /// Finds the node containing the specified item.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>The <see cref="QuadTreeNode{T, C}"/> containing the specified item if found; otherwise, null.</returns>
        internal QuadTreeNode<T, C> FindNode(T item)
        {
            return this.FindNode(item.WorldBounds, item);
        }

        /// <summary>
        /// Finds the node containing the specified item.
        /// </summary>
        /// <param name="itemBounds">The bounds of the item to look for.</param>
        /// <param name="item">The item to look for.</param>
        /// <returns>The <see cref="QuadTreeNode{T, C}"/> containing the specified item if found; otherwise, null.</returns>
        internal QuadTreeNode<T, C> FindNode(RectangleF itemBounds, T item)
        {
            if (!this.Contains(itemBounds))
            {
                return null;
            }

            if (this.backingCollection.Contains(item))
            {
                return this;
            }

            if (this.IsSubDivided())
            {
                return this.topLeftSubNode.FindNode(itemBounds, item) ??
                    this.topRightSubNode.FindNode(itemBounds, item) ??
                    this.bottomLeftSubNode.FindNode(itemBounds, item) ??
                    this.bottomRightSubNode.FindNode(itemBounds, item);
            }

            return null;
        }

        /// <summary>
        /// Finds the node containing the specified bounds.
        /// </summary>
        /// <param name="itemBounds">The bounds to look for.</param>
        /// <returns>The <see cref="QuadTreeNode{T, C}"/> containing the specified item if found; otherwise, null.</returns>
        internal QuadTreeNode<T, C> FindNode(RectangleF itemBounds)
        {
            if (!this.Contains(itemBounds))
            {
                return null;
            }

            QuadTreeNode<T, C> node = null;
            
            if (this.IsSubDivided())
            {
                node = this.topLeftSubNode.FindNode(itemBounds) ??
                    this.topRightSubNode.FindNode(itemBounds) ??
                    this.bottomLeftSubNode.FindNode(itemBounds) ??
                    this.bottomRightSubNode.FindNode(itemBounds);
            }

            return node ?? this;
        }

        /// <summary>
        /// Removes the specified item from self (not children nodes).
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if contained in this node and removed; otherwise, false</returns>
        internal bool RemoveFromSelf(T item)
        {
            if (this.backingCollection.Remove(item))
            {
                item.QuadTreeNode = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether this node is sub divided.
        /// </summary>
        /// <returns><c>true</c> if sub divided; otherwise, <c>false</c></returns>
        private bool IsSubDivided()
        {
            return this.topLeftSubNode != null;
        }

        /// <summary>
        /// Subdivides this node.
        /// </summary>
        private void SubDivide()
        {
            if (this.IsSubDivided())
                throw new InvalidOperationException("Already subdivided.");

            var collection = this.backingCollection;
            this.backingCollection = new C();
            this.topLeftSubNode = new QuadTreeNode<T, C>(this, new RectangleF(this.nodeBounds.X, this.nodeBounds.Y, this.nodeBounds.Width / 2.0f, this.nodeBounds.Height / 2.0f, 0), this.preferredNodeSize);
            this.topRightSubNode = new QuadTreeNode<T, C>(this, new RectangleF(this.nodeBounds.X + this.nodeBounds.Width / 2.0f, this.nodeBounds.Y, this.nodeBounds.Width / 2.0f, this.nodeBounds.Height / 2.0f, 0), this.preferredNodeSize);
            this.bottomLeftSubNode = new QuadTreeNode<T, C>(this, new RectangleF(this.nodeBounds.X, this.nodeBounds.Y + this.nodeBounds.Height / 2.0f, this.nodeBounds.Width / 2.0f, this.nodeBounds.Height / 2.0f, 0), this.preferredNodeSize);
            this.bottomRightSubNode = new QuadTreeNode<T, C>(this, new RectangleF(this.nodeBounds.X + this.nodeBounds.Width / 2.0f, this.nodeBounds.Y + this.nodeBounds.Height / 2.0f, this.nodeBounds.Width / 2.0f, this.nodeBounds.Height / 2.0f, 0), this.preferredNodeSize);

            foreach (var curr in collection)
            {
                this.Add(curr);
            }
        }
    }
}
