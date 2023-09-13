using System;
using System.Drawing.Drawing2D;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��GDI�еı任�������
	/// </summary>
	public class SVGMatrix:DataType.SVGType,Interface.CTS.ISVGMatrix,System.IDisposable
	{
		#region ..���켰����
		public SVGMatrix()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		public SVGMatrix(System.Drawing.Drawing2D.Matrix matrix)
		{
//			if(matrix != null && matrix.Elements[0] != 0 && matrix.Elements[3] != 0 )
			this.gdiMatrix = matrix;
		}
		#endregion

		#region ..˽�б���
		Matrix gdiMatrix = new Matrix();
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�任�����ˮƽ����Ҫ��
		/// </summary>
		public float A
		{
			get{return (float) gdiMatrix.Elements.GetValue(0);}
		}

		/// <summary>
		/// ��ȡ�任�����е�ˮƽŤ��Ҫ��
		/// </summary>
		public float B
		{
			get{return (float) gdiMatrix.Elements.GetValue(1);}
		}

		/// <summary>
		/// ��ȡ�任�����е�����Ť��Ҫ��
		/// </summary>
		public float C
		{
			get{return (float) gdiMatrix.Elements.GetValue(2);}
		}

		/// <summary>
		/// ��ȡ�����еĴ�ֱ����Ҫ��
		/// </summary>
		public float D
		{
			get{return (float) gdiMatrix.Elements.GetValue(3);}
		}

		/// <summary>
		/// ��ȡ�����ˮƽƫ��
		/// </summary>
		public float E
		{
			get{return (float) gdiMatrix.Elements.GetValue(4);}
		}

		/// <summary>
		/// ��ȡ����Ĵ�ֱƫ��
		/// </summary>
		public float F
		{
			get{return (float) gdiMatrix.Elements.GetValue(5);}
		}
		#endregion

		#region ..��������
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
		/// ����GDI Matrix����
		/// </summary>
		/// <returns></returns>
		public System.Drawing.Drawing2D.Matrix GetGDIMatrix()
		{
			return this.gdiMatrix.Clone();
		}
		#endregion

		#region ..��ȡ������ı����
		/// <summary>
		/// ��ȡ������ı����
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

		#region IDisposable ��Ա
		public void Dispose()
		{
			// TODO:  ��� SVGMatrix.Dispose ʵ��
			gdiMatrix.Dispose();
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
