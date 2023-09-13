using System;
using YP.SVG.Text;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// InfoPos 的摘要说明。
	/// </summary>
	internal struct InfoPos
	{
		#region ..构造及消除
		public InfoPos(TextContentInfo info ,int offset)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.Info = info;
			this.Offset = offset;
		}
		#endregion

		#region ..私有变量
		public TextContentInfo Info;
		public int Offset;
		#endregion

		public static bool operator == (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return true;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			return pos1.Info == pos2.Info && pos1.Offset == pos2.Offset;
		}

		public static bool operator != (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return true;
			return pos1.Info != pos2.Info || pos1.Offset != pos2.Offset;
		}

		public static bool operator< (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			int index = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			int index1 = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			if(index < index1)
				return true;
			if(index1 == index)
				return pos1.Offset < pos2.Offset;
			return false;
		}

		public static bool operator > (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			int index = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			int index1 = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			if(index < index1)
				return false;
			if(index1 == index)
				return pos1.Offset > pos2.Offset;
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is InfoPos)
				return this == (InfoPos)obj;
			return base.Equals (obj);
		}


	}
}
