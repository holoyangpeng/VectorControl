using System;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// 提供编辑图形文件属性的设计器
	/// </summary>
    public class ImageEditor : FileEditor
	{
		#region ..Constructor
		public ImageEditor()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.filter = "All Files(*.*)|*.*|Bmp Files(*.bmp)|*.bmp|Jpg files(*.jpg)|*.jpg|Png files(*.png)|*.png|Gif files(*.gif)|*.gif";
		}
		#endregion
	}
}
