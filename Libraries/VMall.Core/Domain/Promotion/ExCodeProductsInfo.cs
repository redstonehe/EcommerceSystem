using System;
namespace VMall.Core
{
	/// <summary>
	/// �һ�����Ʒ��
	/// </summary>
	[Serializable]
	public partial class ExCodeProductsInfo
	{
		public ExCodeProductsInfo()
		{}
		#region Model
		private int _recordid;
		private int _codetypeid=0;
		private int _pid=0;
		/// <summary>
		/// ��¼id
		/// </summary>
		public int RecordId
		{
			set{ _recordid=value;}
			get{return _recordid;}
		}
		/// <summary>
		/// �һ�������id
		/// </summary>
		public int CodeTypeId
		{
			set{ _codetypeid=value;}
			get{return _codetypeid;}
		}
		/// <summary>
		/// ��Ʒid
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
		}
		#endregion Model

	}
}

