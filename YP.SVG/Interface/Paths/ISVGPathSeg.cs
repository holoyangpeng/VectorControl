using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ����һ��·�����ɵ�Ԫ��ͨ����Ϊ
	/// </summary>
	public interface ISVGPathSeg:Interface.DataType.ISVGType
	{
		/// <summary>
		/// ��ȡ·����Ԫ���
		/// </summary>
		short PathSegType{get;}

		/// <summary>
		/// ��ȡ·������
		/// </summary>
		string PathSegTypeAsLetter{get;}

		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		string PathString{get;}

		/// <summary>
		/// ��ȡ·���յ�
		/// </summary>
		System.Drawing.PointF GetLastPoint(ISVGPathSegList svgPathSegList);

		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList);
	}
}
