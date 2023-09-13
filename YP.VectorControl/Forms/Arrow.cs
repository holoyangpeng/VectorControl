using System;
using System.Reflection;
using System.IO;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>定义VectorControl控件环境中使用的箭头。</para>
	/// <para>可以通过<see cref="ArrowSelector">ArrowSelector</see>为用户提供箭头的选择</para>
	/// </summary>
	public class Arrow
    {
        #region ..static 
        internal static SVG.Document.SVGDocument ArrowDocument;

        static Arrow()
        {
            ArrowDocument = YP.SVG.Document.SvgDocumentFactory.CreateSimleDocument();
            // Get the assembly that contains the bitmap resource
            Assembly myAssembly = Assembly.GetAssembly(Type.GetType("YP.VectorControl.Forms.Arrow"));

            // Get the resource stream containing the images
            Stream stream = myAssembly.GetManifestResourceStream("YP.VectorControl.Resource.Document.arrow.xml");

            ArrowDocument.Load(stream);

            stream.Close();
        }
        #endregion

        #region ..构造及消除
        internal Arrow(System.Xml.XmlElement element)
		{
			if(element.Name != "marker")
				throw new Exception("无效的节点");

			this.marker = element;
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		System.Xml.XmlElement marker = null;
		string id = null;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取Shape的ID
		/// </summary>
		internal string ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		/// <summary>
		/// 获取原始SymbolElement
		/// </summary>
		internal System.Xml.XmlElement MarkerElement
		{
			get
			{
				return this.marker;
			}
		}
		#endregion

		#region ..ToString
		public override string ToString()
		{
			if(this.marker != null)
				return this.marker.GetAttribute("id");
			return base.ToString();
		}
		#endregion
	}
}
