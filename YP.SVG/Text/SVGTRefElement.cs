using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Text
{
	/// <summary>
	/// ����tref�ڵ�
	/// </summary>
	public class SVGTRefElement:YP.SVG.Text.SVGTextPositionElement,Interface.Text.ISVGTRefElement, Interface.ISVGPathable
	{
		#region ..���켰����
		public SVGTRefElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            render = new Render.SVGTextRenderer(this);
		}
		#endregion

		#region ..˽�б���
        System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
		YP.SVG.SVGElement refElement = null;
		DataType.SVGString href = DataType.SVGString.Empty;
		#endregion

        #region ..ISVGPathable
        Render.SVGTextRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
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
        #endregion

		#region ..��������
		/// <summary>
		/// ��ȡhref����
		/// </summary>
		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}

		public YP.SVG.SVGElement RefElement
		{
			get
			{
				if(this.refElement == null && this.href.IsEmpty)
				{
					System.Xml.XmlElement element = this.OwnerDocument.GetReferencedNode(this.href.Value,new string[]{"text","tspan"});
					if(element is YP.SVG.SVGElement)
						this.refElement = (YP.SVG.SVGElement)element;
				}
				return this.refElement;
			}
		}
		#endregion

		#region ..���Բ���
		/// <summary>
		/// �����Է����޸�ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
			switch(attributeName)
			{
				case "href":
				case "xlink:href":
					this.href = new DataType.SVGString(attributeValue);//,string.Empty,this);
					break;
			}
		}
		#endregion
	}
}
