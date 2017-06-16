// -----------------------------------------------------------------------
// <copyright file="ThrottledCollection.cs" company="">
// Copyright (C) 2012 Will Graham & Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Provides the ability to batch and queue updates against a collection so that they may be 
    /// performed at a time when it is safe to do so.
    /// </summary>
    /// <typeparam name="T">The collection generic to wrap.</typeparam>
    /// <typeparam name="U">The type of data stored in the collection.</typeparam>
    public class ThrottledCollection<T, U> : ICollection<U>
        where T : ICollection<U>
        where U : class
    {
        #region Private "Members"
        /// <summary>
        /// The internal collection.
        /// </summary>
        private T collection;

        /// <summary>
        /// The pending updates to perform against the collection.
        /// </summary>
        private ConcurrentQueue<CollectionUpdate> pendingUpdates;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledCollection&lt;T, U&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection to throttle</param>
        public ThrottledCollection(T collection)
        {
            this.collection = collection;
            this.pendingUpdates = new ConcurrentQueue<CollectionUpdate>();
        }
        #endregion

        #region UpdateType Enum
        /// <summary>
        /// Represents the type of update being performed.
        /// </summary>
        private enum UpdateType
        {
            /// <summary>
            /// Add update action
            /// </summary>
            Add,

            /// <summary>
            /// Remove update action
            /// </summary>
            Remove,

            /// <summary>
            /// Clear update action
            /// </summary>
            Clear
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        public int Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Queues a request to add the specified item to the collection.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(U item)
        {
            this.Add(item, null, null);
        }

        /// <summary>
        /// Queues a request to add the specified item to the collection.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="onPreUpdateProcessed">The action to perform before the update has been processed (null for no action).</param>
        /// <param name="onUpdateProcessed">The action to perform after the update has been processed (null for no action).</param>
        public void Add(U item, Action<U> onPreUpdateProcessed, Action<U> onUpdateProcessed)
        {
            this.pendingUpdates.Enqueue(new CollectionUpdate(UpdateType.Add, item, onPreUpdateProcessed, onUpdateProcessed));
        }

        /// <summary>
        /// Queues a request to clear the collection.
        /// </summary>
        public void Clear()
        {
            this.Clear(null, null);
        }

        /// <summary>
        /// Queues a request to clear the collection.
        /// </summary>
        /// <param name="onPreUpdateProcessed">The action to perform before the update has been processed (null for no action).</param>
        /// <param name="onUpdateProcessed">The action to perform after the update has been processed (null for no action).</param>
        public void Clear(Action<U> onPreUpdateProcessed, Action<U> onUpdateProcessed)
        {
            this.pendingUpdates.Enqueue(new CollectionUpdate(UpdateType.Clear, null, onPreUpdateProcessed, onUpdateProcessed));
        }

        /// <summary>
        /// Determines whether the specified item is in the collection; does not account for pending updates.
        /// </summary>
        /// <param name="item">The item check for in the collection.</param>
        /// <returns>
        ///   <c>true</c> if the specified item is in the collection; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(U item)
        {
            return this.collection.Contains(item);
        }

        /// <summary>
        /// Copies the elements in the collection to the specified array starting at the given index in the destination.
        /// </summary>
        /// <param name="array">The array into which objects will be copied.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(U[] array, int arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Queues a request to remove the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Always returns true.</returns>
        public bool Remove(U item)
        {
            return this.Remove(item, null, null);
        }

        /// <summary>
        /// Queues a request to remove the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="onPreUpdateProcessed">The action to perform before the update has been processed (null for no action).</param>
        /// <param name="onUpdateProcessed">The action to perform after the update has been processed (null for no action).</param>
        /// <returns>Always returns true.</returns>
        public bool Remove(U item, Action<U> onPreUpdateProcessed, Action<U> onUpdateProcessed)
        {
            this.pendingUpdates.Enqueue(new CollectionUpdate(UpdateType.Remove, item, onPreUpdateProcessed, onUpdateProcessed));
            return true;
        }

        /// <summary>
        /// Processes the pending updates against the collection.
        /// </summary>
        public void ProcessUpdates()
        {
            CollectionUpdate update;
            while (!this.pendingUpdates.IsEmpty && this.pendingUpdates.TryDequeue(out update))
            {
                update.InvokePreUpdateProcessed();

                switch (update.UpdateType)
                {
                    case UpdateType.Add:
                        this.collection.Add(update.Item);
                        break;

                    case UpdateType.Remove:
                        this.collection.Remove(update.Item);
                        break;

                    case UpdateType.Clear:
                        this.collection.Clear();
                        break;
                }

                update.InvokeUpdateProcessed();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<U> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.collection).GetEnumerator();
        }

        /// <summary>
        /// Gets the backing collection (USE THIS WITH CARE).
        /// </summary>
        /// <returns>The backing collection.</returns>
        internal T GetBackingCollection()
        {
            return this.collection;
        }
        #endregion

        #region Private Collection Update Class
        /// <summary>
        /// Represents an update to be carried out against the collection.
        /// </summary>
        private class CollectionUpdate
        {
            #region Private "Members" (heh heh)
            /// <summary>
            /// The type of the update.
            /// </summary>
            private UpdateType updateType;

            /// <summary>
            /// The item against which the action is to be performed.
            /// </summary>
            private U item;

            /// <summary>
            /// Action called after the collection update is processed.
            /// </summary>
            private Action<U> onPreUpdateProcessed;

            /// <summary>
            /// Action called after the collection update is processed.
            /// </summary>
            private Action<U> onUpdateProcessed;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ThrottledCollection{T, U}.CollectionUpdate"/> class.
            /// </summary>
            /// <param name="updateType">Type of the update.</param>
            /// <param name="item">The item to perform this update on.</param>
            /// <param name="onPreUpdateProcessed">Action to perform before this update has been processed.</param>
            /// <param name="onUpdateProcessed">Action to perform after this update has been processed.</param>
            public CollectionUpdate(UpdateType updateType, U item, Action<U> onPreUpdateProcessed, Action<U> onUpdateProcessed)
            {
                this.updateType = updateType;
                this.item = item;
                this.onPreUpdateProcessed = onPreUpdateProcessed;
                this.onUpdateProcessed = onUpdateProcessed;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the type of the update.
            /// </summary>
            /// <value>
            /// The type of the update.
            /// </value>
            public UpdateType UpdateType
            {
                get
                {
                    return this.updateType;
                }
            }

            /// <summary>
            /// Gets the item against which the action is to be performed.
            /// </summary>
            public U Item
            {
                get
                {
                    return this.item;
                }
            }
            #endregion

            #region Public Methods
            public void InvokePreUpdateProcessed()
            {
                this.onPreUpdateProcessed?.Invoke(this.item);
            }

            /// <summary>
            /// Invokes the update processed.
            /// </summary>
            public void InvokeUpdateProcessed()
            {
                this.onUpdateProcessed?.Invoke(this.item);
            }
            #endregion
        }
        #endregion
    }
}