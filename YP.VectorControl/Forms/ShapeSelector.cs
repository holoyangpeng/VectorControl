using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using YP.SVG.Paths;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>形状选择器。</para>
	/// <para>通过形状选择器,可以向用户提供<see cref="Shape">自定义的形状</see>信息,并可以将当前选择的形状设置给<see cref="VectorControl">VectorControl控件</see>，从而可以利用<see cref="Operator.Shape">形状绘制操作Shape</see>绘制您自己定义的路径轮廓。</para>
	/// <para><seealso href="扩展.自定义形状文件格式.html">自定义形状文件的格式定义</seealso></para>
	/// </summary>
	[ToolboxItem(false)]
	public class ShapeSelector:OutlookBar
	{
		#region ..构造及消除
		/// <summary>
		/// 传入自定义形状文件，创建一个形状选择器
		/// </summary>
		/// <param name="configpath">自定义形状文件路径</param>
		public ShapeSelector(string configpath):base()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            this.LoadConfiguration(configpath);
			this.SelectedPathIndex = 0;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint,true);
		}
		#endregion

		#region ..私有变量
		SVG.Document.SVGDocument doc = null;
		SVGPathElement selectedShape = null;
		#endregion

		#region ..Selected
		/// <summary>
		/// 获取或设置当前选择的形状
		/// </summary>
		/// <example>
		/// <code>
		/// this.vectorControl.TemplateShape = this.shapeselector.SelectedShape;
		/// </code>
		/// </example>
        public SVGPathElement SelectedShape
		{
			set
			{
				this.SelectedPathIndex = this.IndexOfShape(this.selectedShape);
			}
			get
			{
                return this.SelectedObject as SVGPathElement;
				
			}
		}
		#endregion

		#region ..分析ShapeDocument
		void ParseShapes(SVG.Document.SVGDocument doc)
		{
            if (doc == null)
                return;
            System.Xml.XmlNodeList list = doc.GetElementsByTagName("group");
            for (int i = 0; i < list.Count; i++)
            {
                System.Xml.XmlElement element = list[i] as System.Xml.XmlElement;
                string enabled = element.GetAttribute("enabled").Trim().ToLower();
                if (element == null ||string.Compare(enabled,"false") ==0)
                    continue;
                ShapeGroup group = new ShapeGroup();
                group.ID = element.GetAttribute("id");
                this.items.Add(group);
                System.Xml.XmlNodeList list1 = element.ChildNodes;//GetElementsByTagName("symbol")
                for (int j = 0; j < list1.Count; j++)
                {
                    SVGPathElement shape = list1[j] as SVGPathElement;
                    if (shape != null)
                        group.Add(shape);
                }

            }
		}
		#endregion

		#region ..获取Shape索引
        int IndexOfShape(SVGPathElement shape)
		{
			int index = this.SelectedIndex;
			if(index >= 0)
			{
				ShapeGroup group = this.items[index] as ShapeGroup;
				if(group != null)
					return group.IndexOf(shape);
			}
			return -1;
		}
		#endregion

        #region ..Load
        public override void LoadConfiguration(string filePath)
        {
            this.Controls.Clear();
            items.Clear();
            try
            {
                doc = SVG.Document.SvgDocumentFactory.CreateSimpleDocumentFromFile(filePath);
                this.ParseShapes(doc);
            }
            catch
            {

            }
            this.AddIconArea();
        }
        #endregion
	}
}
