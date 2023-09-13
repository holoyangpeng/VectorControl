using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ�ֵ�λ·����Ԫ
	/// </summary>
	public abstract class SVGPathSeg:DataType.SVGType,Interface.Paths.ISVGPathSeg
	{
		#region ..���켰����

		#endregion

		#region ..˽�б���
		public short pathSegType = (short)PathSegmentType.PATHSEG_UNKNOWN;
		public string pathSegTypeAsLetter = string.Empty;
		/// <summary>
		/// �ж�·���Ƿ�Ϊ�������
		/// </summary>
		public bool Relative = false;
		public float pathlength = -1;
		public PointF relativeNextControl = PointF.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ·����Ԫ���
		/// </summary>
		public short PathSegType
		{
			get
			{
				return (short)this.pathSegType ;
			}
		}

		/// <summary>
		/// ��ȡ·������
		/// </summary>
		public abstract float GetPathLength(Interface.Paths.ISVGPathSegList svgPathList);

		/// <summary>
		/// ��ȡ·������
		/// </summary>
		public string PathSegTypeAsLetter
		{
			get
			{
				return this.pathSegTypeAsLetter;
			}
		}

		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		public abstract string PathString{get;}
		#endregion

		#region ..��ȡ·���յ�
		/// <summary>
		/// ��ȡ·�����յ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public abstract PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..��·����Ԫ���ӵ�ָ��·��ĩβ
		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public abstract void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..ת��NormalSVGPathSeg
		/// <summary>
		/// ת��Ϊ��ͨ��svgPathSeg
		/// </summary>
		/// <param name="svgPathList">·����Ԫ�б�</param>
		/// <returns></returns>
		public virtual SVGPathSeg ConvertToNormal(Interface.Paths.ISVGPathSegList svgPathList)
		{
			return this;
		}
		#endregion

		#region ..��ȡ��֮��ص�ǰһ�����Ƶ�
		/// <summary>
		/// ��ȡ��֮��ص�ǰһ�����Ƶ�
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ��б�</param>
		/// <returns></returns>
		public virtual PointF GetRelativePreControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			if(this is YP.SVG.Paths.SVGPathSegCurve)
				return ((YP.SVG.Paths.SVGPathSegCurve)this).GetSecondControl(svgPathSegList);
			return this.GetLastPoint(svgPathSegList);
		}
		#endregion

		#region ..��ȡGDI·��
		/// <summary>
		/// ��ȡGDI·��
		/// </summary>
		/// <returns></returns>
		public abstract GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList);
		#endregion

		#region ..������֮��صĺ�һ�����Ƶ�
		/// <summary>
		/// ������֮��ص�ǰһ�����Ƶ�
		/// </summary>
		/// <returns></returns>
		public void SetRelativeNextControl(PointF point)
		{
			this.relativeNextControl = point;
		}
		#endregion

		#region ..��ȡ��֮��صĺ�һ�����Ƶ�
		/// <summary>
		/// ��ȡ��֮��صĺ�һ�����Ƶ�
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ��б�</param>
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
		/// �ж������Ƿ��غ�
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
