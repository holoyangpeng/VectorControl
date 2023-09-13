using System;
using System.Xml;

namespace YP.SVG.Document
{
	/// <summary>
	/// 创建Svg文档
	/// </summary>
	public class SvgDocumentFactory
	{
		public SvgDocumentFactory()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		static System.Random rd = new Random();

		#region ..创建空文档
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

		#region ..从文件创建文档
		public static SVGDocument CreateDocumentFromFile(string filename)
		{
			string temp;
			return CreateDocumentFromFile(filename,out temp);
		}

		/// <summary>
		/// 从指定的路径创建SVG文档
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="info">创建过程中产生的提示信息</param>
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
		/// 创建简单文档
		/// 该文档将不创建诸如节点位置等信息
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

		#region ..创建对象ID
		static System.Collections.Hashtable IDFactory = new System.Collections.Hashtable();
		/// <summary>
		/// 创建对象ID
		/// </summary>
		/// <param name="doc">文档</param>
		/// <param name="key">关键词</param>
		/// <param name="createdelement">需要创建对象的ID</param>
		/// <returns>不重复的对象ID</returns>
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
		/// 创建对象ID
		/// </summary>
		/// <param name="refelement">参考对象</param>
		/// <param name="key">关键词</param>
		/// <param name="createdelement">需要创建ID的对象</param>
		/// <returns>不重复的对象ID</returns>
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
