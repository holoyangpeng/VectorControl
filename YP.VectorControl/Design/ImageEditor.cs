using System;

namespace YP.VectorControl.Design
{
	/// <summary>
	/// �ṩ�༭ͼ���ļ����Ե������
	/// </summary>
    public class ImageEditor : FileEditor
	{
		#region ..Constructor
		public ImageEditor()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.filter = "All Files(*.*)|*.*|Bmp Files(*.bmp)|*.bmp|Jpg files(*.jpg)|*.jpg|Png files(*.png)|*.png|Gif files(*.gif)|*.gif";
		}
		#endregion
	}
}
