using System;
using System.Windows.Forms;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// 处理选区改变事件
	/// </summary>
	internal delegate void SelectionChangedEventHandler(object sender,Text.SelectionChangedEventArgs e);

	/// <summary>
	/// 记录选区改变事件的数据
	/// </summary>
	internal class SelectionChangedEventArgs:System.EventArgs
	{
		public InfoPos OldStartPos;
		public InfoPos OldEndPos;
		public InfoPos NewStartPos ;
		public InfoPos NewEndPos ;

		internal SelectionChangedEventArgs(InfoPos oldstartpos,InfoPos oldendpos,InfoPos newstartpos,InfoPos newendpos)
		{
//			this.OldLength = oldlength;
//			this.OldOffset = oldoffset;
//			this.NewLength = newlength;
//			this.NewOffset = newoffset;
			this.OldEndPos = oldendpos;
			this.OldStartPos = oldstartpos;
			this.NewEndPos = newendpos;
			this.NewStartPos = newstartpos;
		}
	}

	/// <summary>
	/// 实现选区
	/// </summary>
	internal class Selection
	{
		#region ..构造及消除
		internal Selection(TextEditor editor)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.editor = editor;
			this.startPos = new InfoPos(null,0);
			this.endPos = new InfoPos(null,0);
		}
		#endregion

		#region ..事件
		/// <summary>
		/// 当选区发生改变时触发
		/// </summary>
		internal event SelectionChangedEventHandler SelectionChanged;
		#endregion

		#region ..私有变量
		InfoPos startPos;
		InfoPos endPos;
		TextEditor editor = null;
		internal InfoPos OriPos;
		internal InfoPos OriEndPos ;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 判断选区是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.startPos == this.endPos || this.startPos.Info == null || this.endPos.Info == null;
			}
		}

		/// <summary>
		/// 获取选区开始位置
		/// </summary>
		public InfoPos StartPos
		{
			get
			{
				return this.startPos;
			}
		}

		/// <summary>
		/// 获取选区结束位置
		/// </summary>
		public InfoPos EndPos
		{
			get
			{
				return this.endPos;
			}
		}
		#endregion

		#region ..更改选区
		internal void AdaptSelection(InfoPos startpos,InfoPos endpos)
		{
			bool equal1 = this.startPos != startpos;
			bool equal2 = this.endPos != endpos;
			if(equal1 || equal2)
			{
				if(startpos.Info != null && endpos.Info != null)
				{
					int index = editor.OwnerTextElement.TextContentInfos.IndexOf(startpos.Info);
					int index1 =editor.OwnerTextElement.TextContentInfos.IndexOf(endpos.Info);
					if(index > index1)
					{
						InfoPos pos = endpos;
						endpos = startpos;
						startpos = pos;
					}
					else if(index == index1 && startpos.Offset > endpos.Offset)
					{
						InfoPos pos = endpos;
						endpos = startpos;
						startpos = pos;
					}
				}
				
				this.OnSelectionChanged(new SelectionChangedEventArgs(this.startPos,this.endPos,startpos,endpos));
				this.startPos = startpos;
				this.endPos = endpos;
				if(this.IsEmpty)
				{
					this.OriPos = startpos;
					OriEndPos = endpos;
				}
			}
		}
		#endregion

		#region ..清除选区
		/// <summary>
		/// 清除选区
		/// </summary>
		internal void Clear()
		{
			InfoPos pos = new InfoPos(editor.Caret.Info,0);
			this.AdaptSelection(pos,pos);
		}
		#endregion

		#region ..OnSelectionChanged
		protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			if(this.SelectionChanged != null)
				this.SelectionChanged(this,e);
		}
		#endregion
	}
}
