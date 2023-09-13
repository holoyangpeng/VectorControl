using System;
using System.Drawing.Drawing2D;
using System.Drawing;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ��M �� m
	/// </summary>
	public abstract class SVGPathSegMove:SVGPathSeg
	{
		#region ..���켰����
		public SVGPathSegMove(float x,float y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.x = x;
			this.y = y;
		}
		#endregion

		#region ..˽�б���
		float x,y;
		public PointF relativePreControl = PointF.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// �����յ�ĺ�����
		/// </summary>
		public float X
		{
			set
			{
				this.x = value;
			}
			get
			{
				return this.x;
			}
		}

		/// <summary>
		/// �����յ��������
		/// </summary>
		public float Y
		{
			set
			{
				this.y = value;
			}
			get
			{
				return this.y;
			}
		}
		#endregion

		#region ..��ȡ·���յ�
		/// <summary>
		/// ��ȡ·�����յ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			if(!this.Relative)
				return new PointF(this.x,this.y);
			else
			{
				ISVGPathSeg pre = svgPathList.PreviousSibling(this);
				PointF p = PointF.Empty;
				if(pre != null)
				{
					p = pre.GetLastPoint(svgPathList);
				}
				return new PointF(p.X + this.x,p.Y + this.y);
			}
		}
		#endregion

		#region ..��·����Ԫ���ӵ�ָ��·��ĩβ
		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			path.StartFigure();
			lastPoint = this.GetLastPoint(svgPathList);
		}
		#endregion

		#region ..ת��NormalSVGPathSeg
		/// <summary>
		/// ת��Ϊ��ͨ��svgPathSeg
		/// </summary>
		/// <param name="svgPathList">·����Ԫ�б�</param>
		/// <returns></returns>
		public override SVGPathSeg ConvertToNormal(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			PointF endPoint = this.GetLastPoint(svgPathSegList);
			return new SVGPathSegMovetoAbs(endPoint.X,endPoint.Y );
		}
		#endregion

		#region ..��ȡ·������
		/// <summary>
		/// ��ȡ·������
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ�·���б�</param>
		/// <returns></returns>
		public override float GetPathLength(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			return 0;
		}
		#endregion

		#region ..��ȡ��֮��ص�ǰһ�����Ƶ�
		/// <summary>
		/// ��ȡ��֮��ص�ǰһ�����Ƶ�
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ��б�</param>
		/// <returns></returns>
		public override PointF GetRelativePreControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSegClosePath close = (svgPathSegList as SVGPathSegList).GetRelativeClosePathSeg(this);
			if(close != null)
			{
				YP.SVG.Paths.SVGPathSegCurve curve = svgPathSegList.PreviousSibling(close) as YP.SVG.Paths.SVGPathSegCurve;
				if(curve != null && InPoint(this.GetLastPoint(svgPathSegList),curve.GetLastPoint(svgPathSegList)))
					return curve.GetSecondControl(svgPathSegList);
				return PointF.Empty;
			}
			return this.relativePreControl;
		}
		#endregion

		#region ..������֮��ص�ǰһ�����Ƶ�
		/// <summary>
		/// ������֮��ص�ǰһ�����Ƶ�
		/// </summary>
		/// <returns></returns>
		public void SetRelativePreControl(PointF point)
		{
			this.relativePreControl = point;
		}
		#endregion

		#region ..��ȡGDI·��
		/// <summary>
		/// ��ȡGDI·��
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList)
		{
			return new GraphicsPath();
		}
		#endregion
	}
}
