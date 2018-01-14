// -----------------------------------------------------------------------
// <copyright file="Vector2Helper.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Contains <see cref="Vector2"/> helper functions, constants, and extension methods.
    /// </summary>
    public static class Vector2Helper
    {
        #region Constants
        /// <summary>
        /// Represents UP in world space.
        /// </summary>
        public static readonly Vector2 Up = new Vector2(0, -1);

        /// <summary>
        /// Represents RIGHT in world space.
        /// </summary>
        public static readonly Vector2 Right = new Vector2(1, 0);

        /// <summary>
        /// Represents DOWN in world space.
        /// </summary>
        public static readonly Vector2 Down = new Vector2(0, 1);

        /// <summary>
        /// Represents LEFT in world space.
        /// </summary>
        public static readonly Vector2 Left = new Vector2(-1, 0);
        #endregion

        #region Static Methods
        /// <summary>
        /// Gets a <see cref="Vector2"/> from a rotation (in radians).
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The normalized Vector2 representing the rotation.</returns>
        public static Vector2 VectorFromRotation(float rotation)
        {
            return new Vector2((float)Math.Sin(rotation), (float)-Math.Cos(rotation));
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Rotates this vector/point around the origin.
        /// </summary>
        /// <param name="targetVector">This <see cref="Vector2"/> instance.</param>
        /// <param name="rotation">The rotation to perform (in radians)</param>
        /// <returns>The new rotated vector.</returns>
        public static Vector2 Rotate(this Vector2 targetVector, float rotation)
        {
            return targetVector.RotateAbout(Vector2.Zero, rotation);
        }

        /// <summary>
        /// Rotates this point about another by the given rotation.
        /// </summary>
        /// <param name="targetPoint">This <see cref="Vector2"/> instance.</param>
        /// <param name="rotationPoint">The point to rotate about.</param>
        /// <param name="rotation">The rotation to perform (in radians).</param>
        /// <returns>The new rotated point.</returns>
        public static Vector2 RotateAbout(this Vector2 targetPoint, Vector2 rotationPoint, float rotation)
        {
            if (rotation == 0)
            {
                return targetPoint;
            }

            Vector2 difference = targetPoint - rotationPoint; // Center the point of rotation at the origin

            if (difference == Vector2.Zero)
            {
                return targetPoint;
            }
            else
            {
                float angle = difference.AngleBetween(Vector2Helper.Up) + rotation; // Get the new angle from the UP vector
                return (Vector2Helper.VectorFromRotation(angle) * difference.Length()) + rotationPoint; // Get the new vector and move it off the origin
            }
        }

        /// <summary>
        /// Gets the rotation from the vector relative to Vector2Helper.Up.
        /// </summary>
        /// <remarks>This function returns values between 0 and 2pi.</remarks>
        /// <param name="vector">Vector to get rotation from.</param>
        /// <returns>Rotation in radians.</returns>
        public static float Rotation(this Vector2 vector)
        {
            float angle = (float)Math.Atan2(vector.X, -vector.Y);

            if (angle < 0)
            {
                angle += MathHelper.Pi * 2.0f;
            }

            return angle;
        }

        /// <summary>
        /// Gets the angle (in radians) between this vector instance and the other specified vector.
        /// </summary>
        /// <remarks>This function always returns values between -Pi and Pi.</remarks>
        /// <param name="fromVector">This <see cref="Vector2"/> instance.</param>
        /// <param name="targetVector">Vector to find the rotation between.</param>
        /// <returns>The angle between the two vectors. A positive angle denotes that the target vector is clockwise relative to the source; otherwise, counter-clockwise.</returns>
        public static float AngleBetween(this Vector2 fromVector, Vector2 targetVector)
        {
            // As we want to encode direction in the response, it's cheaper to use Rotation() rather than Acos(Dot(A, B)).
            if (targetVector == Vector2.Zero || fromVector == Vector2.Zero)
            {
                return 0.0f;
            }

            float angle = fromVector.Rotation() - targetVector.Rotation();
            if (angle < -MathHelper.Pi)
            {
                angle += MathHelper.Pi * 2.0f;
            }
            else if (angle > MathHelper.Pi)
            {
                angle -= MathHelper.Pi * 2.0f;
            }

            return angle;
        }

        /// <summary>
        /// Converts this <see cref="Vector2"/> to a  <see cref="Vector3"/> setting the z-coord to 0.
        /// </summary>
        /// <param name="targetVector">The target vector.</param>
        /// <returns>The Vector3 version of this instance.</returns>
        public static Vector3 ToVector3(this Vector2 targetVector)
        {
            return new Vector3(targetVector, 0);
        }
        #endregion
    }
}
