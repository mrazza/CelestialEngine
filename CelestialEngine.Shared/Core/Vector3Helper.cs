// -----------------------------------------------------------------------
// <copyright file="Vector3Helper.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Contains <see cref="Vector3"/> helper functions, constants, and extension methods.
    /// </summary>
    public static class Vector3Helper
    {
        #region Extension Methods
        /// <summary>
        /// Converts this <see cref="Vector3"/> to a  <see cref="Vector2"/> by dropping the Z-coord.
        /// </summary>
        /// <param name="targetVector">The target vector.</param>
        /// <returns>The Vector2 version of this instance.</returns>
        public static Vector2 ToVector2(this Vector3 targetVector)
        {
            return new Vector2(targetVector.X, targetVector.Y);
        }
        #endregion
    }
}
