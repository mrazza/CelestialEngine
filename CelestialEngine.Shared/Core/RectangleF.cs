// -----------------------------------------------------------------------
// <copyright file="RectangleF.cs" company="">
// Copyright (C) 2012-2013 Matt Razza & Will Graham
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Defines a floating point rectangle.
    /// </summary>
    public class RectangleF
    {
        #region Public Members
        /// <summary>
        /// The x-coordinate of the top-left side of the rectangle.
        /// </summary>
        public float X;

        /// <summary>
        /// The y-coordinate of the top-left side of the rectangle.
        /// </summary>
        public float Y;

        /// <summary>
        /// The Width of the rectangle.
        /// </summary>
        public float Width;

        /// <summary>
        /// The Height of the rectangle.
        /// </summary>
        public float Height;

        /// <summary>
        /// The rotation (in radians) of the rectangle about it's top-left point.
        /// </summary>
        public float Rotation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> struct.
        /// </summary>
        /// <param name="rect">The rectangle to create this instance off of.</param>
        public RectangleF(Rectangle rect)
            : this(rect.Left, rect.Top, rect.Width, rect.Height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> struct.
        /// </summary>
        /// <param name="x">The x-coordinate of the Y X of the rectangle.</param>
        /// <param name="y">The y-coordinate of the Y X of the rectangle.</param>
        /// <param name="width">The Width of the rectangle.</param>
        /// <param name="height">The Height of the rectangle.</param>
        public RectangleF(float x, float y, float width, float height)
            : this(x, y, width, height, 0.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleF"/> struct.
        /// </summary>
        /// <param name="x">The x-coordinate of the Y X of the rectangle.</param>
        /// <param name="y">The y-coordinate of the Y X of the rectangle.</param>
        /// <param name="width">The Width of the rectangle.</param>
        /// <param name="height">The Height of the rectangle.</param>
        /// <param name="rotation">The rotation (in radians) of the rectangle about it's top-left point.</param>
        public RectangleF(float x, float y, float width, float height, float rotation)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Rotation = rotation;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the area bounds (width/height).
        /// </summary>
        public Vector2 AreaBounds
        {
            get
            {
                return new Vector2(this.Width, this.Height);
            }
        }

        /// <summary>
        /// Gets the point at the center of the rectangle.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(this.X + (this.Width / 2.0f), this.Y + (this.Height / 2.0f)).RotateAbout(this.TopLeft, this.Rotation);
            }
        }

        /// <summary>
        /// Gets the position of the rectangle as a <see cref="Vector2"/> (top left corner).
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
        }

        /// <summary>
        /// Gets the top left corner position of the rectangle.
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
        }

        /// <summary>
        /// Gets the top right corner position of the rectangle.
        /// </summary>
        public Vector2 TopRight
        {
            get
            {
                return new Vector2(this.X + this.Width, this.Y).RotateAbout(this.TopLeft, this.Rotation);
            }
        }

        /// <summary>
        /// Gets the bottom left corner position of the rectangle.
        /// </summary>
        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(this.X, this.Y + this.Height).RotateAbout(this.TopLeft, this.Rotation);
            }
        }

        /// <summary>
        /// Gets the bottom right corner position of the rectangle.
        /// </summary>
        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(this.X + this.Width, this.Y + this.Height).RotateAbout(this.TopLeft, this.Rotation);
            }
        }

        /// <summary>
        /// Gets the vertices of this rectangle starting at the top left (top left, top right, bottom left, bottom right).
        /// </summary>
        /// <value>The vertices.</value>
        public Vector2[] Vertices
        {
            get
            {
                Vector2[] verts = new Vector2[4];
                verts[0] = this.TopLeft;
                verts[1] = this.TopRight;
                verts[2] = this.BottomLeft;
                verts[3] = this.BottomRight;

                return verts;
            }
        }
        #endregion

        #region Operator Overloads
        /// <summary>
        /// Implements the operator *.
        /// Scales the width and height of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The result of the operator.</returns>
        public static RectangleF operator *(RectangleF rect, float scale)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width * scale, rect.Height * scale);
        }

        /// <summary>
        /// Implements the operator /.
        /// Scales the width and height of the rectangle
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The result of the operator.</returns>
        public static RectangleF operator /(RectangleF rect, float scale)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width / scale, rect.Height / scale);
        }

        /// <summary>
        /// Implements the operator +.
        /// Translates the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="translation">The translation to perform.</param>
        /// <returns>The result of the operator.</returns>
        public static RectangleF operator +(RectangleF rect, Vector2 translation)
        {
            return new RectangleF(rect.X + translation.X, rect.Y + translation.Y, rect.Width, rect.Height, rect.Rotation);
        }

        /// <summary>
        /// Implements the operator -.
        /// Translates the rectangle
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="translation">The translation to perform.</param>
        /// <returns>The result of the operator.</returns>
        public static RectangleF operator -(RectangleF rect, Vector2 translation)
        {
            return rect + -translation;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Rotates this <see cref="RectangleF"/> instance about the specified point.
        /// </summary>
        /// <param name="point">Point to rotate about.</param>
        /// <param name="rotation">Rotation to apply.</param>
        /// <returns>The rotated rectangle.</returns>
        public RectangleF RotateAbout(Vector2 point, float rotation)
        {
            RectangleF newRectangle = this;
            Vector2 newPosition = this.Position.RotateAbout(point, rotation);
            newRectangle.X = newPosition.X;
            newRectangle.Y = newPosition.Y;
            newRectangle.Rotation = this.Rotation + rotation;

            return newRectangle;
        }

        /// <summary>
        /// Determines if the specified rectangle intersects with this instance.
        /// </summary>
        /// <param name="otherRectangle">The other rectangle.</param>
        /// <returns>True if it intersects with us, otherwise false.</returns>
        public bool Intersects(Rectangle otherRectangle)
        {
            return this.Intersects(new RectangleF(otherRectangle));
        }

        /// <summary>
        /// Determines if the specified rectangle intersects with this instance.
        /// </summary>
        /// <param name="otherRectangle">The other rectangle.</param>
        /// <returns>True if it intersects with us, otherwise false.</returns>
        public bool Intersects(RectangleF otherRectangle)
        {
            // Special case if no rectangles have rotation
            if (this.Rotation == 0 && otherRectangle.Rotation == 0)
            {
                return !(otherRectangle.X > this.X + this.Width ||
                         otherRectangle.X + otherRectangle.Width < this.X ||
                         otherRectangle.Y > this.Y + this.Height ||
                         otherRectangle.Y + otherRectangle.Height < this.Y);
            }
            else
            {
                // We can't use the special case here, we need to use the seperating axis theorem.

                // Test my normals
                foreach (Vector2 currNormal in this.GetSATNormals())
                {
                    // Get the projections
                    Vector2 myProj = this.Project(currNormal);
                    Vector2 theirProj = otherRectangle.Project(currNormal);

                    // If the projections do not overlap we found a seperating axis
                    if (!(myProj.X <= theirProj.Y && myProj.Y >= theirProj.X))
                    {
                        return false;
                    }
                }

                // Test their normals
                foreach (Vector2 currNormal in otherRectangle.GetSATNormals())
                {
                    // Get the projections
                    Vector2 myProj = this.Project(currNormal);
                    Vector2 theirProj = otherRectangle.Project(currNormal);

                    // If the projects do not overlap we found a seperating axis
                    if (!(myProj.X <= theirProj.Y && myProj.Y >= theirProj.X))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Determines whether the specified rectangle is fully contained by this instance.
        /// </summary>
        /// <param name="otherRectangle">The other rectangle.</param>
        /// <returns>
        ///   <c>true</c> if the specified rectangle is fully contained by this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Rectangle otherRectangle)
        {
            return this.Contains(new RectangleF(otherRectangle));
        }

        /// <summary>
        /// Determines whether the specified point is contained by this instance.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is contained by this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point)
        {
            // We can be clever here
            Vector2 rectSpacePoint = point.RotateAbout(this.Position, -this.Rotation);

            return rectSpacePoint.X >= this.X &&
                       rectSpacePoint.X <= this.X + this.Width &&
                       rectSpacePoint.Y >= this.Y &&
                       rectSpacePoint.Y <= this.Y + this.Height;
        }

        /// <summary>
        /// Determines whether the specified rectangle is fully contained by this instance.
        /// </summary>
        /// <param name="otherRectangle">The other rectangle.</param>
        /// <returns>
        ///   <c>true</c> if the specified rectangle is fully contained by this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(RectangleF otherRectangle)
        {
            // If neither rectangle is rotated we can use this special case.
            if (this.Rotation == 0 && otherRectangle.Rotation == 0)
            {
                return otherRectangle.X >= this.X && 
                       otherRectangle.X + otherRectangle.Width <= this.X + this.Width &&
                       otherRectangle.Y >= this.Y &&
                       otherRectangle.Y + otherRectangle.Height <= this.Y + this.Height;
            }
            else
            {
                // We can't use the special case, we need to use a modified version of the seperating axis theorem.

                // Test my normals
                foreach (Vector2 currNormal in this.GetSATNormals())
                {
                    // Get the projections
                    Vector2 myProj = this.Project(currNormal);
                    Vector2 theirProj = otherRectangle.Project(currNormal);

                    // If their projection is not fully contained in my projection I do not contain them
                    if (myProj.X > theirProj.X || myProj.Y < theirProj.Y)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Converts this instance to a <see cref="Rectangle"/>. There might be loss of percision.
        /// </summary>
        /// <remarks>
        /// This converts a floating point rectangle with rotation to a integer rectangle without rotation.
        /// </remarks>
        /// <returns>Rectangle version of this RectangleF.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)this.X, (int)this.Y, (int)this.Width, (int)this.Height);
        }

        /// <summary>
        /// Converts this instance to a <see cref="VertexPrimitive"/>.
        /// </summary>
        /// <returns>QuadF version of this RectangleF.</returns>
        public VertexPrimitive ToVertexPrimitive()
        {
            return new VertexPrimitive(PrimitiveType.TriangleStrip, this.Vertices);
        }

        /// <summary>
        /// Casts a ray that starts inside this rectangle instance and calculates where the ray intersects
        /// with this rectangle's perimeter.
        /// </summary>
        /// <remarks>
        /// If you pass in a point that does not exist inside this rectangle instance,
        /// the point is returned to you.
        /// </remarks>
        /// <param name="point">Point inside the rectangle to start the ray cast from.</param>
        /// <param name="vector">The directional vector to cast.</param>
        /// <returns>The point on one of this rectangle's sides where the specified vector intersects.</returns>
        public Vector2 CastInternalRay(Vector2 point, Vector2 vector)
        {
            // We can only cast rays internally, if the supplied point does not exist inside
            // this instance return the point
            if (!this.Contains(point))
            {
                return point;
            }

            // So the idea here is that all the calculations are to be one within "rectangle-space"
            // this way we don't need to worry about the fact that the rectangle might be rotated.
            // Once the scalar is calculated we can just apply it to the original vector.
            vector.Normalize(); // The vector must be normalized
            Vector2 rotatedVector = vector.Rotate(-this.Rotation); // Rotate the vector into rectangle space
            Vector2 positionalOffset = point.RotateAbout(this.Position, -this.Rotation) - this.Position; // Calculate the positional offset of the specified point
            Vector2 vectorLimiter = Vector2.Zero; // Init the vector limitor

            // Work on the X portion first, decide if we need to move positively or negatively in the X
            if (rotatedVector.X >= 0)
            {
                vectorLimiter.X = (this.Width - positionalOffset.X) / rotatedVector.X;
            }
            else
            {
                vectorLimiter.X = (-positionalOffset.X) / rotatedVector.X;
            }

            // Work on Y portion now, decide if we need to move positively or negatively in the Y
            if (rotatedVector.Y >= 0)
            {
                vectorLimiter.Y = (this.Height - positionalOffset.Y) / rotatedVector.Y;
            }
            else
            {
                vectorLimiter.Y = (-positionalOffset.Y) / rotatedVector.Y;
            }

            float vectorScalar = MathHelper.Min(vectorLimiter.X, vectorLimiter.Y); // Use the smaller of the two values

            return point + (vectorScalar * vector); // Calculate the resulting point
        }

        /// <summary>
        /// Gets the vertices that exist on the interior of the acute angle created by the specified internal point
        /// and the two vectors emanating from that point.
        /// </summary>
        /// <param name="point">The point on the inside of this rectangle instance where the vectors originate.</param>
        /// <param name="firstVector">The first vector forming the acute angle.</param>
        /// <param name="secondVector">The second vector forming the acute angle.</param>
        /// <returns>A list of vertices that exist on the interior of the acute angle.</returns>
        public List<Vector2> GetInteriorVertices(Vector2 point, Vector2 firstVector, Vector2 secondVector)
        {
            // Get angle bounds
            float minAngle = 0;
            float maxAngle = firstVector.AngleBetween(secondVector);

            if (maxAngle > MathHelper.Pi)
            {
                throw new ArgumentException("Angle created by the two vectors must be acute.", nameof(secondVector));
            }

            // Init the collection
            List<Vector2> vectorCollection = new List<Vector2>(4); // There can not be more than 4

            // Go through each vertex that make up this rectangle
            foreach (Vector2 currVert in this.Vertices)
            {
                // Find the angle between what's defined as UP and the current vertex
                float currAngle = firstVector.AngleBetween(currVert - point);

                // If the vertex is within the min and max angle we need it
                if (currAngle > minAngle && currAngle < maxAngle)
                {
                    // Make sure it's sorted least to greatest (insertion sort)
                    bool added = false;
                    for (int vectorIndex = 0; vectorIndex < vectorCollection.Count; vectorIndex++)
                    {
                        if (currAngle < firstVector.AngleBetween(vectorCollection[vectorIndex] - point))
                        {
                            vectorCollection.Insert(vectorIndex, currVert);
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                    {
                        vectorCollection.Add(currVert);
                    }
                }
            }

            return vectorCollection;
        }

        /// <summary>
        /// Gets the relative extrema of the <see cref="RectangleF"/> instance from the perspective of the specified point.
        /// </summary>
        /// <remarks>This only works if the relative point is OUTSIDE this rectangle instance.</remarks>
        /// <param name="relativePoint">The point in 2D world space to use as the perspective source.</param>
        /// <returns>The two relative extrema points.</returns>
        public Vector2[] GetRelativeExtrema(Vector2 relativePoint)
        {
            // Calculate the initial values
            Vector2 centerVector = relativePoint - this.Center;
            float angle = centerVector.AngleBetween(relativePoint - this.Vertices[0]);

            // Because AngleBetween(), place within -Pi -> Pi
            if (angle > MathHelper.Pi)
            {
                angle = (-MathHelper.Pi * 2.0f) + angle;
            }
            else if (angle < -MathHelper.Pi)
            {
                angle = (MathHelper.Pi * 2.0f) + angle;
            }

            float minAngle = angle;
            float maxAngle = minAngle;
            Vector2 minVert = this.Vertices[0];
            Vector2 maxVert = minVert;

            // Loop through each vertex and update the min and max values
            for (int currVertIndex = 1; currVertIndex < this.Vertices.Length; currVertIndex++)
            {
                angle = centerVector.AngleBetween(relativePoint - this.Vertices[currVertIndex]);

                // Because AngleBetween(), place within -Pi -> Pi
                if (angle > MathHelper.Pi)
                {
                    angle = (-MathHelper.Pi * 2.0f) + angle;
                }
                else if (angle < -MathHelper.Pi)
                {
                    angle = (MathHelper.Pi * 2.0f) + angle;
                }

                if (angle < minAngle)
                {
                    minAngle = angle;
                    minVert = this.Vertices[currVertIndex];
                }
                else if (angle > maxAngle)
                {
                    maxAngle = angle;
                    maxVert = this.Vertices[currVertIndex];
                }
            }

            return new Vector2[] { minVert, maxVert }; // Return the array
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            RectangleF rect = obj as RectangleF;

            if (rect != null)
            {
                return rect.X == this.X && rect.Y == this.Y && rect.Width == this.Width && rect.Height == this.Height && rect.Rotation == this.Rotation;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (int)(this.X + this.Y);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Projects this rectangle onto the specified axis.
        /// </summary>
        /// <remarks>
        /// The result is a <see cref="Vector2"/> where the X component is the min point of the projection on the axis and
        /// the Y component is the max point of the projection on the axis.
        /// </remarks>
        /// <param name="axis">The axis to project onto.</param>
        /// <returns>A <see cref="Vector2"/> containing the min and max points of the projection onto the specified axis.</returns>
        private Vector2 Project(Vector2 axis)
        {
            // Get the list of verts and set initial conditions
            Vector2[] verts = this.Vertices;
            float min = Vector2.Dot(axis, verts[0]);
            float max = min;

            // Project each vert onto the axis and find the extrema
            for (int currVertIndex = 1; currVertIndex < verts.Length; currVertIndex++)
            {
                float curr = Vector2.Dot(axis, verts[currVertIndex]); // Project this vertex onto the axis

                // Update our running min and max values
                if (curr < min)
                {
                    min = curr;
                }
                else if (curr > max)
                {
                    max = curr;
                }
            }

            return new Vector2(min, max); // Return a vector containing the min/max data
        }

        /// <summary>
        /// Builds an array of <see cref="Vector2"/> containing the normals used for the SAT algorithm for this <see cref="RectangleF"/> instance.
        /// </summary>
        /// <remarks>
        /// The array only contains the top and right normals as the bottom and left normals are the same (just rotated 180 degrees).
        /// </remarks>
        /// <returns>IEnumerable of normal vectors.</returns>
        private IEnumerable<Vector2> GetSATNormals()
        {
            Vector2 normal = Vector2Helper.VectorFromRotation(this.Rotation);
            yield return normal;
            yield return normal.Rotate(MathHelper.PiOver2);
        }
        #endregion
    }
}
