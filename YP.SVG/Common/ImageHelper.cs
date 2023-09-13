using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace YP.SVG.Common
{
	/// <summary>
	/// ImageHelper 的摘要说明。
	/// </summary>
	public class ImageHelper
	{
		#region ..static fields
		public static readonly System.Text.RegularExpressions.Regex base64SourceParser = new System.Text.RegularExpressions.Regex(@"^data:image\/[a-zA-Z0-9]+;base64,",System.Text.RegularExpressions.RegexOptions.IgnoreCase);
		#endregion

		#region ..ParseImageSource
		/// <summary>
		/// parse bitmap according to the input href string 
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="imageHref"></param>
		/// <returns></returns>
		public static Bitmap ParseImageSource(SVG.Document.SVGDocument doc, string href)
		{
			try
			{
				string baseuri = doc.BaseURI;
				
				if(base64SourceParser.IsMatch(href))
				{
					string temp = href.Substring(base64SourceParser.Match(href).Length);
					System.IO.Stream stream = doc.GetSourceStream(temp);
					if(stream != null)
						return (Bitmap)System.Drawing.Bitmap.FromStream(stream);
				}
				else
				{
					Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
					uri = new Uri(uri,baseuri);
					uri = new Uri(uri,href);
					System.IO.Stream stream = doc.GetReferencedFile(uri);
					if(stream != null)
						return (Bitmap)System.Drawing.Bitmap.FromStream(stream);
				}
			}
			catch
			{
			}
			return null;
		}
		#endregion

        #region ..GetEncoderInfo
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        #endregion
    }
}
