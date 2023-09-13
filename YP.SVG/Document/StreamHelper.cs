using System;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Runtime.Serialization;

namespace YP.SVG.Document
{
	/// <summary>
	/// provide the help function to parse the stream
	/// </summary>
    public class StreamHelper
	{
		#region ..static fields
		
		#endregion

		#region ..GetStream
		/// <summary>
		/// Get the stream from the uri
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static System.IO.Stream GetStream(Uri uri)
		{
			string absuri = GetAbsolutPath(uri);
			
			
			try
			{
                MemoryStream ms;
				WebRequest req = WebRequest.Create(uri);
				WebResponse resp = req.GetResponse();
				Stream receiveStream = resp.GetResponseStream();
				ms = new MemoryStream();

				byte[] buffer = new byte[100];
				int readByte = receiveStream.Read(buffer, 0, 100);
				while(readByte > 0)
				{
					ms.Write(buffer, 0, readByte);
					readByte = receiveStream.Read(buffer, 0, 100);
				}
				ms.Position = 0;
				return ms;
			}
			catch
			{
				return null;
			}
		}
		#endregion

		#region ..GetAbsolutPath
		/// <summary>
		/// get full path 
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static string GetAbsolutPath(Uri uri)
		{
			return uri.GetLeftPart(System.UriPartial.Path) + uri.Query ;
		}
		#endregion
	}
}
