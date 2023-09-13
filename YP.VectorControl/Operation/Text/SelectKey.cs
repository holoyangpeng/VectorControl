using System;
using System.Collections;
using YP.SVG.Text;
using System.Collections.Generic;

namespace YP.VectorControl.Operation.Text
{
	#region ..Shift + Left¼ü
	/// <summary>
	/// Shift + Left¼ü
	/// </summary>
	internal class ShiftLeft:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null)
			{
				TextContentInfo info = editor.Caret.Info;
				if(info != null)
				{
					int offset = editor.Caret.Offset;
					offset --;
					if(offset < 0)
					{
						int index = editor.OwnerTextElement.TextContentInfos.IndexOf(info);
						if(index - 1>=0)
						{
							info = editor.OwnerTextElement.TextContentInfos[index - 1] as TextContentInfo;
							offset = info.TextContent.Length;
						}

					}
					else if(offset == 0)
					{
						int index = editor.OwnerTextElement.TextContentInfos.IndexOf(info);
						if(index - 1>=0)
						{
							TextContentInfo info1 = editor.OwnerTextElement.TextContentInfos[index - 1] as TextContentInfo;
							if(editor.IsInLineInfo(info1))
							{
								info = info1;
								offset = info.TextContent.Length;
							}
						}
					}
					offset = (int)Math.Max(0,Math.Min(offset,info.TextContent.Length));
					editor.Caret.CaretVisible = true;
					InfoPos pos = new InfoPos(info,offset);
					if(editor.Selection.IsEmpty)
						editor.Selection.AdaptSelection(new InfoPos(editor.Caret.Info,editor.Caret.Offset),pos);
					else
						editor.Selection.AdaptSelection(editor.Selection.OriPos,pos);
					editor.Caret.AdaptCaret(info,offset);
				}
			}
		}
	}
	#endregion

	#region ..Shift + Right¼ü
	/// <summary>
	/// Shift + Right¼ü
	/// </summary>
	internal class ShiftRight:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null)
			{
				TextContentInfo info = editor.Caret.Info;
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(info != null)
				{
					int offset = editor.Caret.Offset;
					offset ++;
					if(offset > info.TextContent.Length)
					{
						int index = list.IndexOf(info);
						if(index + 1<list.Count)
						{
							info = list[index + 1] as TextContentInfo;
							offset = 0;
						}
					}
					else if(offset == info.TextContent.Length )
					{
						int index = list.IndexOf(info);
						if(index + 1<list.Count)
						{
							TextContentInfo info1 = list[index + 1] as TextContentInfo;
							if(editor.IsInLineInfo(info1))
							{
								info = info1;
								offset = 0;
							}
						}
					}
					offset = (int)Math.Max(0,Math.Min(offset,info.TextContent.Length));
					editor.Caret.CaretVisible = true;
					InfoPos pos = new InfoPos(info,offset);
					if(editor.Selection.IsEmpty)
						editor.Selection.AdaptSelection(new InfoPos(editor.Caret.Info,editor.Caret.Offset),pos);
					else
					{
						editor.Selection.AdaptSelection(editor.Selection.OriEndPos,pos);
					}
					editor.Caret.AdaptCaret(info,offset);
				}
			}
		}
	}
	#endregion

	#region ..Shift + Home¼ü
	/// <summary>
	/// Shift + Home¼ü
	/// </summary>
	internal class ShiftHome:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null)
			{
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(list.Count > 0)
				{
					TextContentInfo info = editor.Caret.Info;
					TextContentInfo info1 = editor.GetStartInfo(info);
					if(editor.Selection.IsEmpty)
						editor.Selection.AdaptSelection(new InfoPos(info,editor.Caret.Offset),new InfoPos(info1,0));
					else
						editor.Selection.AdaptSelection(editor.Selection.OriPos,new InfoPos(info1,0));
					editor.Caret.AdaptCaret(info1,0);
				}
			}
		}
	}
	#endregion

	#region ..Shift + End¼ü
	/// <summary>
	/// Shift + End¼ü
	/// </summary>
	internal class ShiftEnd:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null)
			{
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(list.Count > 0)
				{
					TextContentInfo info = editor.Caret.Info;
					TextContentInfo info1 = editor.GetEndInfo(info);
					if(editor.Selection.IsEmpty)
						editor.Selection.AdaptSelection(new InfoPos(info,editor.Caret.Offset),new InfoPos(info1,info1.TextContent.Length));
					else
						editor.Selection.AdaptSelection(editor.Selection.OriPos,new InfoPos(info1,info1.TextContent.Length));
					editor.Caret.AdaptCaret(info1,info1.TextContent.Length);
				}
			}
		}
	}
	#endregion

	#region ..Ctrl + A¼ü
	/// <summary>
	/// Ctrl + A¼ü
	/// </summary>
	internal class CtrlA:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null)
			{
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(list.Count > 0)
				{
					TextContentInfo info = list[0] as TextContentInfo;
					TextContentInfo info1 = list[list.Count -1] as TextContentInfo;
					editor.Caret.AdaptCaret(info,0);
					editor.Selection.AdaptSelection(new InfoPos(info,0),new InfoPos(info1,info1.TextContent.Length));
				}
			}
		}
	}
	#endregion

	#region ..ESC¼ü
	/// <summary>
	/// Ctrl + A¼ü
	/// </summary>
	internal class ESC:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null)
			{
				TextContentInfo info = editor.Caret.Info;
				InfoPos pos = new InfoPos(info,editor.Caret.Offset);
				editor.Selection.AdaptSelection(pos,pos);
			}
		}
	}
	#endregion
}
