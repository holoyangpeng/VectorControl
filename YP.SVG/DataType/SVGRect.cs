using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGRect ��ժҪ˵����
	/// </summary>
	public struct SVGRect:Interface.DataType.ISVGRect
	{
		#region ..���켰����
		public SVGRect(string rectstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.defaultValue = string.Empty;
			this.rect = RectangleF.Empty;
			string replacedStr = Regex.Replace(rectstr, @"(\s|,)+", ",");
			string[] tokens = replacedStr.Split(new char[]{','});
			if(tokens.Length == 4)
			{
				float x = SVGNumber.ParseNumberStr(tokens[0]);
				float y = SVGNumber.ParseNumberStr(tokens[1]);
				float width = SVGNumber.ParseNumberStr(tokens[2]);
				float height = SVGNumber.ParseNumberStr(tokens[3]);
				this.rect = new RectangleF(x,y,width,height);
			}
			tokens = null;
			replacedStr = null;
//			else
//			{
//				throw new SvgException(SvgExceptionType.SVG_INVALID_VALUE_ERR, "Invalid SvgRect value: " + str);
//			}
		}

		/// <summary>
		/// ����RectangleF�ṹ����SVGRect����
		/// </summary>
		/// <param name="oriRect"></param>
		public SVGRect(RectangleF oriRect)
		{
			this.defaultValue = string.Empty;
			this.rect = oriRect;
		}
		#endregion

		#region ..˽�б���
		RectangleF rect;
		string defaultValue;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�X����ֵ
		/// </summary>
		public float X
		{
			get
			{
				return this.rect.X;
			}
			set
			{
				this.rect.X = value;
			}
		}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Y����ֵ
		/// </summary>
		public float Y
		{
			set
			{
				this.rect.Y = value;
			}
			get
			{
				return this.rect.Y;
			}
		}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Width����ֵ
		/// </summary>
		public float Width
		{
			get
			{
				return this.rect.Width;
			}
			set
			{
				this.rect.Width = value;
			}
		}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Height����ֵ
		/// </summary>
		public float Height
		{
			set
			{
				this.rect.Height = value;
			}
			get
			{
				return this.rect.Height;
			}
		}

		/// <summary>
		/// ��ȡ��GDI���νṹ���ı߽�
		/// </summary>
		public RectangleF GDIRect
		{
			get
			{
				return this.rect;
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
		/// �ж϶����Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.rect.IsEmpty;
			}
		}
		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.X.ToString() + " " + this.Y.ToString() + " " + this.Width .ToString() + " " + this.Height.ToString();
		}
		#endregion
	}
}
