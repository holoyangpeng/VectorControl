using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// Operator ��ժҪ˵����
	/// </summary>
	internal class OperatorHelper
	{
		public OperatorHelper()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		/// <summary>
		/// �ж��Ƿ�����״���Ʋ���
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
		/// �ж��Ƿ��Ǳ任����
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
		/// �ж��Ƿ���ѡ�����
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
		/// �ж��Ƿ�Ϊ�㼯����
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
		/// �ж��Ƿ�����ɫ��������
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
//		internal static bool IsColorOperatior(Operator op)
//		{
//			return  op == Operator.InkBottle || op == Operator.PaintBottle;// || op == Operator.ColorPicker;//(op == Operator.GradientTransform ||
//		}

		/// <summary>
		/// �ж��Ƿ�����ͼ����
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsViewOperator(Operator op)
		{
			return op == Operator.ZoomIn || op == Operator.ZoomOut || op == Operator.Roam;
		}

		/// <summary>
		/// �ж��Ƿ������β���
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		internal static bool IsPieOperator(Operator op)
		{
			return op == Operator.Pie || op == Operator.Arc;
		}
	}
}
