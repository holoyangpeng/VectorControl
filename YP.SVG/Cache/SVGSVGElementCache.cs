using System;
using System.Drawing;

namespace YP.SVG.Cache
{
	/// <summary>
	/// store the cache data for the "svg" element
	/// </summary>
	public class SVGSVGElementCache
	{
		/// <summary>
		/// store the cache clip
		/// </summary>
		public System.Drawing.RectangleF CacheClipRect = RectangleF.Empty;

		/// <summary>
		/// store the cache fix transform
		/// </summary>
		public System.Drawing.Drawing2D.Matrix CacheFixTransform = new System.Drawing.Drawing2D.Matrix();
	}
}
