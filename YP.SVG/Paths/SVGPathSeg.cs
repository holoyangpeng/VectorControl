using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现单位路径单元
	/// </summary>
	public abstract class SVGPathSeg:DataType.SVGType,Interface.Paths.ISVGPathSeg
	{
		#region ..构造及消除

		#endregion

		#region ..私有变量
		public short pathSegType = (short)PathSegmentType.PATHSEG_UNKNOWN;
		public string pathSegTypeAsLetter = string.Empty;
		/// <summary>
		/// 判断路径是否为相对坐标
		/// </summary>
		public bool Relative = false;
		public float pathlength = -1;
		public PointF relativeNextControl = PointF.Empty;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取路径单元类别
		/// </summary>
		public short PathSegType
		{
			get
			{
				return (short)this.pathSegType ;
			}
		}

		/// <summary>
		/// 获取路径长度
		/// </summary>
		public abstract float GetPathLength(Interface.Paths.ISVGPathSegList svgPathList);

		/// <summary>
		/// 获取路径命令
		/// </summary>
		public string PathSegTypeAsLetter
		{
			get
			{
				return this.pathSegTypeAsLetter;
			}
		}

		/// <summary>
		/// 获取路径数据的文本表达
		/// </summary>
		public abstract string PathString{get;}
		#endregion

		#region ..获取路径终点
		/// <summary>
		/// 获取路径中终点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public abstract PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..将路径单元附加到指定路径末尾
		/// <summary>
		/// 将路径单元附加到指定路径末尾
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public abstract void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..转换NormalSVGPathSeg
		/// <summary>
		/// 转换为普通的svgPathSeg
		/// </summary>
		/// <param name="svgPathList">路径单元列表</param>
		/// <returns></returns>
		public virtual SVGPathSeg ConvertToNormal(Interface.Paths.ISVGPathSegList svgPathList)
		{
			return this;
		}
		#endregion

		#region ..获取与之相关的前一个控制点
		/// <summary>
		/// 获取与之相关的前一个控制点
		/// </summary>
		/// <param name="svgPathSegList">单元所在的列表</param>
		/// <returns></returns>
		public virtual PointF GetRelativePreControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			if(this is YP.SVG.Paths.SVGPathSegCurve)
				return ((YP.SVG.Paths.SVGPathSegCurve)this).GetSecondControl(svgPathSegList);
			return this.GetLastPoint(svgPathSegList);
		}
		#endregion

		#region ..获取GDI路径
		/// <summary>
		/// 获取GDI路径
		/// </summary>
		/// <returns></returns>
		public abstract GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList);
		#endregion

		#region ..设置与之相关的后一个控制点
		/// <summary>
		/// 设置与之相关的前一个控制点
		/// </summary>
		/// <returns></returns>
		public void SetRelativeNextControl(PointF point)
		{
			this.relativeNextControl = point;
		}
		#endregion

		#region ..获取与之相关的后一个控制点
		/// <summary>
		/// 获取与之相关的后一个控制点
		/// </summary>
		/// <param name="svgPathSegList">单元所在的列表</param>
		/// <returns></returns>
		public virtual PointF GetRelativeNextControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSeg seg = null;
			try
			{
				seg = (YP.SVG.Paths.SVGPathSeg)svgPathSegList.NextSibling(this);
			}
			catch
			{
			}
			if(seg == null || seg is YP.SVG.Paths.SVGPathSegMove)
				return this.relativeNextControl;
			else if(seg is YP.SVG.Paths.SVGPathSegClosePath)
			{
				YP.SVG.Paths.SVGPathSegMove move = (svgPathSegList as SVGPathSegList).GetRelativeStartPathSeg(this);
				if(InPoint(this.GetLastPoint(svgPathSegList),move.GetLastPoint(svgPathSegList)))
					return move.GetRelativeNextControl(svgPathSegList);
				return PointF.Empty;
			}
			else if(seg is YP.SVG.Paths.SVGPathSegCurve)
				return ((YP.SVG.Paths.SVGPathSegCurve)seg).GetFirstControl(svgPathSegList);
			else
				return this.GetLastPoint(svgPathSegList);
		}
		#endregion

		#region ..InPoint
		/// <summary>
		/// 判断两点是否重合
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static bool InPoint(PointF p1 ,PointF p2)
		{
			float dis = (float)Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2));
			return dis < 1;
		}
		#endregion
	}
}
