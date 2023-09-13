using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using YP.SVG.Text;

namespace YP.VectorControl.Operation.Text
{
	#region ..Left¼ü
	/// <summary>
	/// Left¼ü
	/// </summary>
	internal class Left:Action
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
					editor.Caret.AdaptCaret(info,offset);
					InfoPos pos = new InfoPos(info,offset);
					editor.Selection.AdaptSelection(pos,pos);
				}
			}
		}
	}
	#endregion

	#region ..Right¼ü
	/// <summary>
	/// Right¼ü
	/// </summary>
	internal class Right:Action
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
					editor.Caret.AdaptCaret(info,offset);
					InfoPos pos = new InfoPos(info,offset);
					editor.Selection.AdaptSelection(pos,pos);
				}
			}
		}
	}
	#endregion

	#region ..Home¼ü
	/// <summary>
	/// Home¼ü
	/// </summary>
	internal class Home:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null )
			{
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(list.Count > 0)
				{
					TextContentInfo info = editor.Caret.Info;
					info = editor.GetStartInfo(info);
					editor.Caret.AdaptCaret(info,0);
				}
			}
		}
	}
	#endregion

	#region ..End¼ü
	/// <summary>
	/// End¼ü
	/// </summary>
	internal class End:Action
	{
		internal override void Execute(TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null )
			{
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				if(list.Count > 0)
				{
					TextContentInfo info = editor.Caret.Info;
					info = editor.GetEndInfo(info);
					editor.Caret.AdaptCaret(info,info.TextContent.Length);
				}
			}
		}
	}
	#endregion
}
