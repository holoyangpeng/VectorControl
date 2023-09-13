using System;

namespace YP.Base.CSS
{
	/// <summary>
	/// �洢�������������ݣ��ڲ�ʹ��
	/// </summary>
	public class CSSPropertyContent
	{
		#region ..���켰����
		internal CSSPropertyContent(string propertyname,string propertyvalue,string priority,int level)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.Priority = priority;
			this.PropertyName = propertyname;
			this.PropertyValue = propertyvalue;
			this.Level = level;
		}
		#endregion

		#region ..�����ֶ�
		/// <summary>
		/// ��¼��������
		/// </summary>
		internal string PropertyName = string.Empty;

		/// <summary>
		/// ��¼����ֵ
		/// </summary>
		internal string PropertyValue = string.Empty;

		/// <summary>
		/// ��¼��ʾ�������ȼ����ַ���
		/// </summary>
		internal string Priority  = string.Empty;

		/// <summary>
		/// �������Եļ��𣬶��ھ߱�ͬ�����Ƶ����ԣ�ȡ�ϸ߲����Ϊ��������
		/// </summary>
		internal int Level = 0;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���ݵ��ı����
		/// </summary>
		internal string CSSText
		{
			get
			{
				string ret = PropertyName + ":" + PropertyValue;
				if(Priority != null && Priority.Length > 0)
				{
					ret += " !" + Priority;
				}
				return ret;
			}
		}
		#endregion
	}
}
