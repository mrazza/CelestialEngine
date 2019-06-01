// -----------------------------------------------------------------------
// <copyright file="VerticesHelper.cs" company="">
// Copyright (C) 2016 Matt Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using FarseerPhysics.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Extension methods for the <see cref="Vertices"/> type.
    /// </summary>
    public static class VerticesHelper
    {
        /// <summary>
        /// Gets the relative extrema of the <see cref="Vertices"/> instance from the perspective of the specified point.
        /// </summary>
        /// <remarks>This only works if the relative point is OUTSIDE this shape.</remarks>
        /// <param name="relativePoint">The point in 2D world space to use as the perspective source.</param>
        /// <returns>The two relative extrema points.</returns>
        public static RelativeExtrema GetRelativeExtrema(this Vertices verts, Vector2 relativePoint)
        {
            // Calculate the initial values
            Vector2 centerVector = relativePoint - verts.GetCentroid();
            float angle = centerVector.AngleBetween(relativePoint - verts[0]);

            float minAngle = angle;
            float maxAngle = minAngle;
            Vector2 minVert = verts[0];
            Vector2 maxVert = minVert;

            // Loop through each vertex and update the min and max values
            for (int currVertIndex = 1; currVertIndex < verts.Count; currVertIndex++)
            {
                angle = centerVector.AngleBetween(relativePoint - verts[currVertIndex]);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    minVert = verts[currVertIndex];
                }
                else if (angle > maxAngle)
                {
                    maxAngle = angle;
                    maxVert = verts[currVertIndex];
                }
            }

            return new RelativeExtrema(minVert, maxVert, relativePoint);
        }
    }
}
