// -----------------------------------------------------------------------
// <copyright file="VertexCollection.cs" company="">
// Copyright (C) Matthew Razza
// </copyright>
// -----------------------------------------------------------------------

namespace CelestialEngine.Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A simple collection of <see cref="Vector2"/>s that represent a primitive convex shape.
    /// </summary>
    /// <remarks>This class does not provide the complex collision and rotation functionality of <see cref="RectangleF"/>.</remarks>
    public class VertexPrimitive : ICollection<Vector2>, IEnumerable<Vector2>
    {
        #region Private Members
        /// <summary>
        /// The primitive type.
        /// </summary>
        private PrimitiveType primitiveType;

        /// <summary>
        /// Collection of vertices.
        /// </summary>
        private List<Vector2> vertexList;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPrimitive"/> class using the default primitive type and capacity.
        /// </summary>
        /// <remarks>
        /// Defaults to TriangleStrip and 0 capacity.
        /// </remarks>
        public VertexPrimitive()
            : this(PrimitiveType.TriangleStrip, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPrimitive"/> class.
        /// </summary>
        /// <param name="primitiveType">Type of the primitive.</param>
        /// <param name="capacity">The initial capacity of the internal collection.</param>
        public VertexPrimitive(PrimitiveType primitiveType, int capacity)
        {
            this.primitiveType = primitiveType;
            this.vertexList = new List<Vector2>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPrimitive"/> class for an array of vertices.
        /// </summary>
        /// <param name="primitiveType">Type of the primitive.</param>
        /// <param name="vertices">The array of vertices.</param>
        public VertexPrimitive(PrimitiveType primitiveType, Vector2[] vertices)
            : this(primitiveType, vertices.Length)
        {
            foreach (Vector2 currVector in vertices)
            {
                this.Add(currVector);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPrimitive"/> class for an existing instance and applies the specified scale to all vertices.
        /// </summary>
        /// <param name="source">The source vertex primitive.</param>
        /// <param name="scale">The scale factor.</param>
        public VertexPrimitive(VertexPrimitive source, float scale)
            : this(source.primitiveType, source.Count)
        {
            foreach (Vector2 currVector in source)
            {
                this.Add(currVector * scale);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        /// <value>The number of items in the collection..</value>
        public int Count
        {
            get
            {
                return this.vertexList.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the type of the primitive.
        /// </summary>
        /// <value>The type of the primitive.</value>
        public PrimitiveType PrimitiveType
        {
            get
            {
                return this.primitiveType;
            }

            set
            {
                this.primitiveType = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Xna.Framework.Vector2"/> at the specified index.
        /// </summary>
        /// <value>The new Vector2 value.</value>
        /// <returns>The Vector2 value at the specified index.</returns>
        public Vector2 this[int index]
        {
            get
            {
                return this.vertexList[index];
            }

            set
            {
                this.vertexList[index] = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the vertex data as an array of <see cref="VertexPositionTexture"/> objects.
        /// </summary>
        /// <remarks>
        /// This does NOT correctly set the texture coords for any of the vertices.
        /// </remarks>
        /// <returns>The array of vertex data.</returns>
        public VertexPositionTexture[] GetVertexData()
        {
            VertexPositionTexture[] data = new VertexPositionTexture[this.Count];

            for (int currVectorIndex = 0; currVectorIndex < this.Count; currVectorIndex++)
            {
                data[currVectorIndex] = new VertexPositionTexture(this.vertexList[currVectorIndex].ToVector3(), Vector2.Zero);
            }

            return data;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");

            foreach (var curr in this.vertexList)
            {
                if (builder.Length > 1)
                {
                    builder.Append(", ");
                }

                builder.Append(curr.ToString());
            }

            return builder.Append("}").ToString();
        }
        #endregion

        #region Collection Methods
        /// <summary>
        /// Appends the specified vertex to the primitive.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        public void Add(Vector2 vertex)
        {
            this.vertexList.Add(vertex);
        }

        /// <summary>
        /// Appends the vertices in the specified list to this primitive.
        /// </summary>
        /// <param name="vertices">The list of vertices.</param>
        public void Add(List<Vector2> vertices)
        {
            foreach (Vector2 currVector in vertices)
            {
                this.vertexList.Add(currVector);
            }
        }

        /// <summary>
        /// Clears this instance of all vertices.
        /// </summary>
        public void Clear()
        {
            this.vertexList.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex to check..</param>
        /// <returns>
        /// 	<c>true</c> if this instance contains the specified vertex; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 vertex)
        {
            return this.vertexList.Contains(vertex);
        }

        /// <summary>
        /// Copies the entire collection of vertices to the specified array starting at the given array index.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="arrayIndex">The starting index.</param>
        public void CopyTo(Vector2[] array, int arrayIndex)
        {
            this.vertexList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>True if removed; otherwise false</returns>
        public bool Remove(Vector2 vertex)
        {
            return this.vertexList.Remove(vertex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Vector2> GetEnumerator()
        {
            return this.vertexList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.vertexList.GetEnumerator();
        }
        #endregion
    }
}
