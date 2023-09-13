using System;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.CTS;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGPreserveAspectRatio 的摘要说明。
	/// </summary>
	public class SVGPreserveAspectRatio:DataType.SVGType,Interface.CTS.ISVGPreserveAspectRatio
	{
		#region ..构造及消除
		public SVGPreserveAspectRatio(string meetstr)
		{
			Match match = parCheck.Match(meetstr.Trim());
			if(match.Groups["align"].Success)
			{
				switch(match.Groups["align"].Value)
				{
					case "none":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_NONE;
						break;
					case "xMinYMin":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMINYMIN;
						break;
					case "xMidYMin":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMIN;
						break;
					case "xMaxYMin":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMIN;
						break;
					case "xMinYMid":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMINYMID;
						break;
					case "xMaxYMid":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMID;
						break;
					case "xMidYMid":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMID;
						break;
					case "xMidYMax":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMAX;
						break;
					case "xMaxYMax":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMAX;
						break;
					case "xMinYMax":
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMINYMAX;
						break;
					default:
						align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMID;
						break;
				}
			}
			else
			{
				align = SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMID;
			}
					
			if(match.Groups["meet"].Success)
			{
				switch(match.Groups["meet"].Value)
				{
					case "slice":
						meetOrSlice = SVGMeetOrSliceType.SVG_MEETORSLICE_SLICE;
						break;
					case "meet":
						meetOrSlice = SVGMeetOrSliceType.SVG_MEETORSLICE_MEET;
						break;
					default:
						meetOrSlice = SVGMeetOrSliceType.SVG_MEETORSLICE_UNKNOWN;
						break;
				}
			}
			else
			{
				meetOrSlice = SVGMeetOrSliceType.SVG_MEETORSLICE_MEET;
			}
			match = null;
			meetstr = null;
		}
		#endregion

		#region ..静态变量
		private static Regex parCheck = new Regex("^(?<align>[A-Za-z]+)\\s*(?<meet>[A-Za-z]*)$");
		#endregion

		#region ..私有变量
		SVGMeetOrSliceType meetOrSlice;
		SVGPreserveAspectRatioType align;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置视图对齐属性
		/// </summary>
		public SVGPreserveAspectRatioType Align
		{
			get
			{
				return this.align;
			}
			set
			{
				this.align = value;
			}
		}

		/// <summary>
		/// 获取或设置视图的剪切类别
		/// </summary>
		public SVGMeetOrSliceType MeetOrSlice 
		{
			set
			{
				this.meetOrSlice = value;
			}
			get
			{
				return this.meetOrSlice;
			}
		}
		#endregion

		#region ..视图缩放
		/// <summary>
		/// 视图缩放
		/// </summary>
		/// <param name="viewBox">视图边界</param>
		/// <param name="rectToFit">实际边界</param>
		/// <returns></returns>
		public float[] FitToViewBox(RectangleF viewBox, RectangleF rectToFit)
		{
			float translateX = 0;
			float translateY = 0;
			float scaleX = 1;
			float scaleY = 1;

			if(!viewBox.IsEmpty)
			{
				// calculate scale values for non-uniform scaling
				scaleX = rectToFit.Width / viewBox.Width;
				scaleY = rectToFit.Height / viewBox.Height;

				if(Align != SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_NONE)
				{
					// uniform scaling
					if(MeetOrSlice == SVGMeetOrSliceType.SVG_MEETORSLICE_MEET) scaleX = Math.Min(scaleX, scaleY);
					else scaleX = Math.Max(scaleX, scaleY);

					scaleY = scaleX;

					if(Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMAX || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMID || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMIN)
					{
						// align to the middle X
						translateX = (rectToFit.X + rectToFit.Width - viewBox.X - viewBox.Width * scaleX) / 2;
					}
					else if(Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMAX || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMID || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMIN)
					{
						// align to the right X
						translateX = (rectToFit.X + rectToFit.Width - viewBox.X - viewBox.Width * scaleX);
					}

					if(Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMID || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMID || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMINYMID)
					{
						// align to the middle Y
						translateY = (rectToFit.Bottom - viewBox.Y - viewBox.Height * scaleY) / 2;
					}
					else if(Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMAXYMAX || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMIDYMAX || 
						Align == SVGPreserveAspectRatioType.SVG_PRESERVEASPECTRATIO_XMINYMAX)
					{
						// align to the bottom Y
						translateY = (rectToFit.Bottom - viewBox.Y - viewBox.Height * scaleY);
					}
				}
			}
			return new float[]{translateX, translateY, scaleX, scaleY};
		}
		#endregion
	}
}
