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
	/// <para>��״ѡ������</para>
	/// <para>ͨ����״ѡ����,�������û��ṩ<see cref="Shape">�Զ������״</see>��Ϣ,�����Խ���ǰѡ�����״���ø�<see cref="VectorControl">VectorControl�ؼ�</see>���Ӷ���������<see cref="Operator.Shape">��״���Ʋ���Shape</see>�������Լ������·��������</para>
	/// <para><seealso href="��չ.�Զ�����״�ļ���ʽ.html">�Զ�����״�ļ��ĸ�ʽ����</seealso></para>
	/// </summary>
	[ToolboxItem(false)]
	public class ShapeSelector:OutlookBar
	{
		#region ..���켰����
		/// <summary>
		/// �����Զ�����״�ļ�������һ����״ѡ����
		/// </summary>
		/// <param name="configpath">�Զ�����״�ļ�·��</param>
		public ShapeSelector(string configpath):base()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            this.LoadConfiguration(configpath);
			this.SelectedPathIndex = 0;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint,true);
		}
		#endregion

		#region ..˽�б���
		SVG.Document.SVGDocument doc = null;
		SVGPathElement selectedShape = null;
		#endregion

		#region ..Selected
		/// <summary>
		/// ��ȡ�����õ�ǰѡ�����״
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

		#region ..����ShapeDocument
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

		#region ..��ȡShape����
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
