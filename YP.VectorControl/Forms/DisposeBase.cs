using System;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 建立可消除对象的基类,该类统一实现了Dispose方法，并在Dispose方法中统一管理内存。
	/// </summary>
	internal abstract class DisposeBase:System.IDisposable
	{
		#region ..构造及消除
		/// <summary>
		/// 消除对象实例,并进行垃圾回收
		/// </summary>
		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
			GC.Collect(0);
		}
		#endregion
	}
}
