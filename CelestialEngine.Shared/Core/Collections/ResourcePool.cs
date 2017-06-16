// -----------------------------------------------------------------------
// <copyright file="ResourcePool.cs" company="">
// Copyright (C) 2013 Matthew Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Contains an implementation of a managed pool of recycled resources used to relieve pressure on the runtime garbage collector.
    /// </summary>
    /// <typeparam name="T">Type of object this pool is managing.</typeparam>
    public class ResourcePool<T> where T : class
    {
        #region Private Members
        /// <summary>
        /// Linked list that represents the pool of inactive resources.
        /// </summary>
        private LinkedList<PooledObject> inactivePool;

        /// <summary>
        /// Lock object used when accessing the inactive pool.
        /// </summary>
        private object inactivePoolLock;

        /// <summary>
        /// Timer that calls the method to free expired resources.
        /// </summary>
        private Timer releaseResourceTimer;

        /// <summary>
        /// The period of time that an object can sit in the pool before it is released.
        /// </summary>
        private TimeSpan expirationTimeSpan;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePool&lt;T&gt;"/> class.
        /// </summary>
        public ResourcePool(TimeSpan expirationTimeSpan)
        {
            this.expirationTimeSpan = expirationTimeSpan;
            this.inactivePool = new LinkedList<PooledObject>();
            this.inactivePoolLock = new object();
            this.releaseResourceTimer = new Timer(new TimerCallback(this.ReleaseExpiredResources));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets an object from the pool if one exists, otherwise creates one using the specified factory.
        /// </summary>
        /// <param name="factory">The factory to use when creating a new object.</param>
        /// <returns>The object from the pool</returns>
        public T Get(Func<T> factory)
        {
            PooledObject selectedObject = null;

            lock (this.inactivePoolLock)
            {
                if (this.inactivePool.Count > 0)
                {
                    selectedObject = this.inactivePool.First.Value;
                    this.inactivePool.RemoveFirst();
                }
            }

            if (selectedObject == null)
            {
                return factory();
            }

            return selectedObject.ObjectInstance;
        }

        /// <summary>
        /// Gets an object in the pool that matches the specified selector if it exists, otherwise it creates one.
        /// </summary>
        /// <param name="selector">The selector to match against.</param>
        /// <param name="factory">The factory to use when creating a new object.</param>
        /// <returns>An object that matches the specified selector.</returns>
        public T Get(Func<T, bool> selector, Func<T> factory)
        {
            PooledObject selectedObject = null;

            lock (this.inactivePoolLock)
            {
                foreach (PooledObject currObj in this.inactivePool)
                {
                    if (selector(currObj.ObjectInstance))
                    {
                        selectedObject = currObj;
                        break;
                    }
                }

                if (selectedObject != null)
                {
                    this.inactivePool.Remove(selectedObject);
                }
            }

            if (selectedObject == null)
            {
                return factory();
            }

            return selectedObject.ObjectInstance;
        }

        /// <summary>
        /// Releases the specified object into the pool for reuse.
        /// </summary>
        /// <param name="releasedObject">The object to release into the pool.</param>
        public void Release(T releasedObject)
        {
            PooledObject newPoolObject = new PooledObject(releasedObject);

            lock (this.inactivePoolLock)
            {
                this.inactivePool.AddLast(newPoolObject);

                if (this.inactivePool.Count == 1)
                {
                    this.UpdateExpiryTimer();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases the expired resources.
        /// </summary>
        /// <param name="state">The threading state object.</param>
        private void ReleaseExpiredResources(object state)
        {
            DateTime now = DateTime.Now;

            lock (this.inactivePoolLock)
            {
                while (this.inactivePool.First != null && this.inactivePool.First.Value.ReleasedTime + this.expirationTimeSpan <= now)
                {
                    this.inactivePool.RemoveFirst();
                }

                this.UpdateExpiryTimer();
            }
        }

        /// <summary>
        /// Updates the expiry timer.
        /// </summary>
        /// <remarks>This should be called from within a lock().</remarks>
        private void UpdateExpiryTimer()
        {
            if (this.inactivePool.Count > 0)
            {
                this.releaseResourceTimer.Change(this.expirationTimeSpan - (DateTime.Now - this.inactivePool.First.Value.ReleasedTime), new TimeSpan(-1));
            }
            else
            {
                this.releaseResourceTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
        #endregion

        #region PooledObject Internal Class
        /// <summary>
        /// Represents and contains the lifetime of an object released back into the resource pool.
        /// </summary>
        protected class PooledObject
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ResourcePool&lt;T&gt;.PooledObject"/> class using the current time.
            /// </summary>
            /// <param name="objectInstance">The object instance to be pooled.</param>
            public PooledObject(T objectInstance)
                : this(objectInstance, DateTime.Now)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ResourcePool&lt;T&gt;.PooledObject"/> class.
            /// </summary>
            /// <param name="objectInstance">The object instance to be pooled.</param>
            /// <param name="releasedTime">The time the instance was released to the pool.</param>
            public PooledObject(T objectInstance, DateTime releasedTime)
            {
                this.ObjectInstance = objectInstance;
                this.ReleasedTime = releasedTime;
            }
            #endregion

            #region Properties
            /// <summary>
            /// The instance of the object that's being pooled.
            /// </summary>
            public T ObjectInstance
            {
                get;
                private set;
            }

            /// <summary>
            /// The time the object was released into the pool.
            /// </summary>
            public DateTime ReleasedTime
            {
                get;
                private set;
            }
            #endregion
        }
        #endregion
    }
}
