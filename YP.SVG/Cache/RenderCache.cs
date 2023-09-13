using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Cache
{
	/// <summary>
	/// define the class to store the cache info for rendering
	/// </summary>
	public class RenderCache
	{
		#region ..Constructor
		public RenderCache()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..public fields
		/// <summary>
		/// store the cache pen to stroke the path
		/// </summary>
		public Pen CachePen = null;
		/// <summary>
		/// store the cache brush to fill the path
		/// </summary>
		public Brush CacheBrush = null;
		/// <summary>
		/// store the cache path
		/// </summary>
		public GraphicsPath CachePath = null;

		/// <summary>
		/// stroe the cache shadow pen to stroke the shadow path
		/// </summary>
		public Pen CacheShadowPen = null;
		/// <summary>
		/// store the cache shadow brush to fill the shadow path
		/// </summary>
		public Brush CacheShadowBrush = null;

		/// <summary>
		/// store the cache matrix
		/// </summary>
		public System.Drawing.Drawing2D.Matrix CacheTransform = null;

        /// <summary>
        /// store the font
        /// </summary>
        public Font CacheFont = null;

        /// <summary>
        /// 存储渐变画笔的ColorBlend
        /// </summary>
        public ColorBlend GradientColorBlend = null;
		#endregion
	}
}
