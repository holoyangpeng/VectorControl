using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// 定义ISVGMatrix 的一般行为
	/// </summary>
	public interface ISVGMatrix:Interface.DataType.ISVGType
	{
		float A{get;}
		float B{get;}
		float C{get;}
		float D{get;}
		float E{get;}
		float F{get;}
		
		
		ISVGMatrix Multiply(ISVGMatrix secondMatrix);
		ISVGMatrix Multiply(ISVGMatrix secondMatrix,System.Drawing.Drawing2D.MatrixOrder order);
		ISVGMatrix Inverse();
		ISVGMatrix Translate(float x, float y);
		ISVGMatrix Scale(float scaleFactor);
		ISVGMatrix ScaleNonUniform(float scaleFactorX, float scaleFactorY);
		ISVGMatrix Rotate(float angle);
		ISVGMatrix RotateFromVector(float x, float y);
		ISVGMatrix FlipX();
		ISVGMatrix FlipY();
		ISVGMatrix SkewX(float angle);
		ISVGMatrix SkewY(float angle);

		System.Drawing.Drawing2D.Matrix GetGDIMatrix();
	}
}
