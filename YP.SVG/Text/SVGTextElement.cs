using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Text
{
	/// <summary>
	/// 实现text节点
	/// </summary>
	public class SVGTextElement:Text.SVGTextPositionElement,
        Interface.Text.ISVGTextElement,
        Interface.ISVGContainer,
        Interface.ISVGPathable
	{
        static System.Windows.Forms.Label lb = new System.Windows.Forms.Label();

		#region ..构造及消除
		public SVGTextElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            render = new Render.SVGTextRenderer(this);
		}
		#endregion

		#region ..私有变量
		//YP.SVGDom.SVGElementCollection childRenders = new YP.SVGDom.SVGElementCollection();
		#endregion

		#region ..公共属性
		/// <summary>
		/// sets or gets a value indicates whether the current text element is in editing
		/// </summary>
		public bool InEdit = false;

        ///// <summary>
        ///// 获取子级绘制节点
        ///// </summary>
        //public YP.SVGDom.SVGElementCollection ChildElements
        //{
        //    get
        //    {
        //        return this.childRenders;
        //    }
        //}
		#endregion

        #region ..ISVGPathable
        Render.SVGTextRenderer render;

        public override Render.SVGBaseRenderer  SVGRenderer
        {
            get 
            { 
                return this.render; 
            }
        }

        public Render.SVGTextRenderer TextRender
        {
            get
            {
                return this.render;
            }
        }

        /// <summary>
        /// gets the path of the text element
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath SVG.Interface.ISVGPathable.GPath
        {
            get
            {
                if (this.graphicsPath == null)
                {
                    this.graphicsPath = new GraphicsPath();
                    using (Graphics g = lb.CreateGraphics())
                        this.render.RefreshPath(g, this.graphicsPath);
                }
                return this.graphicsPath;
            }
        }

        //Interface.ISVGRenderer Interface.ISVGPathable.Render
        //{
        //    get
        //    {
        //        return this.render;
        //    }
        //}
        #endregion

		#region ..ISVGContainer
		/// <summary>
		/// 判断节点是否是有效的子级节点
		/// </summary>
		/// <param name="child">子级节点</param>
		/// <returns></returns>
		public bool ValidChild(Interface.ISVGElement child)
		{
			return child is Interface.Text.ISVGTSpanElement || child is Interface.Text.ISVGTRefElement ;
		}
		#endregion

		#region ..打散文本对象，将其中的每个字符转换为单个的TextElement
		/// <summary>
		/// 打散文本对象，将其中的每个字符转换为单个的TextElement
		/// </summary>
		/// <returns>打散后的对象列表</returns>
        //public YP.SVGDom.SVGElementCollection Break()
        //{
        //    if(this.TextContentInfos.Count == 0 ||(this.TextContentInfos.Count == 1 && ((TextContentInfo)this.TextContentInfos[0]).TextContent.Length <= 1))
        //        return null;
        //    YP.SVGDom.SVGElementCollection list = new SVGElementCollection();
        //    foreach(TextContentInfo info in this.textContentInfos)
        //    {
        //        YP.SVGDom.SVGElementCollection temp = info.Break();
        //        list.AddRange(temp);
        //    }
        //    return list;
        //}
		#endregion
    }
}
