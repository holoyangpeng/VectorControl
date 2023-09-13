using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// RGBColor
	/// </summary>
	public class RGBColor:DataType.SVGType,Interface.DataType.IRgbColor
	{
		#region ..���켰����
		public RGBColor(string rgbcolorstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			if(rgbcolorstr != null)
			{
				string str = rgbcolorstr.Trim();

                if (string.Compare(str,"none")==0)
                    this.color = Color.Transparent;
                else if(str.StartsWith("rgb("))
				{
					Regex re = new Regex(@"[\d\.\%]+");
					NumberFormatInfo format = new NumberFormatInfo();
					format.NumberDecimalSeparator = ".";

					try
					{
						Match match = re.Match(str);
						int red;
						int green;
						int blue;
						string m;
					
						// get the red value
						m = match.Value;
						if(m.EndsWith("%"))
						{
							m = m.Remove(m.Length-1, 1);
							red = Convert.ToInt32(Single.Parse(m, format) * 2.55);
						}
						else red = Convert.ToInt32(Single.Parse(m, format));

						// get the green value
						match = match.NextMatch();
						m = match.Value;
						if(m.EndsWith("%"))
						{
							m = m.Remove(m.Length-1, 1);
							green = Convert.ToInt32(Single.Parse(m, format) * 2.55);
						}
						else green = Convert.ToInt32(Single.Parse(m, format));

						// get the blue value
						match = match.NextMatch();
						m = match.Value;
						if(m.EndsWith("%"))
						{
							m = m.Remove(m.Length-1, 1);
							blue = Convert.ToInt32(Single.Parse(m, format) * 2.55);
						}
						else blue = Convert.ToInt32(Single.Parse(m, format));
						this.color = Color.FromArgb(red, green, blue);
						match = null;
						m = null;
					}
					catch(Exception exc)
					{
						throw new Exception("rgb() color in the wrong format :: " + str + " :: " + exc.Message);
					}
					finally
					{
						re = null;
						format = null;
						str = null;
					}
				}
				else
				{
					try
					{
						this.color = ColorTranslator.FromHtml(str);
					}
					catch
					{
						this.color = Color.Pink;
					}
					finally
					{
						str = null;
					}
				}
			}
			this.isempty = false;
		}

		public RGBColor(Color color)
		{
			this.color = color;
			this.isempty = false;
		}
		#endregion

		#region ..��̬����
		static RGBColor empty;
		static RGBColor()
		{
			empty = new RGBColor(Color.Empty);
			empty.isempty = true;
		}

		public static RGBColor Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..��GDI��ʾ��ɫ����
		/// <summary>
		/// ��GDI��ʾ��ɫ����
		/// </summary>
		public Color GDIColor
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}
		#endregion

		#region ..˽�б���
		Color color = Color.Empty;
		bool isempty;
		#endregion

		#region ..��������
		public bool IsEmpty
		{
			get
			{
				return this.isempty;
			}
		}

		/// <summary>
		/// ��ȡ��ɫ��Red��ɫ����ֵ
		/// </summary>
		public short R
		{
			get
			{
				return this.color.R;
			}
		}

		/// <summary>
		/// ��ȡ��ɫ��Green��ɫ����ֵ
		/// </summary>
		public short G
		{
			get
			{
				return this.color.G;
			}
		}

		/// <summary>
		/// ��ȡ��ɫ��Blue��ɫ����ֵ
		/// </summary>
		public short B
		{
			get
			{
				return this.color.B;
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
			return ColorHelper.GetColorStringInHex(this.GDIColor);
		}
		#endregion
	}
}
