using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ��C,c,t,T,Q,q,S,s
	/// </summary>
	public abstract class SVGPathSegCurve:SVGPathSeg,Interface.Paths.ISVGPathSegCubic
	{
		#region ..���켰����
		public SVGPathSegCurve(float x,float y,float x1,float y1,float x2,float y2)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.x = x;
			this.y = y;
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
		}
		#endregion

		#region ..˽�б���
		float x,y,x1,y1,x2,y2;
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

		/// <summary>
		/// �����һ���Ƶ�ĺ�����
		/// </summary>
		public float X1
		{
			set
			{
				this.x1 = value;
			}
			get
			{
				return this.x1;
			}
		}

		/// <summary>
		/// �����һ���Ƶ��������
		/// </summary>
		public float Y1
		{
			set
			{
				this.y1 = value;
			}
			get
			{
				return this.y1;
			}
		}

		/// <summary>
		/// ����ڶ����Ƶ�ĺ�����
		/// </summary>
		public float X2
		{
			set
			{
				this.x2 = value;
			}
			get
			{
				return this.x2;
			}
		}

		/// <summary>
		/// ����ڶ����Ƶ��������
		/// </summary>
		public float Y2
		{
			set
			{
				this.y2 = value;
			}
			get
			{
				return this.y2;
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

		#region ..��ȡ��һ�����Ƶ�
		/// <summary>
		/// ��ȡ��һ�����Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public abstract PointF GetFirstControl(Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..��ȡ�ڶ������Ƶ�
		/// <summary>
		/// ��ȡ�ڶ������Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public abstract PointF GetSecondControl(Interface.Paths.ISVGPathSegList svgPathList);
		#endregion

		#region ..��·����Ԫ���ӵ�ָ��·��ĩβ
		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF endPoint = this.GetLastPoint(svgPathList);
			PointF firstControl = this.GetFirstControl(svgPathList);
			PointF secControl = this.GetSecondControl(svgPathList);
			path.AddBezier(lastPoint,firstControl,secControl,endPoint);
			lastPoint = endPoint;
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
			PointF firstControl = this.GetFirstControl(svgPathSegList);
			PointF endControl = this.GetSecondControl(svgPathSegList);
			return new SVGPathSegCurvetoCubicAbs(endPoint.X,endPoint.Y,firstControl.X,firstControl.Y,endControl.X,endControl.Y);
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
				PointF p2 = this.GetFirstControl(svgPathSegList);
				PointF p3 = this.GetSecondControl(svgPathSegList);
				PointF p4 = this.GetLastPoint(svgPathSegList);
				this.pathlength = (float)Paths.CalculateLength.CalculateBezierLengthAtT(p1,p2,p3,p4);
			}
			return this.pathlength;
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
			path.AddBezier(seg.GetLastPoint(svgPathSegList),this.GetFirstControl(svgPathSegList),this.GetSecondControl(svgPathSegList),this.GetLastPoint(svgPathSegList));
			return path;
		}
		#endregion
	}
}
