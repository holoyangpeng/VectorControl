using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace YP.SVG
{
    public partial struct Vector
    {
        #region Constructors
        /// Constructor which sets the vector's initial values
        /// 
        ///  float - The initial X 
        ///  float - THe initial Y  
        public Vector(float x, float y)
        {
            _x = x;
            _y = y;
        }
        #endregion Constructors

        #region .. private fields
        float _x, _y;
        #endregion

        #region Public Methods
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                this._x = value;
            }
        }

        public float Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }

        ///
        /// Length Property - the length of this Vector 
        /// 
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(_x * _x + _y * _y);
            }
        }

        /// 
        /// LengthSquared Property - the squared length of this Vector 
        /// 
        public float LengthSquared
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }

        /// 
        /// Normalize - Updates this Vector to maintain its direction, but to have a length
        /// of 1.  This is equivalent to dividing this Vector by Length
        /// 

        public void Normalize()
        {
            // Avoid overflow 
            this /= Math.Max(Math.Abs(_x), Math.Abs(_y));
            this /= Length;
        }
        /// 
        /// CrossProduct - Returns the cross product: vector1.X*vector2.Y - vector1.Y*vector2.X 
        /// 
        ///  
        /// Returns the cross product: vector1.X*vector2.Y - vector1.Y*vector2.X 
        /// 
        ///  The first Vector  
        ///  The second Vector 
        public static float CrossProduct(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// 

        /// AngleBetween - the angle between 2 vectors
        /// 
        /// 
        /// Returns the the angle in degrees between vector1 and vector2
        /// 
        ///  The first Vector  
        ///  The second Vector 
        public static float AngleBetween(Vector vector1, Vector vector2)
        {
            float sin = vector1._x * vector2._y - vector2._x * vector1._y;
            float cos = vector1._x * vector2._x + vector1._y * vector2._y;
            return (float)(Math.Atan2(sin, cos) * (180 / Math.PI));
        }

        #endregion Public Methods

        #region Public Operators
        /// 
        /// Operator -Vector (unary negation) 
        /// 
        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector._x, -vector._y);
        }

        /// 

        /// Negates the values of X and Y on this Vector
        /// 

        public void Negate()
        {
            _x = -_x;
            _y = -_y;
        }

        /// 

        /// Operator Vector + Vector
        /// 

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        /// 

        /// Add: Vector + Vector
        /// 

        public static Vector Add(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        /// 

        /// Operator Vector - Vector
        /// 

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        /// 

        /// Subtract: Vector - Vector
        /// 

        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        /// 

        /// Operator Vector + PointF
        /// 

        public static PointF operator +(Vector vector, PointF point)
        {
            return new PointF(point.X + vector._x, point.Y + vector._y);
        }
        /// 

        /// Add: Vector + PointF 
        /// 
        public static PointF Add(Vector vector, PointF point)
        {
            return new PointF(point.X + vector._x, point.Y + vector._y);
        }

        /// 
        /// Operator Vector * float 
        /// 

        public static Vector operator *(Vector vector, float scalar)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// 
        /// Multiply: Vector * float 
        /// 

        public static Vector Multiply(Vector vector, float scalar)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// 
        /// Operator float * Vector 
        /// 

        public static Vector operator *(float scalar, Vector vector)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// 
        /// Multiply: float * Vector 
        /// 

        public static Vector Multiply(float scalar, Vector vector)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        /// 
        /// Operator Vector / float 
        /// 

        public static Vector operator /(Vector vector, float scalar)
        {
            return vector * (1.0f / scalar);
        }
        /// 

        /// Multiply: Vector / float
        /// 

        public static Vector Divide(Vector vector, float scalar)
        {
            return vector * (1.0f / scalar);
        }

        /// 
        /// Operator Vector * Vector, interpreted as their dot product
        /// 
        public static float operator *(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }
        /// 

        /// Multiply - Returns the dot product: vector1.X*vector2.X + vector1.Y*vector2.Y
        /// 
        /// 
        /// Returns the dot product: vector1.X*vector2.X + vector1.Y*vector2.Y 
        /// 
        ///  The first Vector  
        ///  The second Vector  
        public static float Multiply(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }
        /// 

        /// Determinant - Returns the determinant det(vector1, vector2)
        /// 

        ///  
        /// Returns the determinant: vector1.X*vector2.Y - vector1.Y*vector2.X
        ///  
        ///  The first Vector 
        ///  The second Vector 
        public static float Determinant(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// 
        /// Explicit conversion to Size.  Note that since Size cannot contain negative values, 
        /// the resulting size will contains the absolute values of X and Y
        /// 
        /// 
        /// Size - A Size equal to this Vector 
        /// 
        ///  Vector - the Vector to convert to a Size  
        public static explicit operator SizeF(Vector vector)
        {
            return new SizeF((float)Math.Abs(vector._x), (float)Math.Abs(vector._y));
        }
        /// 
        /// Explicit conversion to PointF 
        /// 
        ///  
        /// PointF - A PointF equal to this Vector 
        /// 
        ///  Vector - the Vector to convert to a PointF  
        public static explicit operator PointF(Vector vector)
        {
            return new PointF((float)(vector._x), (float)(vector._y));
        }
        #endregion Public Operators
    } 
}
