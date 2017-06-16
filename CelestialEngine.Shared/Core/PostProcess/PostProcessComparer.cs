// -----------------------------------------------------------------------
// <copyright file="PostProcessComparer.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core.PostProcess
{
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of the IComparer interface so that IPostProcess components can be ordered.
    /// </summary>
    internal class PostProcessComparer : IComparer<IPostProcess>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public static int CompareIPostProcess(IPostProcess x, IPostProcess y)
        {
            if (x.RenderPriority - y.RenderPriority == 0)
            {
                // Are these actually the same instance?
                if (x == y)
                {
                    return 0; // Yes; let them be equal!
                }

                return x.GetHashCode() - y.GetHashCode(); // These are not the same instance, find a way to compare them
            }
            else if (x.RenderPriority - y.RenderPriority < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(IPostProcess x, IPostProcess y)
        {
            return PostProcessComparer.CompareIPostProcess(x, y);
        }
    }
}
