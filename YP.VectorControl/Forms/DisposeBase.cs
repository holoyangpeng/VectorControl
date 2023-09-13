using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ��������������Ļ���,����ͳһʵ����Dispose����������Dispose������ͳһ�����ڴ档
	/// </summary>
	internal abstract class DisposeBase:System.IDisposable
	{
		#region ..���켰����
		/// <summary>
		/// ��������ʵ��,��������������
		/// </summary>
		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
			GC.Collect(0);
		}
		#endregion
	}
}
