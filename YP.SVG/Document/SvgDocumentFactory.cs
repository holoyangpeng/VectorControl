using System;
using System.Xml;

namespace YP.SVG.Document
{
	/// <summary>
	/// ����Svg�ĵ�
	/// </summary>
	public class SvgDocumentFactory
	{
		public SvgDocumentFactory()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		static System.Random rd = new Random();

		#region ..�������ĵ�
		public static SVGDocument CreateDocument(string svgfragment)
		{
			SVGDocument doc = new SVGDocument();
			doc.PreserveWhitespace = true;
			doc.LoadXml(svgfragment);
			return doc;
		}

		public static SVGDocument CreateSimleDocument()
		{
			SVGDocument doc = new SVGDocument();
			doc.CreateDetail = false;
			return doc;
		}
		#endregion

		#region ..���ļ������ĵ�
		public static SVGDocument CreateDocumentFromFile(string filename)
		{
			string temp;
			return CreateDocumentFromFile(filename,out temp);
		}

		/// <summary>
		/// ��ָ����·������SVG�ĵ�
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="info">���������в�������ʾ��Ϣ</param>
		/// <returns></returns>
		public static SVGDocument CreateDocumentFromFile(string filename,out string info)
		{
			info = string.Empty;
//			if(!System.IO.File.Exists(filename))
//				return null;
			SVGDocument doc = new SVGDocument();
            try
            {
                doc.PreserveWhitespace = true;
                doc.XmlResolver = null;
                doc.Load(filename);
            }
            catch (Exception e)
            {
                info = e.Message;
                throw e;
            }
            finally
            {
                
            }
			return doc;
		}

		/// <summary>
		/// �������ĵ�
		/// ���ĵ�������������ڵ�λ�õ���Ϣ
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static SVGDocument CreateSimpleDocumentFromFile(string filename)
		{
//			if(!System.IO.File.Exists(filename))
//				return null;
			SVGDocument doc = new SVGDocument();
			doc.CreateDetail = false;
            System.Xml.XmlReader reader = null;
            try
            {
                System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
                settings.ProhibitDtd = true;
                reader = System.Xml.XmlReader.Create(filename, settings);
                doc.XmlResolver = null;
                doc.Load(filename);
                return doc;
            }
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
		}
		#endregion

		#region ..��������ID
		static System.Collections.Hashtable IDFactory = new System.Collections.Hashtable();
		/// <summary>
		/// ��������ID
		/// </summary>
		/// <param name="doc">�ĵ�</param>
		/// <param name="key">�ؼ���</param>
		/// <param name="createdelement">��Ҫ���������ID</param>
		/// <returns>���ظ��Ķ���ID</returns>
		public static string CreateString(YP.SVG.Document.SVGDocument doc,string key,YP.SVG.SVGElement createdelement)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[A-Za-z]*");
			System.Text.RegularExpressions.Match m = regex.Match(key);
			if(m.Success)
			{
				key = m.Groups[0].Value;
			}
			regex = null;
			m = null;
			string name = createdelement.Name;
			int i = 1;
			if(IDFactory.Contains(name))
				i = (int)IDFactory[name] + 1;
			while(true)
			{
				string refid = key + i.ToString();
				System.Xml.XmlNode node = doc.GetReferencedNode(refid,new string[]{name});
				if(node == null || node == createdelement)
				{
					IDFactory[name] = i;
					return refid;
				}
				i ++;
			}
			
		}

		/// <summary>
		/// ��������ID
		/// </summary>
		/// <param name="refelement">�ο�����</param>
		/// <param name="key">�ؼ���</param>
		/// <param name="createdelement">��Ҫ����ID�Ķ���</param>
		/// <returns>���ظ��Ķ���ID</returns>
		public static string CreateString(YP.SVG.SVGElement refelement,string key,YP.SVG.SVGElement createdelement)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[A-Za-z]*");
			System.Text.RegularExpressions.Match m = regex.Match(key);
			if(m.Success)
			{
				key = m.Groups[0].Value;
			}
			regex = null;
			m= null;
			string name = createdelement.Name;
			int i = 1;
			if(IDFactory.Contains(name))
				i = (int)IDFactory[name] + 1;
			while(true)
			{
				string refid = key + i.ToString();
				System.Xml.XmlNode node = refelement.OwnerDocument.GetReferencedNode(refid,refelement);
				System.Xml.XmlNode node1 = refelement.OwnerDocument.GetReferencedNode(refid,new string[]{name});
				if((node == null || node == createdelement) && (node1 == null||node1 == createdelement))
				{
					IDFactory[name] = i;
					return refid;
				}
				i ++;
			}
		}
		#endregion
	}
}
