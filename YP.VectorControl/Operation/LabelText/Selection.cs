using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// Selection 的摘要说明。
	/// </summary>
	internal class Selection
	{
		#region ..构造及消除
		public Selection()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		int offset = 0;
		int length = 0;
		int oldoffset,oldlength = 0;
		#endregion

		#region ..事件
		public event EventHandler SelectionChanged;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取选区的开始索引
		/// </summary>
		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		/// <summary>
		/// 获取选区的长度
		/// </summary>
		public int Length
		{
			get
			{
				return this.length;
			}
		}

		/// <summary>
		/// 判断选区是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.length == 0;
			}
		}

		public int OldOffset
		{
			get
			{
				return this.oldoffset;
			}
		}

		public int OldLength
		{
			get
			{
				return this.oldlength;
			}
		}
		#endregion

		#region ..AdaptSelection
		public void AdaptSelection(int offset,int length)
		{
			if(this.offset != offset || this.length != length)
			{
				int old = this.length;
				this.offset = offset;
				this.length = length;
				if(old != 0 || length != 0)
					this.OnSelectionChanged();
			}
		}
		#endregion

		#region ..OnSelectionChanged
		protected virtual void OnSelectionChanged()
		{
			if(this.SelectionChanged != null)
				this.SelectionChanged(this,EventArgs.Empty);
			this.oldoffset = this.offset;
			this.oldlength = this.length;
		}
		#endregion
	}
}
