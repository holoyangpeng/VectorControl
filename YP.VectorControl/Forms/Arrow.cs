using System;
using System.Reflection;
using System.IO;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>����VectorControl�ؼ�������ʹ�õļ�ͷ��</para>
	/// <para>����ͨ��<see cref="ArrowSelector">ArrowSelector</see>Ϊ�û��ṩ��ͷ��ѡ��</para>
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

        #region ..���켰����
        internal Arrow(System.Xml.XmlElement element)
		{
			if(element.Name != "marker")
				throw new Exception("��Ч�Ľڵ�");

			this.marker = element;
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		System.Xml.XmlElement marker = null;
		string id = null;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡShape��ID
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
		/// ��ȡԭʼSymbolElement
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
