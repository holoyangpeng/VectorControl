using System;
using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现SVG中的点坐标
	/// </summary>
	public struct SVGPoint:Interface.CTS.ISVGPoint
	{
		#region ..构造及消除
		public SVGPoint(float x,float y,string prepointstr,string nextpointstr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
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

		#region ..私有变量
		PointF point;
		bool isEmpty;
		string defaultValue;
		string prePointStr;
		string nextPointStr;
		#endregion

		#region ..静态变量
		static SVGPoint pint = new SVGPoint();

		/// <summary>
		/// 获取空对象
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

		#region ..公共属性
		/// <summary>
		/// 当点处于点集中时，返回前面的点集字符串
		/// </summary>
		public string PrePointStr
		{
			get
			{
				return this.prePointStr;
			}
		}

		/// <summary>
		/// 当点处于点集中时，返回后面的点集字符串
		/// </summary>
		public string NextPointStr
		{
			get
			{
				return this.nextPointStr;
			}
		}


		/// <summary>
		/// 判断对象是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// 获取对象的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// 获取或设置横坐标
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
		/// 获取或设置纵坐标
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

		#region ..根据指定的SVGMatrix进行变换
		/// <summary>
		/// 根据指定的SVGMatrix进行变换
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
