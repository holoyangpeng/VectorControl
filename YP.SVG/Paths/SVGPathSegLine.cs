using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ��L,l,H,h,V,v
	/// </summary>
	public abstract class SVGPathSegLine:SVGPathSeg
	{
		#region ..���켰����
		public SVGPathSegLine(float x,float y)
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
		/// �����յ�����������
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

		#region ..��·����Ԫ���ӵ�ָ��·��ĩβ
		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path">·��</param>
		/// <param name="lastPoint">·����ĩ��</param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF p = this.GetLastPoint(svgPathList);
			path.AddLine(lastPoint,p);
			lastPoint = p;
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
			return new SVGPathSegLinetoAbs(endPoint.X,endPoint.Y );
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
			if(this.pathlength == -1)
			{
				Interface.Paths.ISVGPathSeg pre = svgPathSegList.PreviousSibling(this);
				if(pre == null)
					this.pathlength = 0;
				PointF p1 = pre.GetLastPoint(svgPathSegList);
				PointF p2 = this.GetLastPoint(svgPathSegList);
				this.pathlength = (float)Paths.CalculateLength.CalculateLineLengthAtT(1,p1,p2);
			}
			return this.pathlength;
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
			return PointF.Empty;
		}
		#endregion

		#region ..��ȡGDI·��
		/// <summary>
		/// ��ȡGDI·��
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSeg seg = (YP.SVG.Paths.SVGPathSeg)svgPathSegList.PreviousSibling(this);
			if(seg == null)
				return null;
			GraphicsPath path = new GraphicsPath();
			path.AddLine(seg.GetLastPoint(svgPathSegList),this.GetLastPoint(svgPathSegList));
			return path;
		}
		#endregion
	}
}
