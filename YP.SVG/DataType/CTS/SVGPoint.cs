using System;
using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�еĵ�����
	/// </summary>
	public struct SVGPoint:Interface.CTS.ISVGPoint
	{
		#region ..���켰����
		public SVGPoint(float x,float y,string prepointstr,string nextpointstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.point = new PointF(x,y);
			this.prePointStr = prepointstr;
			this.nextPointStr = nextpointstr;
			this.isEmpty = false;
			this.defaultValue = string.Empty;
		}

		public SVGPoint(float x,float y)
		{
			this.point = new PointF(x,y);
			this.prePointStr = string.Empty;
			this.nextPointStr = string.Empty;
			this.isEmpty = false;
			this.defaultValue = string.Empty;
		}
		#endregion

		#region ..˽�б���
		PointF point;
		bool isEmpty;
		string defaultValue;
		string prePointStr;
		string nextPointStr;
		#endregion

		#region ..��̬����
		static SVGPoint pint = new SVGPoint();

		/// <summary>
		/// ��ȡ�ն���
		/// </summary>
		public static SVGPoint Empty
		{
			get
			{
				pint.isEmpty = true;
				return pint;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ���㴦�ڵ㼯��ʱ������ǰ��ĵ㼯�ַ���
		/// </summary>
		public string PrePointStr
		{
			get
			{
				return this.prePointStr;
			}
		}

		/// <summary>
		/// ���㴦�ڵ㼯��ʱ�����غ���ĵ㼯�ַ���
		/// </summary>
		public string NextPointStr
		{
			get
			{
				return this.nextPointStr;
			}
		}


		/// <summary>
		/// �ж϶����Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// ��ȡ�����Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// ��ȡ�����ú�����
		/// </summary>
		public float X
		{
			set
			{
				this.point = new PointF(value,this.point.Y);
			}
			get
			{
				return this.point.X;
			}
		}

		/// <summary>
		/// ��ȡ������������
		/// </summary>
		public float Y
		{
			set
			{
				this.point = new PointF(this.point.X,value);
			}
			get
			{
				return this.point.Y;
			}
		}
		#endregion

		#region ..����ָ����SVGMatrix���б任
		/// <summary>
		/// ����ָ����SVGMatrix���б任
		/// </summary>
		public Interface.CTS.ISVGPoint MatrixTransform(Interface.CTS.ISVGMatrix matrix)
		{
			PointF[] ps = new PointF[]{this.point};
			System.Drawing.Drawing2D.Matrix m = matrix.GetGDIMatrix();
			if(m != null)
				m.TransformPoints(ps);
			return new SVGPoint(ps[0].X,ps[0].Y);
	}
		#endregion
	}
}
