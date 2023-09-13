using System;

namespace YP.SVG
{
	/// <summary>
	/// ʵ��SVGException
	/// </summary>
	public class SVGException:System.Exception
	{
		#region ..���켰����
		public SVGException(string message,SVGExceptionType exceptionType,System.Exception innerException):base(message,innerException)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.svgExceptionType = exceptionType;
		}
		#endregion

		#region ..˽�б���
		SVGExceptionType svgExceptionType;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public SVGExceptionType SVGExceptionType
		{
			get
			{
				return this.svgExceptionType;
			}
		}
		#endregion
	}
}
