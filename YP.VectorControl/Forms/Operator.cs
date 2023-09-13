using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// Operator 的摘要说明。
	/// </summary>
	internal class OperatorHelper
	{
		public OperatorHelper()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		/// <summary>
		/// 判断是否是形状绘制操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsShapeOperator(Operator op)
		{
			switch(op)
			{
				case Operator.Line:
				case Operator.Ellipse:
				case Operator.Rectangle:
				case Operator.Star:
				case Operator.Image:
				case Operator.Shape:
				case Operator.Pie:
				case Operator.Arc:
//				case Operator.Pencil:
					return true;
			}
			return false;
		}

		/// <summary>
		/// 判断是否是变换操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsTransformOperator(Operator op)
		{
			switch(op)
			{
				case Operator.Transform:
//				case Operator.Scale:
//				case Operator.Rotate:
//				case Operator.Skew:
					return true;
			}
			return false;
		}

		/// <summary>
		/// 判断是否是选择操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsSelectOperation(Operator op)
		{
//			if(op == Operator.Select)
//				return true;
			return false;
		}

		/// <summary>
		/// 判断是否为点集操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsPointsOperation(Operator op)
		{
			if(op == Operator.Polygon || op == Operator.Polyline)
				return true;
			return false;
		}

		/// <summary>
		/// 判断是否是颜色操作工具
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
//		internal static bool IsColorOperatior(Operator op)
//		{
//			return  op == Operator.InkBottle || op == Operator.PaintBottle;// || op == Operator.ColorPicker;//(op == Operator.GradientTransform ||
//		}

		/// <summary>
		/// 判断是否是视图操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsViewOperator(Operator op)
		{
			return op == Operator.ZoomIn || op == Operator.ZoomOut || op == Operator.Roam;
		}

		/// <summary>
		/// 判断是否是扇形操作
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsPieOperator(Operator op)
		{
			return op == Operator.Pie || op == Operator.Arc;
		}
	}
}
