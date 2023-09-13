using System;
using YP.SVG.Text;

namespace YP.VectorControl.Operation.Text
{
	internal delegate void CaretChangedEventHandler(object sender,CaretChangedEventArgs e);

	internal class CaretChangedEventArgs:EventArgs
	{
		YP.SVG.Text.TextContentInfo oldInfo,newInfo;
		int oldOffste = 0,newOffset = 0;
		public CaretChangedEventArgs(YP.SVG.Text.TextContentInfo oldinfo,YP.SVG.Text.TextContentInfo newinfo,int oldoffset,int newoffset)
		{
			this.oldInfo = oldinfo;
			this.newInfo = newinfo;
			this.oldOffste = oldoffset;
			this.newOffset = newoffset;
		}

		public TextContentInfo OldInfo
		{
			get
			{
				return this.oldInfo;
			}
		}

		public TextContentInfo NewInfo
		{
			get
			{
				return this.newInfo;
			}
		}

		public int OldOffset
		{
			get
			{
				return this.oldOffste;
			}
		}

		public int NewOffset
		{
			get
			{
				return this.newOffset;
			}
		}
	}

	/// <summary>
	/// 实现插入符号
	/// </summary>
	internal class Caret
	{
		#region ..构造及消除
		internal Caret(TextEditor editor)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		YP.SVG.Text.SVGTextElement ownerTextElement ;
		int offset = 0;
		bool caretVisible = true;
		internal int OldOffset = 0;
		TextContentInfo caretInfo = null;
		TextEditor _editor=null;
		#endregion

		#region ..事件
		/// <summary>
		/// 当索引改变时触发
		/// </summary>
		internal event CaretChangedEventHandler CaretChanged;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置索引
		/// </summary>
		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public TextContentInfo Info
		{
			get
			{
				return this.caretInfo;
			}
		}

		/// <summary>
		/// 获取或设置插入标志可见性
		/// </summary>
		public bool CaretVisible
		{
			set
			{
				this.caretVisible = value;
			}
			get
			{
				return this.caretVisible;
			}
		}

		/// <summary>
		/// 获取或设置所属的文本对象
		/// </summary>
		public YP.SVG.Text.SVGTextElement OwnerTextElement
		{
			set
			{
				if(this.ownerTextElement != value)
				{
					this.ownerTextElement = value;
					this.offset = 0;
					this.caretVisible = true;
				}
			}
			get
			{
				return this.ownerTextElement;
			}
		}
		#endregion

		#region ..AdaptCaret
		/// <summary>
		/// 用指定的TextContentInfo和Offset更新光标位置
		/// </summary>
		internal void AdaptCaret(TextContentInfo info,int offset)
		{
			if(info != null)
			{
				if(this._editor != null && !this._editor.OwnerTextElement.TextContentInfos.Contains(info))
				{
					info = null;
					if(this._editor.OwnerTextElement.TextContentInfos.Count > 0)
					{
						info = this._editor.OwnerTextElement.TextContentInfos[0] as TextContentInfo;
						offset = 0;
					}
				}
				else
					offset = (int)Math.Max(0,Math.Min(offset,info.TextContent.Length));
			}
			if(info != this.caretInfo || offset != this.offset)
			{
				
				this.OnCaretChanged(new CaretChangedEventArgs(this.caretInfo,info,this.offset,offset));
				this.caretInfo = info;
				this.offset = offset;
			}
		}
		#endregion

		#region ..OnCaretChanged
		protected virtual void OnCaretChanged(CaretChangedEventArgs e)
		{
			if(this.CaretChanged != null)
				this.CaretChanged(this,e);
		}
		#endregion
	}
}
