namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents two points on a plane that are the extreme limits of some shape.
    /// </summary>
    public class RelativeExtrema
    {
        /// <summary>
        /// The min extrema (counter-clockwise).
        /// </summary>
        public Vector2 Min
        {
            get;
            private set;
        }

        /// <summary>
        /// The max extrema (clockwise).
        /// </summary>
        public Vector2 Max
        {
            get;
            private set;
        }

        /// <summary>
        /// The point these extrema are relative to.
        /// </summary>
        public Vector2 RelativePoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="RelativeExtrema"/> with the given min and max extrema.
        /// </summary>
        /// <param name="min">The min extrema value.</param>
        /// <param name="max">The max extrema value.</param>
        /// <param name="RelativePoint">The point the extrema are relative to.</param>
        public RelativeExtrema(Vector2 min, Vector2 max, Vector2 relativePoint)
        {
            this.Min = min;
            this.Max = max;
            this.RelativePoint = relativePoint;
        }

        /// <summary>
        /// Determines if the specified point is between these two relative extrema.
        /// </summary>
        /// <remarks>
        /// If you were to construct to lines that begin at the relative point. One that passes through
        /// the min point and extends to infinity, and one that passes through the max point and extends
        /// to infinity, this function will return true if the specified point lies between those two lines.
        /// </remarks>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the specified point is between the two relative extrema.</returns>
        public bool IsPointBetween(Vector2 point)
        {
            var pointVector = point - this.RelativePoint;
            var minAngle = pointVector.AngleBetween(this.Min - this.RelativePoint);
            var maxAngle = pointVector.AngleBetween(this.Max - this.RelativePoint);
            return  minAngle <= 0 && maxAngle >= 0;
        }

        /// <summary>
        /// Determines if the specified point is beyond the line created by the min and max point when
        /// compared to the relative point.
        /// </summary>
        /// <remarks>
        /// This returns true if the point is on the line.
        /// </remarks>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is beyond the line; false if the point is within.</returns>
        public bool IsPointBeyond(Vector2 point)
        {
            var pointDistance = ((point.X - this.Min.X) * (this.Max.Y - this.Min.Y)) - ((point.Y - this.Min.Y) * (this.Max.X - this.Min.X));
            var relativeDistance = ((this.RelativePoint.X - this.Min.X) * (this.Max.Y - this.Min.Y)) - ((this.RelativePoint.Y - this.Min.Y) * (this.Max.X - this.Min.X));
            return pointDistance == 0 || ((pointDistance > 0) != (relativeDistance > 0));
        }
    }
}
