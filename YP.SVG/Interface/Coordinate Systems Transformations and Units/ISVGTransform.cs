using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// 定义ISVGTransform 的一般行为
	/// </summary>
	public interface ISVGTransform:Interface.DataType.ISVGType
	{
		TransformType TransformType{get;}
		ISVGMatrix Matrix{get;}
		float Angle{get;}

		void SetMatrix(ISVGMatrix matrix);
		void SetTranslate(float tx, float ty);
		void SetScale(float sx, float sy);
		void SetRotate(float angle, float cx, float cy);
		void SetSkewX(float angle);
		void SetSkewY(float angle);
	}
}
