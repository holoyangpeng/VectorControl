using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.CTS;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ�ֶ�ά�任�б�
	/// </summary>
	public class SVGTransformList:DataType.SVGTypeList,Interface.CTS.ISVGTransformList
	{
        static Regex re = new Regex("([A-Za-z]+)\\s*\\(([eE\\-0-9\\.\\,\\s]+)\\)", RegexOptions.Compiled);

		#region ..���켰����
		public SVGTransformList(string transformstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			Match match = re.Match(transformstr);

			SVGTransform transform;
			while(match.Success)
			{
				transform = new SVGTransform(match.Value);
				this.AppendItem(transform);
				match = match.NextMatch();
			}
			transform = null;
			match = null;
		}

		public SVGTransformList(SVGTransform[] transforms)
		{
			this.list.AddRange(transforms);
		}
		#endregion

		#region ..˽�б���
		SVGMatrix finalmatrix = new SVGMatrix();
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���յ�Ť��
		/// </summary>
		public Interface.CTS.ISVGMatrix FinalMatrix
		{
			get
			{
//				if(this.finalmatrix == null)
//				{
					IEnumerator en = list.GetEnumerator();
//					if(this.finalmatrix != null)
//						this.finalmatrix.Dispose();
//					this.finalmatrix = null;
					this.finalmatrix = new SVGMatrix(new System.Drawing.Drawing2D.Matrix());
					Interface.CTS.ISVGTransform transform;
					while(en.MoveNext())
					{
						transform = (Interface.CTS.ISVGTransform) en.Current;
						finalmatrix.Multiply(transform.Matrix);
					}
//				}
				return this.finalmatrix;
			}
		}

		/// <summary>
		/// ��ȡ���͵��ı����ʽ������Transform���ͺ�ֵ������
		/// </summary>
		public string TransformString
		{
			get
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
				foreach(SVGTransform sf in this.list)
					sb.Append(sf.TransformString + " ");
				return sb.ToString();
			}
		}
		#endregion

		#region ..��ȡ��������
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		/// <returns></returns>
		public SVGTransform[] GetTransforms()
		{
			SVGTransform[] transf = new SVGTransform[this.list.Count];
			this.list.CopyTo(transf);
			return transf;
		}
		#endregion

		#region ..����Ƿ�Ϊ��Ч������ֵ
		/// <summary>
		/// ����Ƿ�Ϊ��Ч������ֵ
		/// </summary>
		/// <param name="svgType">��������</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.CTS.ISVGTransform;
		}
		#endregion
 
		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı������ı��������任��Transform��������
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			foreach(SVGTransform sf in this.list)
			{
				sb.Append(sf.ToString()+" ");
			}
			return sb.ToString();
		}
		#endregion
	}
}
