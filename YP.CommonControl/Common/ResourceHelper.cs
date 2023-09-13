using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace YP.CommonControl.Common
{
	public class ResourceHelper
	{
		public static Cursor LoadCursor(Type assemblyType, string cursorName)
		{
			// Get the assembly that contains the bitmap resource
			Assembly myAssembly = Assembly.GetAssembly(assemblyType);

			// Get the resource stream containing the images
			Stream iconStream = myAssembly.GetManifestResourceStream(cursorName);

			if(iconStream == null)
				return Cursors.Default;

			// Load the Icon from the stream
			return new Cursor(iconStream);
		}

        #region ..获取资源流
        /// <summary>
        /// 资源名称
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static System.IO.Stream GetFileStream(string filename)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(Type.GetType("SVGDeveloper.Resource.ResourceOperator"));
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(filename);
            }
            return null;
        }
        #endregion

		public static Icon LoadIcon(Type assemblyType, string iconName)
		{
			// Get the assembly that contains the bitmap resource
			Assembly myAssembly = Assembly.GetAssembly(assemblyType);

			// Get the resource stream containing the images
			Stream iconStream = myAssembly.GetManifestResourceStream(iconName);

			// Load the Icon from the stream
			return new Icon(iconStream);
		}

		public static Icon LoadIcon(Type assemblyType, string iconName, Size iconSize)
		{
			// Load the entire Icon requested (may include several different Icon sizes)
			Icon rawIcon = LoadIcon(assemblyType, iconName);
			
			// Create and return a new Icon that only contains the requested size
			return new Icon(rawIcon, iconSize); 
		}

		public static Bitmap LoadBitmap(Type assemblyType, string imageName)
		{
			return LoadBitmap(assemblyType, imageName, false, new Point(0,0));
		}

		public static Bitmap LoadBitmap(Type assemblyType, string imageName, Point transparentPixel)
		{
			return LoadBitmap(assemblyType, imageName, true, transparentPixel);
		}

		public static ImageList LoadBitmapStrip(Type assemblyType, string imageName, Size imageSize)
		{
			return LoadBitmapStrip(assemblyType, imageName, imageSize, false, new Point(0,0));
		}

		public static ImageList LoadBitmapStrip(Type assemblyType, 
			string imageName, 
			Size imageSize,
			Point transparentPixel)
		{
			return LoadBitmapStrip(assemblyType, imageName, imageSize, true, transparentPixel);
		}

		protected static Bitmap LoadBitmap(Type assemblyType, 
			string imageName, 
			bool makeTransparent, 
			Point transparentPixel)
		{
			// Get the assembly that contains the bitmap resource
			Assembly myAssembly = Assembly.GetAssembly(assemblyType);

			// Get the resource stream containing the images
			Stream imageStream = myAssembly.GetManifestResourceStream(imageName);

			// Load the bitmap from stream
			Bitmap image = new Bitmap(imageStream);

			if (makeTransparent)
			{
				Color backColor = image.GetPixel(transparentPixel.X, transparentPixel.Y);
    
				// Make backColor transparent for Bitmap
				image.MakeTransparent(backColor);
			}
			    
			return image;
		}

		protected static ImageList LoadBitmapStrip(Type assemblyType, 
			string imageName, 
			Size imageSize,
			bool makeTransparent,
			Point transparentPixel)
		{
			// Create storage for bitmap strip
			ImageList images = new ImageList();

			// Define the size of images we supply
			images.ImageSize = imageSize;

			// Get the assembly that contains the bitmap resource
			Assembly myAssembly = Assembly.GetAssembly(assemblyType);

			// Get the resource stream containing the images
			Stream imageStream = myAssembly.GetManifestResourceStream(imageName);

			if(imageStream == null)
				return null;
			// Load the bitmap strip from resource
			Bitmap pics = new Bitmap(imageStream);

			if (makeTransparent)
			{
				Color backColor = pics.GetPixel(transparentPixel.X, transparentPixel.Y);
    
				// Make backColor transparent for Bitmap
				pics.MakeTransparent(backColor);
			}
			    
			// Load them all !
			images.Images.AddStrip(pics);

			return images;
		}
	}
}
