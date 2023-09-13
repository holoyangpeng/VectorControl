using System;
using System.Drawing.Drawing2D;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现GDI中的变换矩阵对象
	/// </summary>
	public class SVGMatrix:DataType.SVGType,Interface.CTS.ISVGMatrix,System.IDisposable
	{
		#region ..构造及消除
		public SVGMatrix()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		public SVGMatrix(System.Drawing.Drawing2D.Matrix matrix)
		{
//			if(matrix != null && matrix.Elements[0] != 0 && matrix.Elements[3] != 0 )
			this.gdiMatrix = matrix;
		}
		#endregion

		#region ..私有变量
		Matrix gdiMatrix = new Matrix();
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取变换矩阵的水平缩放要素
		/// </summary>
		public float A
		{
			get{return (float) gdiMatrix.Elements.GetValue(0);}
		}

		/// <summary>
		/// 获取变换矩阵中的水平扭曲要素
		/// </summary>
		public float B
		{
			get{return (float) gdiMatrix.Elements.GetValue(1);}
		}

		/// <summary>
		/// 获取变换矩阵中的纵向扭曲要素
		/// </summary>
		public float C
		{
			get{return (float) gdiMatrix.Elements.GetValue(2);}
		}

		/// <summary>
		/// 获取矩阵中的垂直缩放要素
		/// </summary>
		public float D
		{
			get{return (float) gdiMatrix.Elements.GetValue(3);}
		}

		/// <summary>
		/// 获取矩阵的水平偏移
		/// </summary>
		public float E
		{
			get{return (float) gdiMatrix.Elements.GetValue(4);}
		}

		/// <summary>
		/// 获取矩阵的垂直偏移
		/// </summary>
		public float F
		{
			get{return (float) gdiMatrix.Elements.GetValue(5);}
		}
		#endregion

		#region ..公共方法
		public Interface.CTS.ISVGMatrix Multiply(Interface.CTS.ISVGMatrix secondMatrix)
		{
			gdiMatrix.Multiply(((SVGMatrix)secondMatrix).gdiMatrix);
			return this;
		}

		public Interface.CTS.ISVGMatrix Multiply(Interface.CTS.ISVGMatrix secondMatrix,MatrixOrder order)
		{
			gdiMatrix.Multiply(((SVGMatrix)secondMatrix).gdiMatrix,order);
			return this;
		}

		public Interface.CTS.ISVGMatrix Inverse()
		{
			gdiMatrix.Invert();
			return this;
		}

		public Interface.CTS.ISVGMatrix Translate(float x, float y)
		{
			gdiMatrix.Translate(x, y);
			return this;
		}

		public Interface.CTS.ISVGMatrix Scale(float scaleFactor)
		{
			if(scaleFactor != 0)
				gdiMatrix.Scale(scaleFactor, scaleFactor);
			return this;
		}

		public Interface.CTS.ISVGMatrix ScaleNonUniform(float scaleFactorX, float scaleFactorY)
		{
			if(scaleFactorX != 0 &&scaleFactorY!= 0)
				gdiMatrix.Scale(scaleFactorX, scaleFactorY);
			return this;
		}

		public Interface.CTS.ISVGMatrix Rotate(float angle)
		{
			gdiMatrix.Rotate(angle);
			return this;
		}

		public Interface.CTS.ISVGMatrix RotateFromVector(float x, float y)
		{
			throw new NotImplementedException();
		}

		public Interface.CTS.ISVGMatrix FlipX()
		{
			Matrix multMatrix = new Matrix(-1, 0, 0, 1, 0, 0);
			gdiMatrix.Multiply(multMatrix);
			return this;
		}

		public Interface.CTS.ISVGMatrix FlipY()
		{
			Matrix multMatrix = new Matrix(1, 0, 0, -1, 0, 0);
			gdiMatrix.Multiply(multMatrix);
			return this;
		}

		public Interface.CTS.ISVGMatrix SkewX(float angle)
		{
			gdiMatrix.Shear(Convert.ToSingle(Math.Tan(angle * Math.PI / 180)), 0);
			return this;
		}

		public Interface.CTS.ISVGMatrix SkewY(float angle)
		{
			gdiMatrix.Shear(0, Convert.ToSingle(Math.Tan(angle * Math.PI / 180)));
			return this;
		}

		/// <summary>
		/// 返回GDI Matrix对象
		/// </summary>
		/// <returns></returns>
		public System.Drawing.Drawing2D.Matrix GetGDIMatrix()
		{
			return this.gdiMatrix.Clone();
		}
		#endregion

		#region ..获取对象的文本表达
		/// <summary>
		/// 获取对象的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
            if (this.gdiMatrix.IsIdentity)
                return string.Empty;
            return "matrix(" + this.A.ToString() + "," + this.B.ToString() + 
					"," + this.C.ToString() + "," + this.D.ToString() + 
					"," + this.E.ToString() + "," + this.F.ToString()+")";
		}

		#endregion

		#region IDisposable 成员
		public void Dispose()
		{
			// TODO:  添加 SVGMatrix.Dispose 实现
			gdiMatrix.Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
